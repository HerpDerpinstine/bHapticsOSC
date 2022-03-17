using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEditor;
using UnityEditor.Animations;
using AnimatorAsCode.V0;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace bHapticsOSC
{
    [CustomEditor(typeof(bHapticsOSCIntegration))]
    public class bHapticsOSCIntegrationEditor : Editor
    {
        private static string SystemName = "bHaptics";
		private static int HeadCost = 6;
		private static int VestCost = 40;
		private static int ArmCost = 6;
		private static int HandCost = 3;
		private static int FootCost = 3;

		private static readonly HumanBodyBones[] HandBonesLeft = new HumanBodyBones[]
		{
			HumanBodyBones.LeftHand,
			HumanBodyBones.LeftIndexProximal,
			HumanBodyBones.LeftIndexIntermediate,
		};

		private static readonly HumanBodyBones[] HandBonesRight = new HumanBodyBones[]
		{
			HumanBodyBones.RightHand,
			HumanBodyBones.RightIndexProximal,
			HumanBodyBones.RightIndexIntermediate,
		};

		private enum HelpBoxType
        {
			NoFXLayer,
			NoExpressionParameters,
			NoDevicesSelected,
			NotEnoughMemory,
			NonSceneObject,
        }

		public override void OnInspectorGUI()
		{
			EditorGUILayout.Space();

			bHapticsOSCIntegration editorComp = (bHapticsOSCIntegration)target;
			if (editorComp.avatar == null)
				return;

			if (editorComp.gameObject.GetComponentsInChildren<bHapticsOSCIntegration>(true).Length > 1)
			{
				Debug.LogError("Only 1 bHapticsOSC Integration component can be used at a time!");
				DestroyImmediate(this);
				return;
			}

			VRCAvatarDescriptor.CustomAnimLayer fx_layer = editorComp.avatar.baseAnimationLayers.FirstOrDefault(x => x.type == VRCAvatarDescriptor.AnimLayerType.FX);
			if (fx_layer.isDefault || (fx_layer.animatorController == null))
			{
				DrawHelpBox(HelpBoxType.NoFXLayer);
				return;
			}

			VRCExpressionParameters expressionParameters = editorComp.avatar.expressionParameters;
			if (expressionParameters == null)
			{
				DrawHelpBox(HelpBoxType.NoExpressionParameters);
				return;
			}

			DrawAllDevices();

			if (!editorComp.IsAnyDeviceSelected())
			{
				DrawHelpBox(HelpBoxType.NoDevicesSelected);
				return;
			}

			int optionsCost = GetTotalCost();
			int currentMemoryUsed = editorComp.avatar.expressionParameters.CalcTotalCost();
			int totalCost = (currentMemoryUsed + optionsCost);
			if (totalCost > VRCExpressionParameters.MAX_PARAMETER_COST)
			{
				DrawHelpBox(HelpBoxType.NotEnoughMemory, (totalCost - VRCExpressionParameters.MAX_PARAMETER_COST).ToString());
				return;
            }

			if (!NonSceneGameObjectCheck())
				return;

			bool integrateButton = GUILayout.Button("APPLY");
			EditorGUILayout.Space();
			if (!integrateButton)
				return;

			AnimatorController animatorController = CloneAsset<AnimatorController>(fx_layer.animatorController);
			if (animatorController == null)
			{
				Debug.LogError("[bHapticsOSC] Unable to Clone Animator Controller!");
				return;
            }

			expressionParameters = CloneAsset<VRCExpressionParameters>(expressionParameters);
			if (expressionParameters == null)
			{
				Debug.LogError("[bHapticsOSC] Unable to Clone Expression Parameters!");
				return;
			}

			SerializedObject avatarObj = new SerializedObject(editorComp.avatar);

			SerializedProperty expressionsProp = avatarObj.FindProperty("expressionParameters");
			expressionsProp.objectReferenceValue = expressionParameters;

			SerializedProperty fxLayerProp = FindAnimationLayerProp(avatarObj, VRCAvatarDescriptor.AnimLayerType.FX);
			SerializedProperty animatorControllerProp = fxLayerProp.FindPropertyRelative("animatorController");
			animatorControllerProp.objectReferenceValue = animatorController;
			avatarObj.ApplyModifiedProperties();

			CreateAllNodes(animatorController, expressionParameters);
			SetupParentConstraints();

			avatarObj.ApplyModifiedProperties();
			AssetDatabase.Refresh();
			GameObject.DestroyImmediate(target);
        }

		private void DrawUIOption(string Name, ref bool Toggle, ref GameObject gameObject)
        {
			GUILayout.BeginVertical(Name, "window");
			EditorGUILayout.Space();
			DrawUIOptionInternal(ref Toggle, ref gameObject);
			EditorGUILayout.Space();
			GUILayout.EndVertical();
			EditorGUILayout.Space();
		}

		private void DrawUIOption(string Name, ref bool Toggle, ref GameObject gameObject, ref bool parentConstraints)
        {

			GUILayout.BeginVertical(Name, "window");
			EditorGUILayout.Space();
			DrawUIOptionInternal(ref Toggle, ref gameObject);
			if (Toggle && (gameObject != null))
				parentConstraints = EditorGUILayout.Toggle("Use ParentConstraints", parentConstraints);
			EditorGUILayout.Space();
			GUILayout.EndVertical();
			EditorGUILayout.Space();
		}

		private void DrawUIOptionInternal(ref bool Toggle, ref GameObject gameObject)
        {
			Toggle = EditorGUILayout.Toggle("Integrate", Toggle);
			if (Toggle)
				gameObject = (GameObject)EditorGUILayout.ObjectField("Mesh GameObject", gameObject, typeof(GameObject), true);
		}

		private void DrawAllDevices()
        {
			bHapticsOSCIntegration editorComp = (bHapticsOSCIntegration)target;

			DrawUIOption("Head", ref editorComp.ToggleHead, ref editorComp.HeadObj);

			DrawUIOption("Vest", ref editorComp.ToggleVest, ref editorComp.VestObj);

			DrawUIOption("Arm Left", ref editorComp.ToggleArmL, ref editorComp.ArmLeftObj);
			DrawUIOption("Arm Right", ref editorComp.ToggleArmR, ref editorComp.ArmRightObj);

			DrawUIOption("Hand Left", ref editorComp.ToggleHandL, ref editorComp.HandLeftObj, ref editorComp.HandLeftParentConstraint);
			DrawUIOption("Hand Right", ref editorComp.ToggleHandR, ref editorComp.HandRightObj, ref editorComp.HandRightParentConstraint);

			DrawUIOption("Foot Left", ref editorComp.ToggleFootL, ref editorComp.FootLeftObj);
			DrawUIOption("Foot Right", ref editorComp.ToggleFootR, ref editorComp.FootRightObj);
		}

		private bool NonSceneGameObjectCheck()
		{
			bHapticsOSCIntegration editorComp = (bHapticsOSCIntegration)target;
			if (!NonSceneGameObjectCheck("Head", ref editorComp.ToggleHead, ref editorComp.HeadObj)
				|| !NonSceneGameObjectCheck("Vest", ref editorComp.ToggleVest, ref editorComp.VestObj)
				|| !NonSceneGameObjectCheck("Arm Left", ref editorComp.ToggleArmL, ref editorComp.ArmLeftObj)
				|| !NonSceneGameObjectCheck("Arm Right", ref editorComp.ToggleArmR, ref editorComp.ArmRightObj)
				|| !NonSceneGameObjectCheck("Hand Left", ref editorComp.ToggleHandL, ref editorComp.HandLeftObj)
				|| !NonSceneGameObjectCheck("Hand Right", ref editorComp.ToggleHandR, ref editorComp.HandRightObj)
				|| !NonSceneGameObjectCheck("Foot Left", ref editorComp.ToggleFootL, ref editorComp.FootLeftObj)
				|| !NonSceneGameObjectCheck("Foot Right", ref editorComp.ToggleFootR, ref editorComp.FootRightObj))
				return false;
			return true;
		}

		private bool NonSceneGameObjectCheck(string Name, ref bool Toggle, ref GameObject gameObject)
        {
			if (!Toggle || (gameObject == null) || gameObject.scene.IsValid())
				return true;

			DrawHelpBox(HelpBoxType.NonSceneObject, Name);
			return false;
        }

		private void DrawHelpBox(HelpBoxType type, string additionalText = null)
        {
			string msg = null;
			MessageType messageType = MessageType.None;

			switch (type)
			{
				case HelpBoxType.NoFXLayer:
					messageType = MessageType.Error;
					msg = "No Custom FX Layer Found!\nIt is required that the Avatar have some form of non-default FX Layer set.";
					goto default;

				case HelpBoxType.NoExpressionParameters:
					messageType = MessageType.Error;
					msg = "No Custom Expression Parameters Found!\nIt is required that the Avatar have some form of Expression Parameters set.";
					goto default;

				case HelpBoxType.NoDevicesSelected:
					messageType = MessageType.Warning;
					msg = "Nothing is Selected!\nPlease check off at least 1 Device to Integrate into the Avatar.";
					goto default;

				case HelpBoxType.NotEnoughMemory:
					messageType = MessageType.Error;
					msg = $"Not enough Expression Parameter Memory!\nIn order to Integrate this many Devices you need {additionalText} more free bits.\nEither free up some space or uncheck some options.";
					goto default;

				case HelpBoxType.NonSceneObject:
					messageType = MessageType.Error;
					msg = $"Non-Scene GameObject set for {additionalText}'s Mesh GameObject!\nPlease change this to a GameObject that is in the Scene.";
					goto default;

				default:
					break;
            }
			if (string.IsNullOrEmpty(msg))
				return;

			EditorGUILayout.HelpBox(msg, messageType);
		}

		private int GetTotalCost()
		{
			bHapticsOSCIntegration editorComp = (bHapticsOSCIntegration)target;
			int total = 0;

			if (editorComp.ToggleHead)
				total += HeadCost;

			if (editorComp.ToggleVest)
				total += VestCost;

			if (editorComp.ToggleArmL)
				total += ArmCost;
			if (editorComp.ToggleArmR)
				total += ArmCost;

			if (editorComp.ToggleHandL)
				total += HandCost;
			if (editorComp.ToggleHandR)
				total += HandCost;

			if (editorComp.ToggleFootL)
				total += FootCost;
			if (editorComp.ToggleFootR)
				total += FootCost;

			return total;
		}

		private void CreateAllNodes(AnimatorController animatorController, VRCExpressionParameters expressionParameters)
		{
			bHapticsOSCIntegration editorComp = (bHapticsOSCIntegration)target;
			AacFlBase aac = editorComp.CreateAnimatorAsCode(SystemName, animatorController);

			if (editorComp.ToggleHead)
				CreateNodes(aac, expressionParameters, editorComp.HeadObj, "Head", HeadCost);

			if (editorComp.ToggleVest)
			{
				CreateNodes(aac, expressionParameters, editorComp.VestObj, "Vest_Front", VestCost / 2);
				CreateNodes(aac, expressionParameters, editorComp.VestObj, "Vest_Back", VestCost / 2);
			}

			if (editorComp.ToggleArmL)
				CreateNodes(aac, expressionParameters, editorComp.ArmLeftObj, "Arm_Left", ArmCost);
			if (editorComp.ToggleArmR)
				CreateNodes(aac, expressionParameters, editorComp.ArmRightObj, "Arm_Right", ArmCost);

			if (editorComp.ToggleHandL)
				CreateNodes(aac, expressionParameters, editorComp.HandLeftObj, "Hand_Left", HandCost);
			if (editorComp.ToggleHandR)
				CreateNodes(aac, expressionParameters, editorComp.HandRightObj, "Hand_Right", HandCost);

			if (editorComp.ToggleFootL)
				CreateNodes(aac, expressionParameters, editorComp.FootLeftObj, "Foot_Left", FootCost);
			if (editorComp.ToggleFootR)
				CreateNodes(aac, expressionParameters, editorComp.FootRightObj, "Foot_Right", FootCost);
		}

		private SerializedProperty FindAnimationLayerProp(SerializedObject avatarObj, VRCAvatarDescriptor.AnimLayerType type)
        {
			SerializedProperty layersArr = null;
			if ((type == VRCAvatarDescriptor.AnimLayerType.Sitting) || (type == VRCAvatarDescriptor.AnimLayerType.TPose) || (type == VRCAvatarDescriptor.AnimLayerType.IKPose))
				layersArr = avatarObj.FindProperty("specialAnimationLayers");
			else
				layersArr = avatarObj.FindProperty("baseAnimationLayers");

			if (layersArr == null)
				return null;

			for (int i = 0; i < layersArr.arraySize; i++)
			{
				SerializedProperty layerProp = layersArr.GetArrayElementAtIndex(i);
				if (layerProp == null)
					continue;

				SerializedProperty typeProp = layerProp.FindPropertyRelative("type");
				if ((typeProp == null) || (typeProp.intValue != (int)type))
					continue;

				return layerProp;
			}

			return null;
		}

		private void SetupParentConstraints()
		{
			bHapticsOSCIntegration editorComp = (bHapticsOSCIntegration)target;
			Animator animator = editorComp.gameObject.GetComponent<Animator>();

			if (animator == null)
				return;

			if (editorComp.ToggleHandL && editorComp.HandLeftParentConstraint && (editorComp.HandLeftObj != null))
				ApplyContraintSources(animator, editorComp.HandLeftObj.GetComponentsInChildren<ParentConstraint>(true), HandBonesLeft);

			if (editorComp.ToggleHandR && editorComp.HandRightParentConstraint && (editorComp.HandRightObj != null))
				ApplyContraintSources(animator, editorComp.HandRightObj.GetComponentsInChildren<ParentConstraint>(true), HandBonesRight);
		}

		private void ApplyContraintSources(Animator animator, ParentConstraint[] parentConstraints, HumanBodyBones[] bones)
		{
			for (int jointIndex = 0; jointIndex < parentConstraints.Length; ++jointIndex)
			{
				var constraintSource = new ConstraintSource();
				constraintSource.weight = 1f;
				constraintSource.sourceTransform = animator.GetBoneTransform(bones[jointIndex]);
				if (constraintSource.sourceTransform != null)
				{
					if (parentConstraints[jointIndex].sourceCount <= 0)
						parentConstraints[jointIndex].AddSource(constraintSource);
					else
						parentConstraints[jointIndex].SetSource(0, constraintSource);
				}
			}
		}

		private T CloneAsset<T>(Object assetObject) where T : Object
        {
			string asset_path = AssetDatabase.GetAssetPath(assetObject);

			string fileName = Path.GetFileNameWithoutExtension(asset_path);
			string ext = Path.GetExtension(asset_path);
			string folderPath = Path.GetDirectoryName(asset_path);

			string newPath = Path.Combine(folderPath, $"{fileName}.bHaptics{ext}");
			AssetDatabase.DeleteAsset(newPath);
			if (!AssetDatabase.CopyAsset(asset_path, newPath))
				return null;

			T obj = AssetDatabase.LoadAssetAtPath<T>(newPath);
			AssetDatabase.ForceReserializeAssets(new[] { newPath });
			return obj;
		}

		private void CreateNodes(AacFlBase aac, VRCExpressionParameters expressionParameters, GameObject obj, string name, int numberOfNodes)
		{
			for (int node = 1; node < numberOfNodes + 1; node++)
			{
				string nodeName = $"{SystemName}_{name}_{node}";
				AddAvatarParameter(expressionParameters, nodeName);
				CreateAnimatorLayerStates(aac, obj, node, nodeName, name);
			}
		}

		private float NameToShaderDeviceIndex(string name, int node)
        {
			switch (name)
            {
				case "Vest_Front":
					return 1.1f;
				case "Vest_Back":
					return 1.2f;

				case "Arm_Left":
					return 2f;
				case "Arm_Right":
					return 3f;

				case "Hand_Left":
					if (node == 1)
						return 4.1f;
					else if (node == 2)
						return 4.2f;
					else
						return 4.3f;

				case "Hand_Right":
					if (node == 1)
						return 5.1f;
					else if (node == 2)
						return 5.2f;
					else
						return 5.3f;

				case "Foot_Left":
					return 6f;
				case "Foot_Right":
					return 7f;

				case "Head":
				default:
					return 0;
            }
        }

		private void CreateAnimatorLayerStates(AacFlBase aac, GameObject obj, int node, string name, string deviceName)
		{
			string layer_name = $"{deviceName}_{node}";

			aac.RemoveAllSupportingLayers(layer_name);
			AacFlLayer layer = aac.CreateSupportingFxLayer(layer_name);

			AacFlFloatParameter inputParam = layer.FloatParameter(name);
			AacFlBoolParameter outputParam = layer.BoolParameter($"{name}_bool");

			AacFlState falseState = layer.NewState("False").WithWriteDefaultsSetTo(false).Drives(outputParam, false);
			AacFlState trueState = layer.NewState("True", 1, 0).WithWriteDefaultsSetTo(false).Drives(outputParam, true);

			if (obj != null)
			{
				float shaderDeviceIndex = NameToShaderDeviceIndex(deviceName, node);
				Renderer renderer = FindRendererFromShaderDeviceIndex(shaderDeviceIndex, obj);
				if (renderer != null)
				{
					if (deviceName.StartsWith("Hand"))
						node = 1;

					AacFlClip falseClip = aac.NewClip().Animating(clip => clip.Animates(renderer, $"material._Node{node}").WithOneFrame(0f));
					AacFlClip trueClip = aac.NewClip().Animating(clip => clip.Animates(renderer, $"material._Node{node}").WithOneFrame(1f));

					falseState = falseState.WithAnimation(falseClip);
					trueState = trueState.WithAnimation(trueClip);
				}
			}

			falseState.TransitionsTo(trueState).When(inputParam.IsGreaterThan(0));
			trueState.TransitionsTo(falseState).When(inputParam.IsLessThan(1));
		}

		private Renderer FindRendererFromShaderDeviceIndex(float index, GameObject obj)
        {
			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
			if ((renderers == null) || (renderers.Length <= 0))
				return null;

			foreach (Renderer renderer in renderers)
            {
				if (renderer == null)
					continue;

				Material[] materials = renderer.sharedMaterials;
				if ((materials == null) || (materials.Length <= 0))
					continue;

				foreach (Material material in materials)
				{
					if ((material == null) || !material.HasProperty("_Device"))
						continue;
					if (material.GetFloat("_Device") == index)
						return renderer;
                }
            }

			return null;
		}
		
		private void AddAvatarParameter(VRCExpressionParameters expressionParameters, string name)
		{
			string newParamName = $"{name}_bool";
			if (expressionParameters.FindParameter(newParamName) != null)
				return;

			SerializedObject avatarParametersObj = new SerializedObject(expressionParameters);

			SerializedProperty avatarParameters = avatarParametersObj.FindProperty("parameters");
			avatarParameters.arraySize += 1;
			SerializedProperty item = avatarParameters.GetArrayElementAtIndex(avatarParameters.arraySize - 1);

			SerializedProperty nameProp = item.FindPropertyRelative("name");
			nameProp.stringValue = newParamName;

			SerializedProperty valueType = item.FindPropertyRelative("valueType");
			valueType.intValue = (int)VRCExpressionParameters.ValueType.Bool;
			
			SerializedProperty defaultValue = item.FindPropertyRelative("defaultValue");
			defaultValue.floatValue = 0;
			
			SerializedProperty saved = item.FindPropertyRelative("saved");
			saved.boolValue = false;

			avatarParametersObj.ApplyModifiedProperties();
		}
	}
}