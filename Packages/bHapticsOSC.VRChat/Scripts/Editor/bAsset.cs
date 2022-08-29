#if UNITY_EDITOR && VRC_SDK_VRCSDK3 && bHapticsOSC_HasAac
using System.IO;
using UnityEditor;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    public static class bAsset
    {
		public static T Clone<T>(T assetObject) where T : Object
		{
			string asset_path = AssetDatabase.GetAssetPath(assetObject);
			string folderPath = Path.GetDirectoryName(asset_path);

			string fileName = Path.GetFileNameWithoutExtension(asset_path);
			string ext = Path.GetExtension(asset_path);

			if (fileName.EndsWith(bHapticsOSCIntegration.SystemName))
            {
				string originalFileName = Path.GetFileNameWithoutExtension(fileName);
				string originalFilePath = Path.Combine(folderPath, $"{originalFileName}{ext}");
				if (File.Exists(originalFilePath))
				{
					asset_path = originalFilePath;
					fileName = originalFileName;
				}
			}

			string newPath = Path.Combine(folderPath, $"{fileName}.{bHapticsOSCIntegration.SystemName}{ext}");
			AssetDatabase.DeleteAsset(newPath);
			if (!AssetDatabase.CopyAsset(asset_path, newPath))
				return null;

			T obj = AssetDatabase.LoadAssetAtPath<T>(newPath);
			AssetDatabase.ForceReserializeAssets(new[] { newPath });
			AssetDatabase.Refresh();
			return obj;
		}
	}
}
#endif