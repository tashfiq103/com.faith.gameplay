namespace com.faith.Gameplay {

    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine;

    public class CoinEarningController : MonoBehaviour {

        #region Custom Variables

        private struct CoinInfo {

            public bool isInitialDisplacementComplete;
            public double amountOfCoinToBeAdded;
            public float initialDistanceFromDisplacement;
            public float initialDistanceFromDisplacementToTarget;
            public Transform coinContainerTransformReference;
            public Vector3 initialPositionOfDisplacement;

        }

        public class MultipleCoinAddController {
            int m_NumberOfCoin;
            Vector3 m_CoinSpawnPosition;
            Vector3 m_InitialPositionDisplacement;
            Vector3 m_InitialDirection;
            double m_Multiplier;
            bool m_ByPassCoinSpawningRule;

            public MultipleCoinAddController (int t_NumberOfCoin, Vector3 t_CoinSpawnPosition, Vector3 t_InitialPositionDisplacement = new Vector3 (), Vector3 t_InitialDirection = new Vector3 (), double t_Multiplier = 1, bool t_ByPassCoinSpawningRule = false) {

                m_NumberOfCoin = t_NumberOfCoin;
                m_CoinSpawnPosition = t_CoinSpawnPosition;
                m_InitialPositionDisplacement = t_InitialPositionDisplacement;
                m_InitialDirection = t_InitialDirection;
                m_Multiplier = t_Multiplier;
                m_ByPassCoinSpawningRule = t_ByPassCoinSpawningRule;
            }

            public IEnumerator ControllerForAddingCoin (float t_DelayBetweenCoins = 0.5f) {

                WaitForSecondsRealtime t_CycleDelay = new WaitForSecondsRealtime (t_DelayBetweenCoins);

                while (m_NumberOfCoin > 0) {

                    CoinEarningController.Instance.AddCoinToUser (
                        m_CoinSpawnPosition,
                        m_InitialPositionDisplacement,
                        m_InitialDirection,
                        m_Multiplier,
                        m_ByPassCoinSpawningRule
                    );
                    m_NumberOfCoin--;
                    yield return t_CycleDelay;
                }
            }
        }

        #endregion

        #region Public Variables

        public static CoinEarningController Instance;

        public PreLoadedPrefab coinContainer;

        [Space (5.0f)]
        public bool isMoveTowardsHUD;
        public Transform coinHUDTransformReference;
        public AnimationCurve curveForCoinScalingTowardsEndPoint;

        [Space (5.0f)]
        public Vector3 animationPosition;
        public AnimationCurve curveForCoinScalingTowardsAnimationPoint;

        [Range (0f, 100f)]
        public float coinMovementSpeed = 10;
        [Range (0f, 90f)]
        public float coinRotationSpeed = 30;
        [Space (5.0f)]
        [Header ("(Optional) : Monetization")]
        public GlobalMonetizationStateController globalMonetizationStateControllerReference;

        #endregion

        #region Private Variables

        private TimeController m_TimeManagerReference;

        private bool m_IsCoinEarnControllerRunning;
        private bool m_IsCoinEarnAllow;

        private float m_BufferTimeForCoinEarn;
        private float m_CurrentBufferTimeForCoinEarn;

        private double m_NumberOfCoinEarnedOnThisLevel;
        private double m_NumberOfCoinStackedToUpdateInUI;
        private double m_CoinEarnStateForLevel;

        private List<CoinInfo> m_ListOfCoinInfo;

        #endregion

        #region Mono Behaviour

        private void Awake () {
            Instance = this;
        }

        #endregion

        #region Configuretion

        private IEnumerator ControllerForCoinEarn () {
            float t_DeltaTime;
            float t_CycleLength = 0.0167f;
            WaitForSecondsRealtime t_CycleDelay = new WaitForSecondsRealtime (t_CycleLength);

            int t_NumberOfAvailableCoinInfo = 0;
            float t_CoinPositionCheckBoundary = coinMovementSpeed * 0.025f;
            float t_ProgressionWithScale;
            float t_CurrentDistance;
            Vector3 t_ZeroScale = Vector3.zero;
            Vector3 t_OneScale = Vector3.one;
            Vector3 t_TargetedPosition;
            Vector3 t_ModifiedPosition;
            Vector3 t_ModifiedScale;
            Vector3 t_RandomRotation;
            CoinInfo t_SelectedCoinInfoReference;

            t_RandomRotation = new Vector3 (
                Random.Range (-1f, 1f),
                Random.Range (-1f, 1f),
                Random.Range (-1f, 1f)
            );

            while (m_IsCoinEarnControllerRunning || t_NumberOfAvailableCoinInfo > 0) {

                t_DeltaTime = m_TimeManagerReference.GetAbsoluteDeltaTime ();

                if (!m_IsCoinEarnAllow &&
                    m_CurrentBufferTimeForCoinEarn <= 0) {

                    m_IsCoinEarnAllow = true;
                } else {

                    m_CurrentBufferTimeForCoinEarn -= t_CycleLength;
                }

                t_NumberOfAvailableCoinInfo = m_ListOfCoinInfo.Count;
                for (int counter = 0; counter < t_NumberOfAvailableCoinInfo; counter++) {

                    t_SelectedCoinInfoReference = m_ListOfCoinInfo[counter];
                    if (!t_SelectedCoinInfoReference.isInitialDisplacementComplete) {

                        t_ModifiedPosition = Vector3.MoveTowards (
                            t_SelectedCoinInfoReference.coinContainerTransformReference.position,
                            t_SelectedCoinInfoReference.initialPositionOfDisplacement,
                            coinMovementSpeed * t_DeltaTime
                        );
                        t_SelectedCoinInfoReference.coinContainerTransformReference.Rotate (t_RandomRotation * coinRotationSpeed);

                        t_CurrentDistance = Vector3.Distance (t_SelectedCoinInfoReference.initialPositionOfDisplacement, t_ModifiedPosition);
                        t_ProgressionWithScale = 1f - (t_CurrentDistance / t_SelectedCoinInfoReference.initialDistanceFromDisplacement);
                        t_ModifiedScale = Vector3.Lerp (
                            t_ZeroScale,
                            t_OneScale * curveForCoinScalingTowardsAnimationPoint.Evaluate (t_ProgressionWithScale),
                            t_ProgressionWithScale
                        );

                        t_SelectedCoinInfoReference.coinContainerTransformReference.localScale = t_ModifiedScale;

                        if (t_CurrentDistance <= t_CoinPositionCheckBoundary) {

                            t_SelectedCoinInfoReference.coinContainerTransformReference.position = t_SelectedCoinInfoReference.initialPositionOfDisplacement;
                            t_SelectedCoinInfoReference.coinContainerTransformReference.localScale = t_OneScale;
                            //t_SelectedCoinInfoReference.initialDistanceFromDisplacementToTarget = isMoveTowardsHUD ? Vector3.Distance(coinHUDTransformReference.position, t_ModifiedPosition) : Vector3.Distance(t_ModifiedPosition + animationPosition, t_ModifiedPosition);
                            t_SelectedCoinInfoReference.isInitialDisplacementComplete = true;
                            m_ListOfCoinInfo[counter] = t_SelectedCoinInfoReference;
                        } else {

                            t_SelectedCoinInfoReference.coinContainerTransformReference.position = t_ModifiedPosition;
                        }
                    } else {

                        if (isMoveTowardsHUD)
                            t_TargetedPosition = coinHUDTransformReference.position;
                        else
                            t_TargetedPosition = t_SelectedCoinInfoReference.initialPositionOfDisplacement + animationPosition;

                        t_ModifiedPosition = Vector3.MoveTowards (
                            t_SelectedCoinInfoReference.coinContainerTransformReference.position,
                            t_TargetedPosition,
                            coinMovementSpeed * 2 * t_DeltaTime
                        );

                        t_CurrentDistance = Vector3.Distance (t_TargetedPosition, t_ModifiedPosition);
                        t_ProgressionWithScale = 1f - Mathf.Clamp01 (t_CurrentDistance / t_SelectedCoinInfoReference.initialDistanceFromDisplacementToTarget);
                        //Debug.Log( t_CurrentDistance + "/" + t_SelectedCoinInfoReference.initialDistanceFromDisplacementToTarget +  " = Scale Value : " + t_ProgressionWithScale);

                        t_ModifiedScale = Vector3.Lerp (
                            t_OneScale,
                            t_OneScale * curveForCoinScalingTowardsEndPoint.Evaluate (t_ProgressionWithScale),
                            t_ProgressionWithScale
                        );

                        if (t_CurrentDistance <= t_CoinPositionCheckBoundary) {

                            t_SelectedCoinInfoReference.coinContainerTransformReference.position = coinHUDTransformReference.position;
                            t_SelectedCoinInfoReference.coinContainerTransformReference.gameObject.SetActive (false);

                            if (isMoveTowardsHUD) {

                                m_NumberOfCoinEarnedOnThisLevel += t_SelectedCoinInfoReference.amountOfCoinToBeAdded;
                                GameManager.Instance.UpdateInGameCurrencyAnimated (t_SelectedCoinInfoReference.amountOfCoinToBeAdded);

                                //UIStateController.Instance.UIGameplayMenuControllerReference.UpdateCoinEarnForLevel (m_NumberOfCoinEarnedOnThisLevel);
                                //AudioController.Instance.PlaySoundFXForCoinEarn();

                            }

                            m_ListOfCoinInfo.RemoveAt (counter);
                            t_NumberOfAvailableCoinInfo--;

                            coinContainer.PushPreloadedPrefab (t_SelectedCoinInfoReference.coinContainerTransformReference.gameObject);
                        } else {

                            t_SelectedCoinInfoReference.coinContainerTransformReference.position = t_ModifiedPosition;
                            t_SelectedCoinInfoReference.coinContainerTransformReference.localScale = t_ModifiedScale;
                            t_SelectedCoinInfoReference.coinContainerTransformReference.Rotate (t_RandomRotation * coinRotationSpeed);

                        }
                    }
                }

                yield return t_CycleDelay;
            }

            StopCoroutine (ControllerForCoinEarn ());

        }

        #endregion

        #region Public Callback

        public void PreProcess (double t_CoinEarnStateForLevel, float t_UpgradeProgressionOfCoinEarn = 0) {
            m_TimeManagerReference = TimeController.Instance;
            m_CoinEarnStateForLevel = t_CoinEarnStateForLevel;
            m_BufferTimeForCoinEarn = 0.1f - (0.09f * t_UpgradeProgressionOfCoinEarn);

            if (!m_IsCoinEarnControllerRunning) {
                m_NumberOfCoinEarnedOnThisLevel = 0;
                m_NumberOfCoinStackedToUpdateInUI = 0;
                m_IsCoinEarnControllerRunning = true;
                m_IsCoinEarnAllow = true;
                m_ListOfCoinInfo = new List<CoinInfo> ();
                StartCoroutine (ControllerForCoinEarn ());

            }
        }

        public void PostProcess () {

            m_IsCoinEarnControllerRunning = false;
        }

        public void AddMultipleCoinToUser (int t_NumberOfCoin, Vector3 t_CoinSpawnPosition, Vector3 t_InitialPositionDisplacement = new Vector3 (), Vector3 t_InitialDirection = new Vector3 (), double t_Multiplier = 1,float t_DelayBetweenCoins = 0.5f) {

            MultipleCoinAddController t_NewMultipleCoinAddController = new MultipleCoinAddController (
                t_NumberOfCoin,
                t_CoinSpawnPosition,
                t_InitialPositionDisplacement,
                t_InitialDirection,
                t_Multiplier,
                true
            );
            StartCoroutine(t_NewMultipleCoinAddController.ControllerForAddingCoin(t_DelayBetweenCoins));
        }

        public void AddCoinToUser (Vector3 t_CoinSpawnPosition, Vector3 t_InitialPositionDisplacement = new Vector3 (), Vector3 t_InitialDirection = new Vector3 (), double t_Multiplier = 1, bool t_ByPassCoinSpawningRule = false) {
            double t_CoinToBeAdded = m_CoinEarnStateForLevel * t_Multiplier * (globalMonetizationStateControllerReference != null ? (globalMonetizationStateControllerReference.IsCoinEarnBoostEnabled () ? 2f : 1f) : 1f);
            m_NumberOfCoinStackedToUpdateInUI += t_CoinToBeAdded;

            if (!isMoveTowardsHUD) {
                m_NumberOfCoinEarnedOnThisLevel += t_CoinToBeAdded;

                GameManager.Instance.UpdateInGameCurrencyAnimated (t_CoinToBeAdded);
                //UIStateController.Instance.UIGameplayMenuControllerReference.UpdateCoinEarnForLevel (m_NumberOfCoinEarnedOnThisLevel);
            }

            if (m_IsCoinEarnAllow || t_ByPassCoinSpawningRule) {
                GameObject t_NewCoin = coinContainer.PullPreloadedPrefab ();
                Transform t_NewCoinTransform = t_NewCoin.transform;
                t_NewCoinTransform.position = t_CoinSpawnPosition;
                t_NewCoinTransform.eulerAngles = t_InitialDirection;
                t_NewCoinTransform.localScale = Vector3.zero;

                CoinInfo t_NewCoinInfo = new CoinInfo () {
                    isInitialDisplacementComplete = animationPosition == Vector3.zero ? true : false,
                    amountOfCoinToBeAdded = m_NumberOfCoinStackedToUpdateInUI,
                    coinContainerTransformReference = t_NewCoinTransform,
                    initialPositionOfDisplacement = t_CoinSpawnPosition + t_InitialPositionDisplacement,
                    initialDistanceFromDisplacement = Vector3.Distance (t_CoinSpawnPosition, t_CoinSpawnPosition + t_InitialPositionDisplacement),
                    initialDistanceFromDisplacementToTarget = isMoveTowardsHUD ? Vector3.Distance (t_CoinSpawnPosition + t_InitialPositionDisplacement + animationPosition, coinHUDTransformReference.position) : Vector3.Distance (t_CoinSpawnPosition, t_CoinSpawnPosition + animationPosition)
                };

                //Debug.Log("Initial Displacement : "  + (animationPosition == Vector3.zero ? Vector3.Distance(t_CoinSpawnPosition + t_InitialPositionDisplacement, isMoveTowardsHUD ? coinHUDTransformReference.position : t_CoinSpawnPosition) : Vector3.Distance(t_CoinSpawnPosition, t_CoinSpawnPosition + t_InitialPositionDisplacement)));
                m_ListOfCoinInfo.Add (t_NewCoinInfo);

                m_CurrentBufferTimeForCoinEarn = m_BufferTimeForCoinEarn;
                m_NumberOfCoinStackedToUpdateInUI = 0;
                m_IsCoinEarnAllow = false;
            }
        }

        public double GetNumberOfCoinEarnedInThisLevel () {

            return m_NumberOfCoinEarnedOnThisLevel;
        }

        #endregion
    }

}