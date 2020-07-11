using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour {

	public static CoinManager Instance;

	#region Custom DataType

	public struct OreData {
		public Vector2 destination;
		public Transform transformReference;
		public TrailRenderer trailRendererReference;

		//----------
		public bool hasReachedMaxSpeed;
		public float timeToReachedMaxVelocity;
		public float speedMeter;

		//----------
		public bool hasMagnetizedByPlayer;
	}

	private struct CoinData {
		public int numberOfCoinToBeSpawned;
		public int coinValue;
		public Color coinColor;

		public float coinSpawnDelay;
		public float timeForNextCoinSpawn;
	}

	#endregion

	//----------
	#region Public Variables
	public Transform spawnCoinParent;
	[Range (1.0f, 10.0f)]
	public float coinFallingSpeed;
	[Range (1.0f, 2.5f)]
	public float coinAcceleration;
	[Range (1.0f, 10.0f)]
	public float coinSpeedBoostOnMagnetized;

	[Space (2.5f)]
	public UnitVector2D coinFallingDirection;
	[Range (0.0f, 25.0f)]
	public float distanceToBeTravel;

	[Space (2.5f)]
	public Transform coinTargetedPosition;

	[Space (2.5f)]
	public GameObject coinPrefeb;
    public Color[] coinDefaultColor;

	[Space (2.5f)]
	[Header ("Coin Animation - Module")]
	public Transform coinInfoParent;
	public GameObject coinInfoPrefab;

	[Space(2.5f)]
	[Header("PowerUp - Magnet")]
	public ParticleSystem magnetEffectParticleSystem;
	public Animator magnetEffectAnimator;

	[Space (2.5f)]
	[Header ("Coin Animation - Module (Depricated)")]
	[Range (0.1f, 5f)]
	public float distanceFromPlane;

	#endregion

	//----------
	#region Private Variables

	private bool m_IsPlayerUsingMagnet;
	private float m_TimeToReachMaxVelocity;
	private float t_CurrentVelocity;

    private Transform m_CoinSpawnPoint;

	private Queue<OreData> m_ActiveCoinInMap;
	private Queue<CoinData> m_CoinData;

    private float m_CurrentGameSpeed;

    #endregion

    //----------
    #region Mono Behaviour Function

    void Awake () {

		Instance = this;
	}

    void Start()
    {
        m_CurrentGameSpeed = GameManager.Instance.GetGameSpeed();
        GameManager.Instance.RegisterEventOnChangingGameSpeec(
            delegate
            {
                m_CurrentGameSpeed = GameManager.Instance.GetGameSpeed();
            });
        
    }

    #endregion

    //----------
    #region Configuretion

    private bool m_IsCoinMovementControllerRunning;
	private bool m_IsGameEnd;

	private float m_MagnetPowerUpDuration = 0;

	private IEnumerator CoinMovementController () {

		int t_NumberOfActiveCoinInMap = 0;
		WaitForEndOfFrame t_CycleDelay = new WaitForEndOfFrame ();

		OreData t_ModifiedCoinData = new OreData ();
		Vector2 t_CoinCurrentPosition = Vector2.zero;

		float t_CurrentTimeForSpeedCalcuation = 0.0f;
		float t_FallingVelocity = 0.0f;

		while (m_IsCoinMovementControllerRunning) {

			t_NumberOfActiveCoinInMap = m_ActiveCoinInMap.Count;

			for (int coinIndex = 0; coinIndex < t_NumberOfActiveCoinInMap; coinIndex++) {

				t_ModifiedCoinData = m_ActiveCoinInMap.Dequeue ();

				if (t_ModifiedCoinData.transformReference != null) {

					//Get : Current Position					
					t_CoinCurrentPosition = t_ModifiedCoinData.transformReference.position;

					//Defining : Current Velocity
					if (!t_ModifiedCoinData.hasReachedMaxSpeed) {

						t_CurrentTimeForSpeedCalcuation = Time.time;

						if (t_CurrentTimeForSpeedCalcuation < t_ModifiedCoinData.timeToReachedMaxVelocity) {
							t_FallingVelocity = 1.0f - ((t_ModifiedCoinData.timeToReachedMaxVelocity - t_CurrentTimeForSpeedCalcuation) / m_TimeToReachMaxVelocity);
							t_FallingVelocity *= (Time.deltaTime * coinFallingSpeed);
						} else {

							t_FallingVelocity = Time.deltaTime * coinFallingSpeed;
							t_ModifiedCoinData.hasReachedMaxSpeed = true;
						}
					} else {

						t_FallingVelocity = Time.deltaTime * coinFallingSpeed;
					}

					//Check : if magnetized by player
					if (m_IsPlayerUsingMagnet &&
						!t_ModifiedCoinData.hasMagnetizedByPlayer) {

						t_ModifiedCoinData.hasMagnetizedByPlayer = true;
						t_ModifiedCoinData.trailRendererReference.enabled = true;
					}

					if (t_ModifiedCoinData.hasMagnetizedByPlayer) {

						t_FallingVelocity *= coinSpeedBoostOnMagnetized;

						t_ModifiedCoinData.transformReference.position = Vector2.MoveTowards (
							t_CoinCurrentPosition,
							coinTargetedPosition.position,
							t_FallingVelocity * m_CurrentGameSpeed
						);

						m_ActiveCoinInMap.Enqueue (t_ModifiedCoinData);
					} else {
						//if not magnetized
						t_ModifiedCoinData.transformReference.position = Vector2.MoveTowards (
							t_CoinCurrentPosition,
							t_ModifiedCoinData.destination,
							t_FallingVelocity * m_CurrentGameSpeed
                        );

						if (Vector2.Distance (t_ModifiedCoinData.transformReference.position, t_ModifiedCoinData.destination) <= 0.1f) {

							Destroy (t_ModifiedCoinData.transformReference.gameObject);
						} else {

							m_ActiveCoinInMap.Enqueue (t_ModifiedCoinData);
						}
					}

				}
			}

			yield return t_CycleDelay;
		}

		StopCoroutine (CoinMovementController ());
	}

	private IEnumerator MagnetAsPowerUpController (float t_Duration) {

		m_MagnetPowerUpDuration = t_Duration;

		float t_CycleDuration = 0.1f;
		WaitForSeconds t_CycleDelay = new WaitForSeconds (t_CycleDuration);
		WaitUntil t_GamePauseAndResumeState = new WaitUntil (() => {
			if (GameManager.Instance.IsGamePaused ()) {
				return false;
			} else {
				return true;
			}
		});

        /*
         * Call : Any Visual
         * UIGameplayPowerUpController.Instance.AddPowerUp(0,t_Duration,false);
         */
        EnablePlayerMagnetPulling();

		magnetEffectAnimator.SetTrigger("ACTIVE");
		magnetEffectParticleSystem.Play();

		while (m_MagnetPowerUpDuration > 0) {

			yield return t_GamePauseAndResumeState;

			m_MagnetPowerUpDuration -= t_CycleDuration;

			yield return t_CycleDelay;
		}
		
		DisablePlayerMagnetPulling ();

		m_MagnetPowerUpDuration = 0;

		magnetEffectAnimator.SetTrigger("DEACTIVE");
		magnetEffectParticleSystem.Clear();

		yield return new WaitForSeconds(0.25f);
			
		magnetEffectParticleSystem.Stop();

		StopCoroutine (MagnetAsPowerUpController (0f));
	}

	private IEnumerator CoinCollectionControllerAfterGameEnd () {

		if (m_IsSuccessfullyEnded) {

			yield return new WaitForSeconds (.25f);

			m_MagnetPowerUpDuration = 0;

			EnablePlayerMagnetPulling ();
			WaitForEndOfFrame t_CycleDelay = new WaitForEndOfFrame ();

			while (m_ActiveCoinInMap.Count > 0) {

				yield return t_CycleDelay;
			}

			DisablePlayerMagnetPulling ();

		} else {
			int t_NumberOfActiveCoinInMap = m_ActiveCoinInMap.Count;
			for (int coinIndex = 0; coinIndex < t_NumberOfActiveCoinInMap; coinIndex++) {
				Destroy (m_ActiveCoinInMap.Dequeue ().transformReference.gameObject);
			}
		}

		StopCoinMovementController ();
		StopCoroutine (CoinCollectionControllerAfterGameEnd ());

	}

	private void StartCoinMovementController () {

		m_IsCoinMovementControllerRunning = true;
		StartCoroutine (CoinMovementController ());
	}

	private void StopCoinMovementController () {

		m_IsCoinMovementControllerRunning = false;
	}

	#endregion

	//----------
	#region Public Callback

	private int m_NumberOfCoinToBeAdd;
	private float m_CoinSpawnDifference;

	private bool m_IsSuccessfullyEnded;

	public void PreProcess () {

		m_ActiveCoinInMap = new Queue<OreData> ();
		m_CoinData = new Queue<CoinData> ();
		m_TimeToReachMaxVelocity = coinFallingSpeed / coinAcceleration;

		m_IsGameEnd = false;
		StartCoinMovementController ();
	}

	public void RestoreToDefault (bool t_IsSuccessfullyEnded) {

		m_IsGameEnd = true;
		m_IsSuccessfullyEnded = t_IsSuccessfullyEnded;
		DisableMagnetAsPowerUp();
		StartCoroutine (CoinCollectionControllerAfterGameEnd ());
	}

	public void AddOre (Transform t_CoinRefeerence, TrailRenderer t_TrailRendererReference) {

		t_CoinRefeerence.parent = spawnCoinParent;

		OreData t_NewCoinData = new OreData ();

		t_NewCoinData.hasReachedMaxSpeed = false;
		t_NewCoinData.speedMeter = 0.0f;
		t_NewCoinData.timeToReachedMaxVelocity = Time.time + m_TimeToReachMaxVelocity;

		t_NewCoinData.transformReference = t_CoinRefeerence;
		t_NewCoinData.trailRendererReference = t_TrailRendererReference;
		t_NewCoinData.destination = (Vector2) t_CoinRefeerence.transform.position +
			new Vector2 (
				coinFallingDirection.x * distanceToBeTravel,
				coinFallingDirection.y * distanceToBeTravel
			);

		m_ActiveCoinInMap.Enqueue (t_NewCoinData);
	}

	public void EnableMagnetAsPowerUp (float t_Duration) {

		if (m_MagnetPowerUpDuration == 0) {

			StartCoroutine (MagnetAsPowerUpController (t_Duration));
		} else {

			m_MagnetPowerUpDuration += t_Duration;
            /*
         * Call : Any Visual
         * UIGameplayPowerUpController.Instance.UpdateRemainingTime(0,t_Duration);
         */

        }
    }

	public void DisableMagnetAsPowerUp () {

		m_MagnetPowerUpDuration = 0;
	}

	public bool IsPlayerUsingMagnet () {
		return m_IsPlayerUsingMagnet;
	}

	public void EnablePlayerMagnetPulling () {

		m_IsPlayerUsingMagnet = true;
	}

	public void DisablePlayerMagnetPulling () {

		m_IsPlayerUsingMagnet = false;
	}

    public void AddCoinToUser(int t_NumberOfCoin) {

        AddCoinToUser(t_NumberOfCoin, coinDefaultColor[Random.Range(1, coinDefaultColor.Length - 1)]);
    }

    public void AddCoinToUser(int t_NumberOfCoin, Color t_CoinColor) {

        AddCoinToUser(t_NumberOfCoin, t_CoinColor, null);
    }


    public void AddCoinToUser (int t_NumberOfCoin, Color t_CoinColor,Transform t_CoinSpawnPoint) {

        m_CoinSpawnPoint = t_CoinSpawnPoint;

		CoinData t_NewCoinData = new CoinData ();

		if (t_NumberOfCoin % 25 == 0) {

			t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 25;
			t_NewCoinData.coinValue = 25;
		} else if (t_NumberOfCoin % 10 == 0) {

			t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 10;
			t_NewCoinData.coinValue = 10;
		}else if (t_NumberOfCoin % 7 == 0) {

			t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 7;
			t_NewCoinData.coinValue = 7;
		}else if (t_NumberOfCoin % 6 == 0) {

			t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 6;
			t_NewCoinData.coinValue = 6;
		} else if (t_NumberOfCoin % 5 == 0) {

			t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 5;
			t_NewCoinData.coinValue = 5;
		} else if (t_NumberOfCoin % 4 == 0) {

			t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 4;
			t_NewCoinData.coinValue = 4;
		} else if (t_NumberOfCoin % 3 == 0) {

			t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 3;
			t_NewCoinData.coinValue = 3;
		} else {

			t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin;
			t_NewCoinData.coinValue = 1;
		}

		t_NewCoinData.coinColor = t_CoinColor;
		t_NewCoinData.coinSpawnDelay = 1f / t_NumberOfCoin;
		t_NewCoinData.timeForNextCoinSpawn = Time.time;

		if (m_CoinData.Count == 0) {
			m_CoinData.Enqueue (t_NewCoinData);
			StartCoroutine (AddCoinToUserController ());
		} else {
			m_CoinData.Enqueue (t_NewCoinData);
		}

	}

	public IEnumerator AddCoinToUserController () {

		WaitForEndOfFrame t_CycleDelay = new WaitForEndOfFrame ();

		CoinData t_CurrentCoinData;

		Vector2 t_CoinTargetedPosition;
		Vector2 t_SpawnPoint;
		float t_CurrentTime;
		float t_CoinSpawnTime = Time.time;

		while (m_CoinData.Count > 0) {

			t_CurrentCoinData = m_CoinData.Dequeue ();

			if (t_CurrentCoinData.numberOfCoinToBeSpawned > 0) {

				t_CurrentTime = Time.time;
				if (t_CurrentTime > t_CurrentCoinData.timeForNextCoinSpawn) {

					t_CurrentCoinData.timeForNextCoinSpawn = t_CurrentTime + t_CurrentCoinData.coinSpawnDelay;
					t_CoinTargetedPosition = coinTargetedPosition.transform.position;

					GameObject t_NewCoin = Instantiate (
						coinPrefeb,
                        Vector3.zero  /* Coin Position : PlaneManager.Instance.GetOreBreakerPosition ()*/,
                        Quaternion.identity
					);
					t_NewCoin.name = "CollectableCoin (" + m_NumberOfCoinToBeAdd + ")";

					t_NewCoin.transform.parent = coinTargetedPosition.transform;
                    t_NewCoin.transform.position = m_CoinSpawnPoint != null ? m_CoinSpawnPoint.position : transform.position;
					t_SpawnPoint = new Vector2 (
						Random.Range (0f, 1f) <= 0.5f ? Random.Range (t_CoinTargetedPosition.x, t_CoinTargetedPosition.x - distanceFromPlane) : Random.Range (t_CoinTargetedPosition.x, t_CoinTargetedPosition.x + distanceFromPlane),
						Random.Range (0f, 1f) <= 0.5f ? Random.Range (t_CoinTargetedPosition.y, t_CoinTargetedPosition.y - distanceFromPlane) : Random.Range (t_CoinTargetedPosition.y, t_CoinTargetedPosition.y + distanceFromPlane)
					);

					t_NewCoin.GetComponent<CoinMovement> ().StartCoinMovement (coinInfoParent, coinInfoPrefab, spawnCoinParent, t_SpawnPoint, t_CurrentCoinData.coinColor, t_CurrentCoinData.coinValue,true);
					t_CurrentCoinData.numberOfCoinToBeSpawned--;
				}
			}

			if (t_CurrentCoinData.numberOfCoinToBeSpawned > 0)
				m_CoinData.Enqueue (t_CurrentCoinData);

			yield return t_CycleDelay;
		}

		/* (Depricated? : Not Sure) 
		while (m_NumberOfCoinToBeAdd > 0) {

			t_CurrentTime = Time.time;

			if (t_CurrentTime > t_CoinSpawnTime) {

				t_CoinSpawnTime = t_CurrentTime + m_CoinSpawnDifference;
				t_PlayerPosition = playerReference.transform.position;

				GameObject t_NewCoin = Instantiate (
					coinPrefeb,
					PlaneManager.Instance.GetOreBreakerPosition (),
					Quaternion.identity
				);
				t_NewCoin.name = "CollectableCoin (" + m_NumberOfCoinToBeAdd + ")";

				t_NewCoin.transform.parent = playerReference.transform;
				t_SpawnPoint = new Vector2 (
					Random.Range (0f, 1f) <= 0.5f ? Random.Range (t_PlayerPosition.x, t_PlayerPosition.x - distanceFromPlane) : Random.Range (t_PlayerPosition.x, t_PlayerPosition.x + distanceFromPlane),
					Random.Range (0f, 1f) <= 0.5f ? Random.Range (t_PlayerPosition.y, t_PlayerPosition.y - distanceFromPlane) : Random.Range (t_PlayerPosition.y, t_PlayerPosition.y + distanceFromPlane)
				);

				t_NewCoin.GetComponent<CoinMovement> ().StartCoinMovement (coinTargetedPosition, spawnCoinParent, t_SpawnPoint);
				m_NumberOfCoinToBeAdd--;
			}

			yield return t_CycleDelay;
		} */

		StopCoroutine (AddCoinToUserController ());
	}

	#endregion
}