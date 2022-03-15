#if UNITY_EDITOR
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using UnityEditor;

namespace bHapticsOSC
{
    [AddComponentMenu("bHapticsOSC Integration")]
    [ExecuteInEditMode]
    public class bHapticsOSCIntegration : MonoBehaviour
    {
        public VRCAvatarDescriptor avatar;
        public string assetKey;

		public bool ToggleHead = false;
		public GameObject HeadObj;

		public bool ToggleVest = false;
		public GameObject VestObj;

		public bool ToggleArmL = false;
		public bool ToggleArmR = false;
		public GameObject ArmLeftObj;
		public GameObject ArmRightObj;

		public bool ToggleHandL = false;
		public bool ToggleHandR = false;
		public bool HandLeftParentConstraint = true;
		public bool HandRightParentConstraint = true;
		public GameObject HandLeftObj;
		public GameObject HandRightObj;

		public bool ToggleFootL = false;
		public bool ToggleFootR = false;
		public GameObject FootLeftObj;
		public GameObject FootRightObj;

		public void Awake()
        {
            avatar = gameObject.GetComponent<VRCAvatarDescriptor>();
            if (avatar == null)
            {
                GameObject.DestroyImmediate(this);
                return;
            }

            if (string.IsNullOrEmpty(assetKey) || string.IsNullOrEmpty(assetKey.Trim()))
                assetKey = GUID.Generate().ToString();
        }
    }
}
#endif