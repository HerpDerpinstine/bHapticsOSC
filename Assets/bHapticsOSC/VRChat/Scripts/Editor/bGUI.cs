using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    public static class bGUI
    {
        private static Color SeperatorColor = new Color(0.16f, 0.16f, 0.16f, 1f);
        private static Texture2D SeperatorTexture;
        private static GUIStyle SeperatorStyle;

        private static GUIStyle ButtonStyle;
        private static GUIStyle HeaderButtonStyle;

        public static Sprite Rig;
        private static GUIStyle RigStyle;

        public static Dictionary<bDeviceType, bGUITemplateElements> Elements = new Dictionary<bDeviceType, bGUITemplateElements>();

        static bGUI()
        {
            SeperatorTexture = new Texture2D(1, 1);
            SeperatorTexture.SetPixels(new Color[] { SeperatorColor });
            SeperatorTexture.Apply();

            SeperatorStyle = new GUIStyle(GUI.skin.box);
            SeperatorStyle.normal.background = SeperatorTexture;
            SeperatorStyle.normal.textColor = SeperatorColor;

            ButtonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter };
            HeaderButtonStyle = new GUIStyle(ButtonStyle) { contentOffset = new Vector2(-5, 1) };

            Rig = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/rig.png", typeof(Sprite));
            RigStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter };

            Elements[bDeviceType.HEAD] = new bGUITemplateElements
            {
                NotSelected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactal.png", typeof(Sprite)),
                Selected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactal_selected.png", typeof(Sprite)),
                Prefab = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactal_prefab.png", typeof(Sprite)),
                Style = new GUIStyle(RigStyle) { contentOffset = new Vector2(2, 0) }
            };

            Elements[bDeviceType.VEST] = new bGUITemplateElements
            {
                NotSelected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactsuit.png", typeof(Sprite)),
                Selected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactsuit_selected.png", typeof(Sprite)),
                Prefab = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactsuit_prefab.png", typeof(Sprite)),
                Style = new GUIStyle(RigStyle) { contentOffset = new Vector2(2, 0) }
            };

            Elements[bDeviceType.ARM_LEFT] = new bGUITemplateElements
            {
                NotSelected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyA_left.png", typeof(Sprite)),
                Selected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyA_left_selected.png", typeof(Sprite)),
                Prefab = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyA_left_prefab.png", typeof(Sprite)),
                Style = new GUIStyle(RigStyle) { contentOffset = new Vector2(62, 0) }
            };

            Elements[bDeviceType.ARM_RIGHT] = new bGUITemplateElements
            {
                NotSelected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyA_right.png", typeof(Sprite)),
                Selected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyA_right_selected.png", typeof(Sprite)),
                Prefab = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyA_right_prefab.png", typeof(Sprite)),
                Style = new GUIStyle(RigStyle) { contentOffset = new Vector2(-58, 0) }
            };

            Elements[bDeviceType.HAND_LEFT] = new bGUITemplateElements
            {
                NotSelected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyH_left.png", typeof(Sprite)),
                Selected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyH_left_selected.png", typeof(Sprite)),
                Prefab = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyH_left_prefab.png", typeof(Sprite)),
                Style = new GUIStyle(RigStyle) { contentOffset = new Vector2(78, 0) }
            };

            Elements[bDeviceType.HAND_RIGHT] = new bGUITemplateElements
            {
                NotSelected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyH_right.png", typeof(Sprite)),
                Selected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyH_right_selected.png", typeof(Sprite)),
                Prefab = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyH_right_prefab.png", typeof(Sprite)),
                Style = new GUIStyle(RigStyle) { contentOffset = new Vector2(-73, 0) }
            };

            // Gloves

            Elements[bDeviceType.FOOT_LEFT] = new bGUITemplateElements
            {
                NotSelected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyF_left.png", typeof(Sprite)),
                Selected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyF_left_selected.png", typeof(Sprite)),
                Prefab = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyF_left_prefab.png", typeof(Sprite)),
                Style = new GUIStyle(RigStyle) { contentOffset = new Vector2(54, 0) }
            };

            Elements[bDeviceType.FOOT_RIGHT] = new bGUITemplateElements
            {
                NotSelected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyF_right.png", typeof(Sprite)),
                Selected = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyF_right_selected.png", typeof(Sprite)),
                Prefab = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/bHapticsOSC/VRChat/Textures/UI/tactosyF_right_prefab.png", typeof(Sprite)),
                Style = new GUIStyle(RigStyle) { contentOffset = new Vector2(-50, 0) }
            };
        }

        public static void DrawSeparator()
        {
            EditorGUILayout.Space();
            GUILayout.Box(string.Empty, SeperatorStyle, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.Space();
        }

        public static void DrawToggle(string text, ref bool value)
        {
            Rect toggleRect = GUILayoutUtility.GetRect(new GUIContent(text), GUI.skin.toggle);
            toggleRect.width = GUI.skin.toggle.CalcSize(new GUIContent(text)).x;

            value = GUI.Toggle(toggleRect, value, text);
            EditorGUIUtility.AddCursorRect(toggleRect, MouseCursor.Link);
        }

        public static void DrawSection(string name, System.Action draw, System.Action headerDraw = null)
        {
            GUILayout.BeginVertical(name, "window");
            headerDraw?.Invoke();
            if (!string.IsNullOrEmpty(name))
            {
                GUILayout.Space(-8);
                DrawSeparator();
                GUILayout.Space(-3);
            }
            draw?.Invoke();
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        public static void DrawRig()
        {
            Texture2D spriteTexture = SpriteUtility.GetSpriteTexture(Rig, false);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label(spriteTexture, RigStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void DrawTemplateButton(bHapticsOSCIntegration editorComp, bDeviceType device)
        {
            bGUITemplateElements templateElement = Elements[device];
            if (templateElement == null)
                return;

            bDeviceTemplate template = bDevice.AllTemplates[device];

            bool isSelected = (editorComp.CurrentTemplate == template);
            Sprite sprite = isSelected ? templateElement.Selected : templateElement.NotSelected;

            Texture2D spriteTexture = SpriteUtility.GetSpriteTexture(sprite, false);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            Rect spriteRect = GUILayoutUtility.GetRect(new GUIContent(spriteTexture), templateElement.Style);
            spriteRect.width = spriteTexture.width;

            Vector2 originalOffset = templateElement.Style.contentOffset;
            templateElement.Style.contentOffset = Vector2.zero;
            spriteRect.x += originalOffset.x;
            spriteRect.y += originalOffset.y;

            if (GUI.Button(spriteRect, spriteTexture, templateElement.Style) && !isSelected)
            {
                isSelected = true;
                editorComp.CurrentTemplate = template;
            }

            if ((templateElement.Prefab != null) && (editorComp.AllUserSettings[template].CurrentPrefab != null))
            {
                Texture2D spriteTexture2 = SpriteUtility.GetSpriteTexture(templateElement.Prefab, false);
                GUI.Label(spriteRect, spriteTexture2, templateElement.Style);
            }

            templateElement.Style.contentOffset = originalOffset;

            if (!isSelected)
                EditorGUIUtility.AddCursorRect(spriteRect, MouseCursor.Link);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void DrawButton(string text, System.Action onClick)
        {
            GUIContent content = new GUIContent(text);

            Rect buttonRect = GUILayoutUtility.GetRect(content, ButtonStyle);
            if (GUI.Button(buttonRect, content, ButtonStyle))
                onClick?.Invoke();

            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
        }

        public static void DrawHeaderButton(string text, System.Action onClick)
        {
            GUIContent content = new GUIContent(text);

            Rect buttonRect = GUILayoutUtility.GetRect(content, HeaderButtonStyle);
            buttonRect.width = HeaderButtonStyle.CalcSize(content).x + 6;

            Vector2 originalOffset = HeaderButtonStyle.contentOffset;
            HeaderButtonStyle.contentOffset = Vector2.zero;
            buttonRect.x += originalOffset.x;
            buttonRect.y += originalOffset.y;

            if (GUI.Button(buttonRect, content, HeaderButtonStyle))
                onClick?.Invoke();

            HeaderButtonStyle.contentOffset = originalOffset;

            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
        }

        public enum HelpBoxType
        {
            NoFXLayer,
            NoExpressionParameters,
            NothingSelected,
            NotEnoughMemory,
            NonSceneObject,
        }
        public static void DrawHelpBox(HelpBoxType type, string additionalText = null)
        {
            string msg = null;
            MessageType messageType = MessageType.None;

            switch (type)
            {
                case HelpBoxType.NoFXLayer:
                    messageType = MessageType.Error;
                    msg = "No Custom FX Layer Found!\nIt is required that the Avatar have some form of non-default FX Layer set.";
                    goto default;

                case HelpBoxType.NothingSelected:
                    messageType = MessageType.Warning;
                    msg = "Nothing is Added!\nPlease add at least 1 Device to Integrate into the Avatar.";
                    goto default;

                case HelpBoxType.NotEnoughMemory:
                    messageType = MessageType.Error;
                    msg = $"Not enough Expression Parameter Memory!\nIn order to Integrate this many options you need {additionalText} more free bits.\nEither free up some space or get rid of some Devices.";
                    goto default;

                default:
                    break;
            }
            if (string.IsNullOrEmpty(msg))
                return;

            EditorGUILayout.HelpBox(msg, messageType);
            EditorGUILayout.Space();
        }
    }
}