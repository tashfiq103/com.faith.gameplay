namespace com.faith.gameplay
{
    using UnityEngine;

    //using com.faith.ai;

    public class GeoLocationManager : MonoBehaviour
    {
        #region Custom Variables

        [System.Serializable]
        public struct MapInfo
        {
            public Vector2Int levelRange;

            [Space(5.0f)]
            [Header("Settings   :   UI")]
            public Sprite mapIcon;
            public string mapName;

            [Space(5.0f)]
            [Header("Settings   :   Gameplay")]
            public GameObject mapContainer;
            [Space(2.5f)]
            public AudioClip backgroundMusic;
            [Space(2.5f)]
            //public NPCRoamingManager[] NPCRoamingManagers;

            [Space(5.0f)]
            [Header("Settings   :   GameSpecefic")]
            public GameObject[] activeObjects;
            public Sprite[] spriteContainerForDecyle;
        }

        #endregion

        #region Public Variables

        public MapInfo[] mapInfo;

        #endregion

        #region Configuretion

        private bool IsValidMapIndex(int t_MapIndex)
        {

            if (t_MapIndex >= 0 && t_MapIndex < mapInfo.Length)
            {

                return true;
            }

            Debug.LogError("Invalid MapIndex");

            return false;
        }

        #endregion

        #region Public Callback

        public int GetMapIndexBasedOnLevel(int t_CurrentLevel)
        {

            t_CurrentLevel++;
            int t_NumberOfMap = mapInfo.Length;
            int t_MapIndex = t_NumberOfMap - 1;
            for (int mapIndex = 0; mapIndex < t_NumberOfMap; mapIndex++)
            {

                if (t_CurrentLevel >= mapInfo[mapIndex].levelRange.x && t_CurrentLevel <= mapInfo[mapIndex].levelRange.y)
                {

                    t_MapIndex = mapIndex;
                    break;
                }
            }

            return t_MapIndex;
        }

        public Vector2Int GetLevelRangeForCurrentLevel(int t_CurrentLevel)
        {

            return mapInfo[GetMapIndexBasedOnLevel(t_CurrentLevel)].levelRange;
        }

        public Vector2Int GetLevelRange(int t_MapIndex)
        {

            if (IsValidMapIndex(t_MapIndex))
            {

                return mapInfo[t_MapIndex].levelRange;
            }

            return Vector2Int.zero;
        }

        public Sprite GetMapIconForCurrentLevel(int t_CurrentLevel)
        {

            return mapInfo[GetMapIndexBasedOnLevel(t_CurrentLevel)].mapIcon;
        }

        public Sprite GetMapIcon(int t_MapIndex)
        {

            if (IsValidMapIndex(t_MapIndex))
            {

                return mapInfo[t_MapIndex].mapIcon;
            }

            return null;
        }

        public string GetMapNameForCurrentLevel(int t_CurrentLevel)
        {

            return mapInfo[GetMapIndexBasedOnLevel(t_CurrentLevel)].mapName;
        }

        public string GetMapName(int t_MapIndex)
        {

            if (IsValidMapIndex(t_MapIndex))
            {

                return mapInfo[t_MapIndex].mapName;
            }

            return null;
        }

        public AudioClip GetBackgroundMusicForCurrentLevel(int t_CurrentLevel)
        {

            return mapInfo[GetMapIndexBasedOnLevel(t_CurrentLevel)].backgroundMusic;
        }

        public AudioClip GetBackgroundMusic(int t_MapIndex)
        {

            if (IsValidMapIndex(t_MapIndex))
            {

                return mapInfo[t_MapIndex].backgroundMusic;
            }

            return null;
        }

        public void ActiveObjectForMapForCurrentLevel(int t_CurrentLevel)
        {

            ActiveObjectForMap(GetMapIndexBasedOnLevel(t_CurrentLevel));
        }

        public void ActiveObjectForMap(int t_MapIndex)
        {
            if (IsValidMapIndex(t_MapIndex))
            {

                int t_NumberOfActiveObject = mapInfo[t_MapIndex].activeObjects.Length;
                for (int i = 0; i < t_NumberOfActiveObject; i++)
                {

                    mapInfo[t_MapIndex].activeObjects[i].SetActive(true);
                }
            }
        }

        public void DeactiveObjectForMapForCurrentLevel(int t_CurrentLevel)
        {

            DeactiveObjectForMap(GetMapIndexBasedOnLevel(t_CurrentLevel));
        }

        public void DeactiveObjectForMap(int t_MapIndex)
        {

            if (IsValidMapIndex(t_MapIndex))
            {

                int t_NumberOfActiveObject = mapInfo[t_MapIndex].activeObjects.Length;
                for (int i = 0; i < t_NumberOfActiveObject; i++)
                {

                    mapInfo[t_MapIndex].activeObjects[i].SetActive(false);
                }
            }
        }

        public Sprite[] GetSpriteContainerForDecyleOfCurrentLevel(int t_CurrentLevel)
        {

            return GetSpriteContainerForDecyle(GetMapIndexBasedOnLevel(t_CurrentLevel));
        }

        public Sprite[] GetSpriteContainerForDecyle(int t_MapIndex)
        {

            if (IsValidMapIndex(t_MapIndex))
            {

                return mapInfo[t_MapIndex].spriteContainerForDecyle;
            }

            return null;
        }

        public void ActivateMap(int t_CurrentLevel)
        {

            int t_MapIndex = GetMapIndexBasedOnLevel(t_CurrentLevel);
            //Activating Map
            for (int counter = 0; counter <= t_MapIndex; counter++)
            {

                if (counter != t_MapIndex)
                {

                    if (mapInfo[counter].mapContainer.activeInHierarchy)
                    {
                        mapInfo[counter].mapContainer.SetActive(false);
                        DeactiveObjectForMap(counter);
                    }
                }
                else
                {

                    if (counter == 0)
                    {
                        int t_PreviousMapIndex = mapInfo.Length - 1;

                        if (mapInfo[t_PreviousMapIndex].mapContainer.activeInHierarchy)
                        {
                            mapInfo[t_PreviousMapIndex].mapContainer.SetActive(false);
                            DeactiveObjectForMap(t_PreviousMapIndex);
                        }
                    }

                    if (!mapInfo[t_MapIndex].mapContainer.activeInHierarchy)
                    {
                        mapInfo[t_MapIndex].mapContainer.SetActive(true);
                        ActiveObjectForMap(t_MapIndex);
                        // int t_NumberOfNPCRoamingManager = mapInfo[t_MapIndex].NPCRoamingManagers.Length;
                        // for (int npcRoamingManagerIndex = 0; npcRoamingManagerIndex < t_NumberOfNPCRoamingManager; npcRoamingManagerIndex++)
                        // {
                        //     mapInfo[t_MapIndex].NPCRoamingManagers[npcRoamingManagerIndex].PreProcess();
                        // }
                    }
                }
            }
        }

        public void DeactivateMap(int t_CurrentLevel)
        {
            int t_NextLevelMapIndex = GetMapIndexBasedOnLevel(t_CurrentLevel + 1);
            int t_MapIndex = GetMapIndexBasedOnLevel(t_CurrentLevel);

            if (t_NextLevelMapIndex != t_MapIndex)
            {

                // int t_NumberOfNPCRoamingManager = mapInfo[t_MapIndex].NPCRoamingManagers.Length;
                // for (int npcRoamingManagerIndex = 0; npcRoamingManagerIndex < t_NumberOfNPCRoamingManager; npcRoamingManagerIndex++)
                // {

                //     mapInfo[t_MapIndex].NPCRoamingManagers[npcRoamingManagerIndex].PostProcess(true);
                // }
            }
        }

        #endregion
    }
}

