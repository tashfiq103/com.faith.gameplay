namespace com.faith.GameplayService
{
    using UnityEngine;
    using com.faith.Gameplay;

    public class TutorialController : MonoBehaviour
    {
        #region Public Variables

        public static TutorialController Instance;

        [Header("Reference  :   External")]
        public LevelManager levelManagerReference;

        [Space(5.0f)]
        [Range(1, 5)]
        public int showTutorialUntillLevel = 3;
        public Animator tutorialAnimator;

        #endregion

        #region Private Variables

        private bool m_IsTutorialShowing = false;
        private bool m_IsTutorialDisappearByUserInput;

        #endregion

        #region Mono Behaviour

        private void Awake()
        {
            if (Instance == null)
            {

                Instance = this;
            }
        }

        #endregion

        #region Configuretion

        private void OnTouchDown(Vector3 t_TouchPosition) {

            HideTutorial();
        }

        #endregion

        #region Public Callback

        public void ShowTutorial(bool t_IsTutorialDisappearByUserInput = false)
        {

            m_IsTutorialDisappearByUserInput = t_IsTutorialDisappearByUserInput;

            if (levelManagerReference != null)
            {

                int t_CurrentLevel = levelManagerReference.GetCurrentLevel();
                if (!m_IsTutorialShowing && t_CurrentLevel < showTutorialUntillLevel)
                {

                    m_IsTutorialShowing = true;
                    tutorialAnimator.SetTrigger("APPEAR");

                    if(m_IsTutorialDisappearByUserInput)
                        GlobalTouchController.Instance.OnTouchDown += OnTouchDown;
                }
            }
            else {

                if (!m_IsTutorialShowing)
                {

                    m_IsTutorialShowing = true;
                    tutorialAnimator.SetTrigger("APPEAR");

                    if (m_IsTutorialDisappearByUserInput)
                        GlobalTouchController.Instance.OnTouchDown += OnTouchDown;
                }
            }

            
        }

        public void HideTutorial()
        {

            if (m_IsTutorialShowing)
            {

                m_IsTutorialShowing = false;
                tutorialAnimator.SetTrigger("DISAPPEAR");

                if (m_IsTutorialDisappearByUserInput)
                    GlobalTouchController.Instance.OnTouchDown -= OnTouchDown;
            }
        }

        #endregion

    }
}

