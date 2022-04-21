using UnityEngine;
using UnityEngine.Animations;

namespace bHapticsOSC.VRChat
{
    public static class bConstraints
    {
		private static readonly HumanBodyBones[] HandBonesLeft = new HumanBodyBones[]
		{
			HumanBodyBones.LeftHand,
			HumanBodyBones.LeftIndexProximal,
			HumanBodyBones.LeftIndexIntermediate,
		};

		private static readonly HumanBodyBones[] HandBonesRight = new HumanBodyBones[]
		{
			HumanBodyBones.RightHand,
			HumanBodyBones.RightIndexProximal,
			HumanBodyBones.RightIndexIntermediate,
		};

		public static void Apply(bHapticsOSCIntegration editorComp)
		{
			if (ShouldApply(editorComp, bDeviceType.HAND_LEFT, out bUserSettings leftHandSettings))
				ApplyContraintSources(editorComp.avatarAnimator, leftHandSettings.CurrentPrefab.GetComponentsInChildren<ParentConstraint>(true), HandBonesLeft);

			if (ShouldApply(editorComp, bDeviceType.HAND_RIGHT, out bUserSettings rightHandSettings))
				ApplyContraintSources(editorComp.avatarAnimator, rightHandSettings.CurrentPrefab.GetComponentsInChildren<ParentConstraint>(true), HandBonesRight);
		}

		public static bool ShouldApply(bHapticsOSCIntegration editorComp, bDeviceType deviceType, out bUserSettings deviceSettings)
        {
			deviceSettings = editorComp.AllUserSettings[bDevice.AllTemplates[deviceType]];
			return ((deviceSettings.CurrentPrefab != null) && deviceSettings.ApplyParentConstraints);
		}

		private static void ApplyContraintSources(this Animator animator, ParentConstraint[] parentConstraints, HumanBodyBones[] bones)
		{
			for (int jointIndex = 0; jointIndex < parentConstraints.Length; ++jointIndex)
			{
				var constraintSource = new ConstraintSource();
				constraintSource.weight = 1f;
				constraintSource.sourceTransform = animator.GetBoneTransform(bones[jointIndex]);
				if (constraintSource.sourceTransform != null)
				{
					if (parentConstraints[jointIndex].sourceCount <= 0)
						parentConstraints[jointIndex].AddSource(constraintSource);
					else
						parentConstraints[jointIndex].SetSource(0, constraintSource);
				}
			}
		}
	}
}
