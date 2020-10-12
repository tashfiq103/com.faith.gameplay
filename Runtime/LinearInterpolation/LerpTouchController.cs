namespace com.faith.gameplay {

    using UnityEngine;
    using com.faith.gameplay.service;

    public class LerpTouchController : MonoBehaviour
    {

        #region Public Variables

        [Space(5.0f)]
        [Range(0f, 1f)]
        public float scrollSpeedOnHorizontal = 0.01f;
        [Range(0f, 1f)]
        public float scrollSpeedOnVertical = 0.01f;

        [Space(5.0f)]
        public LerpWaypointController[] lerpEventControllerForHorizontal;
        [Space(5.0f)]
        public LerpWaypointController[] lerpEventControllerForVertical;
        #endregion

        #region private Variables

        private bool m_IsLerpTouchControllerRunning;
        private bool m_IsGlobalTouchControllerAdded;

        private int m_NumberOfLerpEventControllerAtHorizontal;
        private int m_NumberOfLerpEventControllerAtVertical;
        
        private float m_InitialScrollSpeedOnHorizontal;
        private float m_InitialScrollSpeedOnVertical;
        private float m_LerpValueOnHorizontal;
        private float m_LerpValueOnVertical;

        private Vector3 m_PreviousTouchPosition;

        private Vector3 m_TouchDownPosition;
        private Vector3 m_TouchPosition;
        private Vector3 m_TouchUpPosition;


        #endregion

        #region Mono Behaviour

        private void Awake()
        {
            m_InitialScrollSpeedOnHorizontal = scrollSpeedOnHorizontal;
            m_InitialScrollSpeedOnVertical = scrollSpeedOnVertical;
        }

        private void Start()
        {
            PreProcess();
        }

        #endregion

        #region Configuretion

        private void PreProcess() {

            m_NumberOfLerpEventControllerAtHorizontal   = lerpEventControllerForHorizontal.Length;
            m_NumberOfLerpEventControllerAtVertical     = lerpEventControllerForVertical.Length;

            m_LerpValueOnHorizontal = 0.5f;
            m_LerpValueOnVertical = 0.5f;

        }

        private void OnTouchDown(Vector3 t_TouchPosition, int touchIndex) {

            m_TouchDownPosition = t_TouchPosition;
            m_PreviousTouchPosition = t_TouchPosition;

        }

        private void OnTouch(Vector3 t_TouchPosition, int touchIndex) {

            m_TouchPosition = t_TouchPosition;

            //Horizontal
            if (m_TouchPosition.x > m_PreviousTouchPosition.x)
            {
                m_LerpValueOnHorizontal = Mathf.MoveTowards(
                        m_LerpValueOnHorizontal,
                        1f,
                        scrollSpeedOnHorizontal
                    );
            }
            else if (m_TouchPosition.x < m_PreviousTouchPosition.x)
            {
                m_LerpValueOnHorizontal = Mathf.MoveTowards(

                        m_LerpValueOnHorizontal,
                        0f,
                        scrollSpeedOnHorizontal
                    );
            }

            //Vertical
            if (m_TouchPosition.y > m_PreviousTouchPosition.y)
            {
                m_LerpValueOnVertical = Mathf.MoveTowards(
                        m_LerpValueOnVertical,
                        1f,
                        scrollSpeedOnVertical
                    );
            }
            else if (m_TouchPosition.y < m_PreviousTouchPosition.y)
            {
                m_LerpValueOnVertical = Mathf.MoveTowards(
                        m_LerpValueOnVertical,
                        0f,
                        scrollSpeedOnVertical
                    );
            }
            
            for (int counter = 0; counter < m_NumberOfLerpEventControllerAtHorizontal; counter++)
            {

                lerpEventControllerForHorizontal[counter].LerpThroughPositions(m_LerpValueOnHorizontal);
            }

            for (int counter = 0; counter < m_NumberOfLerpEventControllerAtVertical; counter++)
            {

                lerpEventControllerForVertical[counter].LerpThroughPositions(m_LerpValueOnVertical);
            }

            m_PreviousTouchPosition = m_TouchPosition;

        }

        private void OnTouchUp(Vector3 t_TouchPosition, int touchIndex) {

            m_TouchUpPosition = t_TouchPosition;
        }

        #endregion

        #region Public Callback

        public void SnapToLerpPoint() {

            for (int counter = 0; counter < m_NumberOfLerpEventControllerAtHorizontal; counter++)
            {

                lerpEventControllerForHorizontal[counter].SnapToLerpPoint(m_LerpValueOnHorizontal);
            }

            for (int counter = 0; counter < m_NumberOfLerpEventControllerAtVertical; counter++)
            {

                lerpEventControllerForVertical[counter].SnapToLerpPoint(m_LerpValueOnVertical);
            }
        }

        public void ResetLerpValue(float t_Point) {

            if (t_Point >= 0 && t_Point <= 1)
            {
                m_LerpValueOnHorizontal = t_Point;
                m_LerpValueOnVertical = t_Point;
            }
            else {

                Debug.LogError("Invalid Request of lerp point");
            }
        }

        public void ResetScrollSpeed() {

            scrollSpeedOnHorizontal = m_InitialScrollSpeedOnHorizontal;
            scrollSpeedOnVertical   = m_InitialScrollSpeedOnVertical;
        }

        public void ChangeScrollSpeed(float t_Value) {

            scrollSpeedOnHorizontal = t_Value;
            scrollSpeedOnVertical = t_Value;
        }

        public void ChangeScrollSpeedOfHorizontal(float t_Value) {

            scrollSpeedOnHorizontal = t_Value;
        }

        public void ChangeScrollSpeedOnVertical(float t_Value) {

            scrollSpeedOnVertical = t_Value;
        }

        public void EnableLerpTouchController(bool t_IsGlobalTouchControllerAdded = true, bool t_IsRecalculateTouchController = false, bool t_IsRecalculateWaypointController = false) {

            m_IsGlobalTouchControllerAdded = t_IsGlobalTouchControllerAdded;
            if (m_IsGlobalTouchControllerAdded) {
                AddGlobalTouchController();
            }

            if (t_IsRecalculateTouchController)
                PreProcess();

            if (t_IsRecalculateWaypointController) {

                for (int counter = 0; counter < m_NumberOfLerpEventControllerAtHorizontal; counter++)
                {

                    lerpEventControllerForHorizontal[counter].EnableMovement(true);
                }

                for (int counter = 0; counter < m_NumberOfLerpEventControllerAtVertical; counter++)
                {

                    lerpEventControllerForVertical[counter].EnableMovement(true);
                }
            }

            

            m_IsLerpTouchControllerRunning = true;
        }

        public void DisableLerpTouchController() {

            if (m_IsGlobalTouchControllerAdded) {

                m_IsGlobalTouchControllerAdded = false;
                RemoveGlobalTouchController();
            }

            m_IsLerpTouchControllerRunning = false;

            for (int counter = 0; counter < m_NumberOfLerpEventControllerAtHorizontal; counter++)
            {

                lerpEventControllerForHorizontal[counter].DisableMovement();
            }

            for (int counter = 0; counter < m_NumberOfLerpEventControllerAtVertical; counter++)
            {

                lerpEventControllerForVertical[counter].DisableMovement();
            }
        }

        public void AddGlobalTouchController() {

            GlobalTouchController.Instance.OnTouchDown += OnTouchDown;
            GlobalTouchController.Instance.OnTouch += OnTouch;
            GlobalTouchController.Instance.OnTouchUp += OnTouchUp;
        }

        public void RemoveGlobalTouchController() {

            GlobalTouchController.Instance.OnTouchDown -= OnTouchDown;
            GlobalTouchController.Instance.OnTouch -= OnTouch;
            GlobalTouchController.Instance.OnTouchUp -= OnTouchUp;
        }

        #endregion
    }
}


