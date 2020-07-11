namespace com.faith.Gameplay
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public class PowerUpController : MonoBehaviour
    {
        #region Custom Variables

        [System.Serializable]
        public class PowerUp
        {
            public string skillName;
            [Tooltip("if 'isStakable = true', the power up going to be stack if you take again. Else, it will be reset")]
            public bool isStackable;
            [Range(0.0f, 20.0f)]
            public float durationOfPowerUp = 5f;

            [Space(5.0f)]
            [Header("Pre Warning State")]
            [Range(0.0f, 0.5f)]
            public float warningBeforeEndTime = 0.1f;

            [HideInInspector]
            public bool IsPreWarningEventTriggered = false;
            [HideInInspector]
            public float preEndTimeForPowerUp;
            [HideInInspector]
            public float remainingTimeForPowerUp = 0f;

            [HideInInspector]
            public UnityAction OnPowerUpEnd;
            [HideInInspector]
            public UnityAction OnPreWarningForEndOfPowerUp;
        }

        #endregion

        #region Public Variables

        public PowerUp[] powerUp;

        #endregion

        #region Private Variables

        private bool        m_IsPowerUpLifeCycleControllerRunning;

        private int         m_NumberOfPowerUp;

        #endregion

        #region Mono Behaviour

        private void Awake()
        {
            m_NumberOfPowerUp = powerUp.Length;
            for (int powerUpIndex = 0; powerUpIndex < m_NumberOfPowerUp; powerUpIndex++)
            {
                powerUp[powerUpIndex].preEndTimeForPowerUp = powerUp[powerUpIndex].durationOfPowerUp - ((1f - powerUp[powerUpIndex].warningBeforeEndTime) * powerUp[powerUpIndex].durationOfPowerUp);
            }
        }

        #endregion

        #region Configuretion

        private bool IsValidPowerUp(int t_PowerUpIndex) {

            if (t_PowerUpIndex >= 0 && t_PowerUpIndex < m_NumberOfPowerUp)
                return true;

            Debug.LogError("Invalid PowerUp Index");
            return false;
        }

        private IEnumerator ControllerForPowerUpLifeCycleController() {

            float t_CycleLength = 0.0167f;
            WaitForSeconds t_CycleDelay = new WaitForSeconds(t_CycleLength);

            int t_NumberOfActivePowerUp;

            while (m_IsPowerUpLifeCycleControllerRunning) {

                t_NumberOfActivePowerUp = 0;

                for (int powerUpIndex = 0; powerUpIndex < m_NumberOfPowerUp; powerUpIndex++) {

                    if (powerUp[powerUpIndex].remainingTimeForPowerUp > 0) {

                        if (!powerUp[powerUpIndex].IsPreWarningEventTriggered && (powerUp[powerUpIndex].remainingTimeForPowerUp <= powerUp[powerUpIndex].warningBeforeEndTime))
                        {

                            powerUp[powerUpIndex].IsPreWarningEventTriggered = true;

                            if (powerUp[powerUpIndex].OnPreWarningForEndOfPowerUp != null)
                            {
                                powerUp[powerUpIndex].OnPreWarningForEndOfPowerUp.Invoke();
                                powerUp[powerUpIndex].OnPreWarningForEndOfPowerUp = null;
                            }
                        }

                        powerUp[powerUpIndex].remainingTimeForPowerUp -= t_CycleLength;

                        if (powerUp[powerUpIndex].remainingTimeForPowerUp <= 0) {

                            if (powerUp[powerUpIndex].OnPowerUpEnd != null)
                            {
                                powerUp[powerUpIndex].OnPowerUpEnd.Invoke();
                                powerUp[powerUpIndex].OnPowerUpEnd = null;
                            }
                        }

                        t_NumberOfActivePowerUp++;
                    }
                }

                if (t_NumberOfActivePowerUp == 0) {
                    break;
                }
                    

                yield return t_CycleDelay;
            }

            StopCoroutine(ControllerForPowerUpLifeCycleController());
            m_IsPowerUpLifeCycleControllerRunning = false;
        }

        #endregion

        #region Public Callback

        public bool IsPowerUpActive(int t_PowerUpIndex) {

            if (IsValidPowerUp(t_PowerUpIndex)) {

                if (powerUp[t_PowerUpIndex].remainingTimeForPowerUp > 0)
                    return true;
            }

            return false;
        }

        public void ActivePowerUp(
            int t_PowerUpIndex,
            UnityAction OnPowerUpStart = null,
            UnityAction OnPreWarningForEndOfPowerUp = null,
            UnityAction  OnPowerUpEnd = null) {

            if (IsValidPowerUp(t_PowerUpIndex)) {

                if (OnPowerUpStart != null)
                    OnPowerUpStart.Invoke();

                powerUp[t_PowerUpIndex].IsPreWarningEventTriggered = false;
                powerUp[t_PowerUpIndex].OnPreWarningForEndOfPowerUp = OnPreWarningForEndOfPowerUp;
                powerUp[t_PowerUpIndex].OnPowerUpEnd                = OnPowerUpEnd;

                if (IsPowerUpActive(t_PowerUpIndex) && powerUp[t_PowerUpIndex].isStackable)
                    powerUp[t_PowerUpIndex].remainingTimeForPowerUp += powerUp[t_PowerUpIndex].durationOfPowerUp;
                else
                    powerUp[t_PowerUpIndex].remainingTimeForPowerUp = powerUp[t_PowerUpIndex].durationOfPowerUp;

                if (!m_IsPowerUpLifeCycleControllerRunning) {

                    m_IsPowerUpLifeCycleControllerRunning = true;
                    StartCoroutine(ControllerForPowerUpLifeCycleController());
                }
            }
        }

        public void ResetPowerUp() {

            for (int powerUpIndex = 0; powerUpIndex < m_NumberOfPowerUp; powerUpIndex++) {

                powerUp[powerUpIndex].remainingTimeForPowerUp = 0;
            }
        }

        #endregion

    }
}


