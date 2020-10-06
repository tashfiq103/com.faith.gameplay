namespace com.faith.gameplay
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Events;

    using com.faith.core;
    using com.faith.gameplay.service;

    public class PlayerControllerOnNavMesh : MonoBehaviour
    {

        #region Editor

#if UNIT_EDITOR

	public bool showOutline;

#endif

        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (areaOfInnerBorderOutline > areaOfOuterBorderOutline)
            {
                areaOfInnerBorderOutline = areaOfOuterBorderOutline;
            }

            if (m_JoyesticTransformReference == null)
            {

                if (visualSetOfController != null)
                {
                    m_JoyesticTransformReference = visualSetOfController.transform;
                }
                else
                {

                    m_JoyesticTransformReference = transform;
                }
            }
            else
            {

                if (visualSetOfController != null
                    && m_JoyesticTransformReference != visualSetOfController.transform)
                {
                    m_JoyesticTransformReference = visualSetOfController.transform;
                }
            }

            if (m_JoyesticTransformReference != null)
            {

                if (pointer != null)
                {

                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(
                        pointer.position,
                        areaOfInnerBorderOutline
                    );
                }

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(
                    m_JoyesticTransformReference.position,
                    areaOfInnerBorderOutline
                );

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(
                    m_JoyesticTransformReference.position,
                    areaOfOuterBorderOutline
                );
            }
        }

        #endregion

        //----------

        #region Public Variables

        public NavMeshAgent playerNavMeshReference;


        [Space(5.0f)]
        public float maxVelocity;
        public AnimationCurve velocityCurve;

        [Space(5.0f)]
        [Header("Config : Visual Representation")]
        public bool showControllerOnTouch;
        public Camera orthographicCamera;
        public GameObject visualSetOfController;

        [Space(5.0f)]
        [Header("Config : Outer Border Outline")]
        [Range(0.0f, 5.0f)]
        public float areaOfOuterBorderOutline;
        public Sprite outerBorderOutlineImage;
        public SpriteRenderer outerBorderOutlineSR;

        [Space(5.0f)]
        [Header("Config : Inner Border Outline")]
        [Range(0.0f, 5.0f)]
        public float areaOfInnerBorderOutline;
        public Sprite innerBorderOutlineImage;
        public SpriteRenderer innerBorderOutlineSR;

        [Space(5.0f)]
        [Header("Config : Pointer")]
        public Transform pointer;
        public Sprite pointerImage;
        public SpriteRenderer pointerSR;

        [Space(5.0f)]
        public UnityEvent OnTouchDownEvent;
        public UnityEvent OnTouchUpEvent;

        #endregion

        #region Private Variables

        private float m_ZCordPosition = 1f;

        private bool m_IsTouchControllerEnabled;
        private bool m_IsTouchActive;

        private Vector3 m_PreviousPosition;
        private Vector3 m_TouchDownPosition;
        private Vector3 m_TouchPosition;
        private Vector3 m_TouchUpPosition;

        //---------------------------------
        private Transform m_JoyesticTransformReference;

        private bool m_IsJoyesticControlEnable = true;
        private bool m_IsJoyesticControlActive;

        private float m_ModifiedVelocity;
        private float m_DifferenceInBoundary;

        //---------------------------------
        private Vector2 m_OrthographicSize;
        private Vector2 m_ScreenResolution;

        //---------------------------------
        private Transform m_PlayerTransformReference;

        #endregion

        #region Mono Behaviour

        private void Awake()
        {

            m_PlayerTransformReference = playerNavMeshReference.transform;

            m_IsTouchActive = false;

            m_JoyesticTransformReference = visualSetOfController.transform;

            if (visualSetOfController.activeInHierarchy)
            {
                visualSetOfController.SetActive(false);
            }

            m_DifferenceInBoundary = areaOfOuterBorderOutline - areaOfInnerBorderOutline;
        }

        private void Start()
        {

            if (DeviceInfoManager.Instance.IsPortraitMode())
            {

                m_OrthographicSize = new Vector2(
                        orthographicCamera.orthographicSize / DeviceInfoManager.Instance.GetAspectRatioFactor(),
                        orthographicCamera.orthographicSize
                    );

            }
            else
            {

                m_OrthographicSize = new Vector2(
                        orthographicCamera.orthographicSize,
                        orthographicCamera.orthographicSize / DeviceInfoManager.Instance.GetAspectRatioFactor()
                    );
            }

            m_ScreenResolution = new Vector2(
                    Screen.width,
                    Screen.height
                );


        }

        private void Update()
        {
            if (IsTouchControllerEnabled())
            {

                TouchStateController();

            }
        }

        #endregion

        #region Configuretion   :   Joyestic

        private Vector3 GetTouchToScreenPoint(Vector2 t_TouchPosition)
        {

            float t_PositionX = 0;
            float t_PositionY = 0;

            Vector2 t_PositionInPercentile = new Vector2(
                    t_TouchPosition.x / m_ScreenResolution.x,
                    t_TouchPosition.y / m_ScreenResolution.y
                );

            if (t_PositionInPercentile.x <= 0.5f)
            {
                t_PositionX = (0.5f - t_PositionInPercentile.x) * 2f * m_OrthographicSize.x;
                t_PositionX *= -1f;
            }
            else
            {

                t_PositionX = (t_PositionInPercentile.x - 0.5f) * 2f * m_OrthographicSize.x;
            }

            if (t_PositionInPercentile.y <= 0.5f)
            {
                t_PositionY = (0.5f - t_PositionInPercentile.y) * 2f * m_OrthographicSize.y;
                t_PositionY *= -1f;
            }
            else
            {

                t_PositionY = (t_PositionInPercentile.y - 0.5f) * 2f * m_OrthographicSize.y;
            }



            Vector3 t_Result = new Vector3(
                    t_PositionX,
                    t_PositionY,
                    m_ZCordPosition
                );

            return t_Result;
        }

        private IEnumerator JoyesticController()
        {

            WaitForEndOfFrame t_CycleDelay = new WaitForEndOfFrame();

            Vector2 t_JoyesticDirectionVector = Vector2.zero;
            Vector2 t_JoyesticModifiedPosition = Vector2.zero;
            Vector2 t_RequestedPosition = Vector2.zero;
            Vector3 t_TouchToScreenPoint;

            float t_CurrentDistanceFromPointer = 0.0f;
            float t_RotationInRadian = 0.0f;

            float t_DistanceFromInncerBoundary = 0.0f;
            float t_VelocityCurveValue = 0;

            while (m_IsJoyesticControlActive)
            {

                t_TouchToScreenPoint = GetTouchToScreenPoint(m_TouchPosition);

                t_CurrentDistanceFromPointer = Vector3.Distance(m_JoyesticTransformReference.localPosition, t_TouchToScreenPoint);

                if (t_CurrentDistanceFromPointer <= areaOfInnerBorderOutline)
                {
                    pointer.localPosition = new Vector3(
                            0f,
                            0f,
                            m_ZCordPosition
                        );
                    m_ModifiedVelocity = 0.0f;
                }
                else
                {

                    if (m_TouchDownPosition != m_TouchPosition)
                    {
                        t_JoyesticDirectionVector = MathFunction.GetUnitVector(
                            m_TouchDownPosition,
                            m_TouchPosition
                        );
                    }
                    else
                    {

                        t_JoyesticDirectionVector = Vector2.zero;
                    }

                    t_DistanceFromInncerBoundary = Vector3.Distance(m_JoyesticTransformReference.localPosition, pointer.localPosition) - areaOfInnerBorderOutline;

                    if (t_CurrentDistanceFromPointer >= areaOfInnerBorderOutline && t_CurrentDistanceFromPointer < areaOfOuterBorderOutline)
                    {

                        t_VelocityCurveValue = (t_CurrentDistanceFromPointer - areaOfInnerBorderOutline) / m_DifferenceInBoundary;

                        pointer.localPosition = t_TouchToScreenPoint - m_JoyesticTransformReference.localPosition;
                        m_ModifiedVelocity = velocityCurve.Evaluate(t_VelocityCurveValue) * maxVelocity;
                    }
                    else
                    {
                        t_RotationInRadian = MathFunction.GetRotationInRadian(t_JoyesticDirectionVector);

                        t_JoyesticModifiedPosition = new Vector2(
                            (Mathf.Cos(t_RotationInRadian) * areaOfOuterBorderOutline),
                            (Mathf.Sin(t_RotationInRadian) * areaOfOuterBorderOutline)
                        );

                        pointer.localPosition = t_JoyesticModifiedPosition;
                        m_ModifiedVelocity = maxVelocity;
                    }
                }

                yield return t_CycleDelay;
            }

            StopCoroutine(JoyesticController());
        }

        #endregion

        #region Configuretion   : Touch Controller

        private void TouchStateController()
        {

#if UNITY_EDITOR

            if (Input.GetMouseButtonDown(0))
            {
                OnTouchDown(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                OnTouch(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnTouchUp(Input.mousePosition);
            }

#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0) {

            switch (Input.GetTouch(0).phase) {

                case TouchPhase.Began:
                    OnTouchDown(Input.GetTouch(0).position);
                    break;
                case TouchPhase.Stationary:
                    OnTouch(Input.GetTouch(0).position);
                    break;
                case TouchPhase.Moved:
                    OnTouch(Input.GetTouch(0).position);
                    break;
                case TouchPhase.Ended:
                    OnTouchUp(Input.GetTouch(0).position);
                    break;
                case TouchPhase.Canceled:
                    OnTouchUp(Input.GetTouch(0).position);
                    break;
            }
        }
#endif

        }

        private void OnTouchDown(Vector2 t_TouchPosition)
        {

            m_PreviousPosition = m_TouchPosition;
            m_TouchDownPosition = new Vector3(
                    t_TouchPosition.x,
                    t_TouchPosition.y,
                    m_ZCordPosition
                );


            m_JoyesticTransformReference.localPosition = GetTouchToScreenPoint(t_TouchPosition);
            pointer.localPosition = new Vector3(
                    0f,
                    0f,
                    m_ZCordPosition
                );

            if (showControllerOnTouch &&
                !visualSetOfController.activeInHierarchy)
            {

                visualSetOfController.SetActive(true);
            }

            m_IsJoyesticControlActive = true;
            StartCoroutine(JoyesticController());

            m_IsTouchActive = true;

            OnTouchDownEvent.Invoke();
        }

        private void OnTouch(Vector2 t_TouchPosition)
        {

            if (m_PreviousPosition != m_TouchPosition)
                m_PreviousPosition = m_TouchPosition;

            m_TouchPosition = new Vector3(
                    t_TouchPosition.x,
                    t_TouchPosition.y,
                    m_ZCordPosition
                ); ;
        }

        private void OnTouchUp(Vector2 t_TouchPosition)
        {

            m_TouchUpPosition = new Vector3(
                    t_TouchPosition.x,
                    t_TouchPosition.y,
                    m_ZCordPosition
                ); ;

            if (showControllerOnTouch &&
                visualSetOfController.activeInHierarchy)
            {

                pointer.localPosition = Vector2.zero;
                visualSetOfController.SetActive(false);
            }

            m_IsJoyesticControlActive = false;

            m_IsTouchActive = false;

            OnTouchUpEvent.Invoke();
        }

        #endregion


        #region Configuretion   :   Player Movement

        private Vector3 GetDirectionalVector()
        {
            //return Vector3.Normalize(m_TouchPosition - m_TouchDownPosition);
            return Vector3.Normalize(m_TouchPosition - m_PreviousPosition);
        }

        private IEnumerator PlayerMovementController()
        {

            float t_CycleLength = 0.0167f;
            WaitForSeconds t_CycleDelay = new WaitForSeconds(t_CycleLength);
            WaitUntil t_WaitUntilTouchActive = new WaitUntil(() =>
            {

                if (m_IsTouchActive)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            playerNavMeshReference.enabled = true;

            float t_PlayerMovementSpeed;

            Vector3 t_CurrentPosition = Vector3.zero;
            Vector3 t_DirectionalVector;
            Vector3 t_NextPosition;

            while (IsTouchControllerEnabled())
            {

                t_PlayerMovementSpeed = GetPlayerMovementSpeed();

                t_CurrentPosition = playerNavMeshReference.destination;

                t_DirectionalVector = (GetDirectionalVector() * 1);
                Debug.Log("Direction Vector : " + t_DirectionalVector);
                t_NextPosition = new Vector3(
                        t_CurrentPosition.x + t_DirectionalVector.x,
                        t_CurrentPosition.y,
                        t_CurrentPosition.z + t_DirectionalVector.y
                    );


                playerNavMeshReference.SetDestination(t_NextPosition);

                playerNavMeshReference.speed = t_PlayerMovementSpeed;

                yield return t_CycleDelay;

                if (!m_IsTouchActive)
                {

                    playerNavMeshReference.SetDestination(m_PlayerTransformReference.position);
                    yield return t_WaitUntilTouchActive;
                }
            }


            playerNavMeshReference.enabled = false;

            StopCoroutine(PlayerMovementController());
        }

        #endregion

        #region Public Callback :  Touch Controller

        public bool IsTouchControllerEnabled()
        {

            return m_IsTouchControllerEnabled;
        }

        public bool IsTouchActive()
        {

            return m_IsTouchActive;
        }

        public void EnableTouchController()
        {

            if (!IsTouchControllerEnabled())
            {

                m_IsTouchControllerEnabled = true;
                StartCoroutine(PlayerMovementController());
            }

        }

        public void DisableTouchController()
        {

            m_IsTouchActive = false;
            m_IsTouchControllerEnabled = false;
        }

        #endregion

        #region Public Callback :   Player Movement

        public void LookAtTarget(Transform t_LookAtTarget)
        {

            m_PlayerTransformReference.LookAt(t_LookAtTarget);
        }

        public float GetPlayerMovementSpeed()
        {

            return m_ModifiedVelocity;
        }

        #endregion
    }
}


