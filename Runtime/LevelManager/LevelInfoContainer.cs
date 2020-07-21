namespace com.faith.gameplay
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LevelInfo", menuName = "FAITH/Gameplay/Create LevelInfo", order = 1)]
    public class LevelInfoContainer : ScriptableObject
    {
        #region Custom Variables

        [System.Serializable]
        public struct LevelInfo
        {
            public int numberOfColorGun;
            public float durationOfEachRound;
        }

        #endregion

        #region Public Variables

        public List<LevelInfo> levelInfos;

        #endregion

        #region Public Callback

#if UNITY_EDITOR

        public void ResetInfos()
        {
            levelInfos = new List<LevelInfo>();
        }

        public void GenerateLevelInfo(
                int t_NumberOfColorGun,
                float t_DurationOfEachRound
            )
        {

            levelInfos.Add(new LevelInfo()
            {
                numberOfColorGun = t_NumberOfColorGun,
                durationOfEachRound = t_DurationOfEachRound
            });

        }

#endif

        public bool IsValidLevel(int t_Level)
        {

            if (t_Level >= 0 && t_Level < levelInfos.Count)
            {
                return true;
            }
            else
            {
                Debug.LogError("Invalid Level Index : " + t_Level);
                return false;
            }
        }


        public int GetNumberOfColorGun(int t_Level)
        {

            if (IsValidLevel(t_Level))
            {
                return levelInfos[t_Level].numberOfColorGun;
            }
            else
            {

                return -1;
            }
        }

        public float GetDurationOfEachRound(int t_Level)
        {

            if (IsValidLevel(t_Level))
            {
                return levelInfos[t_Level].durationOfEachRound;
            }
            else
            {

                return -1;
            }
        }

        #endregion
    }

}

