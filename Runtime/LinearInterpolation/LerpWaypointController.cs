namespace com.faith.Math
{
    using UnityEngine;
    using com.faith.GameplayService;

    public class LerpWaypointController : MonoBehaviour
    {
        #region Custom Variables

        [System.Serializable]
        public struct Waypoint
        {
            public Transform waypointPosition;
        }

        #endregion

        #region Public Variables

        public bool useOnTouchControl;

        [Space(5.0f)]
        public bool isLerpInverse;

        [Range(0.0f, 1f)]
        public float lerpingSpeed = 0.25f;
        [Range(0f,1f)]
        public float initialValueOfLerp;
        [Space(5.0f)]
        public Transform objectTransform;

        [Space(5.0f)]
        public Waypoint[] waypoints;

        #endregion

        #region Private Variables

        private bool m_IsMovementEnabled = false;

        private int     m_NumberOfLerpPoints;

        private float m_LowerBoundaryOfLerp;
        private float m_UpperBoundaryOflerp;
        
        private float m_AbsoluteLerpValueInRange;

        private float m_CurrentValueOfLerp;

        private float[] m_LerpPointForWaypoints;

        private Quaternion m_ModifiedRotation;
        private Vector3 m_ModifiedPosition;

        private Vector2 m_ScreenResolution;

        private Vector2 m_TouchDownPosition;
        private Vector2 m_TouchPosition;
        private Vector2 m_TouchUpPosition;

        #endregion

        #region Mono Behaviour

        private void Awake()
        {
            PreProcess();
        } 
        
        #endregion

        #region Configuretion

        private void PreProcess() {

            if (useOnTouchControl)
            {
                m_ScreenResolution = new Vector2(

                        Screen.width,
                        Screen.height
                    );
            }

            m_CurrentValueOfLerp = initialValueOfLerp;
            
            int t_NumberOfWaypoints = waypoints.Length;
            if (t_NumberOfWaypoints > 1)
            {

                float t_CurrentDistance;
                float t_TotalDistance = 0;

                m_NumberOfLerpPoints = t_NumberOfWaypoints - 1;

                m_LerpPointForWaypoints = new float[m_NumberOfLerpPoints];
                float[] t_DistanceOnLerpPoint = new float[m_NumberOfLerpPoints];

                for (int counter = 0; counter < m_NumberOfLerpPoints; counter++)
                {

                    t_CurrentDistance = Vector3.Distance(waypoints[counter].waypointPosition.position, waypoints[counter + 1].waypointPosition.position);
                    t_TotalDistance += t_CurrentDistance;

                    t_DistanceOnLerpPoint[counter] = t_CurrentDistance;
                }

                float t_PreviousLerpPoint = 0;
                for (int counter = 0; counter < m_NumberOfLerpPoints; counter++)
                {

                    if (counter != 0)
                        t_PreviousLerpPoint = m_LerpPointForWaypoints[counter - 1];

                    m_LerpPointForWaypoints[counter] = t_PreviousLerpPoint + (t_DistanceOnLerpPoint[counter] / t_TotalDistance);
                }

                objectTransform.position = GetPositionThroughLerpValue(m_CurrentValueOfLerp);
                objectTransform.rotation = GetRotationThroughLerpValue(m_CurrentValueOfLerp);
            }
        }

        private bool IsValidLerpValue(float t_LerpValue)
        {

            if (t_LerpValue >= 0 && t_LerpValue <= 1)
                return true;

            return false;
        }
        
        private void OnTouchDown(Vector3 t_TouchPosition)
        {

            m_TouchDownPosition = t_TouchPosition;
        }

        private void OnTouch(Vector3 t_TouchPosition)
        {

            m_TouchPosition = t_TouchPosition;

            //--------------------------------
            //MODIFY HERE : For Custom Touch Control
            //--------------------------------

            float t_LerpValue = t_TouchPosition.x / m_ScreenResolution.x;
            LerpThroughPositions(t_LerpValue);
        }

        private void OnTouchUp(Vector3 t_TouchPosition)
        {

            m_TouchUpPosition = t_TouchPosition;
        }

        #endregion

        #region Public Callback

        public void EnableMovement(bool t_Recalculate = false)
        {

            if (t_Recalculate)
                PreProcess();

            if (useOnTouchControl) {

                GlobalTouchController.Instance.OnTouchDown  += OnTouchDown;
                GlobalTouchController.Instance.OnTouch      += OnTouch;
                GlobalTouchController.Instance.OnTouchUp    += OnTouchUp;
            }

            m_IsMovementEnabled = true;
        }

        public void DisableMovement()
        {
            if (useOnTouchControl)
            {
                GlobalTouchController.Instance.OnTouchDown  -= OnTouchDown;
                GlobalTouchController.Instance.OnTouch      -= OnTouch;
                GlobalTouchController.Instance.OnTouchUp    -= OnTouchUp;
            }
            m_IsMovementEnabled = false;
        }

        public void SnapToLerpPoint(float t_LerpValue) {

            LerpThroughPositions(t_LerpValue, isLerpInverse ? 0 : 1);
        }

        public void LerpThroughPositions(float t_LerpValue, float t_LerpSpeed = -1)
        {

            if (IsValidLerpValue(t_LerpValue))
            {
                
                m_CurrentValueOfLerp = Mathf.Lerp(
                        m_CurrentValueOfLerp,
                        isLerpInverse ? 1 - t_LerpValue : t_LerpValue,
                        t_LerpSpeed == -1 ? lerpingSpeed : t_LerpSpeed
                    );

                for (int lerpPointIndex = 0; lerpPointIndex < m_NumberOfLerpPoints; lerpPointIndex++)
                {

                    if (lerpPointIndex == 0)
                    {

                        if (m_CurrentValueOfLerp < m_LerpPointForWaypoints[lerpPointIndex])
                        {

                            m_AbsoluteLerpValueInRange = m_CurrentValueOfLerp / m_LerpPointForWaypoints[lerpPointIndex];

                            m_ModifiedPosition = Vector3.Lerp(
                                    waypoints[lerpPointIndex].waypointPosition.position,
                                    waypoints[lerpPointIndex + 1].waypointPosition.position,
                                    m_AbsoluteLerpValueInRange
                                );

                            m_ModifiedRotation = Quaternion.Lerp(
                                waypoints[lerpPointIndex].waypointPosition.rotation,
                                waypoints[lerpPointIndex + 1].waypointPosition.rotation,
                                m_AbsoluteLerpValueInRange
                            );

                            objectTransform.position = m_ModifiedPosition;
                            objectTransform.rotation = m_ModifiedRotation;

                            break;
                        }
                    }
                    else
                    {

                        if (m_CurrentValueOfLerp >= m_LerpPointForWaypoints[lerpPointIndex - 1] && m_CurrentValueOfLerp < m_LerpPointForWaypoints[lerpPointIndex])
                        {

                            m_LowerBoundaryOfLerp = m_LerpPointForWaypoints[lerpPointIndex - 1];
                            m_UpperBoundaryOflerp = m_LerpPointForWaypoints[lerpPointIndex];

                            m_AbsoluteLerpValueInRange = (m_CurrentValueOfLerp - m_LowerBoundaryOfLerp) / (m_UpperBoundaryOflerp - m_LowerBoundaryOfLerp);

                            m_ModifiedPosition = Vector3.Lerp(
                                waypoints[lerpPointIndex].waypointPosition.position,
                                waypoints[lerpPointIndex + 1].waypointPosition.position,
                                m_AbsoluteLerpValueInRange
                            );

                            m_ModifiedRotation = Quaternion.Lerp(
                                waypoints[lerpPointIndex].waypointPosition.rotation,
                                waypoints[lerpPointIndex + 1].waypointPosition.rotation,
                                m_AbsoluteLerpValueInRange
                            );

                            objectTransform.position = m_ModifiedPosition;
                            objectTransform.rotation = m_ModifiedRotation;

                            break;
                        }
                    }
                }
            }
        }

        public float GetCurrentLerpedValue() {

            return m_AbsoluteLerpValueInRange;
        }

        public Vector3 GetPositionThroughLerpValue(float t_LerpValue) {

            Vector3 t_Result = waypoints[0].waypointPosition.position;

            for (int lerpPointIndex = 0; lerpPointIndex < m_NumberOfLerpPoints; lerpPointIndex++)
            {
                if (lerpPointIndex == 0)
                {

                    if (t_LerpValue < m_LerpPointForWaypoints[lerpPointIndex])
                    {

                        float t_AbsoluteLerpValueInRange = t_LerpValue / m_LerpPointForWaypoints[lerpPointIndex];

                        t_Result = Vector3.Lerp(
                                waypoints[lerpPointIndex].waypointPosition.position,
                                waypoints[lerpPointIndex + 1].waypointPosition.position,
                                t_AbsoluteLerpValueInRange
                            );

                        break;
                    }
                }
                else
                {

                    if (t_LerpValue >= m_LerpPointForWaypoints[lerpPointIndex - 1] && t_LerpValue < m_LerpPointForWaypoints[lerpPointIndex])
                    {

                        float t_LowerBoundaryOfLerp = m_LerpPointForWaypoints[lerpPointIndex - 1];
                        float t_UpperBoundaryOflerp = m_LerpPointForWaypoints[lerpPointIndex];

                        float t_AbsoluteLerpValueInRange = (t_LerpValue - t_LowerBoundaryOfLerp) / (t_UpperBoundaryOflerp - t_LowerBoundaryOfLerp);

                        t_Result = Vector3.Lerp(
                            waypoints[lerpPointIndex].waypointPosition.position,
                            waypoints[lerpPointIndex + 1].waypointPosition.position,
                            t_AbsoluteLerpValueInRange
                        );

                        break;
                    }
                }
            }

            return  t_Result;
        }

        public Quaternion GetRotationThroughLerpValue(float t_LerpValue) {

            Quaternion t_Result = waypoints[0].waypointPosition.rotation;

            for (int lerpPointIndex = 0; lerpPointIndex < m_NumberOfLerpPoints; lerpPointIndex++)
            {

                if (lerpPointIndex == 0)
                {

                    if (t_LerpValue < m_LerpPointForWaypoints[lerpPointIndex])
                    {

                        float t_AbsoluteLerpValueInRange = t_LerpValue / m_LerpPointForWaypoints[lerpPointIndex];

                        t_Result = Quaternion.Lerp(
                            waypoints[lerpPointIndex].waypointPosition.rotation,
                            waypoints[lerpPointIndex + 1].waypointPosition.rotation,
                            t_AbsoluteLerpValueInRange
                        );

                        break;
                    }
                }
                else
                {

                    if (t_LerpValue >= m_LerpPointForWaypoints[lerpPointIndex - 1] && t_LerpValue < m_LerpPointForWaypoints[lerpPointIndex])
                    {

                        float t_LowerBoundaryOfLerp = m_LerpPointForWaypoints[lerpPointIndex - 1];
                        float t_UpperBoundaryOflerp = m_LerpPointForWaypoints[lerpPointIndex];

                        float t_AbsoluteLerpValueInRange = (t_LerpValue - t_LowerBoundaryOfLerp) / (t_UpperBoundaryOflerp - t_LowerBoundaryOfLerp);

                        t_Result = Quaternion.Lerp(
                            waypoints[lerpPointIndex].waypointPosition.rotation,
                            waypoints[lerpPointIndex + 1].waypointPosition.rotation,
                            t_AbsoluteLerpValueInRange
                        );

                        break;
                    }
                }
            }

            return  t_Result;
        }

        #endregion
    }

}

