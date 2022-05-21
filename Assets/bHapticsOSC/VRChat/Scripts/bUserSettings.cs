#if UNITY_EDITOR && VRC_SDK_VRCSDK3 && bHapticsOSC_HasAac
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    [System.Serializable]
    public class bUserSettings : ScriptableObject
    {
        [SerializeField]
        public HumanBodyBones Bone;
        [SerializeField]
        public bool ApplyParentConstraints = true;
        [SerializeField]
        public GameObject CurrentPrefab;
        [SerializeField]
        public List<string> CustomContactTags = new List<string>();

        [SerializeField]
        public Color TouchView_Default = new Color(0, 0, 0, 0);
        private Color touchView_Default = new Color(0, 0, 0, 0);
        [SerializeField]
        public Color TouchView_Triggered = new Color(0, 1, 1, 0.5f);
        private Color touchView_Triggered = new Color(0, 1, 1, 0.5f);

        [SerializeField]
        private bool _showMesh = true;
        public System.Action<bUserSettings> OnShowMeshChange;
        public bool ShowMesh
        {
            get => _showMesh;
            set
            {
                if (_showMesh == value)
                    return;
                _showMesh = value;
                OnShowMeshChange?.Invoke(this);
            }
        }

        public void FindExistingPrefab(bDeviceTemplate device)
        {
            if (CurrentPrefab != null)
                return;
            foreach (GameObject obj in (GameObject[])FindObjectsOfType(typeof(GameObject)))
            {
                if (!PrefabUtility.IsPartOfAnyPrefab(obj))
                    continue;

                Object objPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj);
                if ((objPrefab != device.Prefab) && (objPrefab != device.PrefabMesh))
                    continue;

                _showMesh = (objPrefab == device.PrefabMesh);
                //if (_showMesh)
                //    bShader.GetTouchViewColors(device.ShaderIndex, obj, ref TouchView_Default, ref TouchView_Triggered);

                CurrentPrefab = obj;
                CustomContactTags.Clear();
                bContacts.ScanForExistingTags(this);

                break;
            }
        }

        public void SwapPrefabs(Animator animator, GameObject newPrefab, bool resetTransform = false)
        {
            if (CurrentPrefab != null)
                Undo.RecordObject(CurrentPrefab, $"[{bHapticsOSCIntegration.SystemName}] Swapped Prefabs");

            Transform parent = animator.GetBoneTransform(Bone);
            GameObject spawnedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);

            Undo.RegisterCreatedObjectUndo(spawnedPrefab, $"[{bHapticsOSCIntegration.SystemName}] Swapped Prefabs");
            Undo.SetTransformParent(spawnedPrefab.transform, parent, $"[{bHapticsOSCIntegration.SystemName}] Swapped Prefabs");

            GameObject baseObj = newPrefab;
            if (!resetTransform && (CurrentPrefab != null))
                baseObj = CurrentPrefab;

            spawnedPrefab.transform.localPosition = baseObj.transform.localPosition;
            spawnedPrefab.transform.localEulerAngles = baseObj.transform.localEulerAngles;
            spawnedPrefab.transform.localScale = baseObj.transform.localScale;

            string[] currentTags = CustomContactTags.ToArray();

            Color currentTouchViewDefault = TouchView_Default;
            Color currentTouchViewTriggered = TouchView_Triggered;

            if (CurrentPrefab != null)
                Undo.DestroyObjectImmediate(CurrentPrefab);

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

            CustomContactTags.Clear();
            CustomContactTags.AddRange(currentTags);

            TouchView_Default = currentTouchViewDefault;
            TouchView_Triggered = currentTouchViewTriggered;

            CurrentPrefab = spawnedPrefab;
        }

        public void SelectCurrentPrefab()
            => Selection.activeGameObject = CurrentPrefab;

        public void DestroyCurrentPrefab()
        {
            if (CurrentPrefab == null)
                return;
            Undo.DestroyObjectImmediate(CurrentPrefab);
            CurrentPrefab = null;
            CustomContactTags.Clear();
            TouchView_Default = touchView_Default;
            TouchView_Triggered = touchView_Triggered;
        }

        public void Reset()
        {
            DestroyCurrentPrefab();
            _showMesh = false;
            ShowMesh = true;
            ApplyParentConstraints = true;
            CustomContactTags.Clear();
            TouchView_Default = touchView_Default;
            TouchView_Triggered = touchView_Triggered;
        }
    }
}
#endif