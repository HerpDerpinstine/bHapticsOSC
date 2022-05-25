#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    [InitializeOnLoad]
    internal class bURLs : Editor
    {
        [MenuItem("bHapticsOSC/Wiki")]
        private static void OpenWiki()
            => Application.OpenURL("https://github.com/HerpDerpinstine/bHapticsOSC/wiki");

        [MenuItem("bHapticsOSC/GitHub Repository")]
        private static void OpenRepository()
            => Application.OpenURL("https://github.com/HerpDerpinstine/bHapticsOSC");

        [MenuItem("bHapticsOSC/bHaptics Homepage")]
        private static void OpenbHapticsHomepage()
            => Application.OpenURL("https://www.bhaptics.com/");
    }
}
#endif