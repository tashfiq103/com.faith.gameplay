namespace com.faith.gameplay {

    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    using com.faith.math;

    public class UserRetensionManager : MonoBehaviour
    {

        public static UserRetensionManager Instance;

        #region Public Variables

        [Header("Reference")]
        public SkillTree playerSkillTree;

        [Space(2.5f)]
        public GameObject holderReference;

        [Space(2.5f)]
        public Animator offlineRewardAnimator;
        public Animator animatorForClaimRewardWithAd;

        [Space(2.5f)]
        public TextMeshProUGUI awardAmountText;

        [Space(2.5f)]
        public Image backgroundImageReference;

        [Space(2.5f)]
        public Button claimRewardButton;
        public Button claimRewardWithAdButton;
        public GameObject adButtonBlocker;

        [Space(5f)]
        [Range(0f, 1f)]
        public float rewardThreshold;
        [Range(1, 720)]
        public int maximumDelayCounter;

        #endregion

        #region Private Variables

        private string IS_USER_IN_GAME = "IS_USER_IN_GAME";
        private string IS_USER_ALLOWED_TO_BE_REWARDED = "IS_USER_ALLOWED_TO_BE_REWARDED";
        private string IS_USER_AWARDED_FOR_RETURN = "IS_USER_AWARDED_FOR_RETURN";

        private string YEAR_REFERENCE_ON_APPLICATION_PAUSE = "YEAR_REFERENCE_ON_APPLICATION_PAUSE";
        private string MONTH_REFERENCE_ON_APPLICATION_PAUSE = "MONTH_REFERENCE_ON_APPLICATION_PAUSE";
        private string DAY_REFERENCE_ON_APPLICATION_PAUSE = "DAY_REFERENCE_ON_APPLICATION_PAUSE";
        private string HOUR_REFERENCE_ON_APPLICATION_PAUSE = "HOUR_REFERENCE_ON_APPLICATION_PAUSE";
        private string MINIUTE_REFERENCE_ON_APPLICATION_PAUSE = "MINIUTE_REFERENCE_ON_APPLICATION_PAUSE";

        private bool m_IsAwardPicked;

        #endregion

        #region Mono Behaviour

        void Awake()
        {

            Instance = this;

            /* 
                Initial Award For User When Launching the Application
            */
            SetUserAllowedToBeRewardedAsFocusRegain();
            ShowPanelForAwardUserForRetain();
        }

        /// <summary>
        /// Callback sent to all game objects when the player gets or loses focus.
        /// </summary>
        /// <param name="focusStatus">The focus state of the application.</param>
        void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                Debug.Log("Focus : YES");
                OnApplicationRetainFocus();
            }
            else
            {
                Debug.Log("Focus : NO");
                OnApplicationLostFocus();
            }
        }

        /// <summary>
        /// Callback sent to all game objects before the application is quit.
        /// </summary>
        void OnApplicationQuit()
        {
            OnApplicationLostFocus();
        }

        #endregion

        //----------------------------------------
        #region Configuretion	:	StateMachine

        private bool IsUserInGame()
        {

            if (PlayerPrefs.GetInt(IS_USER_IN_GAME, 0) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetUserAllowedToBeRewardedAsFocusRegain()
        {

            PlayerPrefs.SetInt(IS_USER_ALLOWED_TO_BE_REWARDED, 1);
        }

        private void SetUserPreventToBeRewardedAsFocusLost()
        {

            PlayerPrefs.SetInt(IS_USER_ALLOWED_TO_BE_REWARDED, 0);
        }

        private bool IsUserAllowedToBeRewarded()
        {

            if (PlayerPrefs.GetInt(IS_USER_ALLOWED_TO_BE_REWARDED) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetUserToBeAwrdedForReturn()
        {
            PlayerPrefs.SetInt(IS_USER_AWARDED_FOR_RETURN, 1);
        }

        private void ResetUserToBeAwrdedForReturn()
        {
            PlayerPrefs.SetInt(IS_USER_AWARDED_FOR_RETURN, 1);
        }

        private bool IsUserBeAwrdedForReturn()
        {
            if (PlayerPrefs.GetInt(IS_USER_AWARDED_FOR_RETURN, 0) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        //----------------------------------------
        #region Configuretion	:	User Retension

        private float CalculateAbscenceInMinute()
        {

            float t_Result = 0f;

            System.DateTime t_CurrentTime = System.DateTime.Now;

            int t_LastLoginYear = PlayerPrefs.GetInt(YEAR_REFERENCE_ON_APPLICATION_PAUSE, t_CurrentTime.Year);
            int t_LastLoginMonth = PlayerPrefs.GetInt(MONTH_REFERENCE_ON_APPLICATION_PAUSE, t_CurrentTime.Month);
            int t_LastLoginDay = PlayerPrefs.GetInt(DAY_REFERENCE_ON_APPLICATION_PAUSE, t_CurrentTime.Day);
            int t_LastLoginHour = PlayerPrefs.GetInt(HOUR_REFERENCE_ON_APPLICATION_PAUSE, t_CurrentTime.Hour);
            int t_LastLoginMiniute = PlayerPrefs.GetInt(MINIUTE_REFERENCE_ON_APPLICATION_PAUSE, t_CurrentTime.Minute);

            System.DateTime t_PreviousDateTime = new System.DateTime(
                t_LastLoginYear, t_LastLoginMonth, t_LastLoginDay, t_LastLoginHour, t_LastLoginMiniute, 0
            );

            System.TimeSpan t_TimeDifference = t_CurrentTime.Subtract(t_PreviousDateTime);

            float t_PassedMininutes = t_TimeDifference.Minutes >= maximumDelayCounter ? maximumDelayCounter : t_TimeDifference.Minutes;
            t_Result = t_PassedMininutes / maximumDelayCounter;

            return t_Result;
        }

        private void OnApplicationLostFocus()
        {

            System.DateTime t_DayTime = System.DateTime.Now;

            PlayerPrefs.SetInt(YEAR_REFERENCE_ON_APPLICATION_PAUSE, t_DayTime.Year);
            PlayerPrefs.SetInt(MONTH_REFERENCE_ON_APPLICATION_PAUSE, t_DayTime.Month);
            PlayerPrefs.SetInt(DAY_REFERENCE_ON_APPLICATION_PAUSE, t_DayTime.Day);
            PlayerPrefs.SetInt(HOUR_REFERENCE_ON_APPLICATION_PAUSE, t_DayTime.Hour);
            PlayerPrefs.SetInt(MINIUTE_REFERENCE_ON_APPLICATION_PAUSE, t_DayTime.Minute);

            SetUserToBeAwrdedForReturn();
        }

        private void OnApplicationRetainFocus()
        {

            SetUserAllowedToBeRewardedAsFocusRegain();

            if (!IsUserInGame())
            {
                ShowPanelForAwardUserForRetain();
            }
        }

        private void GiveAward(long t_RewardAmount, int t_CurrentLevel = 0)
        {

            m_IsAwardPicked = true;

            UnityAction t_OnRewardAnimationComplete = new UnityAction(delegate {

                SetUserPreventToBeRewardedAsFocusLost();
                ResetUserToBeAwrdedForReturn();
                holderReference.SetActive(false);
            });


            CoinReward.Instance.AwardCoin(t_CurrentLevel, t_RewardAmount, t_OnRewardAnimationComplete);
        }

        #endregion

        //----------------------------------------
        #region Public Callback

        private bool m_IsShowingAwardPanel = false;

        public void SetUserStateAsInGame()
        {

            PlayerPrefs.SetInt(IS_USER_IN_GAME, 1);
        }

        public void SetUserAsOutGame()
        {

            PlayerPrefs.SetInt(IS_USER_IN_GAME, 0);
        }

        public void ShowPanelForAwardUserForRetain()
        {

            StartCoroutine(AwardUserForRetain());
        }

        public IEnumerator AwardUserForRetain()
        {

            if (!m_IsShowingAwardPanel)
            {
                float t_AmountOfAbscense = CalculateAbscenceInMinute();

                if (t_AmountOfAbscense >= rewardThreshold &&
                    IsUserAllowedToBeRewarded() &&
                    IsUserBeAwrdedForReturn())
                {

                    //Initial Delay : So Math Function Create It's Instance
                    yield return new WaitForSeconds(0.25f);

                    m_IsShowingAwardPanel = true;
                    m_IsAwardPicked = false;

                    long t_UpgradeCostForEngine = (long)playerSkillTree.GetUpgradeCostForNextLevel(0);
                    long t_UpgradeCostOfFuel = (long)playerSkillTree.GetUpgradeCostForNextLevel(1);
                    long t_RewardAmount = (long)(t_UpgradeCostForEngine * 0.75f) + System.Convert.ToInt64(Random.Range(0f, 0.25f) * Mathf.Abs(t_UpgradeCostForEngine - t_UpgradeCostOfFuel));
                    t_RewardAmount = System.Convert.ToInt64(((t_RewardAmount * 0.75f) + ((t_RewardAmount * 0.25f) * System.Convert.ToInt64(playerSkillTree.GetCurrentLevelOfSkill(3) / (float)playerSkillTree.GetMaximumLevelOfSkill(3)))) * t_AmountOfAbscense);

                    //SUPER LOCK
                    t_RewardAmount = t_RewardAmount >= 100000000 ? 100000000 : t_RewardAmount;

                    bool t_IsRewardAdReadyToBeDisplayed = false;

                    //Color t_PrimaryColor = GameThemeManager.Instance.GetPrimaryColorOfCurrentColorTheme();
                    //Color t_HighlighterColor = GameThemeManager.Instance.GetHighlighterColorColorOfCurrentColorTheme();
                    //Color t_BackgroundColor = new Color(
                    //    t_HighlighterColor.r,
                    //    t_HighlighterColor.g,
                    //    t_HighlighterColor.b,
                    //    0.4f
                    //);
                    //backgroundImageReference.color = t_BackgroundColor;

                    claimRewardButton.onClick.RemoveAllListeners();
                    claimRewardButton.onClick.AddListener(delegate {

                        Debug.Log("Claim");
                        GiveAward(t_RewardAmount);
                    });

                    claimRewardWithAdButton.onClick.RemoveAllListeners();
                    claimRewardWithAdButton.onClick.AddListener(delegate {

                        Debug.Log("Claim with Ad");
                        GiveAward(t_RewardAmount * 2);


                    });

                    WaitForSeconds t_CycleDelay = new WaitForSeconds(0.25f);

                    awardAmountText.text = t_RewardAmount.ToString();

                    holderReference.SetActive(true);
                    offlineRewardAnimator.SetTrigger("APPEAR");


                    while (!m_IsAwardPicked)
                    {

                        if (!t_IsRewardAdReadyToBeDisplayed)
                        {

                            awardAmountText.text = " " + MathFunction.Instance.GetCurrencyInFormat(t_RewardAmount) + " (" + MathFunction.Instance.GetCurrencyInFormat(t_RewardAmount * 2) + ")";

                            animatorForClaimRewardWithAd.SetTrigger("ACTIVE");
                            t_IsRewardAdReadyToBeDisplayed = true;
                        }

                        yield return t_CycleDelay;
                    }

                    offlineRewardAnimator.SetTrigger("DISAPPEAR");
                    animatorForClaimRewardWithAd.SetTrigger("DEACTIVE");
                    yield return new WaitForSeconds(0.25f);


                }
            }

            m_IsShowingAwardPanel = false;
            StopCoroutine(AwardUserForRetain());
        }

        #endregion
    }
}

