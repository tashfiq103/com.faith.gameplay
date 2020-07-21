namespace com.faith.gameplay
{
    using UnityEngine;

    public class SpriteSplatter : MonoBehaviour
    {
        #region Public Variables

        public SpriteRenderer spriteRendererReference;
        public Animator animatorReference;

        #endregion

        #region Public Callback

        public void ShowSplat(Color t_SplatColor, int t_SortingOrder = -1)
        {

            if (t_SortingOrder != -1)
            {

                spriteRendererReference.sortingOrder = t_SortingOrder;
            }

            spriteRendererReference.color = t_SplatColor;
            animatorReference.SetTrigger("APPEAR");
        }

        public void HideSplat()
        {

            animatorReference.SetTrigger("DISAPPEAR");
        }

        #endregion
    }
}


