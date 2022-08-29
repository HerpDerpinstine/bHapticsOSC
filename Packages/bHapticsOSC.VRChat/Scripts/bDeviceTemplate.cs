#if UNITY_EDITOR && VRC_SDK_VRCSDK3 && bHapticsOSC_HasAac
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    public class bDeviceTemplate
    {
        public string Name;
        public float ShaderIndex;
        public int NodeCount;
        public HumanBodyBones Bone;
        public bool HasBone;

        public GameObject Prefab;
        public GameObject PrefabMesh;

        public bool HasParentConstraints;
    }
}
#endif