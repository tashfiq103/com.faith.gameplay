namespace com.faith.gameplay
{
    using System.Collections.Generic;
    using UnityEngine;

    using com.faith.math;

    public class PreLoadedPrefab : MonoBehaviour
    {
        #region Custom Variables

        [System.Serializable]
        public struct PrefabInfo
        {
            [Range(0f, 1f)]
            public float ratioOfSpawn;
            public GameObject prefabReference;

        }

        [System.Serializable]
        public struct ActivePrefabInfo
        {
            public int t_IndexOfPreloadedPrefabList;
            public GameObject gameObjectReference;
        }

        #endregion

        #region Public Variables

        [Range(1, 1000)]
        public int numberOfPrefabForEachType;
        public PrefabInfo[] prefabInfoReferences;

        #endregion

        #region Private Variables

        private List<GameObject>[] m_ListOfPreloadedPrefab;
        private List<ActivePrefabInfo> m_ListOfActivePreloadedPrefab;
        private PriorityBound[] priorityBoundary;

        #endregion

        #region Mono Behaviour

        private void Awake()
        {

            int t_NumberOfPrefabOnEachType = 0;
            int t_NumberOfPrefab = prefabInfoReferences.Length;

            m_ListOfActivePreloadedPrefab = new List<ActivePrefabInfo>();
            m_ListOfPreloadedPrefab = new List<GameObject>[t_NumberOfPrefab];
            for (int listIndex = 0; listIndex < t_NumberOfPrefab; listIndex++)
            {
                m_ListOfPreloadedPrefab[listIndex] = new List<GameObject>();
            }

            for (int prefabIndex = 0; prefabIndex < t_NumberOfPrefab; prefabIndex++)
            {

                t_NumberOfPrefabOnEachType = (int)(numberOfPrefabForEachType * prefabInfoReferences[prefabIndex].ratioOfSpawn);
                for (int index = 0; index < t_NumberOfPrefabOnEachType; index++)
                {

                    CreateObject(prefabIndex);
                }
            }
        }

        private void Start()
        {
            CalculatePriority();
        }

        #endregion

        #region Configuretion

        private void CalculatePriority()
        {

            int t_NumberOfPrefab = prefabInfoReferences.Length;
            float[] t_SpawnPriority = new float[t_NumberOfPrefab];

            for (int index = 0; index < t_NumberOfPrefab; index++)
            {
                t_SpawnPriority[index] = prefabInfoReferences[index].ratioOfSpawn;
            }

            priorityBoundary = MathFunction.GetPriorityBound(t_SpawnPriority);
        }

        private void CreateObject(int t_PrefabIndex)
        {

            GameObject t_NewPrefab = Instantiate(
                            prefabInfoReferences[t_PrefabIndex].prefabReference,
                            transform.position,
                            Quaternion.identity
                        );

            t_NewPrefab.SetActive(false);
            t_NewPrefab.transform.SetParent(transform);
            t_NewPrefab.transform.localPosition = Vector3.zero;
            t_NewPrefab.transform.localEulerAngles = Vector3.zero;

            m_ListOfPreloadedPrefab[t_PrefabIndex].Add(t_NewPrefab);
        }

        private void Push(GameObject t_ObjectReference)
        {

            int t_NumberOfActivePreloadedPrefab = m_ListOfActivePreloadedPrefab.Count;
            for (int index = 0; index < t_NumberOfActivePreloadedPrefab; index++)
            {

                if (t_ObjectReference == m_ListOfActivePreloadedPrefab[index].gameObjectReference)
                {

                    m_ListOfPreloadedPrefab[m_ListOfActivePreloadedPrefab[index].t_IndexOfPreloadedPrefabList].Add(m_ListOfActivePreloadedPrefab[index].gameObjectReference);
                    m_ListOfActivePreloadedPrefab.RemoveAt(index);
                    m_ListOfActivePreloadedPrefab.TrimExcess();
                    break;
                }
            }
        }

        private GameObject PopSpeceficObject(GameObject t_ObjectRefrence)
        {

            GameObject t_Result = null;

            int t_NumberOfAvailableObject = prefabInfoReferences.Length;
            for (int index = 0; index < t_NumberOfAvailableObject; index++)
            {

                if (prefabInfoReferences[index].prefabReference == t_ObjectRefrence)
                {

                    if (m_ListOfPreloadedPrefab[index].Count == 0)
                        CreateObject(index);

                    t_Result = m_ListOfPreloadedPrefab[index][0];
                    t_Result.SetActive(true);

                    m_ListOfPreloadedPrefab[index].RemoveAt(0);
                    m_ListOfPreloadedPrefab[index].TrimExcess();

                    m_ListOfActivePreloadedPrefab.Add(new ActivePrefabInfo()
                    {
                        t_IndexOfPreloadedPrefabList = index,
                        gameObjectReference = t_Result
                    });

                    break;
                }
            }

            return t_Result;
        }

        private GameObject Pop()
        {

            GameObject t_Result = null;

            float t_Probability = Random.Range(0f, 1f);
            int t_NumberOfPriorityBound = priorityBoundary.Length;
            for (int index = 0; index < t_NumberOfPriorityBound; index++)
            {

                if (t_Probability >= priorityBoundary[index].lowerPriority && t_Probability <= priorityBoundary[index].higherPriority)
                {

                    if (m_ListOfPreloadedPrefab[index].Count == 0)
                        CreateObject(index);

                    t_Result = m_ListOfPreloadedPrefab[index][0];
                    t_Result.SetActive(true);

                    m_ListOfPreloadedPrefab[index].RemoveAt(0);
                    m_ListOfPreloadedPrefab[index].TrimExcess();

                    m_ListOfActivePreloadedPrefab.Add(new ActivePrefabInfo()
                    {
                        t_IndexOfPreloadedPrefabList = index,
                        gameObjectReference = t_Result
                    });

                    break;
                }
            }

            return t_Result;
        }

        #endregion

        #region Public Callback

        public GameObject PullPreloadedPrefab(GameObject t_RequestedObject = null)
        {
            if (t_RequestedObject == null)
                return Pop();

            return PopSpeceficObject(t_RequestedObject);
        }

        public void PushPreloadedPrefab(GameObject t_ObjectReference, bool t_IsDeactivateObject = true)
        {

            if (t_IsDeactivateObject)
                t_ObjectReference.SetActive(false);

            Push(t_ObjectReference);
        }

        #endregion
    }
}


