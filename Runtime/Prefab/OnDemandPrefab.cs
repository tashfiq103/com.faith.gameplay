namespace com.faith.gameplay
{
    using System.Collections.Generic;
    using UnityEngine;
    using com.faith.core;

    [System.Serializable]
    public struct Item
    {

#if UNITY_EDITOR
        [HideInInspector]
        public string name;
#endif

        public GameObject obstaclePrefab;
        [Range(0, 1000)]
        public int requiredLevel;
        [Range(0.0f, 1.0f)]
        public float spawnProbability;
    }

    public class OnDemandPrefab : MonoBehaviour
    {

        public static OnDemandPrefab Instance;

        //----------
        #region Public Variables

        public bool spawnOnExactLevel;
        public Item[] items;

        #endregion

        //----------
        #region Configuretion		:	Class

        void Awake()
        {

            Instance = this;
        }

        #endregion

        //----------
        #region  Public Callback	:	Class

        public GameObject GetObject(List<GameObject> t_RequestedObject)
        {
            GameObject t_Result = null;

            //Using GameplayManager
            //int t_DifficultyLevel = GameplayManager.Instance.GetCurrentDifficultyLevel();

            int t_DifficultyLevel = 0; // NEED TO FIX

            List<int> t_ListOfObstacleCanBeSpawned = new List<int>();
            for (int obstacleIndex = 0; obstacleIndex < items.Length; obstacleIndex++)
            {

                if (t_RequestedObject != null)
                {

                    if (t_RequestedObject.Contains(items[obstacleIndex].obstaclePrefab) &&
                        (spawnOnExactLevel ? items[obstacleIndex].requiredLevel == t_DifficultyLevel : items[obstacleIndex].requiredLevel <= t_DifficultyLevel))
                    {
                        //Debug.Log ("Found : " + items[obstacleIndex].obstaclePrefab.name);
                        t_ListOfObstacleCanBeSpawned.Add(obstacleIndex);
                    }
                }
                else
                {

                    if (spawnOnExactLevel ? items[obstacleIndex].requiredLevel == t_DifficultyLevel : items[obstacleIndex].requiredLevel <= t_DifficultyLevel)
                    {
                        //Debug.Log ("Found : " + items[obstacleIndex].obstaclePrefab.name);
                        t_ListOfObstacleCanBeSpawned.Add(obstacleIndex);
                    }
                }
            }

            if (t_ListOfObstacleCanBeSpawned.Count == 1)
            {
                t_Result = items[t_ListOfObstacleCanBeSpawned[0]].obstaclePrefab;
            }
            else
            {
                float[] t_SpawnPriority = new float[t_ListOfObstacleCanBeSpawned.Count];
                for (int obstacleIndex = 0; obstacleIndex < t_SpawnPriority.Length; obstacleIndex++)
                {
                    t_SpawnPriority[obstacleIndex] = items[t_ListOfObstacleCanBeSpawned[obstacleIndex]].spawnProbability;

                }
                PriorityBound[] t_AbsoluteSpawnPriority = MathFunction.GetPriorityBound(t_SpawnPriority);

                float t_SelectedProbability = Random.Range(0.0f, 1.0f);

                for (int shortListIndex = items.Length - 1; shortListIndex >= 0; shortListIndex--)
                {

                    if (t_ListOfObstacleCanBeSpawned.Contains(shortListIndex))
                    {

                        int t_OriginIndex = t_ListOfObstacleCanBeSpawned.IndexOf(shortListIndex);
                        if (t_SelectedProbability >= t_AbsoluteSpawnPriority[t_OriginIndex].lowerPriority &&
                            t_SelectedProbability < t_AbsoluteSpawnPriority[t_OriginIndex].higherPriority)
                        {

                            t_Result = items[t_OriginIndex].obstaclePrefab;
                            break;
                        }
                    }
                }
            }

            return t_Result;
        }

        #endregion
    }
}

