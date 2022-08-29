#if UNITY_EDITOR && VRC_SDK_VRCSDK3 && bHapticsOSC_HasAac
using System.Collections.Generic;
using System.Linq;
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

		public static void GetTouchViewColors(float shaderIndex, GameObject obj, ref Color defaultCol, ref Color triggeredCol)
        {
			Renderer renderer = FindRenderersFromIndex(shaderIndex, obj).FirstOrDefault();

			if (renderer == null)
				return;

			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(materialPropertyBlock);

			defaultCol = materialPropertyBlock.GetColor("_DefaultColor");
			triggeredCol = materialPropertyBlock.GetColor("_TouchColor");
		}

		public static void SetTouchViewColors(Renderer renderer, Color defaultCol, Color triggeredCol)
		{
			if (renderer == null)
				return;

			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();

			materialPropertyBlock.SetColor("_DefaultColor", defaultCol);
			materialPropertyBlock.SetColor("_TouchColor", triggeredCol);

			renderer.SetPropertyBlock(materialPropertyBlock);
		}
	}
}
#endif