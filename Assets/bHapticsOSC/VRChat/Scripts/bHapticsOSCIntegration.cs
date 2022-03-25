#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AnimatorAsCode.V0;
using VRC.SDK3.Avatars.Components;

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
		public GameObject ArmLeftObj;

		public bool ToggleArmR = false;
		public GameObject ArmRightObj;

		public bool ToggleHandL = false;
		public GameObject HandLeftObj;
		public bool HandLeftParentConstraint = true;

		public bool ToggleHandR = false;
		public GameObject HandRightObj;
		public bool HandRightParentConstraint = true;

		public bool ToggleFootL = false;
		public GameObject FootLeftObj;

		public bool ToggleFootR = false;
		public GameObject FootRightObj;

		public bool IsAnyDeviceSelected()
			=> ToggleHead
				|| ToggleVest
				|| ToggleArmL
				|| ToggleArmR
				|| ToggleHandL
				|| ToggleHandR
				|| ToggleFootL
				|| ToggleFootR;

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

		public AacFlBase CreateAnimatorAsCode(string name, AnimatorController animatorController)
		{
			AacFlBase aac = AacV0.Create(new AacConfiguration
			{
				SystemName = name,
				AvatarDescriptor = avatar,
				AnimatorRoot = transform,
				DefaultValueRoot = transform,
				AssetContainer = animatorController,
				AssetKey = assetKey,
				DefaultsProvider = new AacDefaultsProvider(false)
			});
			return aac;
		}
	}
}
#endif