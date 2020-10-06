namespace com.faith.gameplay {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using TMPro;

    using com.faith.core;

    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance;

        //----------
        #region Public Varaibles

        public bool initialFinanceBoost;
        [Range(0, 10000000)]
        public int initialBoostAmount;

        [Space(2.5f)]
        [Header("Configuretion      :   In Game Currency")]
        public bool enableCurrencyForMultiLocation;
        public TextMeshProUGUI coinText;

        [Space(2.5f)]
        [Header("Configuretion      :   Game Pause/Resume")]

        public bool isPauseFeatureEnabled;

        [Space(2.5f)]
        public Image transparentBackground;
        public TextMeshProUGUI pauseAndCounterText;
        public TextMeshProUGUI tapAnyWhereText;

        [Space(2.5f)]
        [Range(0f, 5f)]
        public float pauseSceneAppearDuration;
        [Range(0f, 5f)]
        public float pauseSceneDisappearDuration;

        #endregion

        //----------
        #region Private Variables


        private enum UserRequisitionForPauseAndResume
        {
            userRequestForPause,
            userRequestForResume
        }

        private UserRequisitionForPauseAndResume m_UserRequestForPauseAndResume;

        private bool m_IsGamePausedFeaturedAllowed;
        private bool m_IsForceResumeExecuted = false;
        private bool m_IsVisualStateControlleRunning = false;
        private bool m_IsStateInputForPause = false;

        private float gameSpeed = 1.0f;

        private float m_CurrentGameSpeed;
        private float m_PauseStartingTime;
        private float m_PauseDuration;

        private string IN_GAME_CURRENCY_REFERENCE = "IN_GAME_CURRENCY_REFERENCE";
        private string IS_FINANCIAL_BOOST_GIVEN = "IS_FINANCIAL_BOOST_GIVEN";

        private List<UnityAction> m_OnSpeedChangeEvent;

        #endregion

        //----------
        #region Mono Function

        void Awake()
        {

            m_OnSpeedChangeEvent = new List<UnityAction>();



            if (Instance == null)
            {

                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {

                Destroy(gameObject);
            }

            PreProcess();
        }

        private void Start()
        {
            if (initialFinanceBoost)
            {

                if (PlayerPrefs.GetInt(IS_FINANCIAL_BOOST_GIVEN, 0) == 0)
                {

                    PlayerPrefs.SetString(GetFormatedReferenceForStoringCurrency(), initialBoostAmount.ToString());
                    PlayerPrefs.SetInt(IS_FINANCIAL_BOOST_GIVEN, 1);
                }
            }
        }


        void Update()
        {

            if (IsGamePaused())
            {

                m_PauseDuration = Time.time - m_PauseStartingTime;
            }

            if (coinText != null)
            {

                coinText.text = MathFunction.GetCurrencyInFormat(GetInGameCurrency());

            }
        }
        #endregion

        //----------
        #region Public Callback		:	Game Pause/Resume

        public void EnableGamePauseFeature()
        {

            if (isPauseFeatureEnabled)
            {

                m_IsStateInputForPause = true;
                m_IsForceResumeExecuted = false;
                m_UserRequestForPauseAndResume = UserRequisitionForPauseAndResume.userRequestForPause;

                m_IsGamePausedFeaturedAllowed = true;
            }
        }

        public void DisableGamePauseFeature()
        {

            if (isPauseFeatureEnabled)
                m_IsGamePausedFeaturedAllowed = false;
        }

        public bool IsGamePauseFeatureEnabled()
        {

            return m_IsGamePausedFeaturedAllowed;
        }

        public void RegisterEventOnChangingGameSpeec(UnityAction t_RegisterEvent)
        {
            if (!m_OnSpeedChangeEvent.Contains(t_RegisterEvent))
            {
                m_OnSpeedChangeEvent.Add(t_RegisterEvent);
            }
        }

        public float GetGameSpeed()
        {

            return gameSpeed;
        }

        public void SetGameSpeed(float newGameSpeed)
        {

            if (newGameSpeed > 1.0f)
                newGameSpeed = 1.0f;

            gameSpeed = newGameSpeed;
        }

        public bool IsGamePaused()
        {

            if (gameSpeed == 0.0f)
                return true;
            else
                return false;
        }

        public float GetPauseDuration()
        {

            return m_PauseDuration;
        }

        public void PauseGame()
        {

            m_UserRequestForPauseAndResume = UserRequisitionForPauseAndResume.userRequestForPause;

            if (m_IsGamePausedFeaturedAllowed && !m_IsVisualStateControlleRunning && m_IsStateInputForPause)
            {

                m_PauseStartingTime = Time.time;
                m_PauseDuration = 0.0f;

                m_CurrentGameSpeed = gameSpeed;

                m_IsVisualStateControlleRunning = true;
                transparentBackground.raycastTarget = true;
                transparentBackground.gameObject.SetActive(true);
                StartCoroutine(ControllerForPauseAndResumeGame(true));
            }

        }

        public void ResumeGame()
        {

            m_UserRequestForPauseAndResume = UserRequisitionForPauseAndResume.userRequestForResume;

            if (m_IsGamePausedFeaturedAllowed && !m_IsVisualStateControlleRunning && !m_IsStateInputForPause)
            {
                m_IsVisualStateControlleRunning = true;
                transparentBackground.raycastTarget = false;
                StartCoroutine(ControllerForPauseAndResumeGame(false));
            }

            //Old
            //gameSpeed = m_CurrentGameSpeed;
        }

        public void ForceResume()
        {

            m_IsForceResumeExecuted = true;
        }

        #endregion

        //----------
        #region Public Callback		:	InGameCurrency

        public double GetInGameCurrency()
        {
            return System.Convert.ToDouble(PlayerPrefs.GetString(GetFormatedReferenceForStoringCurrency(), "0"));
        }

        public void UpdateInGameCurrencyAnimated(double t_CurrencyAmount)
        {
            UpdateInGameCurrencyAnimated(t_CurrencyAmount, 1f, null);
        }

        public void UpdateInGameCurrencyAnimated(double t_CurrencyAmount, float t_AnimationDuration)
        {
            UpdateInGameCurrencyAnimated(t_CurrencyAmount, t_AnimationDuration, null);
        }

        public void UpdateInGameCurrencyAnimated(double t_CurrencyAmount, float t_AnimationDuration, UnityAction t_OnAnimationFinished)
        {

            if (!m_IsCurrencyAnimationRunning)
            {
                m_IsCurrencyAnimationRunning = true;
                StartCoroutine(CurrencyAnimationController(t_CurrencyAmount, t_AnimationDuration, t_OnAnimationFinished));
            }
            else
            {

                UpdateInGameCurrency(t_CurrencyAmount);
            }
        }


        public void UpdateInGameCurrency(double t_CurrencyAmount)
        {
            double t_CURRENT_GAME_CURRENCY = GetInGameCurrency();
            PlayerPrefs.SetString(GetFormatedReferenceForStoringCurrency(), System.Convert.ToString(t_CURRENT_GAME_CURRENCY + t_CurrencyAmount));

        }

        public void DeductInGameCurrency(double t_CurrencyAmount)
        {

            UpdateInGameCurrency(-t_CurrencyAmount);
        }

        public void ResetInGameCurrency()
        {

            if (enableCurrencyForMultiLocation)
            {
                //int t_NumberOfGeoLocation = GeoLocationManager.Instance.regionsInfo.Length;
                //for (int index = 0; index < t_NumberOfGeoLocation; index++) {

                //    PlayerPrefs.SetInt(IN_GAME_CURRENCY_REFERENCE + "_" + index, 0);
                //}
            }
            else
            {
                PlayerPrefs.SetInt(IN_GAME_CURRENCY_REFERENCE, 0);

            }

            PlayerPrefs.SetInt(IS_FINANCIAL_BOOST_GIVEN, 0);
        }

        #endregion

        //----------
        #region Configuretion       :       Pause/Resume State Visual Controller

        private void PreProcess()
        {

            if (gameSpeed == 0.0f)
                gameSpeed = 1.0f;

            m_CurrentGameSpeed = gameSpeed;
        }

        private void InvokeRegisteredEvent()
        {

            int t_NumberOfRegisteredEvent = m_OnSpeedChangeEvent.Count;
            for (int eventIndex = 0; eventIndex < t_NumberOfRegisteredEvent; eventIndex++)
            {

                m_OnSpeedChangeEvent[eventIndex].Invoke();
            }
        }

        private IEnumerator ControllerForPauseAndResumeGame(bool t_IsPausing)
        {

            //Touch Sensetivity Locking System
            if (t_IsPausing)
                yield return new WaitForSeconds(0.25f);

            WaitForEndOfFrame t_CycleDelay = new WaitForEndOfFrame();

            float t_CurrentTime = Time.time;
            float t_AbsoluteDuration = t_IsPausing ? pauseSceneAppearDuration : pauseSceneDisappearDuration;
            float t_EndTimeForChangingPhase = t_CurrentTime + t_AbsoluteDuration;

            float t_Progression = 0f;
            Color t_ColorForTransparentBackground = transparentBackground.color;
            Color t_ColorOfPauseAndCounterText = pauseAndCounterText.color;
            Color t_ColorOfTapAnyWhereText = tapAnyWhereText.color;

            if (m_UserRequestForPauseAndResume == UserRequisitionForPauseAndResume.userRequestForPause && t_IsPausing)
            {

                while (t_CurrentTime < t_EndTimeForChangingPhase)
                {

                    if (m_IsForceResumeExecuted)
                        break;

                    t_CurrentTime = Time.time;

                    t_Progression = (t_EndTimeForChangingPhase - t_CurrentTime) / t_AbsoluteDuration;

                    gameSpeed = m_CurrentGameSpeed * t_Progression;

                    transparentBackground.color = new Color(
                            t_ColorForTransparentBackground.r,
                            t_ColorForTransparentBackground.g,
                            t_ColorForTransparentBackground.b,
                            0.8f * (1f - t_Progression)
                        );
                    pauseAndCounterText.color = new Color(
                            t_ColorOfPauseAndCounterText.r,
                            t_ColorOfPauseAndCounterText.g,
                            t_ColorOfPauseAndCounterText.b,
                            1f - t_Progression
                        );
                    tapAnyWhereText.color = new Color(
                            t_ColorOfTapAnyWhereText.r,
                            t_ColorOfTapAnyWhereText.g,
                            t_ColorOfTapAnyWhereText.b,
                            1f - t_Progression
                        );

                    InvokeRegisteredEvent();

                    yield return t_CycleDelay;
                }

                gameSpeed = 0;
                transparentBackground.color = new Color(
                        t_ColorForTransparentBackground.r,
                        t_ColorForTransparentBackground.g,
                        t_ColorForTransparentBackground.b,
                        0.8f
                    );
                pauseAndCounterText.color = new Color(
                            t_ColorOfPauseAndCounterText.r,
                            t_ColorOfPauseAndCounterText.g,
                            t_ColorOfPauseAndCounterText.b,
                            1f
                        );
                tapAnyWhereText.color = new Color(
                            t_ColorOfTapAnyWhereText.r,
                            t_ColorOfTapAnyWhereText.g,
                            t_ColorOfTapAnyWhereText.b,
                            1f
                        );

                m_IsStateInputForPause = false;

            }
            else if (!t_IsPausing)
            {

                // Resume

                while (t_CurrentTime < t_EndTimeForChangingPhase)
                {

                    if (m_IsForceResumeExecuted)
                        break;

                    t_CurrentTime = Time.time;

                    t_Progression = 1.0f - ((t_EndTimeForChangingPhase - t_CurrentTime) / t_AbsoluteDuration);

                    gameSpeed = m_CurrentGameSpeed * t_Progression;

                    transparentBackground.color = new Color(
                            t_ColorForTransparentBackground.r,
                            t_ColorForTransparentBackground.g,
                            t_ColorForTransparentBackground.b,
                            0.8f * (1f - t_Progression)
                        );
                    pauseAndCounterText.color = new Color(
                            t_ColorOfPauseAndCounterText.r,
                            t_ColorOfPauseAndCounterText.g,
                            t_ColorOfPauseAndCounterText.b,
                            1f - t_Progression
                        );
                    tapAnyWhereText.color = new Color(
                            t_ColorOfTapAnyWhereText.r,
                            t_ColorOfTapAnyWhereText.g,
                            t_ColorOfTapAnyWhereText.b,
                            1f - t_Progression
                        );

                    InvokeRegisteredEvent();

                    yield return t_CycleDelay;
                }

                gameSpeed = m_CurrentGameSpeed;
                transparentBackground.color = new Color(
                        t_ColorForTransparentBackground.r,
                        t_ColorForTransparentBackground.g,
                        t_ColorForTransparentBackground.b,
                        0f
                    );
                pauseAndCounterText.color = new Color(
                            t_ColorOfPauseAndCounterText.r,
                            t_ColorOfPauseAndCounterText.g,
                            t_ColorOfPauseAndCounterText.b,
                            0f
                        );
                tapAnyWhereText.color = new Color(
                            t_ColorOfTapAnyWhereText.r,
                            t_ColorOfTapAnyWhereText.g,
                            t_ColorOfTapAnyWhereText.b,
                            0f
                        );

                transparentBackground.gameObject.SetActive(false);
                InvokeRegisteredEvent();

                m_IsStateInputForPause = true;

            }
            else
            {

                transparentBackground.gameObject.SetActive(false);
            }

            //------------------------------------
            //Force Resume
            if (m_IsForceResumeExecuted)
            {

                gameSpeed = 1f;
                transparentBackground.color = new Color(
                        t_ColorForTransparentBackground.r,
                        t_ColorForTransparentBackground.g,
                        t_ColorForTransparentBackground.b,
                        0f
                    );
                pauseAndCounterText.color = new Color(
                            t_ColorOfPauseAndCounterText.r,
                            t_ColorOfPauseAndCounterText.g,
                            t_ColorOfPauseAndCounterText.b,
                            0f
                        );
                tapAnyWhereText.color = new Color(
                            t_ColorOfTapAnyWhereText.r,
                            t_ColorOfTapAnyWhereText.g,
                            t_ColorOfTapAnyWhereText.b,
                            0f
                        );

                transparentBackground.gameObject.SetActive(false);
                InvokeRegisteredEvent();

                m_IsStateInputForPause = true;

                m_IsForceResumeExecuted = false;
            }
            //------------------------------------

            m_IsVisualStateControlleRunning = false;
            StopCoroutine(ControllerForPauseAndResumeGame(false));
        }

        #endregion

        //----------
        #region Configuretion		:	In GameCurrency

        private bool m_IsCurrencyAnimationRunning = false;

        private string GetFormatedReferenceForStoringCurrency()
        {

            if (enableCurrencyForMultiLocation)
            {
                return IN_GAME_CURRENCY_REFERENCE + "_" /*+ GeoLocationManager.Instance.GetSelectedRegion()*/;
            }
            else
            {
                return IN_GAME_CURRENCY_REFERENCE;
            }
        }

        private IEnumerator CurrencyAnimationController(double t_CurrencyAmount, float t_AnimationDuration, UnityAction t_OnAnimationFinished)
        {
            float t_CycleLength = 0.0167f;
            WaitForSeconds t_CycleDelay = new WaitForSeconds(t_CycleLength);

            double t_CurrentChunkOfCurrencyToBeAdd;
            double t_RemainingCurrencyToAdd = t_CurrencyAmount;
            float t_RemainingTimeForAnimation = t_AnimationDuration;


            while (t_RemainingTimeForAnimation > 0)
            {

                yield return t_CycleDelay;
                t_RemainingTimeForAnimation -= t_CycleLength;

                t_CurrentChunkOfCurrencyToBeAdd = (1f - (t_RemainingTimeForAnimation / t_AnimationDuration)) * t_RemainingCurrencyToAdd;
                t_RemainingCurrencyToAdd -= t_CurrentChunkOfCurrencyToBeAdd;
                UpdateInGameCurrency(t_CurrentChunkOfCurrencyToBeAdd);
            }

            UpdateInGameCurrency(t_RemainingCurrencyToAdd);

            if (t_OnAnimationFinished != null)
                t_OnAnimationFinished.Invoke();

            StopCoroutine(CurrencyAnimationController(0f, 0f, null));

            m_IsCurrencyAnimationRunning = false;
        }

        #endregion

    }
}
