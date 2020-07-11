namespace com.faith.gameplay
{
    using UnityEngine;

    public class LevelManager : MonoBehaviour
    {

        #region Public Variables

#if UNITY_EDITOR

        [HideInInspector]
        public bool overridePreviousData;

#endif

        public LevelInfoContainer levelInfoContainer;

        [Space(5.0f)]
        [Header("Configuretion : Level Design")]
        [Range(1, 1000)]
        public int maxLevel = 1;


        [Space(5.0f)]
        [Header("Configuretion  :   NumberOfColorGun")]
        public Vector2Int numberOfColorGun;
        public AnimationCurve curveForNumberOfColorGun;

        [Space(5.0f)]
        [Header("Configuretion  :   DurationOfEachRound")]
        public Vector2Int durationOfEachRound;
        public AnimationCurve curveForDurationOfEachRound;

        #endregion

        #region Private Variables

        private string PP_LEVEL_TRACKER = "PP_LEVEL_TRACKER";

        #endregion

        #region Mono Behaviour

        #endregion

        #region Public Callback :   Editor

#if UNITY_EDITOR

        public int GetNumberOfColorGun(int t_Level)
        {

            float t_TempGameProgression = t_Level / ((float)maxLevel);
            int t_NumberOfAITowCar = (int)(numberOfColorGun.x + ((numberOfColorGun.y - numberOfColorGun.x) * curveForNumberOfColorGun.Evaluate(t_TempGameProgression)));
            return t_NumberOfAITowCar;
        }

        public float GetDurationOfEachRound(int t_Level)
        {

            float t_TempGameProgression = t_Level / ((float)maxLevel);
            float t_DurationOfEachRound = (durationOfEachRound.x + ((durationOfEachRound.y - durationOfEachRound.x) * curveForDurationOfEachRound.Evaluate(t_TempGameProgression)));
            return t_DurationOfEachRound;
        }

#endif

        #endregion

        #region Public Callback :   Level

        public bool IsValidLevel(int t_Level)
        {

            if (t_Level >= 0 && t_Level < maxLevel)
            {

                return true;
            }
            else
            {

                return false;
            }
        }

        public int GetCurrentLevel()
        {

            return PlayerPrefs.GetInt(PP_LEVEL_TRACKER, 0);
        }

        public void IncreaseLevel(int t_IncrementValue = 1)
        {

            int t_CurrentLevel = GetCurrentLevel();

#if UNITY_IOS
            //FacebookAnalyticsManager.Instance.FBALevelComplete(t_CurrentLevel);
            //FirebaseAnalyticsEventController.Instance.UpdateGameProgression("Level Achieved", t_CurrentLevel);
#endif

            int t_NewLevel = t_CurrentLevel + t_IncrementValue;
            if (t_NewLevel >= maxLevel)
            {
                t_NewLevel = 0;
            }

            PlayerPrefs.SetInt(PP_LEVEL_TRACKER, t_NewLevel);
            //UIStateController.Instance.UpdateLevelInfo(t_NewLevel);
            //UIRateUsController.Instance.AskUserForReview();
        }

        public void DecreaseLevel(int t_DecrementValue = 1)
        {

            int t_NewLevel = GetCurrentLevel() - t_DecrementValue;
            if (t_NewLevel < 0)
            {
                PlayerPrefs.SetInt(PP_LEVEL_TRACKER, maxLevel - 1);
            }
            else
            {

                PlayerPrefs.SetInt(PP_LEVEL_TRACKER, t_NewLevel);
            }
        }

        public float GetLevelProgression()
        {
            return (GetCurrentLevel() / ((float)maxLevel));
        }

        public void ResetLevel()
        {
            PlayerPrefs.SetInt(PP_LEVEL_TRACKER, 0);
        }

        public int GetNumberOfColorGunForCurrentLevel()
        {

            return levelInfoContainer.GetNumberOfColorGun(GetCurrentLevel());
        }

        public float GetDurationOfEachRoundForCurrentLevel()
        {

            return levelInfoContainer.GetDurationOfEachRound(GetCurrentLevel());
        }

        #endregion
    }

}

