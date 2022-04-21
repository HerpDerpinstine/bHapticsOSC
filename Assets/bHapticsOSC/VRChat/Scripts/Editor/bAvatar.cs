using UnityEditor;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace bHapticsOSC.VRChat
{
    public static class bAvatar
    {
		public static SerializedProperty FindAnimationLayerProp(SerializedObject avatarObj, VRCAvatarDescriptor.AnimLayerType type)
		{
			SerializedProperty layersArr = null;
			if ((type == VRCAvatarDescriptor.AnimLayerType.Sitting) || (type == VRCAvatarDescriptor.AnimLayerType.TPose) || (type == VRCAvatarDescriptor.AnimLayerType.IKPose))
				layersArr = avatarObj.FindProperty("specialAnimationLayers");
			else
				layersArr = avatarObj.FindProperty("baseAnimationLayers");

			if (layersArr == null)
				return null;

			for (int i = 0; i < layersArr.arraySize; i++)
			{
				SerializedProperty layerProp = layersArr.GetArrayElementAtIndex(i);
				if (layerProp == null)
					continue;

				SerializedProperty typeProp = layerProp.FindPropertyRelative("type");
				if ((typeProp == null) || (typeProp.intValue != (int)type))
					continue;

				return layerProp;
			}

			return null;
		}

		public static void AddParameter(this VRCExpressionParameters expressionParameters, string name, VRCExpressionParameters.ValueType valueType, float defaultValue = 0f, bool saved = false)
		{
			string newParamName = $"{name}_bool";
			if (expressionParameters.FindParameter(newParamName) != null)
				return;

			SerializedObject avatarParametersObj = new SerializedObject(expressionParameters);

			SerializedProperty avatarParameters = avatarParametersObj.FindProperty("parameters");
			avatarParameters.arraySize += 1;
			SerializedProperty item = avatarParameters.GetArrayElementAtIndex(avatarParameters.arraySize - 1);

			SerializedProperty nameProp = item.FindPropertyRelative("name");
			nameProp.stringValue = newParamName;

			SerializedProperty valueTypeProp = item.FindPropertyRelative("valueType");
			valueTypeProp.intValue = (int)valueType;

			SerializedProperty defaultValueProp = item.FindPropertyRelative("defaultValue");
			defaultValueProp.floatValue = defaultValue;

			SerializedProperty savedProp = item.FindPropertyRelative("saved");
			savedProp.boolValue = saved;

			avatarParametersObj.ApplyModifiedProperties();
		}
	}
}
