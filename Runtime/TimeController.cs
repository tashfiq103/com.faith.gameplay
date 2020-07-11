namespace com.faith.gameplay
{
    using UnityEngine;
    using UnityEngine.Events;

    public class TimeController : MonoBehaviour
    {
        #region Public Variables

#if UNITY_EDITOR

        public bool showDebugPanel;

#endif

        public static TimeController Instance;

        [Header("Configuretion      :   Default")]
        public float defaultDurationForSlowDown = 2f;
        public float defaultSlowDownFactor = 0.5f;
        public AnimationCurve curveMultiplierForSlowDownFactor = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1f), new Keyframe(1, 0f) });

        [Space(5.0f)]
        public float defaultDurationForRestore = 2;
        public float defaultRestoreFactor = 1;
        public AnimationCurve curveMultiplierForRestoreFactor = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0f), new Keyframe(1, 1f) });

        #endregion

        #region Private Variables

        private float m_AbsoluteTimeScale = 1f;

        private float m_InitialTimeScaleOnGameStart = 1f;
        private float m_InitialFixedDeltaTimeOnGameStart = 0.02f;

        private float m_InitialTimeScale;
        private float m_InitialFixedDeltaTime;

        private float m_TargetedTimeScale;
        private float m_TargetedFixedDeltaTime;

        private float m_CurrentTime;
        private float m_TimeWhenTransationStart;
        private float m_TimeWhenTransationEnd;
        private float m_DurationForTimeTransation;

        private AnimationCurve progressiveCurveForTimeBending;

        private UnityAction OnTimeTransationEnd;

        #endregion

        #region Mono Behaviour

        private void Awake()
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

            m_InitialTimeScaleOnGameStart = Time.timeScale;
            m_InitialFixedDeltaTimeOnGameStart = Time.fixedDeltaTime;

            m_AbsoluteTimeScale = m_InitialTimeScaleOnGameStart;

            EnableAndDisableMonoBehaviour(false);
        }


        private void Update()
        {

            m_CurrentTime += (1f / Time.timeScale) * Time.deltaTime;
            if (m_CurrentTime < m_TimeWhenTransationEnd)
            {

                float t_Progress = (m_CurrentTime - m_TimeWhenTransationStart) / m_DurationForTimeTransation;
                m_AbsoluteTimeScale = Mathf.Lerp(
                        m_InitialTimeScale,
                        m_TargetedTimeScale,
                        progressiveCurveForTimeBending.Evaluate(t_Progress)
                    );
                Time.timeScale = m_AbsoluteTimeScale;

                Time.fixedDeltaTime = Mathf.Lerp(
                        m_InitialFixedDeltaTime,
                        m_TargetedFixedDeltaTime,
                        progressiveCurveForTimeBending.Evaluate(t_Progress)
                    );

            }
            else
            {

                Time.timeScale = m_TargetedTimeScale;
                Time.fixedDeltaTime = m_TargetedFixedDeltaTime;

                m_AbsoluteTimeScale = m_TargetedTimeScale;

                EnableAndDisableMonoBehaviour(false);

                OnTimeTransationEnd?.Invoke();
            }
        }

        #endregion

        #region Configuretion

        private void EnableAndDisableMonoBehaviour(bool t_IsEnable = true)
        {
            enabled = t_IsEnable;
        }

        private void SetConfiguretionForTimeBending(
            bool t_IsTransationFromCurrentTimeScale,
            float durationForSlowDown,
            float slowDownFactor,
            UnityAction OnTimeTransationEnd)
        {

            if (slowDownFactor == 0)
            {
                m_TargetedTimeScale = m_InitialTimeScaleOnGameStart * defaultSlowDownFactor;
                m_TargetedFixedDeltaTime = m_InitialFixedDeltaTimeOnGameStart * defaultSlowDownFactor;
            }
            else
            {

                m_TargetedTimeScale = m_InitialTimeScaleOnGameStart * slowDownFactor;
                m_TargetedFixedDeltaTime = m_InitialFixedDeltaTimeOnGameStart * slowDownFactor;
            }

            if (durationForSlowDown == 0)
            {
                m_DurationForTimeTransation = defaultDurationForSlowDown;
            }
            else
            {

                m_DurationForTimeTransation = durationForSlowDown;
            }

            m_CurrentTime = Time.time;
            m_TimeWhenTransationStart = m_CurrentTime;
            m_TimeWhenTransationEnd = m_CurrentTime + m_DurationForTimeTransation;

            m_InitialTimeScale = t_IsTransationFromCurrentTimeScale ? Time.timeScale : m_InitialTimeScaleOnGameStart;
            m_InitialFixedDeltaTime = t_IsTransationFromCurrentTimeScale ? Time.fixedDeltaTime : m_InitialFixedDeltaTimeOnGameStart;

            this.OnTimeTransationEnd = OnTimeTransationEnd;

            EnableAndDisableMonoBehaviour(true);
        }

        #endregion

        #region Public Callback

        public float GetAbsoluteTimeScale()
        {

            return m_AbsoluteTimeScale;
        }

        public float GetAbsoluteDeltaTime()
        {

            return Time.deltaTime * (1f / m_AbsoluteTimeScale);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slowDownFactor"> Make sure the value stays from [0,1]</param>
        /// <param name="durationForSlowDown"></param>
        /// <param name="t_IsTransationFromCurrentTimeScale"></param>
        public void DoSlowMotion(
            bool t_IsTransationFromCurrentTimeScale = true,
            float durationForSlowDown = 0f,
            float slowDownFactor = 0f,
            AnimationCurve progressiveCurveForSlowDownFactor = null,
            UnityAction OnTimeTransationEnd = null)
        {

            slowDownFactor = Mathf.Clamp01(slowDownFactor);

            if (progressiveCurveForSlowDownFactor == null)
                progressiveCurveForTimeBending = curveMultiplierForSlowDownFactor;
            else
                progressiveCurveForTimeBending = progressiveCurveForSlowDownFactor;

            SetConfiguretionForTimeBending(
                    t_IsTransationFromCurrentTimeScale,
                    durationForSlowDown,
                    slowDownFactor,
                    OnTimeTransationEnd
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fastForwardFactor">Make sure the value is greater than 1 </param>
        /// <param name="durationForFastForward"></param>
        /// <param name="t_IsTransationFromCurrentTimeScale"></param>
        public void DoFastMotion(
            bool t_IsTransationFromCurrentTimeScale = true,
            float durationForFastForward = 0f,
            float fastForwardFactor = 0f,
            AnimationCurve progressiveCurveForFastMotionFactor = null,
            UnityAction OnTimeTransationEnd = null)
        {

            fastForwardFactor = Mathf.Clamp(fastForwardFactor, 1, 2);

            if (progressiveCurveForFastMotionFactor == null)
                progressiveCurveForTimeBending = curveMultiplierForRestoreFactor;
            else
                progressiveCurveForTimeBending = progressiveCurveForFastMotionFactor;

            SetConfiguretionForTimeBending(
                    t_IsTransationFromCurrentTimeScale,
                    durationForFastForward,
                    fastForwardFactor,
                    OnTimeTransationEnd
                );
        }

        public void RestoreToInitialTimeScale(UnityAction OnTimeTransationEnd = null)
        {

            progressiveCurveForTimeBending = curveMultiplierForRestoreFactor;
            SetConfiguretionForTimeBending(
                    true,
                    defaultDurationForRestore,
                    m_InitialTimeScaleOnGameStart,
                    OnTimeTransationEnd
                );
        }

        #endregion
    }
}


