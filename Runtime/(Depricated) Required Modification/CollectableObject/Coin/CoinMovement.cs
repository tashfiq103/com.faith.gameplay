using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinMovement : MonoBehaviour {

	#region Public Variables

	public SpriteRenderer coinCircleReference;
	public ParticleSystem coinSparkeTrailReference;

	#endregion

	#region Private Variables

	private Vector2 m_InitialScale;

	private Transform m_CoinParentInHeirarchy;
	private Vector2 m_SpawnPosition;
	private int m_CoinValue;

	[Header ("Coin Animation - Module")]
	private Transform m_CoinInfoParent;
	private GameObject m_CoinInfoPrefab;
	private Color m_CoinColor;

	[Header ("Coin Animation - Module (Depricated)")]
	private Transform m_TargetedPosition;

	#endregion

	private bool m_IsCoinMovementControllerRunnning;
	private bool m_ShowCoinUpdateInGameplay;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start () {
		m_InitialScale = transform.localScale;
	}

	/* public void StartCoinMovement (Transform t_TargetedPosition, Transform t_CoinParentInHeirarchy, Vector2 t_SpawnPosition, Color t_CoinColor,int t_CoinValue) {

		m_TargetedPosition = t_TargetedPosition;
		m_CoinParentInHeirarchy = t_CoinParentInHeirarchy;
		m_SpawnPosition = t_SpawnPosition;

		ParticleSystem.MainModule t_MainModuleReference = coinSparkeTrailReference.main;
		t_MainModuleReference.startColor = t_CoinColor;
		coinCircleReference.color = t_CoinColor;

		m_CoinValue = t_CoinValue;

		m_IsCoinMovementControllerRunnning = true;
		StartCoroutine (CoinMovementController ());
	} */

	public void StartCoinMovement (Transform t_CoinInfoParent, GameObject t_CoinInfoPrefab, Transform t_CoinParentInHeirarchy, Vector2 t_SpawnPosition, Color t_CoinColor, int t_CoinValue, bool t_ShowCoinUpdateInGameplay) {

		m_CoinInfoParent = t_CoinInfoParent;
		m_CoinInfoPrefab = t_CoinInfoPrefab;
		m_CoinParentInHeirarchy = t_CoinParentInHeirarchy;
		m_SpawnPosition = t_SpawnPosition;

		m_CoinColor = t_CoinColor;

		ParticleSystem.MainModule t_MainModuleReference = coinSparkeTrailReference.main;
		t_MainModuleReference.startColor = m_CoinColor;
		coinCircleReference.color = m_CoinColor;

		m_CoinValue = t_CoinValue;

		m_ShowCoinUpdateInGameplay = t_ShowCoinUpdateInGameplay;

		m_IsCoinMovementControllerRunnning = true;
		StartCoroutine (CoinMovementController ());
	}

	private IEnumerator CoinMovementController () {

		transform.parent = m_CoinParentInHeirarchy;

		WaitForEndOfFrame t_CycleDelay = new WaitForEndOfFrame ();

		bool t_HasReachedSpawnPoint = false;
		//float t_InitialDistance = Vector2.Distance (transform.position, m_TargetedPosition.position);

		RectTransform t_RectTransformReferenceForCoinInfo = null;

		Vector2 t_CurrentScale;
		Vector2 t_CurrentPosition;
		Vector2 t_TargetedPositionOfCurrencyInfo = Vector2.zero;
		float t_CurrentDistance;
		float t_MovementSpeed;
		float t_Progression;

		bool t_HasSpawnedCoinCurrency = false;
		int t_BonusCoin = 0;
		float t_CurrentTime = Time.time;
		float t_EndTimeForReachingSpawnPoint = t_CurrentTime + 0.5f;
		float t_EndTimeForFadingCoin = t_EndTimeForReachingSpawnPoint + 0.5f;
		float t_EndTimeForShowingCoinInfo = t_EndTimeForFadingCoin + 1f;

		while (m_IsCoinMovementControllerRunnning) {

			t_CurrentTime = Time.time;

			if (!t_HasReachedSpawnPoint) {

				if (t_CurrentTime < t_EndTimeForReachingSpawnPoint) {

					t_Progression = 1.0f - ((t_EndTimeForReachingSpawnPoint - t_CurrentTime) / 0.5f);
					t_CurrentPosition = transform.position;
					t_CurrentPosition = Vector2.Lerp (
						t_CurrentPosition,
						m_SpawnPosition,
						t_Progression
					);
					transform.position = t_CurrentPosition;
				} else {

					t_HasReachedSpawnPoint = true;
					transform.position = m_SpawnPosition;
					transform.parent = m_CoinParentInHeirarchy;

				}

			} else {

				if (t_CurrentTime < t_EndTimeForFadingCoin) {

					t_Progression = (t_EndTimeForFadingCoin - t_CurrentTime) / 0.5f;

					if(t_Progression >= 0.5f){

						t_CurrentScale = m_InitialScale * (t_Progression - 0.5f) * 2.0f;
						transform.localScale = t_CurrentScale;
					}

				} else if (t_CurrentTime < t_EndTimeForShowingCoinInfo) {

					if (!t_HasSpawnedCoinCurrency) {

                        //PlaySound
						//AudioManager.Instance.PlaySound("SoundFX - CashCounter");

						t_HasSpawnedCoinCurrency = true;
						
						transform.localScale = Vector2.zero;

						//Module 2
						m_CoinInfoPrefab = Instantiate (
							m_CoinInfoPrefab,
							transform.position,
							Quaternion.identity
						);

						t_RectTransformReferenceForCoinInfo = m_CoinInfoPrefab.GetComponent<RectTransform> ();
						t_RectTransformReferenceForCoinInfo.SetParent (m_CoinInfoParent);
						t_RectTransformReferenceForCoinInfo.anchoredPosition = m_SpawnPosition;
						t_TargetedPositionOfCurrencyInfo = m_SpawnPosition + (Vector2.up);

                        t_BonusCoin = 0;

						TextMeshProUGUI t_TMPROReference = m_CoinInfoPrefab.GetComponent<TextMeshProUGUI> ();
						t_TMPROReference.color = m_CoinColor;
                        t_TMPROReference.text = t_BonusCoin == 0 ? ("+" + m_CoinValue + " $") : ("+" + m_CoinValue + " (" + t_BonusCoin + ") $");
                        //t_TMPROReference.text = MathFunction.Instance.GetCurrencyInFormat(m_CoinValue) + " $";

                        if (m_ShowCoinUpdateInGameplay)
                        {
                            
                             //Define A GameplayController for your game
                            //UIStateController.Instance.ReferenceOfGameplayController.UpdateCoinCounter(m_CoinValue + t_BonusCoin);
                            GameManager.Instance.UpdateInGameCurrency(m_CoinValue + t_BonusCoin);

                        }
                        else
                        {
                            GameManager.Instance.UpdateInGameCurrency(m_CoinValue + t_BonusCoin);
                        }
					}

					t_Progression = (t_EndTimeForShowingCoinInfo - t_CurrentTime) / 1.0f;

					t_RectTransformReferenceForCoinInfo.anchoredPosition = Vector2.Lerp (
						t_RectTransformReferenceForCoinInfo.anchoredPosition,
						t_TargetedPositionOfCurrencyInfo,
						1f - t_Progression
					);
				} else {
					break;
				}

				//PostSpawn : Effect (0)
				/* t_MovementSpeed = Time.deltaTime * 10.0f;

				t_CurrentPosition = transform.position;
				t_CurrentDistance = Vector2.Distance (transform.position, m_TargetedPosition.position);

				t_Progression = (1.0f - (t_CurrentDistance / t_InitialDistance));
				t_CurrentScale = m_InitialScale + (t_Progression * m_InitialScale);

				transform.localScale = t_CurrentScale;
				transform.position = Vector2.MoveTowards (
					t_CurrentPosition,
					m_TargetedPosition.position,
					t_MovementSpeed
				);

				if (t_CurrentDistance <= 0.1f) {
					break;
				} */
			}

			yield return t_CycleDelay;
		}

		yield return new WaitForSeconds (1f);

		StopCoroutine (CoinMovementController ());
		Destroy (m_CoinInfoPrefab);
		Destroy (gameObject);

	}

}