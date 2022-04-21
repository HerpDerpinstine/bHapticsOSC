using AnimatorAsCode.V0;
using System.Collections.Generic;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    public static class bAnimator
    {
		public static void CreateAllNodes(bHapticsOSCIntegration editorComp)
        {
			AacFlBase aac = editorComp.CreateAnimatorAsCode();

			foreach (KeyValuePair<bDeviceType, bDeviceTemplate> keyValuePair in bDevice.AllTemplates)
            {
				if (keyValuePair.Value.NodeCount <= 0)
					continue;

				bUserSettings userSettings = null;
				if ((keyValuePair.Key == bDeviceType.VEST_FRONT) || (keyValuePair.Key == bDeviceType.VEST_BACK))
					userSettings = editorComp.AllUserSettings[bDevice.AllTemplates[bDeviceType.VEST]];
				else
					userSettings = editorComp.AllUserSettings[keyValuePair.Value];
				if (userSettings.CurrentPrefab == null)
					continue;

				for (int node = 1; node < keyValuePair.Value.NodeCount + 1; node++)
				{
					string nodeName = $"{bHapticsOSCIntegration.SystemName}_{keyValuePair.Value.Name.Replace(' ', '_')}_{node}";
					CreateAnimatorLayerStates(node, nodeName, aac, editorComp, keyValuePair);
				}
			}
		}

		private static void CreateAnimatorLayerStates(int node, string nodeName, AacFlBase aac, bHapticsOSCIntegration editorComp, KeyValuePair<bDeviceType, bDeviceTemplate> keyValuePair)
		{
			float shaderDeviceIndex = bShader.GetShaderIndex(keyValuePair.Key, node);
			Renderer[] renderers = bShader.FindRenderersFromIndex(shaderDeviceIndex, editorComp.avatar.gameObject);
			if (renderers.Length <= 0)
				return;

			string layer_name = $"{keyValuePair.Value.Name.Replace(' ', '_')}_{node}";

			aac.RemoveAllSupportingLayers(layer_name);
			AacFlLayer layer = aac.CreateSupportingFxLayer(layer_name);

			AacFlBoolParameter boolParam = layer.BoolParameter(nodeName);

			AacFlState falseState = layer.NewState("False").WithWriteDefaultsSetTo(false);
			falseState.TransitionsFromEntry().When(boolParam.IsFalse());
			falseState.Exits().AfterAnimationFinishes();

			AacFlState trueState = layer.NewState("True", 1, 0).WithWriteDefaultsSetTo(false);
			trueState.TransitionsFromEntry().When(boolParam.IsTrue());
			trueState.Exits().AfterAnimationFinishes();

			foreach (Renderer renderer in renderers)
			{
				AacFlClip falseClip = aac.NewClip().Animating(clip => clip.Animates(renderer, $"material._Node{node}").WithOneFrame(0f));
				falseState = falseState.WithAnimation(falseClip);

				AacFlClip trueClip = aac.NewClip().Animating(clip => clip.Animates(renderer, $"material._Node{node}").WithOneFrame(1f));
				trueState = trueState.WithAnimation(trueClip);
			}
		}
	}
}
