namespace com.faith.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using TMPro;

    public class ChacterExpressionAttribute : MonoBehaviour
    {
        #region Public Variables

        public Image    expressionBackgroundImageReference;
        public Image    expressionImageReference;
        public TextMeshProUGUI characterDialouge;
        public Animator animatorReference;
        public ParticleSystem expressionparticle;

        #endregion

        #region Private Variables


        #endregion

        #region Configuretion

        private IEnumerator ControllerForDisposing(Transform t_FollowedCustomer, Vector3 t_Displacemenet)
        {

            float t_CycleLength = 0.0167f;
            float t_RemainingTimeLeftForAnimationToEnd = 1f;
            WaitForSeconds t_CycleDelay = new WaitForSeconds(t_CycleLength);

            Transform t_TransformReference = transform;
            Vector3 t_ModifiedPosition;

            if(expressionparticle != null)
                expressionparticle.Stop();
            animatorReference.SetTrigger("DISAPPEAR");

            while (t_RemainingTimeLeftForAnimationToEnd > 0f)
            {

                if (t_FollowedCustomer != null)
                {
                    t_ModifiedPosition = t_FollowedCustomer.position + t_Displacemenet;
                    t_TransformReference.position = t_ModifiedPosition;
                }

                yield return t_CycleDelay;
                t_RemainingTimeLeftForAnimationToEnd -= t_CycleLength;
            }

            StopCoroutine(ControllerForDisposing(null, Vector3.zero));

            Destroy(gameObject);
        }

        #endregion

        #region Public Callback

        public void ChangeEmojiSprite(Sprite t_ExpressionSprite, bool t_ShowChangeAnimation)
        {

            expressionImageReference.sprite = t_ExpressionSprite;

            if (t_ShowChangeAnimation)
            {
                if(expressionparticle != null){

                    expressionparticle.Clear();
                    expressionparticle.Play();
                }   
                
                animatorReference.SetTrigger("SWITCH");
            }

        }

        public void ChangeDialouge(string t_Dialouge, bool t_ShowChangeAnimation, Color t_DialougeColor = new Color()) {

            characterDialouge.text  = t_Dialouge;
            characterDialouge.color = t_DialougeColor == new Color() ? Color.black : t_DialougeColor;


            if (t_ShowChangeAnimation)
            {
                if(expressionparticle != null){
                    
                    expressionparticle.Clear();
                    expressionparticle.Play();
                }
                
                animatorReference.SetTrigger("SWITCH");
            }
        }

        public void PreProcess(Sprite t_ExpressionSprite = null, string t_Dialouge = "", Color t_DialougeColor = new Color())
        {
            animatorReference.SetTrigger("APPEAR");

            if(t_ExpressionSprite != null){
                
                if(expressionparticle != null)
                    expressionparticle.Play();
                ChangeEmojiSprite(t_ExpressionSprite, false);
            }else{
                if(expressionBackgroundImageReference != null)
                    expressionBackgroundImageReference.gameObject.SetActive(false);
                expressionImageReference.gameObject.SetActive(false);
            }
                

            ChangeDialouge(t_Dialouge, false, t_DialougeColor);
        }

        public void PostProcess(Transform t_FollowedCustomer, Vector3 t_Displacemenet)
        {

            StartCoroutine(ControllerForDisposing(t_FollowedCustomer, t_Displacemenet));
        }

        #endregion

    }
}


