
namespace com.faith.gameplay
{
    using System.Collections.Generic;
    using UnityEngine;

    public class QuestManager : MonoBehaviour
    {

        public static QuestManager Instance;

        #region Custom Variables

        [System.Serializable]
        public struct Quest
        {

            public string questID;

            [Range(0, 1000)]
            public int requiredLevel;

            [Space(5.0f)]
            [Range(0, 1000)]
            public int requiredNumberOfAchievement;

            [Space(5.0f)]
            [Range(0, 100000)]
            public int questReward;
            [Range(1, 5)]
            public int maximumSubQuestLevel;

            [Space(5.0f)]
            [Range(0f, 3f)]
            public float achievementScalingThroughLevel;
            [Range(1, 3)]
            public float questRewardScalingThroughLevel;
        }

        #endregion

#if UNITY_EDITOR

        public bool ed_ShowQuest;

#endif


        #region Public Variables

        [Space(15f)]
        [Range(0, 1000)]
        public int requiredLevelForQuest;
        [Range(0, 10)]
        public int maximumNumberOfActiveQuest;

        [Space(5.0f)]
        [Header("Quests")]
        public Quest[] quests;

        #endregion

        #region Private Variables

        private string QUEST_INFO = "QUEST_INFO_";

        private string NUMBER_OF_ACTIVE_QUEST = "NUMBER_OF_ACTIVE_QUEST";

        private string NUMBER_OF_SUBQUEST = "NUMBER_OF_SUBQUEST";
        private string SUB_QUEST_PROGRESSION = "SUB_QUEST_PROGRESSION_";
        private string SUB_QUEST_ACHIEVEMENT = "SUB_QUEST_ACHIEVEMENT_";
        private string SUB_QUEST_REWARD = "SUB_QUEST_REWARD";

        #endregion



        #region Mono Behaviour

        void Awake()
        {
            if (Instance == null)
            {

                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {

                Destroy(gameObject);
            }
        }

        #endregion

        #region Configuretion

        private string FormatedQuestID(int t_QuestID)
        {

            return "(" + t_QuestID + ")_";
        }

        private void SetNumberOfActiveQuest(int t_NewNumberOfActiveQuest)
        {

            PlayerPrefs.SetInt(NUMBER_OF_ACTIVE_QUEST, t_NewNumberOfActiveQuest);
        }

        private void UpdateProgression(int t_QuestIndex, int t_SubQuestIndex, int t_CurrentProgression)
        {

            PlayerPrefs.SetInt(SUB_QUEST_PROGRESSION + FormatedQuestID(t_QuestIndex) + FormatedQuestID(t_SubQuestIndex) + "TRACKER", t_CurrentProgression + 1);

            if (GetCurrentProgressionOfSubQuest(t_QuestIndex, t_SubQuestIndex) >= GetAchievementRequiredOfSubQuest(t_QuestIndex, t_SubQuestIndex))
            {

                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(t_QuestIndex) + FormatedQuestID(t_SubQuestIndex) + "IsComplete", 1);

                if (IsAllSubQuestComplete(t_QuestIndex))
                {

                    PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(t_QuestIndex) + "IsComplete", 1);

                    int t_CurrentSizeOfActiveQuest = GetNumberOfActiveQuest();

                    if (t_CurrentSizeOfActiveQuest > 1)
                    {

                        for (int index = t_QuestIndex; index < t_CurrentSizeOfActiveQuest; index++)
                        {

                            if (index == (t_CurrentSizeOfActiveQuest - 1))
                            {
                                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(index) + "EditorQuestIndex", 0);
                                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(index) + "IsComplete", 0);

                                int t_NumberOfActiveSubQuest = GetNumberOfSubQuest(index);

                                for (int subQuestIndex = 0; subQuestIndex < t_NumberOfActiveSubQuest; subQuestIndex++)
                                {

                                    PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(index) + FormatedQuestID(subQuestIndex) + "IsComplete", 0);
                                    PlayerPrefs.SetInt(SUB_QUEST_PROGRESSION + FormatedQuestID(index) + FormatedQuestID(subQuestIndex) + "TRACKER", 0);
                                    PlayerPrefs.SetInt(SUB_QUEST_ACHIEVEMENT + FormatedQuestID(index) + FormatedQuestID(subQuestIndex) + "TRACKER", 0);
                                }

                                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(index) + NUMBER_OF_SUBQUEST, 0);
                            }
                            else
                            {

                                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(index) + "EditorQuestIndex", PlayerPrefs.GetInt(QUEST_INFO + FormatedQuestID(index + 1) + "EditorQuestIndex"));
                                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(index) + "IsComplete", PlayerPrefs.GetInt(QUEST_INFO + FormatedQuestID(index + 1) + "IsComplete"));

                                int t_NumberOfActiveSubQuest = GetNumberOfSubQuest(index + 1);

                                for (int subQuestIndex = 0; subQuestIndex < t_NumberOfActiveSubQuest; subQuestIndex++)
                                {

                                    PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(index) + FormatedQuestID(subQuestIndex) + "IsComplete", PlayerPrefs.GetInt(QUEST_INFO + FormatedQuestID(index + 1) + FormatedQuestID(subQuestIndex) + "IsComplete"));
                                    PlayerPrefs.SetInt(SUB_QUEST_PROGRESSION + FormatedQuestID(index) + FormatedQuestID(subQuestIndex) + "TRACKER", PlayerPrefs.GetInt(SUB_QUEST_PROGRESSION + FormatedQuestID(index + 1) + FormatedQuestID(subQuestIndex) + "TRACKER"));
                                    PlayerPrefs.SetInt(SUB_QUEST_ACHIEVEMENT + FormatedQuestID(index) + FormatedQuestID(subQuestIndex) + "TRACKER", PlayerPrefs.GetInt(SUB_QUEST_ACHIEVEMENT + FormatedQuestID(index + 1) + FormatedQuestID(subQuestIndex) + "TRACKER"));
                                }

                                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(index) + NUMBER_OF_SUBQUEST, PlayerPrefs.GetInt(QUEST_INFO + FormatedQuestID(index + 1) + NUMBER_OF_SUBQUEST));
                            }
                        }
                    }

                    SetNumberOfActiveQuest(t_CurrentSizeOfActiveQuest - 1);
                }
            }
        }

        private bool IsQuestAlreadyPicked(int t_QuestIndex)
        {
            return false;
            int t_NumberOfActiveQuest = GetNumberOfActiveQuest();
            for (int questIndex = 0; questIndex < t_NumberOfActiveQuest; questIndex++)
            {

            }
        }

        #endregion

        #region Public Callback     :   Get Info

        public int GetEditorQuestIndex(int t_QuestIndex)
        {
            return PlayerPrefs.GetInt(QUEST_INFO + FormatedQuestID(t_QuestIndex) + "EditorQuestIndex");
        }

        public int GetSubQuestId(int t_QuestIndex)
        {

            int t_NumberOfActiveSubQuest = GetNumberOfSubQuest(t_QuestIndex);

            for (int subQuestIndex = 0; subQuestIndex < t_NumberOfActiveSubQuest; subQuestIndex++)
            {

                if (!IsSubQuestComplete(t_QuestIndex, subQuestIndex))
                {

                    return subQuestIndex;
                }
            }

            return 0;
        }

        public int GetNumberOfActiveQuest()
        {

            return PlayerPrefs.GetInt(NUMBER_OF_ACTIVE_QUEST, 0);
        }

        public int GetNumberOfSubQuest(int t_QuestIndex)
        {

            return PlayerPrefs.GetInt(QUEST_INFO + FormatedQuestID(t_QuestIndex) + NUMBER_OF_SUBQUEST, 0);
        }

        public int GetCurrentProgressionOfQuest(int t_QuestId)
        {

            int t_NumberOfActiveSubQuest = GetNumberOfSubQuest(t_QuestId);

            for (int subQuestIndex = 0; subQuestIndex < t_NumberOfActiveSubQuest; subQuestIndex++)
            {

                if (!IsSubQuestComplete(t_QuestId, subQuestIndex))
                {

                    return GetCurrentProgressionOfSubQuest(t_QuestId, subQuestIndex);
                }
            }

            return 0;
        }

        public int GetCurrentProgressionOfSubQuest(int t_QuestIndex, int t_SubQuestIndex)
        {

            return PlayerPrefs.GetInt(SUB_QUEST_PROGRESSION + FormatedQuestID(t_QuestIndex) + FormatedQuestID(t_SubQuestIndex) + "TRACKER");
        }

        public int GetAchievementRequiredForQuest(int t_QuestIndex)
        {

            int t_NumberOfActiveSubQuest = GetNumberOfSubQuest(t_QuestIndex);

            for (int subQuestIndex = 0; subQuestIndex < t_NumberOfActiveSubQuest; subQuestIndex++)
            {

                if (!IsSubQuestComplete(t_QuestIndex, subQuestIndex))
                {

                    return GetAchievementRequiredOfSubQuest(t_QuestIndex, subQuestIndex);
                }
            }

            return 0;
        }

        public int GetAchievementRequiredOfSubQuest(int t_QuestIndex, int t_SubQuestIndex)
        {

            return PlayerPrefs.GetInt(SUB_QUEST_ACHIEVEMENT + FormatedQuestID(t_QuestIndex) + FormatedQuestID(t_SubQuestIndex) + "TRACKER", 0);
        }

        public bool IsSubQuestComplete(int t_QuestIndex, int t_SubQuestIndex)
        {

            if (PlayerPrefs.GetInt(QUEST_INFO + FormatedQuestID(t_QuestIndex) + FormatedQuestID(t_SubQuestIndex) + "IsComplete") == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetRewardAmountForQuest(int t_QuestIndex)
        {

            int t_NumberOfActiveSubQuest = GetNumberOfSubQuest(t_QuestIndex);

            for (int subQuestIndex = 0; subQuestIndex < t_NumberOfActiveSubQuest; subQuestIndex++)
            {

                if (!IsSubQuestComplete(t_QuestIndex, subQuestIndex))
                {

                    return GetRewardAmountForSubQuest(t_QuestIndex, subQuestIndex);
                }
            }

            return 0;
        }

        public int GetRewardAmountForSubQuest(int t_QuestIndex, int t_SubQuestIndex)
        {
            return PlayerPrefs.GetInt(SUB_QUEST_REWARD + FormatedQuestID(t_QuestIndex) + FormatedQuestID(t_SubQuestIndex) + "AMOUNT", 0);
        }

        public bool IsAllSubQuestComplete(int t_QuestIndex)
        {

            int t_CompletedQuestCounter = 0;
            int t_NumberOfSubQuest = GetNumberOfSubQuest(t_QuestIndex);
            for (int questIndex = 0; questIndex < t_NumberOfSubQuest; questIndex++)
            {

                if (IsSubQuestComplete(t_QuestIndex, questIndex))
                {
                    t_CompletedQuestCounter++;
                }
            }

            if (t_CompletedQuestCounter == t_NumberOfSubQuest)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsQuestComplete(int t_QuestIndex)
        {

            if (PlayerPrefs.GetInt(QUEST_INFO + FormatedQuestID(t_QuestIndex) + "IsComplete") == 1)
            {
                return true;
            }
            else
            {

                return false;
            }
        }

        public bool IsAllQuestsComplete()
        {

            int t_CompletedQuestCounter = 0;
            for (int questIndex = 0; questIndex < maximumNumberOfActiveQuest; questIndex++)
            {

                if (IsQuestComplete(questIndex))
                {
                    t_CompletedQuestCounter++;
                }
            }

            if (t_CompletedQuestCounter == maximumNumberOfActiveQuest)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion

        #region Public Callback     :   Execution



        public void ResetAllQuest()
        {
            int t_NumberOfActiveQuest = GetNumberOfActiveQuest();

            for (int questIndex = 0; questIndex < t_NumberOfActiveQuest; questIndex++)
            {

                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(questIndex) + "EditorQuestIndex", 0);
                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(questIndex) + "IsComplete", 0);

                int t_NumberOfActiveSubQuest = GetNumberOfSubQuest(questIndex);

                for (int subQuestIndex = 0; subQuestIndex < t_NumberOfActiveSubQuest; subQuestIndex++)
                {

                    PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(questIndex) + FormatedQuestID(subQuestIndex) + "IsComplete", 0);
                    PlayerPrefs.SetInt(SUB_QUEST_PROGRESSION + FormatedQuestID(questIndex) + FormatedQuestID(subQuestIndex) + "TRACKER", 0);
                    PlayerPrefs.SetInt(SUB_QUEST_ACHIEVEMENT + FormatedQuestID(questIndex) + FormatedQuestID(subQuestIndex) + "TRACKER", 0);
                    PlayerPrefs.SetInt(SUB_QUEST_REWARD + FormatedQuestID(questIndex) + FormatedQuestID(subQuestIndex) + "AMOUNT", 0);

                }

                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(questIndex) + NUMBER_OF_SUBQUEST, 0);
            }

            SetNumberOfActiveQuest(0);
        }

        public void CreateQuest(int t_CurrentLevel, int t_RequestedReward = 0)
        {

            if (GetNumberOfActiveQuest() < maximumNumberOfActiveQuest)
            {


                List<int> t_IndexListOfAvailableQuest = new List<int>();
                for (int questIndex = 0; questIndex < quests.Length; questIndex++)
                {

                    if (quests[questIndex].requiredLevel <= t_CurrentLevel)
                    {

                        t_IndexListOfAvailableQuest.Add(questIndex);
                    }
                }

                int t_SelectedQuestIndex = t_IndexListOfAvailableQuest[Random.Range(0, t_IndexListOfAvailableQuest.Count)];
                int t_OrderIndex = GetNumberOfActiveQuest();

                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(t_OrderIndex) + "EditorQuestIndex", t_SelectedQuestIndex);
                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(t_OrderIndex) + "IsComplete", 0);

                int t_NumberOfSubQuest = Random.Range(1, quests[t_SelectedQuestIndex].maximumSubQuestLevel);
                PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(t_OrderIndex) + NUMBER_OF_SUBQUEST, t_NumberOfSubQuest);
                for (int subQuestIndex = 0; subQuestIndex < t_NumberOfSubQuest; subQuestIndex++)
                {

                    PlayerPrefs.SetInt(QUEST_INFO + FormatedQuestID(t_OrderIndex) + FormatedQuestID(subQuestIndex) + "IsComplete", 0);
                    PlayerPrefs.SetInt(SUB_QUEST_PROGRESSION + FormatedQuestID(t_OrderIndex) + FormatedQuestID(subQuestIndex) + "TRACKER", 0);

                    int t_RequiredAchievement = quests[t_SelectedQuestIndex].requiredNumberOfAchievement;
                    t_RequiredAchievement += Mathf.CeilToInt((t_RequiredAchievement) * quests[t_SelectedQuestIndex].achievementScalingThroughLevel * subQuestIndex);
                    Debug.Log("Required Achievement : " + t_RequiredAchievement);

                    PlayerPrefs.SetInt(SUB_QUEST_ACHIEVEMENT + FormatedQuestID(t_OrderIndex) + FormatedQuestID(subQuestIndex) + "TRACKER", t_RequiredAchievement);

                    if (t_RequestedReward == 0)
                    {
                        int t_Reward = quests[t_SelectedQuestIndex].questReward;
                        t_Reward += Mathf.CeilToInt((t_Reward) * quests[t_SelectedQuestIndex].questRewardScalingThroughLevel * subQuestIndex);

                        PlayerPrefs.SetInt(SUB_QUEST_REWARD + FormatedQuestID(t_OrderIndex) + FormatedQuestID(subQuestIndex) + "AMOUNT", t_Reward);
                    }
                    else
                    {

                        PlayerPrefs.SetInt(SUB_QUEST_REWARD + FormatedQuestID(t_OrderIndex) + FormatedQuestID(subQuestIndex) + "AMOUNT", t_RequiredAchievement);
                    }
                }

                SetNumberOfActiveQuest(t_OrderIndex + 1);
            }
            else
            {

                Debug.LogWarning("Maximum Limit Reached : For creating quest");
            }
        }

        public void UpdateQuestProgress(string t_QuestId)
        {

            int t_NumberOfActiveQuest = GetNumberOfActiveQuest();
            int t_QuestIndex;
            for (int questIndex = 0; questIndex < t_NumberOfActiveQuest; questIndex++)
            {
                t_QuestIndex = GetEditorQuestIndex(questIndex);
                if (quests[t_QuestIndex].questID == t_QuestId)
                {

                    UpdateQuestProgress(t_QuestIndex);
                    break;
                }
            }
        }

        public void UpdateQuestProgress(int t_QuestId)
        {

            int t_NumberOfActiveSubQuest = GetNumberOfSubQuest(t_QuestId);
            for (int subQuestIndex = 0; subQuestIndex < t_NumberOfActiveSubQuest; subQuestIndex++)
            {

                if (!IsSubQuestComplete(t_QuestId, subQuestIndex))
                {

                    int t_CurrentProgression = GetCurrentProgressionOfSubQuest(t_QuestId, subQuestIndex);
                    UpdateProgression(t_QuestId, subQuestIndex, t_CurrentProgression);
                    break;
                }
            }
        }

        #endregion
    }

}

