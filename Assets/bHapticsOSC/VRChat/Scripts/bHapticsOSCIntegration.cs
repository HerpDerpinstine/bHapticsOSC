#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AnimatorAsCode.V0;
using VRC.SDK3.Avatars.Components;
using System.Collections.Generic;

namespace bHapticsOSC.VRChat
{
    [AddComponentMenu("bHapticsOSC Integration")]
    [ExecuteInEditMode]
    [System.Serializable]
    public class bHapticsOSCIntegration : MonoBehaviour
    {
        public static string SystemName = "bHapticsOSC";

        [SerializeField]
        public VRCAvatarDescriptor avatar;
        [SerializeField]
        public Animator avatarAnimator;
        [SerializeField]
        public string assetKey;

        [SerializeField]
        public bDeviceTemplate CurrentTemplate;
        [SerializeField]
        public Dictionary<bDeviceTemplate, bUserSettings> AllUserSettings;

        public VRCAvatarDescriptor.CustomAnimLayer fx_layer;
        public AnimatorController animatorControllerClone;

        //private static int AudioLinkCost = 8;
        //[SerializeField]
        //public bool AudioLink = false;

        public void Validate()
	    {
            avatar = gameObject.GetComponent<VRCAvatarDescriptor>();
            if (avatar == null)
            {
                Debug.LogError("No VRCAvatarDescriptor Detected!");
                DestroyImmediate(this);
                return;
            }
            
            if (gameObject.GetComponentsInChildren<bHapticsOSCIntegration>(true).Length > 1)
            {
                Debug.LogError("Only 1 bHapticsOSC Integration component can be used at a time!");
                DestroyImmediate(this);
                return;
            }

            avatarAnimator = gameObject.GetComponent<Animator>();
            if (avatarAnimator == null)
            {
                Debug.LogError("Avatar must have an Animator!");
                DestroyImmediate(this);
                return;
            }

            if (string.IsNullOrEmpty(assetKey) || string.IsNullOrEmpty(assetKey.Trim()))
                assetKey = GUID.Generate().ToString();
        }

		public AacFlBase CreateAnimatorAsCode()
		{
			AacFlBase aac = AacV0.Create(new AacConfiguration
			{
				SystemName = SystemName,
				AvatarDescriptor = avatar,
				AnimatorRoot = transform,
				DefaultValueRoot = transform,
				AssetContainer = animatorControllerClone,
				AssetKey = assetKey,
				DefaultsProvider = new AacDefaultsProvider(false)
			});
			return aac;
		}

        public bool IsReadyToApply()
        {
            //if (AudioLink)
            //    return true;
            foreach (bUserSettings settings in AllUserSettings.Values)
                if (settings.CurrentPrefab != null)
                    return true;
            return false;
        }

        //public void ResetExtras()
        //{
        //}
    }
}
#endif