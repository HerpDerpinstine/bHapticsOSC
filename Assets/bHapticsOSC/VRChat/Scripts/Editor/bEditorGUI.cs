using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;

namespace bHapticsOSC.VRChat
{
    [CustomEditor(typeof(bHapticsOSCIntegration))]
    public class bEditorGUI : Editor
	{
		private bHapticsOSCIntegration editorComp;

		public override void OnInspectorGUI()
		{
			EditorGUILayout.Space();

			editorComp = (bHapticsOSCIntegration)target;
			editorComp.Validate();
			if (editorComp.avatar == null)
				return;

			editorComp.fx_layer = editorComp.avatar.baseAnimationLayers.FirstOrDefault(x => x.type == VRCAvatarDescriptor.AnimLayerType.FX);
			if (editorComp.fx_layer.isDefault || (editorComp.fx_layer.animatorController == null))
			{
				bGUI.DrawHelpBox(bGUI.HelpBoxType.NoFXLayer);
				return;
			}
			if (editorComp.CurrentTemplate == null)
				editorComp.CurrentTemplate = bDevice.AllTemplates[bDeviceType.VEST];

			if (editorComp.AllUserSettings == null)
            {
				editorComp.AllUserSettings = new Dictionary<bDeviceTemplate, bUserSettings>();
				for (int i = 0; i < bDevice.AllTemplates.Values.Count; i++)
				{
					bDeviceTemplate template = bDevice.AllTemplates.Values.ElementAt(i);
					if (!template.HasBone)
						continue;
					bUserSettings newSettings = new bUserSettings
					{
						Bone = template.Bone,
						OnShowMeshChange = (bUserSettings thisSettings) => thisSettings.SwapPrefabs(editorComp.avatarAnimator, thisSettings.ShowMesh ? template.PrefabMesh : template.Prefab)
					};
					newSettings.FindExistingPrefab(template);
					editorComp.AllUserSettings[template] = newSettings;
				}
			}
			
			bUserSettings userSettings = editorComp.AllUserSettings[editorComp.CurrentTemplate];
			
			bGUI.DrawSection(string.Empty, () =>
			{
				// Rig
				EditorGUILayout.Space(-8);
				bGUI.DrawRig();

				// Head
				EditorGUILayout.Space(-(bGUI.Rig.rect.height - 3));
				bGUI.DrawTemplateButton(editorComp, bDeviceType.HEAD);

				// Arms
				EditorGUILayout.Space(bGUI.Elements[bDeviceType.VEST].NotSelected.rect.height - 44);
				bGUI.DrawTemplateButton(editorComp, bDeviceType.ARM_RIGHT);
				EditorGUILayout.Space(-(bGUI.Elements[bDeviceType.ARM_RIGHT].NotSelected.rect.height + 6));
				bGUI.DrawTemplateButton(editorComp, bDeviceType.ARM_LEFT);

				// Vest
				// Rendering After Arms because Layering Derp
				EditorGUILayout.Space(-(bGUI.Elements[bDeviceType.VEST].NotSelected.rect.height - 8));
				bGUI.DrawTemplateButton(editorComp, bDeviceType.VEST);

				// Hands
				EditorGUILayout.Space(-24);
				bGUI.DrawTemplateButton(editorComp, bDeviceType.HAND_RIGHT);
				EditorGUILayout.Space(-(bGUI.Elements[bDeviceType.HAND_RIGHT].NotSelected.rect.height + 6));
				bGUI.DrawTemplateButton(editorComp, bDeviceType.HAND_LEFT);

				// Gloves

				// Feet
				EditorGUILayout.Space(142);
				bGUI.DrawTemplateButton(editorComp, bDeviceType.FOOT_RIGHT);
				EditorGUILayout.Space(-(bGUI.Elements[bDeviceType.FOOT_RIGHT].NotSelected.rect.height + 6));
				bGUI.DrawTemplateButton(editorComp, bDeviceType.FOOT_LEFT);

				// END
				EditorGUILayout.Space(12);

				// Selected Device
				bGUI.DrawSection(editorComp.CurrentTemplate.Name, () =>
				{
					if (userSettings.CurrentPrefab == null)
					{
						bGUI.DrawButton("+ ADD DEVICE", () => userSettings.SwapPrefabs(editorComp.avatarAnimator, editorComp.CurrentTemplate.PrefabMesh));
						return;
					}

					bool localShowMesh = userSettings.ShowMesh;
					bGUI.DrawToggle("Show Mesh", ref localShowMesh);
					userSettings.ShowMesh = localShowMesh;

					if (editorComp.CurrentTemplate.HasParentConstraints)
					{
						GUILayout.Space(6);
						bGUI.DrawToggle("Apply ParentConstraints", ref userSettings.ApplyParentConstraints);
					}

					GUILayout.Space(12);

					// Position Editor
					userSettings.CurrentPrefab.transform.localPosition = EditorGUILayout.Vector3Field("Position", userSettings.CurrentPrefab.transform.localPosition);
					GUILayout.Space(6);

					// Rotation Editor
					userSettings.CurrentPrefab.transform.localEulerAngles = EditorGUILayout.Vector3Field("Rotation", userSettings.CurrentPrefab.transform.localEulerAngles);
					GUILayout.Space(6);

					// Scale Editor
					userSettings.CurrentPrefab.transform.localScale = EditorGUILayout.Vector3Field("Scale", userSettings.CurrentPrefab.transform.localScale);
					GUILayout.Space(12);

					// Custom Contact Tags
					//GUILayout.Space(12);

					GUILayout.BeginHorizontal();
					bGUI.DrawButton("SELECT IN SCENE", userSettings.SelectCurrentPrefab);
					bGUI.DrawButton("REMOVE DEVICE", userSettings.DestroyCurrentPrefab);
					GUILayout.EndHorizontal();
				},
				() =>
				{
					if (userSettings.CurrentPrefab == null)
						return;

					GUILayout.Space(-20);

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.FlexibleSpace();
					GUILayout.FlexibleSpace();
					GUILayout.FlexibleSpace();

					bGUI.DrawHeaderButton("RESET", userSettings.Reset);

					GUILayout.EndHorizontal();
					GUILayout.Space(-GUILayoutUtility.GetLastRect().height);
				});

				GUILayout.Space(-4);
			});

			bGUI.DrawSeparator();

			/*
			bGUI.DrawSection("Extras", () =>
			{
				bGUI.DrawToggle("Udon AudioLink Extension Support", ref editorComp.AudioLink);
			},
			() =>
			{
				GUILayout.Space(-20);

				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.FlexibleSpace();
				GUILayout.FlexibleSpace();
				GUILayout.FlexibleSpace();

				bGUI.DrawHeaderButton("RESET", editorComp.ResetExtras);

				GUILayout.EndHorizontal();
				GUILayout.Space(-GUILayoutUtility.GetLastRect().height);
			});

			bGUI.DrawSeparator();
			*/

			if (!editorComp.IsReadyToApply())
			{
				bGUI.DrawHelpBox(bGUI.HelpBoxType.NothingSelected);
				return;
			}

			bGUI.DrawButton("APPLY INTEGRATION", () =>
			{
				EditorUtility.DisplayProgressBar(bHapticsOSCIntegration.SystemName, "Cloning FX Layer...", 0);
				if (!CloneAnimatorAsset())
				{
					EditorUtility.ClearProgressBar();
					EditorUtility.DisplayDialog(bHapticsOSCIntegration.SystemName, "Unable to Clone Animator Controller!", "OK");
				}
				else
				{
					EditorUtility.DisplayProgressBar(bHapticsOSCIntegration.SystemName, "Modifying Avatar...", 0.5f);
					ApplySerializedChanges();
					bAnimator.CreateAllNodes(editorComp);

					if (bConstraints.ShouldApply(editorComp, bDeviceType.HAND_LEFT, out bUserSettings leftHandSettings)
						|| bConstraints.ShouldApply(editorComp, bDeviceType.HAND_RIGHT, out bUserSettings rightHandSettings))
					{
						EditorUtility.DisplayProgressBar(bHapticsOSCIntegration.SystemName, "Applying ParentConstraints...", 0.9f);
						bConstraints.Apply(editorComp);
					}

					EditorUtility.ClearProgressBar();
					EditorUtility.DisplayDialog(bHapticsOSCIntegration.SystemName, "Integration Complete!\nThe Avatar is now setup for bHapticsOSC support.", "OK");
					DestroyImmediate(editorComp);
				}
			});

			GUILayout.Space(6);

			if (GUI.changed)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}

		private bool CloneAnimatorAsset()
        {
			editorComp.animatorControllerClone = (AnimatorController)bAsset.Clone(editorComp.fx_layer.animatorController);
			return (editorComp.animatorControllerClone != null);
		}

		private void ApplySerializedChanges()
		{
			editorComp.avatarAnimator.runtimeAnimatorController = editorComp.animatorControllerClone;

			SerializedObject avatarObj = new SerializedObject(editorComp.avatar);

			SerializedProperty fxLayerProp = bAvatar.FindAnimationLayerProp(avatarObj, VRCAvatarDescriptor.AnimLayerType.FX);
			SerializedProperty animatorControllerProp = fxLayerProp.FindPropertyRelative("animatorController");
			animatorControllerProp.objectReferenceValue = editorComp.animatorControllerClone;
			avatarObj.ApplyModifiedPropertiesWithoutUndo();
		}
	}
}