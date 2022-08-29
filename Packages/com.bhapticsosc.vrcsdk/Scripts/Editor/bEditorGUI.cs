#if UNITY_EDITOR && VRC_SDK_VRCSDK3 && bHapticsOSC_HasAac
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
			serializedObject.Update();
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

			if (editorComp.AllUserSettings == null)
            {
				editorComp.AllUserSettings = new Dictionary<bDeviceTemplate, bUserSettings>();
				for (int i = 0; i < bDevice.AllTemplates.Values.Count; i++)
				{
					bDeviceTemplate template = bDevice.AllTemplates.Values.ElementAt(i);
					if (!template.HasBone)
						continue;

					bUserSettings newSettings = CreateInstance<bUserSettings>();
					newSettings.Bone = template.Bone;
					newSettings.OnShowMeshChange = (bUserSettings thisSettings) => thisSettings.SwapPrefabs(editorComp.avatarAnimator, thisSettings.ShowMesh ? template.PrefabMesh : template.Prefab);
					editorComp.AllUserSettings[template] = newSettings;
				}
			}

			if (editorComp.AllCustomContactTagsContainers == null)
			{
				editorComp.AllCustomContactTagsContainers = new Dictionary<bUserSettings, bReorderableListContainer<string>>();
				foreach (bUserSettings settings in editorComp.AllUserSettings.Values)
					editorComp.AllCustomContactTagsContainers[settings] = new bReorderableListContainer<string>("Custom Contact Tags", "New_Tag", bGUI.LabelStyle, new SerializedObject(settings).FindProperty("CustomContactTags"));
			}

			editorComp.FindExistingPrefabs(bDevice.AllTemplates);

			bDeviceTemplate CurrentTemplate = bDevice.AllTemplates[editorComp.CurrentDevice];
			bUserSettings userSettings = editorComp.AllUserSettings[CurrentTemplate];
			bReorderableListContainer<string> CustomContactTagsContainer = editorComp.AllCustomContactTagsContainers[userSettings];
			
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
				bGUI.DrawSection(CurrentTemplate.Name, () =>
				{
					if (userSettings.CurrentPrefab == null)
					{
						if (bGUI.DrawButton("+ ADD DEVICE"))
							userSettings.Reset();
						return;
					}

					userSettings.ShowMesh = bGUI.DrawToggle("Show Mesh", userSettings.ShowMesh, userSettings);
					GUILayout.Space(6);

					if (CurrentTemplate.HasParentConstraints)
					{
						userSettings.ApplyParentConstraints = bGUI.DrawToggle("Apply ParentConstraints", userSettings.ApplyParentConstraints, userSettings);
						GUILayout.Space(6);
					}

					// Transform Editor
					userSettings.CurrentPrefab.transform.localPosition = bGUI.DrawVector3Field("Position", userSettings.CurrentPrefab.transform.localPosition, userSettings.CurrentPrefab.transform);
					userSettings.CurrentPrefab.transform.localEulerAngles = bGUI.DrawVector3Field("Rotation", userSettings.CurrentPrefab.transform.localEulerAngles, userSettings.CurrentPrefab.transform);

					userSettings.CurrentPrefab.transform.localScale = bGUI.DrawVector3Field("Scale", userSettings.CurrentPrefab.transform.localScale, userSettings.CurrentPrefab.transform);

					GUILayout.Space(10);

					// Custom Contact Tags
					CustomContactTagsContainer.Draw();

					// END
					bGUI.DrawSeparator();
					GUILayout.BeginHorizontal();
					if (bGUI.DrawButton("SELECT IN SCENE"))
						userSettings.SelectCurrentPrefab();
					if (bGUI.DrawButton("REMOVE DEVICE", userSettings, false))
						userSettings.DestroyCurrentPrefab();
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

					if (bGUI.DrawHeaderButton("RESET", userSettings, false))
						userSettings.Reset();

					GUILayout.EndHorizontal();
					GUILayout.Space(-GUILayoutUtility.GetLastRect().height);
				});

				GUILayout.Space(-4);
			});
			bGUI.DrawSeparator();

			bGUI.DrawSection("Extras", () =>
			{
				editorComp.extraSettings.AnimatorWriteDefaults = bGUI.DrawToggle("Animator Write Defaults", editorComp.extraSettings.AnimatorWriteDefaults, editorComp.extraSettings);
				//bGUI.DrawToggle("Udon AudioLink Extension Support", ref editorComp.AudioLink);
			},
			() =>
			{
				GUILayout.Space(-20);

				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.FlexibleSpace();
				GUILayout.FlexibleSpace();
				GUILayout.FlexibleSpace();

				if (bGUI.DrawHeaderButton("RESET", editorComp.extraSettings, false))
					editorComp.extraSettings.Reset();

				GUILayout.EndHorizontal();
				GUILayout.Space(-GUILayoutUtility.GetLastRect().height);
			});

			bGUI.DrawSeparator();

			if (!editorComp.IsReadyToApply())
			{
				bGUI.DrawHelpBox(bGUI.HelpBoxType.NotReadyToApply);
				return;
			}

			if (bGUI.DrawButton("APPLY INTEGRATION"))
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
					bContacts.ApplyNewTags(editorComp);

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
			}

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
#endif