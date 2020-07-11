namespace com.faith.Gameplay
{
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;

    public class TutorialManager : MonoBehaviour
    {

        public static TutorialManager Instance;

        [System.Serializable]
        public struct Tutorial
        {
            [Range(0.0f, 10.0f)]
            public float initialDelay;

            [Space(5.0f)]
            public string name;
            public string tutorialText;
            public TextMeshProUGUI tutorialTextReference;

            [Space(10.0f)]
            public GameObject[] activeObject;
            [Space(5.0f)]
            public GameObject[] deactiveObject;

            [Space(10.0f)]
            [Header("Tutorial : Break Statement")]
            public bool breakByCallback;
            [Tooltip("If : The direction are set to (0,0), it will break once the user tap on the screen. Else, it will wait for the right swipe to breakdown")]
            public Vector2 swipeDirection;

            [Space(10.0f)]
            [Header("Tutorial : Callback")]
            public UnityEvent OnTutorialStart;
            [Space(5.0f)]
            public UnityEvent OnTutorialEnd;

            [HideInInspector]
            public string TRACE_ID_FOR_COMPLETE;
        }

        #region Public Variables

        public UnityEvent OnStartTutorialManager;
        [Space(5.0f)]
        public Tutorial[] tutorial;

        #endregion

        //----------
        #region Private Variables

        private int m_TutorialIndex;
        private bool m_TraceUserInput;

        private Vector2 m_TouchPositionDown;
        private Vector2 m_TouchPositionUp;

        private bool m_CompleteByTap;
        private bool m_CompleteBySwipe;

        #endregion

        //----------
        #region MonoBehaviour

        void Start()
        {

            Instance = this;

            PreProcess();
            OnStartTutorialManager.Invoke();
        }

        void Update()
        {

            if (m_TraceUserInput)
                TraceUserInput();
        }

        #endregion

        //----------
        #region Public Callback

        public void CompleteTutorialByCallback(int t_PassedIndex)
        {

            if (m_TutorialIndex == t_PassedIndex
            && tutorial[m_TutorialIndex].breakByCallback)
            {
                Debug.Log("Breaked By Callback");
                m_TraceUserInput = false;
            }
        }

        public void StartTutorial(string name)
        {

            bool m_IsFound = false;
            for (int tutorialIndex = 0; tutorialIndex < tutorial.Length; tutorialIndex++)
            {

                if (name == tutorial[tutorialIndex].name)
                {

                    m_IsFound = true;
                    StartTutorial(tutorialIndex);
                    break;
                }
            }

            if (!m_IsFound)
                Debug.LogError("Invalid Tutorial Name");
        }

        public void StartTutorial(int index)
        {

            if (index < tutorial.Length)
            {
                if (PlayerPrefs.GetInt(tutorial[index].TRACE_ID_FOR_COMPLETE) == 0)
                {

                    m_TutorialIndex = index;
                    StartCoroutine(TutorialLifeCycleController());
                }
                else
                    Debug.LogWarning("You have already completed the tutorial, please reset to see it again");
            }
            else
                Debug.LogError("Invalid Tutorial Index");
        }

        public void ResetTutorial(string name)
        {

            bool m_IsFound = false;
            for (int tutorialIndex = 0; tutorialIndex < tutorial.Length; tutorialIndex++)
            {

                if (name == tutorial[tutorialIndex].name)
                {

                    m_IsFound = true;
                    ResetTutorial(tutorialIndex);
                    break;
                }
            }

            if (!m_IsFound)
                Debug.LogError("Invalid Tutorial Name");
        }

        public void ResetTutorial(int index)
        {

            if (index < tutorial.Length)
                PlayerPrefs.SetInt(tutorial[m_TutorialIndex].TRACE_ID_FOR_COMPLETE, 0);
            else
                Debug.LogError("Invalid Tutorial Index");
        }

        public void ResetAllTutorial()
        {

            for (int tutorialIndex = 0; tutorialIndex < tutorial.Length; tutorialIndex++)
            {

                PlayerPrefs.SetInt("TRACE_ID_FOR_COMPLETE_" + tutorial[tutorialIndex].name + "(" + tutorialIndex.ToString() + ")", 0);
            }
        }

        #endregion

        //----------
        #region Settings : Tutorial

        private void PreProcess()
        {

            for (int tutorialIndex = 0; tutorialIndex < tutorial.Length; tutorialIndex++)
            {

                tutorial[tutorialIndex].TRACE_ID_FOR_COMPLETE = "TRACE_ID_FOR_COMPLETE_" + tutorial[tutorialIndex].name + "(" + tutorialIndex.ToString() + ")";
            }
        }

        private void TraceUserInput()
        {

#if UNITY_EDITOR

            if (Input.GetMouseButtonDown(0))
            {
                OnUserTouchDown();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnUserTouchUp();
            }

#elif UNITY_ANDROID || UNITY_IOS

		if (Input.touchCount >= 1) {

			switch (Input.GetTouch (0).phase) {
				case TouchPhase.Began:
					OnUserTouchDown ();
					break;
				case TouchPhase.Ended:
					OnUserTouchUp ();
					break;
			}
		}
#endif
        }

        private IEnumerator TutorialLifeCycleController()
        {

            yield return new WaitForSeconds(tutorial[m_TutorialIndex].initialDelay);

            m_TraceUserInput = true;

            tutorial[m_TutorialIndex].tutorialTextReference.text = tutorial[m_TutorialIndex].tutorialText;

            //Defining : Complete Statement
            if (tutorial[m_TutorialIndex].breakByCallback)
            {
                m_CompleteByTap = false;
                m_CompleteBySwipe = false;
            }
            else
            {

                if (tutorial[m_TutorialIndex].swipeDirection == Vector2.zero)
                {

                    m_CompleteByTap = true;
                    m_CompleteBySwipe = false;
                }
                else
                {

                    m_CompleteByTap = false;
                    m_CompleteBySwipe = true;
                }
            }

            //PreProcess - Tutorial Start

            for (int activeIndex = 0; activeIndex < tutorial[m_TutorialIndex].activeObject.Length; activeIndex++)
            {

                tutorial[m_TutorialIndex].activeObject[activeIndex].SetActive(true);
            }

            for (int deactiveIndex = 0; deactiveIndex < tutorial[m_TutorialIndex].deactiveObject.Length; deactiveIndex++)
            {

                tutorial[m_TutorialIndex].deactiveObject[deactiveIndex].SetActive(false);
            }

            tutorial[m_TutorialIndex].OnTutorialStart.Invoke();

            //Thread - Wait
            while (m_TraceUserInput)
            {

                yield return new WaitForEndOfFrame();
            }

            //PostProcess - Tutorial End

            for (int activeIndex = 0; activeIndex < tutorial[m_TutorialIndex].activeObject.Length; activeIndex++)
            {

                tutorial[m_TutorialIndex].activeObject[activeIndex].SetActive(false);
            }

            for (int deactiveIndex = 0; deactiveIndex < tutorial[m_TutorialIndex].deactiveObject.Length; deactiveIndex++)
            {

                tutorial[m_TutorialIndex].deactiveObject[deactiveIndex].SetActive(true);
            }

            PlayerPrefs.SetInt(tutorial[m_TutorialIndex].TRACE_ID_FOR_COMPLETE, 1);

            tutorial[m_TutorialIndex].OnTutorialEnd.Invoke();
        }

        private void OnUserTouchDown()
        {

            if (m_CompleteByTap)
            {
                Debug.Log("Complete By Tap");
                m_TraceUserInput = false;
            }
            else if (m_CompleteBySwipe)
            {

#if UNITY_EDITOR
                m_TouchPositionDown = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID || UNITY_IOS
			m_TouchPositionDown = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
#endif
            }
        }

        private void OnUserTouchUp()
        {

            if (m_CompleteBySwipe)
            {

#if UNITY_EDITOR
                m_TouchPositionUp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID || UNITY_IOS
			m_TouchPositionUp = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
#endif

                bool m_VALID_X = false;
                bool m_VALID_Y = false;
                float m_Distance = 0.0f;

                //Validation : X
                if (tutorial[m_TutorialIndex].swipeDirection.x == 0)
                    m_VALID_X = true;
                else
                {

                    m_Distance = m_TouchPositionUp.x - m_TouchPositionDown.x;

                    if (tutorial[m_TutorialIndex].swipeDirection.x >= 0)
                    {

                        if (m_Distance >= 1.5f)
                            m_VALID_X = true;
                    }
                    else if (tutorial[m_TutorialIndex].swipeDirection.x < 0)
                    {

                        if (m_Distance <= -1.5f)
                            m_VALID_X = true;
                    }
                }

                //Validation : Y
                if (tutorial[m_TutorialIndex].swipeDirection.y == 0)
                    m_VALID_Y = true;
                else
                {

                    m_Distance = m_TouchPositionUp.y - m_TouchPositionDown.y;

                    if (tutorial[m_TutorialIndex].swipeDirection.y >= 0)
                    {

                        if (m_Distance >= 0.45f)
                            m_VALID_Y = true;
                    }
                    else if (tutorial[m_TutorialIndex].swipeDirection.y < 0)
                    {

                        if (m_Distance <= -0.45)
                            m_VALID_Y = true;
                    }
                }

                if (m_VALID_X && m_VALID_Y)
                    m_TraceUserInput = false;
            }
        }

        #endregion
    }
}

