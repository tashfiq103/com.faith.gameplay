namespace com.faith.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    using com.faith.Math;
    using com.faith.GameplayService;

    public class PointEarningController : MonoBehaviour
    {
        #region Custom Variables

        private struct PointInfo
        {

            public bool         isInitialDisplacementComplete;
            public double       amountOfPointToBeAdded;
            public float        initialDistanceFromDisplacement;
            public float        initialDistanceFromDisplacementToTarget;
            public Transform    pointContainerTransformReference;
            public Vector3      initialPositionOfDisplacement;

        }

        #endregion

        #region Public Variables

        public static PointEarningController Instance;

        public PreLoadedPrefab pointContainer;

        [Space(5.0f)]
        public Transform    transformRefereneOfWorldSpaceCanvas;
        public Transform    transformReferenceOfCamera;
        public Vector3      animationPosition;

        [Range(0f, 100f)]
        public float pointMovementSpeed = 10;
        [Space(5.0f)]
        [Header("(Optional) : Monetization")]
        public GlobalMonetizationStateController globalMonetizationStateControllerReference;

        #endregion

        #region Private Variables

        private bool    m_IsPointEarnControllerRunning;

        private double  m_NumberOfPointEarnedOnThisLevel;
        private double  m_NumberOfPointStackedToUpdateInUI;
        private double  m_PointEarnStateForLevel;

        private List<PointInfo> m_ListOfCoinInfo;

        #endregion

        #region Mono Behaviour

        private void Awake()
        {
            Instance = this;
        }

        #endregion

        #region Configuretion

        private IEnumerator ControllerForPointEarn()
        {
            float t_DeltaTime;
            float t_CycleLength = 0.0167f;
            WaitForSeconds t_CycleDelay = new WaitForSeconds(t_CycleLength);

            int t_NumberOfAvailableCoinInfo = 0;
            float t_CoinPositionCheckBoundary = pointMovementSpeed * 0.025f;
            float t_ScaleValue;
            float t_CurrentDistance;
            Vector3 t_ZeroScale = Vector3.zero;
            Vector3 t_OneScale = Vector3.one;
            Vector3 t_TargetedPosition;
            Vector3 t_ModifiedPosition;
            Vector3 t_ModifiedScale;
            PointInfo t_SelectedCoinInfoReference;

            while (m_IsPointEarnControllerRunning || t_NumberOfAvailableCoinInfo > 0)
            {

                t_DeltaTime = Time.deltaTime;

                t_NumberOfAvailableCoinInfo = m_ListOfCoinInfo.Count;
                for (int counter = 0; counter < t_NumberOfAvailableCoinInfo; counter++)
                {

                    t_SelectedCoinInfoReference = m_ListOfCoinInfo[counter];
                    if (!t_SelectedCoinInfoReference.isInitialDisplacementComplete)
                    {

                        t_ModifiedPosition = Vector3.MoveTowards(
                                t_SelectedCoinInfoReference.pointContainerTransformReference.position,
                                t_SelectedCoinInfoReference.initialPositionOfDisplacement,
                                pointMovementSpeed * t_DeltaTime
                            );

                        t_CurrentDistance = Vector3.Distance(t_SelectedCoinInfoReference.initialPositionOfDisplacement, t_ModifiedPosition);

                        t_ModifiedScale = Vector3.Lerp(
                                t_ZeroScale,
                                t_OneScale,
                                1f - (t_CurrentDistance / t_SelectedCoinInfoReference.initialDistanceFromDisplacement)
                            );

                        t_SelectedCoinInfoReference.pointContainerTransformReference.localScale = t_ModifiedScale;

                        if (t_CurrentDistance <= t_CoinPositionCheckBoundary)
                        {

                            t_SelectedCoinInfoReference.pointContainerTransformReference.position = t_SelectedCoinInfoReference.initialPositionOfDisplacement;
                            t_SelectedCoinInfoReference.pointContainerTransformReference.localScale = t_OneScale;
                            t_SelectedCoinInfoReference.initialDistanceFromDisplacementToTarget = Vector3.Distance(t_ModifiedPosition + animationPosition, t_ModifiedPosition);
                            t_SelectedCoinInfoReference.isInitialDisplacementComplete = true;
                            m_ListOfCoinInfo[counter] = t_SelectedCoinInfoReference;
                        }
                        else
                        {

                            t_SelectedCoinInfoReference.pointContainerTransformReference.position = t_ModifiedPosition;
                        }
                    }
                    else
                    {
                        t_TargetedPosition = t_SelectedCoinInfoReference.initialPositionOfDisplacement + animationPosition;

                        t_ModifiedPosition = Vector3.MoveTowards(
                                t_SelectedCoinInfoReference.pointContainerTransformReference.position,
                                t_TargetedPosition,
                                pointMovementSpeed * 2 * t_DeltaTime
                            );

                        t_CurrentDistance = Vector3.Distance(t_TargetedPosition, t_ModifiedPosition);
                        t_ScaleValue = t_CurrentDistance / t_SelectedCoinInfoReference.initialDistanceFromDisplacementToTarget;
                        t_ScaleValue = t_ScaleValue > 1 ? 0f : 1f - t_ScaleValue;

                        t_ModifiedScale = Vector3.Lerp(
                                t_OneScale,
                                t_ZeroScale,
                                t_ScaleValue
                         );

                        if (t_CurrentDistance <= t_CoinPositionCheckBoundary)
                        {
                            t_SelectedCoinInfoReference.pointContainerTransformReference.gameObject.SetActive(false);

                            m_ListOfCoinInfo.RemoveAt(counter);
                            t_NumberOfAvailableCoinInfo--;

                            pointContainer.PushPreloadedPrefab(t_SelectedCoinInfoReference.pointContainerTransformReference.gameObject);
                        }
                        else
                        {

                            t_SelectedCoinInfoReference.pointContainerTransformReference.position = t_ModifiedPosition;
                            t_SelectedCoinInfoReference.pointContainerTransformReference.localScale = t_ModifiedScale;

                        }
                    }
                }

                yield return t_CycleDelay;
            }

            StopCoroutine(ControllerForPointEarn());

        }

        #endregion

        #region Public Callback

        public void PreProcess(double t_PointEarnStateForLevel, float t_UpgradeProgressionOfPointEarn = 0)
        {

            m_PointEarnStateForLevel = t_PointEarnStateForLevel;

            if (!m_IsPointEarnControllerRunning)
            {
                m_NumberOfPointEarnedOnThisLevel = 0;
                m_NumberOfPointStackedToUpdateInUI = 0;
                m_IsPointEarnControllerRunning = true;
                m_ListOfCoinInfo = new List<PointInfo>();
                StartCoroutine(ControllerForPointEarn());

            }
        }

        public void PostProcess()
        {
            m_IsPointEarnControllerRunning = false;
        }

        public void AddPointToUser(Vector3 t_CoinSpawnPosition, Vector3 t_InitialPositionDisplacement, double t_Multiplier = 1, Color t_PointTextColor = new Color())
        {
            double t_CoinToBeAdded = m_PointEarnStateForLevel * t_Multiplier * (globalMonetizationStateControllerReference != null ? (globalMonetizationStateControllerReference.IsCoinEarnBoostEnabled() ? 2f : 1f) : 1f);

            m_NumberOfPointStackedToUpdateInUI += t_CoinToBeAdded;
            m_NumberOfPointEarnedOnThisLevel += t_CoinToBeAdded;

            //AudioController.Instance.PlaySoundFXForCoinEarn();
            //UIStateController.Instance.UpdateUIForCoinEarn(m_NumberOfPointEarnedOnThisLevel);

            GameObject t_NewCoin = pointContainer.PullPreloadedPrefab();
            Transform t_NewCoinTransform = t_NewCoin.transform;
            t_NewCoinTransform.SetParent(transformRefereneOfWorldSpaceCanvas);
            t_NewCoinTransform.position = t_CoinSpawnPosition;
            t_NewCoinTransform.rotation = Quaternion.LookRotation(t_CoinSpawnPosition - transformReferenceOfCamera.position);
            t_NewCoinTransform.localScale = Vector3.zero;

            //SettingPoint
            if (t_NewCoin.GetComponent<TextMeshProUGUI>())
            {
                t_NewCoin.GetComponent<TextMeshProUGUI>().text = MathFunction.Instance.GetCurrencyInFormatInNonDecimal(t_CoinToBeAdded)+ "$";
                if (t_PointTextColor != new Color(0, 0, 0, 0))
                    t_NewCoin.GetComponent<TextMeshProUGUI>().color = t_PointTextColor;
            }
            else if (t_NewCoin.GetComponent<TextMeshPro>()) {

                t_NewCoin.GetComponent<TextMeshPro>().text = MathFunction.Instance.GetCurrencyInFormatInNonDecimal(t_CoinToBeAdded) + "$";
                if (t_PointTextColor != new Color(0, 0, 0, 0))
                    t_NewCoin.GetComponent<TextMeshPro>().color = t_PointTextColor;
            }
            

            PointInfo t_NewCoinInfo = new PointInfo()
            {
                isInitialDisplacementComplete = false,
                amountOfPointToBeAdded = m_NumberOfPointStackedToUpdateInUI,
                pointContainerTransformReference = t_NewCoinTransform,
                initialPositionOfDisplacement = t_CoinSpawnPosition + t_InitialPositionDisplacement,
                initialDistanceFromDisplacement = Vector3.Distance(t_CoinSpawnPosition, t_CoinSpawnPosition + t_InitialPositionDisplacement)
            };

            m_ListOfCoinInfo.Add(t_NewCoinInfo);

            m_NumberOfPointStackedToUpdateInUI = 0;
        }

        public double GetNumberOfPointEarnedInThisLevel()
        {
            return m_NumberOfPointEarnedOnThisLevel;
        }

        #endregion
    }
}


