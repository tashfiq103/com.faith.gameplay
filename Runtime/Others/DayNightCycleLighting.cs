
namespace com.faith.gameplay
{
	using System.Collections;
	using UnityEngine;

	public class DayNightCycleLighting : MonoBehaviour
	{

		public static DayNightCycleLighting Instance;

		#region Public Variables

		[Header("Reference : DirectionalLights")]
		public Light directionalLight;
		[Space(2.5f)]
		public Gradient directionalLightingColorThroughCycle;
		[Space(2.5f)]
		public float directionalLightIntensityMultiplier = 0.35f;
		public AnimationCurve directionalLightIntensityThroughCycle;
		[Space(2.5f)]
		public Vector3 eulerAngleOfDirectionalLightOnSunRise;
		public Vector3 eulerAngleOfDirectionalLightOnSunSet;

		[Space(5.0f)]
		[Header("Reference : Non - DirectionalLights")]
		public Light[] nonDirectionalLights;




		#endregion

		#region Private Variables

		// DIRECTIONAL LIGHT
		private Transform m_TransformReferenceOfDirectionalLight;

        private float m_RemainingTimeOfDayNightCycle;


        private float m_InitialDirectionalLightIntensity;
		private Keyframe[] m_KeyFrameOfDirectionalLightIntensity;

		private Color m_InitialColorOfDirectionalLight;
		private GradientColorKey[] m_GradientColorKeyForDirectionalLight;



		private bool m_IsDayLightTransitionRunning;

		private float m_DayLength;

		#endregion

		#region Mono Behaviour

		private void Awake()
		{
			if (Instance == null)
			{

				Instance = this;
			}

			m_TransformReferenceOfDirectionalLight = directionalLight.transform;

			m_InitialDirectionalLightIntensity = directionalLight.intensity;
			m_KeyFrameOfDirectionalLightIntensity = directionalLightIntensityThroughCycle.keys;

			m_InitialColorOfDirectionalLight = directionalLight.color;
			m_GradientColorKeyForDirectionalLight = directionalLightingColorThroughCycle.colorKeys;

		}


        #endregion

        #region Configuretion

        private float GetDirectionalLightIntensityThroughDayCycle(float t_DayCycleProgression) {

            return directionalLightIntensityMultiplier * directionalLightIntensityThroughCycle.Evaluate(t_DayCycleProgression);
        }

        private Color GetDirectionalLightColorThroughDayCycle(float t_DayCycleProgression) {

            return directionalLightingColorThroughCycle.Evaluate(t_DayCycleProgression);
        }

		private IEnumerator ControllerForDayLightTransition()
		{

			float t_CycleLength = 0.033f;
			WaitForSeconds t_CycleDelay = new WaitForSeconds(t_CycleLength);

			float t_Progression;

			Vector3 t_ModifiedEulerAngleOfLight;

			while (m_RemainingTimeOfDayNightCycle > 0)
			{

				t_Progression = 1f - (m_RemainingTimeOfDayNightCycle / m_DayLength);

				t_ModifiedEulerAngleOfLight = Vector3.Lerp(
						eulerAngleOfDirectionalLightOnSunRise,
						eulerAngleOfDirectionalLightOnSunSet,
						t_Progression
					);
				m_TransformReferenceOfDirectionalLight.localEulerAngles = t_ModifiedEulerAngleOfLight;

				directionalLight.color = GetDirectionalLightColorThroughDayCycle(t_Progression);
				directionalLight.intensity = GetDirectionalLightIntensityThroughDayCycle(t_Progression);

				m_RemainingTimeOfDayNightCycle -= t_CycleLength;
				yield return t_CycleDelay;
			}

			m_IsDayLightTransitionRunning = false;
			StopCoroutine(ControllerForDayLightTransition());
		}

		#endregion

		#region Public Callback :   Time

		public bool IsDayLightTransitionAlreadyRunning()
		{
			return m_IsDayLightTransitionRunning;
		}

		public void StartDayLightTransition(float t_DayLength)
		{

            m_DayLength                     = t_DayLength;
            m_RemainingTimeOfDayNightCycle  = m_DayLength;

            if (!IsDayLightTransitionAlreadyRunning())
			{
				m_IsDayLightTransitionRunning = true;
				StartCoroutine(ControllerForDayLightTransition());
			}
			else
			{
				Debug.LogWarning("DayLight Transtion already running");
			}
		}

		#endregion

		#region Public Callback :   Liner Interpolation

		public void ResetDayLightTransitionAsLinearInterpolation()
		{
			m_TransformReferenceOfDirectionalLight.localEulerAngles = eulerAngleOfDirectionalLightOnSunRise;
			directionalLight.color = GetDirectionalLightColorThroughDayCycle(0);
			directionalLight.intensity = GetDirectionalLightIntensityThroughDayCycle(0);
		}

		public void DayLightTransitionAsLinearInterpolation(float t_Progression)
		{

			Vector3 t_ModifiedEulerAngleOfLight = Vector3.Lerp(
						eulerAngleOfDirectionalLightOnSunRise,
						eulerAngleOfDirectionalLightOnSunSet,
						t_Progression
					);
			m_TransformReferenceOfDirectionalLight.localEulerAngles = t_ModifiedEulerAngleOfLight;

			directionalLight.color = GetDirectionalLightColorThroughDayCycle(t_Progression);
			directionalLight.intensity = GetDirectionalLightIntensityThroughDayCycle(t_Progression);
		}

		#endregion
	}
}


