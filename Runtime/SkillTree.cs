namespace com.faith.Gameplay
{
    using System;
    using UnityEngine;

    #region Custom Variables

    [System.Serializable]
    public struct Skill
    {

        [Header("Config : Level")]
        public string skillName;
        [Range(0, 1000)]
        public int initialLevel;
        [Range(0, 1000)]
        public int maximumLevel;

        [Space(5.0f)]
        [Header("Config : Upgrade State")]
        [Range(0.0f, 100.0f)]
        public float stateBaseValue;
        [Range(0.0f, 1.0f)]
        public float stateIncreaseFactor;
        public bool stateIncreaseByPercentage;

        [Space(5.0f)]
        [Header("Config : Upgrade Cost")]
        [Range(1, 10000000000000)]
        public double baseCostValue;
        [Range(0.00000001f, 0.5f)]
        public float costIncreasedFactor;
    }

    #endregion

    public class SkillTree : MonoBehaviour
    {

        //----------
        #region Public Variables

        public Skill[] skill;

        #endregion

        //----------
        #region Mono Behaviour

        void Awake()
        {

            CheckAndSetInitialLevel();
        }

        #endregion

        //----------
        #region Configuretion

        private void CheckAndSetInitialLevel()
        {

            int t_NumberOfSkill = skill.Length;
            for (int skillIndex = 0; skillIndex < t_NumberOfSkill; skillIndex++)
            {

                if (GetCurrentLevelOfSkill(skillIndex) < skill[skillIndex].initialLevel)
                {

                    int t_NumberOfLevelNeedsToBeUpgraded = skill[skillIndex].initialLevel - GetCurrentLevelOfSkill(skillIndex);
                    for (int upgradeIndex = 0; upgradeIndex < t_NumberOfLevelNeedsToBeUpgraded; upgradeIndex++)
                    {

                        UpgradeSkill(skillIndex);
                    }
                }
            }
        }

        #endregion

        //----------
        #region Public Callback : SkillTreeInformation

        public int GetTotalNumberOfSkill()
        {
            return skill.Length;
        }

        public string GetSkillName(int skillIndex)
        {

            if (skillIndex < skill.Length)
            {
                return skill[skillIndex].skillName;
            }
            else
            {
                Debug.LogAssertion("Invalid SkillIndex");
                return "Invalid SkillIndex";
            }
        }

        #endregion

        //Level & State Information
        //----------
        #region Public Callback : GetCurrentLevel

        public int GetCurrentLevelOfSkill(string skillName)
        {

            bool m_IsValidSkillName = false;
            int m_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == skillName)
                {

                    m_SkillIndex = skillIndex;
                    m_IsValidSkillName = true;
                    break;
                }
            }

            if (m_IsValidSkillName)
                return GetCurrentLevelOfSkill(m_SkillIndex);
            else
            {
                Debug.LogAssertion("Invalid Skill Index");
                return m_SkillIndex;
            }
        }

        public int GetCurrentLevelOfSkill(int skillIndex)
        {

            if (skillIndex < skill.Length)
            {

                return PlayerPrefs.GetInt(skill[skillIndex].skillName, 0);
            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
                return -1;
            }
        }

        #endregion

        #region Public Callback	:	Upgrade Cost

        public float GetUpgradeProgression(string t_SkillName)
        {

            bool t_IsValidSkillName = false;
            int t_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == t_SkillName)
                {

                    t_SkillIndex = skillIndex;
                    t_IsValidSkillName = true;
                    break;
                }
            }

            if (t_IsValidSkillName)
                return GetUpgradeProgression(t_SkillIndex);
            else
            {
                Debug.LogAssertion("Invalid Skill Index");
                return t_SkillIndex;
            }
        }

        public float GetUpgradeProgression(int t_SkillIndex)
        {

            if (t_SkillIndex < skill.Length)
            {

                return GetCurrentLevelOfSkill(t_SkillIndex) / (GetMaximumLevelOfSkill(t_SkillIndex) * 1.0f);
            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
                return -1;
            }
        }

        public double GetUpgradeCostForNextLevel(string t_SkillName)
        {

            bool t_IsValidSkillName = false;
            int t_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == t_SkillName)
                {

                    t_SkillIndex = skillIndex;
                    t_IsValidSkillName = true;
                    break;
                }
            }

            if (t_IsValidSkillName)
                return GetUpgradeCostForNextLevel(t_SkillIndex);
            else
            {
                Debug.LogAssertion("Invalid Skill Index");
                return t_SkillIndex;
            }
        }

        public double GetUpgradeCostForNextLevel(int t_SkillIndex)
        {

            double t_Result = 0;

            if (t_SkillIndex < skill.Length)
            {

                int t_UpgradedLevel = GetCurrentLevelOfSkill(t_SkillIndex) - skill[t_SkillIndex].initialLevel;

                if (t_UpgradedLevel >= skill[t_SkillIndex].maximumLevel)
                {

                    t_Result = -1;
                    Debug.Log("Already Reached Maximum Level");
                }
                else
                {

                    double t_BaseCostValue = skill[t_SkillIndex].baseCostValue;
                    float t_CostIncreaseFactor = skill[t_SkillIndex].costIncreasedFactor;
                    float t_MaxLevel = skill[t_SkillIndex].maximumLevel;

                    double t_Factor = 0.0f;
                    double t_ModifiedBaseValue = 0.0f;

                    for (int levelIndex = 0; levelIndex <= t_UpgradedLevel; levelIndex++)
                    {
                        if (levelIndex == 0)
                        {
                            t_Result = t_BaseCostValue;
                        }
                        else
                        {

                            if (skill[t_SkillIndex].stateIncreaseByPercentage)
                            {
                                t_Factor = (t_BaseCostValue * (levelIndex / t_MaxLevel));
                                t_ModifiedBaseValue = t_Factor <= 2.0 ? 2.0 : t_Factor;
                                t_Result += Math.Pow(t_ModifiedBaseValue, (t_CostIncreaseFactor * levelIndex));
                            }
                            else
                            {

                                t_Result += (t_Result * levelIndex) * (1f + t_CostIncreaseFactor);
                            }
                        }
                    }

                }
            }
            else
                Debug.LogAssertion("Invalid Skill Index");

            //if (DeviceInfoManager.Instance.IsDevice_iPad())
            //{
            //    return (long)(t_Result * 1.33f);
            //}
            //else if (DeviceInfoManager.Instance.IsDevice_iPhoneX())
            //{
            //    return (long)(t_Result * 0.9f);
            //}
            //else
            //{
            //    return (long)t_Result;
            //}
            return t_Result;
        }

        #endregion

        //----------
        #region Public Callback : Upgrade Level

        public void UpgradeSkill(string skillName)
        {

            bool m_IsValidSkillName = false;
            int m_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == skillName)
                {

                    m_SkillIndex = skillIndex;
                    m_IsValidSkillName = true;
                    break;
                }
            }

            if (m_IsValidSkillName)
                UpgradeSkill(m_SkillIndex);
            else
                Debug.LogAssertion("Invalid Skill Index");
        }

        public void UpgradeSkill(int skillIndex)
        {

            if (skillIndex < skill.Length)
            {

                if (PlayerPrefs.GetInt(skill[skillIndex].skillName, 0) < skill[skillIndex].maximumLevel)
                    PlayerPrefs.SetInt(skill[skillIndex].skillName, (PlayerPrefs.GetInt(skill[skillIndex].skillName, 0) + 1));
                else
                    Debug.Log("The player has already reached the maximum level of : " + skill[skillIndex].skillName);
            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
            }
        }

        #endregion

        //----------
        #region Public Callback : Degrade Level

        public void DegradeSkill(string skillName)
        {

            bool m_IsValidSkillName = false;
            int m_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == skillName)
                {

                    m_SkillIndex = skillIndex;
                    m_IsValidSkillName = true;
                    break;
                }
            }

            if (m_IsValidSkillName)
                DegradeSkill(m_SkillIndex);
            else
                Debug.LogAssertion("Invalid Skill Index");
        }

        public void DegradeSkill(int skillIndex)
        {

            if (skillIndex < skill.Length)
            {

                if (PlayerPrefs.GetInt(skill[skillIndex].skillName, 0) > 0)
                    PlayerPrefs.SetInt(skill[skillIndex].skillName, (PlayerPrefs.GetInt(skill[skillIndex].skillName, 0) - 1));
                else
                    Debug.LogWarning("The Skill Level Cannot Be Degrade than 0 : " + skill[skillIndex].skillName);
            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
            }
        }

        #endregion

        //----------
        #region Public Callback : Reset Level

        public void ResetSkill(string skillName)
        {

            bool m_IsValidSkillName = false;
            int m_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == skillName)
                {

                    m_SkillIndex = skillIndex;
                    m_IsValidSkillName = true;
                    break;
                }
            }

            if (m_IsValidSkillName)
                ResetSkill(m_SkillIndex);
            else
                Debug.LogAssertion("Invalid Skill Index");
        }

        public void ResetSkill(int skillIndex)
        {

            if (skillIndex < skill.Length)
            {

                PlayerPrefs.SetInt(skill[skillIndex].skillName, 0);
            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
            }
        }

        #endregion

        //----------
        #region Public Callback	: GetCurrentStat

        public float GetCurrentStatOfSkill(string skillName)
        {

            bool m_IsValidSkillName = false;
            int m_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == skillName)
                {

                    m_SkillIndex = skillIndex;
                    m_IsValidSkillName = true;
                    break;
                }
            }

            if (m_IsValidSkillName)
                return GetCurrentStatOfSkill(m_SkillIndex);
            else
            {
                Debug.LogAssertion("Invalid Skill Index");
                return (float)m_SkillIndex;
            }
        }

        public float GetCurrentStatOfSkill(int skillIndex)
        {

            if (skillIndex < skill.Length)
            {

                int m_CurrentLevel = GetCurrentLevelOfSkill(skillIndex);
                float m_CurrentStat = 0.0f;

                m_CurrentStat = skill[skillIndex].stateBaseValue;

                for (int index = 0; index < m_CurrentLevel; index++)
                {

                    if (skill[skillIndex].stateIncreaseByPercentage)
                    {

                        m_CurrentStat += skill[skillIndex].stateIncreaseFactor * m_CurrentStat;

                    }
                    else
                        m_CurrentStat += skill[skillIndex].stateIncreaseFactor * skill[skillIndex].stateBaseValue;
                }

                return m_CurrentStat;

            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
                return -1.0f;
            }
        }

        #endregion

        //----------
        #region Public Callback : GetStatOfSpeceficLevel

        public float GetStatOfLevelAtSkill(string skillName, int skillLevel)
        {

            bool m_IsValidSkillName = false;
            int m_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == skillName)
                {

                    m_SkillIndex = skillIndex;
                    m_IsValidSkillName = true;
                    break;
                }
            }

            if (m_IsValidSkillName)
                return GetStatOfLevelAtSkill(m_SkillIndex, skillLevel);
            else
            {
                Debug.LogAssertion("Invalid Skill Index");
                return (float)m_SkillIndex;
            }
        }

        public float GetStatOfLevelAtSkill(int skillIndex, int skillLevel)
        {

            if (skillIndex < skill.Length)
            {

                if (skillLevel < skill[skillIndex].maximumLevel)
                {

                    float m_CurrentStat = skill[skillIndex].stateBaseValue;
                    for (int index = 0; index < skillLevel; index++)
                    {

                        if (skill[skillIndex].stateIncreaseByPercentage)
                            m_CurrentStat += skill[skillIndex].stateIncreaseFactor * m_CurrentStat;
                        else
                            m_CurrentStat += skill[skillIndex].stateIncreaseFactor * skill[skillIndex].stateBaseValue;

                    }
                    return m_CurrentStat;
                }
                else
                {

                    Debug.LogAssertion("Invalid Requirest, Requested Level(" + skillLevel + ") > Maximum Level(" + skill[skillIndex].maximumLevel + ")");
                    return -1.0f;
                }

            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
                return -1.0f;
            }
        }

        #endregion

        //Base State Information
        //----------
        #region Public Callback : GetMaximumLevel

        public int GetMaximumLevelOfSkill(string skillName)
        {

            bool m_IsValidSkillName = false;
            int m_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == skillName)
                {

                    m_SkillIndex = skillIndex;
                    m_IsValidSkillName = true;
                    break;
                }
            }

            if (m_IsValidSkillName)
                return GetMaximumLevelOfSkill(m_SkillIndex);
            else
            {
                Debug.LogAssertion("Invalid Skill Index");
                return m_SkillIndex;
            }
        }

        public int GetMaximumLevelOfSkill(int skillIndex)
        {

            if (skillIndex < skill.Length)
            {

                return skill[skillIndex].maximumLevel;
            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
                return -1;
            }
        }

        #endregion

        //----------
        #region Public Callback : GetBaseValue

        public float GetBaseValueOfSkill(string skillName)
        {

            bool m_IsValidSkillName = false;
            int m_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == skillName)
                {

                    m_SkillIndex = skillIndex;
                    m_IsValidSkillName = true;
                    break;
                }
            }

            if (m_IsValidSkillName)
                return GetBaseValueOfSkill(m_SkillIndex);
            else
            {
                Debug.LogAssertion("Invalid Skill Index");
                return m_SkillIndex;
            }
        }

        public float GetBaseValueOfSkill(int skillIndex)
        {

            if (skillIndex < skill.Length)
            {

                return skill[skillIndex].stateBaseValue;
            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
                return -1;
            }
        }

        #endregion

        //----------
        #region Public Callback : IsIncreaseByPercentage

        public bool IsIncreaseByPercentage(string skillName)
        {

            bool m_IsValidSkillName = false;
            int m_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == skillName)
                {

                    m_SkillIndex = skillIndex;
                    m_IsValidSkillName = true;
                    break;
                }
            }

            if (m_IsValidSkillName)
                return IsIncreaseByPercentage(m_SkillIndex);
            else
            {
                Debug.LogAssertion("Invalid Skill Index");
                return false;
            }
        }

        public bool IsIncreaseByPercentage(int skillIndex)
        {

            if (skillIndex < skill.Length)
            {

                return skill[skillIndex].stateIncreaseByPercentage;
            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
                return false;
            }
        }

        #endregion

        //----------
        #region Public Callback : GetIncraseFactor

        public float GetIncraseFactorOfSkill(string skillName)
        {

            bool m_IsValidSkillName = false;
            int m_SkillIndex = -1;

            for (int skillIndex = 0; skillIndex < skill.Length; skillIndex++)
            {
                if (skill[skillIndex].skillName == skillName)
                {

                    m_SkillIndex = skillIndex;
                    m_IsValidSkillName = true;
                    break;
                }
            }

            if (m_IsValidSkillName)
                return GetIncraseFactorOfSkill(m_SkillIndex);
            else
            {
                Debug.LogAssertion("Invalid Skill Index");
                return -1.0f;
            }
        }

        public float GetIncraseFactorOfSkill(int skillIndex)
        {

            if (skillIndex < skill.Length)
            {

                return skill[skillIndex].stateIncreaseFactor;
            }
            else
            {

                Debug.LogAssertion("Invalid Skill Index");
                return -1.0f;
            }
        }

        #endregion
    }
}

