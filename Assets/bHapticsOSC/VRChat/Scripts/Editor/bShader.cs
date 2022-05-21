#if UNITY_EDITOR && VRC_SDK_VRCSDK3 && bHapticsOSC_HasAac
using System.Collections.Generic;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    public static class bShader
    {
		public static Renderer[] FindRenderersFromIndex(float index, GameObject obj)
		{
			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
			if ((renderers == null) || (renderers.Length <= 0))
				return null;

			List<Renderer> output = new List<Renderer>();
			foreach (Renderer renderer in renderers)
			{
				if (renderer == null)
					continue;

				Material[] materials = renderer.sharedMaterials;
				if ((materials == null) || (materials.Length <= 0))
					continue;

				foreach (Material material in materials)
				{
					if ((material == null) || !material.HasProperty("_Device"))
						continue;
					if (material.GetFloat("_Device") == index)
					{
						output.Add(renderer);
						break;
					}
				}
			}

			return output.ToArray();
		}

		public static float GetShaderIndex(this bDeviceType type, int node = 1)
		{
			if (node < 1)
				node = 1;
			if (node > 3)
				node = 3;
			float index = bDevice.AllTemplates[type].ShaderIndex;
			switch (type) 
			{
				case bDeviceType.HAND_RIGHT: 
					return index + (node * 0.1f);
				case bDeviceType.HAND_LEFT: 
					return index + (node * 0.1f);
				default:
					return index;
			};
		}
	}
}
#endif