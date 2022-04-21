﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    public static class bDevice
    {
        public static Dictionary<bDeviceType, bDeviceTemplate> AllTemplates;

        static bDevice()
        {
            AllTemplates = new Dictionary<bDeviceType, bDeviceTemplate>();

            AllTemplates[bDeviceType.HEAD] = new bDeviceTemplate { Name = "Head", HasBone = true, Bone = HumanBodyBones.Head, NodeCount = 6 };

            AllTemplates[bDeviceType.VEST] = new bDeviceTemplate { Name = "Vest", HasBone = true, Bone = HumanBodyBones.Chest };
            AllTemplates[bDeviceType.VEST_FRONT] = new bDeviceTemplate { Name = "Vest Front", NodeCount = 20, ShaderIndex = 1.1f };
            AllTemplates[bDeviceType.VEST_BACK] = new bDeviceTemplate { Name = "Vest Back", NodeCount = 20, ShaderIndex = 1.2f };

            AllTemplates[bDeviceType.ARM_LEFT] = new bDeviceTemplate { Name = "Arm Left", HasBone = true, Bone = HumanBodyBones.LeftLowerArm, NodeCount = 6, ShaderIndex = 2f };
            AllTemplates[bDeviceType.ARM_RIGHT] = new bDeviceTemplate { Name = "Arm Right", HasBone = true, Bone = HumanBodyBones.RightLowerArm, NodeCount = 6, ShaderIndex = 3f };

            AllTemplates[bDeviceType.HAND_LEFT] = new bDeviceTemplate { Name = "Hand Left", HasBone = true, Bone = HumanBodyBones.LeftHand, NodeCount = 3, ShaderIndex = 4f, HasParentConstraints = true };
            AllTemplates[bDeviceType.HAND_RIGHT] = new bDeviceTemplate { Name = "Hand Right", HasBone = true, Bone = HumanBodyBones.RightHand, NodeCount = 3, ShaderIndex = 5f, HasParentConstraints = true };

            // Gloves

            AllTemplates[bDeviceType.FOOT_LEFT] = new bDeviceTemplate { Name = "Foot Left", HasBone = true, Bone = HumanBodyBones.LeftFoot, NodeCount = 3, ShaderIndex = 8f };
            AllTemplates[bDeviceType.FOOT_RIGHT] = new bDeviceTemplate { Name = "Foot Right", HasBone = true, Bone = HumanBodyBones.RightFoot, NodeCount = 3, ShaderIndex = 9f };

            foreach (bDeviceTemplate settings in AllTemplates.Values)
            {
                string nameWithoutSpaces = settings.Name.Replace(" ", "");
                settings.Prefab = (GameObject)EditorGUIUtility.Load($"Assets/bHapticsOSC/VRChat/Prefabs/Without Mesh/{nameWithoutSpaces}.prefab");
                settings.PrefabMesh = (GameObject)EditorGUIUtility.Load($"Assets/bHapticsOSC/VRChat/Prefabs/With Mesh/{nameWithoutSpaces}.prefab");
            }
        }
    }
}