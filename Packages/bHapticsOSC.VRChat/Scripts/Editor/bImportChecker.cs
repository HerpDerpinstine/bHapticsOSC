#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    [InitializeOnLoad]
    public class bImportChecker : Editor
    {
        static bImportChecker() =>
            Refresh();

        [InitializeOnLoadMethod]

        public static void Refresh()
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string definitionsStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            List<string> definitionsTbl = string.IsNullOrEmpty(definitionsStr)
                ? new List<string>()
                : definitionsStr.Split(';').ToList();
            bool shouldApplyDefinitions = false;

            VRCSDKCheck(ref definitionsTbl, ref shouldApplyDefinitions);
            AacCheck(ref definitionsTbl, ref shouldApplyDefinitions);

            if (shouldApplyDefinitions)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", definitionsTbl.ToArray()));
        }

        private static void VRCSDKCheck(ref List<string> definitionsTbl, ref bool shouldApplyDefinitions)
        {
            if (definitionsTbl.Contains("VRC_SDK_VRCSDK3"))
            {
                if (definitionsTbl.Contains("bHapticsOSC_VRCSDKWarning"))
                {
                    definitionsTbl.Remove("bHapticsOSC_VRCSDKWarning");
                    shouldApplyDefinitions = true;
                }
            }
            else
            {
                if (!definitionsTbl.Contains("bHapticsOSC_VRCSDKWarning"))
                {
                    definitionsTbl.Add("bHapticsOSC_VRCSDKWarning");
                    shouldApplyDefinitions = true;
                    VRCSDKWarning();
                }
            }
        }

#if !VRC_SDK_VRCSDK3
        [MenuItem("bHapticsOSC/VRChat SDK 3.0 is Required!")]
#endif
        private static void VRCSDKWarning()
        {
            Debug.LogError("bHapticsOSC requires VRChat SDK 3.0!");
            EditorUtility.DisplayDialog("bHapticsOSC", "bHapticsOSC requires VRChat SDK 3.0!\nPlease import it.", "OK");
            Application.OpenURL("https://docs.vrchat.com/docs/setting-up-the-sdk#step-2---importing-the-sdk");
        }

        private static void AacCheck(ref List<string> definitionsTbl, ref bool shouldApplyDefinitions)
        {
            if (HasAac())
            {
                if (!definitionsTbl.Contains("bHapticsOSC_HasAac"))
                {
                    definitionsTbl.Add("bHapticsOSC_HasAac");
                    shouldApplyDefinitions = true;
                }

                if (definitionsTbl.Contains("bHapticsOSC_AacWarning"))
                {
                    definitionsTbl.Remove("bHapticsOSC_AacWarning");
                    shouldApplyDefinitions = true;
                }
            }
            else
            {
                if (definitionsTbl.Contains("bHapticsOSC_HasAac"))
                {
                    definitionsTbl.Remove("bHapticsOSC_HasAac");
                    shouldApplyDefinitions = true;
                }

                if (!definitionsTbl.Contains("bHapticsOSC_AacWarning"))
                {
                    definitionsTbl.Add("bHapticsOSC_AacWarning");
                    shouldApplyDefinitions = true;
                    AacWarning();
                }
            }
        }

#if !bHapticsOSC_HasAac
        [MenuItem("bHapticsOSC/Animator as Code is Required!")]
#endif
        private static void AacWarning()
        {
            Debug.LogError("bHapticsOSC requires Animator as Code!");
            EditorUtility.DisplayDialog("bHapticsOSC", "bHapticsOSC requires Animator as Code!\nPlease import it.", "OK");
            Application.OpenURL("https://github.com/HerpDerpinstine/bHapticsOSC/wiki/VRChat:-Custom-Avatar-Integration-~-Importing-Animator-As-Code");
        }

        private static bool HasAac()
        {
            try { return Assembly.Load("AnimatorAsCodeFramework") != null; } catch { }
            return false;
        }
    }
}
#endif