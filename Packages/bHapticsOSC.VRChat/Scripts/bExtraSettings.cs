#if UNITY_EDITOR && VRC_SDK_VRCSDK3 && bHapticsOSC_HasAac
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    [System.Serializable]
    public class bExtraSettings : ScriptableObject
    {
        [SerializeField]
        public bool AnimatorWriteDefaults = true;
		
        public void Reset()
        {
            AnimatorWriteDefaults = true;
        }
    }
}
#endif