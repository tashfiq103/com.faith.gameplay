namespace com.faith.Gameplay
{
    using UnityEngine;
    using UnityEngine.Events;

    using System.Collections;
    using System.Collections.Generic;

    public class CharacterExpressionController : MonoBehaviour
    {

        public static CharacterExpressionController Instance;

        #region Custom Variables

        [System.Serializable]
        public struct Expression
        {
            public Sprite expressionSprite;
        }

        [System.Serializable]
        public struct ActiveExpression
        {
            public bool flagedForDispose;

            public float duration;
            public Vector3 displacement;
            public Transform customerTransformReference;

            public Transform expressionUI;
            public ChacterExpressionAttribute expressionAttribute;

            public UnityAction OnCustomerExpressionComplete;
        }

        #endregion

        #region Public Variables


        [Space(5.0f)]
        public Transform cameraTransformReference;
        public Transform customerExpressionParent;
        public GameObject customerExpressionPrefab;


        #endregion

        #region Private Variables

        private List<ActiveExpression> m_ListOfActiveExpression;

        

        #endregion

        #region Mono Behaviour

        private void Awake()
        {

            Instance = this;

            m_ListOfActiveExpression = new List<ActiveExpression>();
        }

        #endregion

        #region Configuretion

        private bool m_IsCustomerExpressionControllerRunning = false;

        private Transform m_TargetedPosition;
        private Vector3 m_Displacement;
        private float m_Duration;

        private int IsExpressionAlreadyCreatedForCustomer(Transform t_CustomerTransformReference)
        {

            int t_NumberOfActiveExpression = m_ListOfActiveExpression.Count;
            for (int expressionIndex = 0; expressionIndex < t_NumberOfActiveExpression; expressionIndex++)
            {

                if (m_ListOfActiveExpression[expressionIndex].customerTransformReference == t_CustomerTransformReference)
                {
                    return expressionIndex;
                }
            }

            return -1;
        }

        private IEnumerator ControllerForLifeCycleOfCustomerExpression()
        {

            float t_CycleLength = 0.0167f;
            WaitForSeconds t_CycleDelay = new WaitForSeconds(t_CycleLength);

            //----------------------------
            int t_NumberOfActiveExpression;
            ActiveExpression t_ModifiedActiveExpression;
            Vector3 t_ModifiedPositionForExpressionUI;
            Quaternion t_ModifiedRotation;
            //----------------------------

            List<int> t_IndexOfActiveExpressionToBeRecycled;

            while (m_IsCustomerExpressionControllerRunning)
            {

                t_NumberOfActiveExpression = m_ListOfActiveExpression.Count;
                t_IndexOfActiveExpressionToBeRecycled = new List<int>();

                // Decrement & Dispose Call
                for (int expressionIndex = 0; expressionIndex < t_NumberOfActiveExpression; expressionIndex++)
                {

                    t_ModifiedActiveExpression = m_ListOfActiveExpression[expressionIndex];

                    if (m_ListOfActiveExpression[expressionIndex].duration <= 0)
                    {
                        t_IndexOfActiveExpressionToBeRecycled.Add(expressionIndex);
                    }
                    else
                    {

                        //Decrementing Time
                        t_ModifiedActiveExpression.duration -= t_CycleLength;

                        //Changing Position of UI
                        if (m_ListOfActiveExpression[expressionIndex].flagedForDispose)
                        {
                            // Customer Instance already destroyed but if the expression still alive
                            t_IndexOfActiveExpressionToBeRecycled.Add(expressionIndex);
                        }
                        else
                        {
                            t_ModifiedPositionForExpressionUI = t_ModifiedActiveExpression.customerTransformReference.position + t_ModifiedActiveExpression.displacement;

                            t_ModifiedActiveExpression.expressionUI.position = t_ModifiedPositionForExpressionUI;

                            t_ModifiedRotation = Quaternion.Slerp(
                                    t_ModifiedActiveExpression.expressionUI.rotation,
                                    Quaternion.LookRotation(cameraTransformReference.position - t_ModifiedActiveExpression.expressionUI.position),
                                    0.25f
                                );
                            t_ModifiedActiveExpression.expressionUI.rotation = t_ModifiedRotation;
                        }
                    }

                    m_ListOfActiveExpression[expressionIndex] = t_ModifiedActiveExpression;
                }

                //Dispose
                if (t_IndexOfActiveExpressionToBeRecycled.Count > 0)
                {

                    int t_SelectedIndex;
                    int t_NumberOfExpressionToBeDisposed = t_IndexOfActiveExpressionToBeRecycled.Count;

                    //Called : PostProcess
                    for (int loopCounter = 0; loopCounter < t_NumberOfExpressionToBeDisposed; loopCounter++)
                    {

                        t_SelectedIndex = t_IndexOfActiveExpressionToBeRecycled[loopCounter] - loopCounter;
                        m_ListOfActiveExpression[t_SelectedIndex].OnCustomerExpressionComplete?.Invoke();
                        m_ListOfActiveExpression[t_SelectedIndex].expressionAttribute.PostProcess(
                            m_ListOfActiveExpression[t_SelectedIndex].customerTransformReference,
                            m_ListOfActiveExpression[t_SelectedIndex].displacement
                            );
                        m_ListOfActiveExpression.RemoveAt(t_SelectedIndex);
                    }

                    m_ListOfActiveExpression.TrimExcess();
                }

                yield return t_CycleDelay;
            }

            StopCoroutine(ControllerForLifeCycleOfCustomerExpression());

        }

        private GameObject ConfigureCharacterEmojiAndDialouge(
            float t_Duration,
            Vector3 t_Displacement,
            Transform t_CustomerTransformReference,
            Sprite  t_Emoji = null,
            string  t_Dialouge = "",
            Color   t_DialougeColor = new Color(),
            UnityAction OnCustomerExpressionComplete = null,
            GameObject t_ExpressionPrefab = null) {
            
            GameObject t_NewExpressionUI;

            // if   : anything except '-1' returned, means the expression already assigned for the following customer
            // else : A new expression need to be created
            int t_ListedExpresionIndex = IsExpressionAlreadyCreatedForCustomer(t_CustomerTransformReference);

            if (t_ListedExpresionIndex != -1)
            {
                ActiveExpression t_ModifiedExpression = m_ListOfActiveExpression[t_ListedExpresionIndex];
                t_ModifiedExpression.duration = t_Duration;

                if(t_Emoji != null){
                    t_ModifiedExpression.expressionAttribute.ChangeEmojiSprite(t_Emoji, true);
                }
                if (t_Dialouge != "")
                    t_ModifiedExpression.expressionAttribute.ChangeDialouge(t_Dialouge, true, t_DialougeColor);

                m_ListOfActiveExpression[t_ListedExpresionIndex] = t_ModifiedExpression;
                t_NewExpressionUI = m_ListOfActiveExpression[t_ListedExpresionIndex].expressionUI.gameObject;
            }
            else
            {

                t_NewExpressionUI = Instantiate(
                        t_ExpressionPrefab == null ? customerExpressionPrefab : t_ExpressionPrefab,
                        transform.position,
                        Quaternion.identity
                    );
                t_NewExpressionUI.transform.SetParent(customerExpressionParent);

                ChacterExpressionAttribute t_NewExpressionAttriubte = t_NewExpressionUI.GetComponent<ChacterExpressionAttribute>();
                t_NewExpressionAttriubte.PreProcess(t_Emoji, t_Dialouge, t_DialougeColor);

                ActiveExpression t_NewExpression = new ActiveExpression()
                {
                    duration = t_Duration,
                    displacement = t_Displacement,
                    customerTransformReference = t_CustomerTransformReference,
                    expressionUI = t_NewExpressionUI.transform,
                    expressionAttribute = t_NewExpressionAttriubte,
                    OnCustomerExpressionComplete = OnCustomerExpressionComplete
                };

                m_ListOfActiveExpression.Add(t_NewExpression);

                if (!m_IsCustomerExpressionControllerRunning)
                {

                    m_IsCustomerExpressionControllerRunning = true;
                    StartCoroutine(ControllerForLifeCycleOfCustomerExpression());
                }
            }

            return t_NewExpressionUI;
        }

        #endregion

        #region Public Callback

        public GameObject ShowDialouge(
            float t_Duration,
            Vector3 t_Displacement,
            Transform t_CustomerTransformReference,
            string t_Dialouge,
            Color t_DialougeColor = new Color(),
            UnityAction OnCustomerExpressionComplete = null,
            GameObject t_ExpressionPrefab = null) {

           return ConfigureCharacterEmojiAndDialouge(
                    t_Duration,
                    t_Displacement,
                    t_CustomerTransformReference,
                    null,
                    t_Dialouge,
                    t_DialougeColor,
                    OnCustomerExpressionComplete,
                    t_ExpressionPrefab
                );
        }

        public GameObject ShowEmoji(
            float t_Duration,
            Vector3 t_Displacement,
            Transform t_CustomerTransformReference,
            Sprite t_ExpressionImage,
            UnityAction OnCustomerExpressionComplete = null,
            GameObject t_ExpressionPrefab = null)
        {
            return ConfigureCharacterEmojiAndDialouge(
                t_Duration,
                t_Displacement,
                t_CustomerTransformReference,
                t_ExpressionImage,
                "",
                new Color(),
                OnCustomerExpressionComplete,
                t_ExpressionPrefab);
        }

        public GameObject ShowExpression(
            float t_Duration,
            Vector3 t_Displacement,
            Transform t_CustomerTransformReference,
            Sprite t_ExpressionImage,
            string t_Dialouge,
            Color t_DialougeColor = new Color(),
            UnityAction OnCustomerExpressionComplete = null,
            GameObject t_ExpressionPrefab = null)
        {

            return ConfigureCharacterEmojiAndDialouge(
                t_Duration,
                t_Displacement,
                t_CustomerTransformReference,
                t_ExpressionImage,
                t_Dialouge,
                t_DialougeColor,
                OnCustomerExpressionComplete,
                t_ExpressionPrefab);
        }

        public void DisposeExression(Transform t_CustomerTransformReference)
        {

            int t_NumberOfActiveExpression = m_ListOfActiveExpression.Count;
            for (int expressionIndex = 0; expressionIndex < t_NumberOfActiveExpression; expressionIndex++)
            {

                if (m_ListOfActiveExpression[expressionIndex].customerTransformReference == t_CustomerTransformReference)
                {

                    ActiveExpression t_ModifiedActiveExpression = m_ListOfActiveExpression[expressionIndex];
                    t_ModifiedActiveExpression.flagedForDispose = true;
                    m_ListOfActiveExpression[expressionIndex] = t_ModifiedActiveExpression;

                    break;
                }
            }
        }

        public void DisposeAllExpression()
        {

            int t_NumberOfActiveExpression = m_ListOfActiveExpression.Count;
            for (int expressionIndex = 0; expressionIndex < t_NumberOfActiveExpression; expressionIndex++)
            {

                ActiveExpression t_ModifiedActiveExpression = m_ListOfActiveExpression[expressionIndex];
                t_ModifiedActiveExpression.flagedForDispose = true;
                m_ListOfActiveExpression[expressionIndex] = t_ModifiedActiveExpression;
            }
        }

        #endregion



    }
}


