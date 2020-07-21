namespace com.faith.gameplay
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    using com.faith.gameplay.service;

    [RequireComponent(typeof(OnDemandPrefab))]
    public class CoinReward : MonoBehaviour
    {

        private struct CoinData
        {
            public int numberOfCoinToBeSpawned;
            public int coinValue;
            public Color coinColor;

            public float coinSpawnDelay;
            public float timeForNextCoinSpawn;
        }

        #region Public Variables

        public static CoinReward Instance;

        public Transform coinInfoParent;
        public GameObject coinInfoPrefab;
        public GameObject coinPrefeb;
        [Range(0f, 1f)]
        public float spawnRadius;

        #endregion

        //--------------------------------------------------
        #region Mono Behaviour

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {

            if (Instance == null)
            {

                m_CoinListReference = gameObject.GetComponent<OnDemandPrefab>();

                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {

                Destroy(gameObject);
            }
        }

        #endregion

        //--------------------------------------------------
        #region Configuretion

        private OnDemandPrefab m_CoinListReference;
        private Queue<CoinData> m_CoinData;

        private UnityAction m_OnRewardGivenComplete;

        private bool m_IsAlreadyAwardingCoin;
        private long m_AwardedCoinAmount;

        private IEnumerator AwardCoinController(int t_CurrentLevel)
        {

            int t_ListOfCoin = m_CoinListReference.items.Length;
            List<GameObject> t_FilteredList = new List<GameObject>();
            for (int index = 0; index < t_ListOfCoin; index++)
            {

                if (m_CoinListReference.items[index].requiredLevel <= t_CurrentLevel)
                {
                    t_FilteredList.Add(m_CoinListReference.items[index].obstaclePrefab);
                }
            }

            WaitForEndOfFrame t_CycleDelay = new WaitForEndOfFrame();

            GameObject t_CoinObject;
            CoinAttribute t_CoinAttributeReference;
            int t_CoinValue;
            int t_NumberOfCoinPerFrame = Mathf.CeilToInt(t_CurrentLevel / 5.0f);
            t_NumberOfCoinPerFrame = 1 + (t_NumberOfCoinPerFrame >= 20 ? 20 : t_NumberOfCoinPerFrame);
            long t_RemainingCoinToAward = m_AwardedCoinAmount;

            while (m_IsAlreadyAwardingCoin)
            {

                for (int coinLoopIndex = 0; coinLoopIndex < t_NumberOfCoinPerFrame; coinLoopIndex++)
                {

                    t_CoinObject = m_CoinListReference.GetObject(t_FilteredList);
                    if (t_CoinObject != null)
                    {

                        t_CoinAttributeReference = t_CoinObject.GetComponent<CoinAttribute>();
                        t_CoinValue = t_CoinAttributeReference.coinToBeAwarded;
                        if (t_RemainingCoinToAward >= t_CoinValue)
                        {

                            AddCoinToUser(t_CoinValue, t_CoinAttributeReference.particleColor);
                            t_RemainingCoinToAward -= t_CoinValue;
                        }
                    }
                    else
                    {

                        Debug.Log("###Found Null Coin To Reward");
                    }

                }

                //Discard any list item that cannot be meet the remaining coin
                bool t_HasCheckedFinished = false;
                while (!t_HasCheckedFinished)
                {

                    t_HasCheckedFinished = true;

                    for (int index = 0; index < t_FilteredList.Count; index++)
                    {
                        t_CoinAttributeReference = t_FilteredList[index].GetComponent<CoinAttribute>();
                        if (t_CoinAttributeReference.coinToBeAwarded > t_RemainingCoinToAward)
                        {
                            t_FilteredList.RemoveAt(index);
                            t_FilteredList.TrimExcess();
                            t_HasCheckedFinished = false;
                            break;
                        }
                    }

                    //Debug
                    //string t_RemainingCoin = "RemainingCoin : " + t_RemainingCoinToAward + ", ";
                    //for (int index = 0; index < t_FilteredList.Count; index++) {

                    //    t_RemainingCoin += t_FilteredList[index].name + ", ";
                    //}
                    //Debug.Log(t_RemainingCoin);
                }

                if (t_RemainingCoinToAward <= 0)
                {
                    break;
                }
                else
                    yield return t_CycleDelay;
            }

            m_IsAlreadyAwardingCoin = false;

            if (m_OnRewardGivenComplete != null)
                m_OnRewardGivenComplete.Invoke();

            StopCoroutine(AwardCoinController(0));

        }

        private void AddCoinToUser(int t_NumberOfCoin, Color t_CoinColor)
        {

            CoinData t_NewCoinData = new CoinData();

            if (t_NumberOfCoin % 1000000 == 0)
            {

                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 1000000;
                t_NewCoinData.coinValue = 1000000;
            }
            else if (t_NumberOfCoin % 500000 == 0)
            {

                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 500000;
                t_NewCoinData.coinValue = 500000;
            }
            else if (t_NumberOfCoin % 250000 == 0)
            {
                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 250000;
                t_NewCoinData.coinValue = 250000;
            }
            else if (t_NumberOfCoin % 100000 == 0)
            {
                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 100000;
                t_NewCoinData.coinValue = 100000;

            }
            else if (t_NumberOfCoin % 25000 == 0)
            {
                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 25000;
                t_NewCoinData.coinValue = 25000;

            }
            else if (t_NumberOfCoin % 10000 == 0)
            {
                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 10000;
                t_NewCoinData.coinValue = 10000;

            }
            else if (t_NumberOfCoin % 2500 == 0)
            {
                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 2500;
                t_NewCoinData.coinValue = 2500;

            }
            else if (t_NumberOfCoin % 1000 == 0)
            {
                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 1000;
                t_NewCoinData.coinValue = 1000;

            }
            else if (t_NumberOfCoin % 500 == 0)
            {
                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 500;
                t_NewCoinData.coinValue = 500;

            }
            else if (t_NumberOfCoin % 250 == 0)
            {
                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 250;
                t_NewCoinData.coinValue = 250;

            }
            else if (t_NumberOfCoin % 50 == 0)
            {

                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 50;
                t_NewCoinData.coinValue = 50;

            }
            else if (t_NumberOfCoin % 25 == 0)
            {

                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 25;
                t_NewCoinData.coinValue = 25;

            }
            else if (t_NumberOfCoin % 10 == 0)
            {

                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 10;
                t_NewCoinData.coinValue = 10;

            }
            else if (t_NumberOfCoin % 5 == 0)
            {

                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin / 5;
                t_NewCoinData.coinValue = 5;

            }
            else
            {

                t_NewCoinData.numberOfCoinToBeSpawned = t_NumberOfCoin;
                t_NewCoinData.coinValue = 1;

            }

            t_NewCoinData.coinColor = t_CoinColor;
            t_NewCoinData.coinSpawnDelay = 1f / t_NumberOfCoin;
            t_NewCoinData.timeForNextCoinSpawn = Time.time;

            if (m_CoinData.Count == 0)
            {

                m_CoinData.Enqueue(t_NewCoinData);
                StartCoroutine(AddCoinToUserController());
            }
            else
            {
                m_CoinData.Enqueue(t_NewCoinData);
            }
        }

        private IEnumerator AddCoinToUserController()
        {


            Vector2 t_DeviceScreenSize = DeviceInfoManager.Instance.GetScaledValueForSprite(0.9f);
            t_DeviceScreenSize *= 0.4f;

            WaitForEndOfFrame t_CycleDelay = new WaitForEndOfFrame();

            CoinData t_CurrentCoinData;

            Vector2 t_CoinPosition;
            Vector2 t_SpawnPoint;
            float t_CurrentTime;
            float t_CoinSpawnTime = Time.time;

            while (m_CoinData.Count > 0)
            {

                t_CurrentCoinData = m_CoinData.Dequeue();

                if (t_CurrentCoinData.numberOfCoinToBeSpawned > 0)
                {

                    t_CurrentTime = Time.time;
                    if (t_CurrentTime > t_CurrentCoinData.timeForNextCoinSpawn)
                    {

                        t_CurrentCoinData.timeForNextCoinSpawn = t_CurrentTime + t_CurrentCoinData.coinSpawnDelay;
                        t_CoinPosition = new Vector2(
                            Random.Range(-t_DeviceScreenSize.x, t_DeviceScreenSize.x),
                            Random.Range(-t_DeviceScreenSize.y, t_DeviceScreenSize.y)
                        );

                        GameObject t_NewCoin = Instantiate(
                            coinPrefeb,
                            Vector3.zero,
                            Quaternion.identity
                        );

                        t_NewCoin.transform.parent = transform;
                        t_SpawnPoint = new Vector2(
                            Random.Range(0f, 1f) <= 0.5f ? Random.Range(t_CoinPosition.x, t_CoinPosition.x - spawnRadius) : Random.Range(t_CoinPosition.x, t_CoinPosition.x + spawnRadius),
                            Random.Range(0f, 1f) <= 0.5f ? Random.Range(t_CoinPosition.y, t_CoinPosition.y - spawnRadius) : Random.Range(t_CoinPosition.y, t_CoinPosition.y + spawnRadius)
                        );

                        t_NewCoin.GetComponent<CoinMovement>().StartCoinMovement(coinInfoParent, coinInfoPrefab, transform, t_SpawnPoint, t_CurrentCoinData.coinColor, t_CurrentCoinData.coinValue, false);
                        t_CurrentCoinData.numberOfCoinToBeSpawned--;
                    }
                }

                if (t_CurrentCoinData.numberOfCoinToBeSpawned > 0)
                    m_CoinData.Enqueue(t_CurrentCoinData);

                yield return t_CycleDelay;
            }

            yield return new WaitForSeconds(2f);

            //coinInfoParent.gameObject.SetActive(false);

            StopCoroutine(AddCoinToUserController());
        }

        #endregion

        //--------------------------------------------------
        #region Public Callback

        public void AwardCoin(int t_CurrentLevel, long t_CoinAmount, UnityAction t_OnRewardGivenComplete)
        {

            //coinInfoParent.gameObject.SetActive(true);

            m_AwardedCoinAmount = t_CoinAmount;
            m_OnRewardGivenComplete = t_OnRewardGivenComplete;

            m_CoinData = new Queue<CoinData>();

            m_IsAlreadyAwardingCoin = true;
            StartCoroutine(AwardCoinController(t_CurrentLevel));
        }

        #endregion

    }
}

