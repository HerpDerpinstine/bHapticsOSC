#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    [System.Serializable]
    public class bUserSettings : Object
    {
        [SerializeField]
        public HumanBodyBones Bone;
        [SerializeField]
        public bool ApplyParentConstraints = true;
        [SerializeField]
        public GameObject CurrentPrefab;
        //[SerializeField]
        //public string[] CustomContactTags = new string[0];

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
            foreach (GameObject obj in (GameObject[])FindObjectsOfType(typeof(GameObject)))
            {
                if (!PrefabUtility.IsPartOfAnyPrefab(obj))
                    continue;

                Object objPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj);
                if ((objPrefab != device.Prefab) && (objPrefab != device.PrefabMesh))
                    continue;

                _showMesh = (objPrefab == device.PrefabMesh);
                CurrentPrefab = obj;
                break;
            }
        }

        public void SwapPrefabs(Animator animator, GameObject newPrefab)
        {
            Transform parent = animator.GetBoneTransform(Bone);
            GameObject spawnedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);
            spawnedPrefab.transform.SetParent(parent);
            GameObject baseObj = newPrefab;
            if (CurrentPrefab != null)
                baseObj = CurrentPrefab;

            spawnedPrefab.transform.localPosition = baseObj.transform.localPosition;
            spawnedPrefab.transform.localEulerAngles = baseObj.transform.localEulerAngles;
            spawnedPrefab.transform.localScale = baseObj.transform.localScale;

            DestroyCurrentPrefab();
            CurrentPrefab = spawnedPrefab;
        }

        public void SelectCurrentPrefab()
            => Selection.activeGameObject = CurrentPrefab;

        public void DestroyCurrentPrefab()
        {
            if (CurrentPrefab == null)
                return;
            DestroyImmediate(CurrentPrefab);
            CurrentPrefab = null;
        }

        public void Reset()
        {
            DestroyCurrentPrefab();
            OnShowMeshChange?.Invoke(this);
            ShowMesh = true;
            ApplyParentConstraints = true;

            //CustomContactTags.Clear();
        }
    }
}
#endif