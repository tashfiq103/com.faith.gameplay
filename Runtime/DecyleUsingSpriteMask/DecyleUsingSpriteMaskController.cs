namespace com.faith.Gameplay
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    [RequireComponent(typeof(PreLoadedPrefab))]
    public class DecyleUsingSpriteMaskController : MonoBehaviour
    {

        public static DecyleUsingSpriteMaskController Instance;

        #region Public Variables

        [Space(5.0f)]
        public Gradient defaultSplatGradient;
        [Range(0f, 1f)]
        public float rotationVarient;
        [Range(0f, 0.33f)]
        public float scaleVarient;
        [Range(1f, 5f)]
        public float splatSize = 1f;

        [Space(5.0f)]
        public PreLoadedPrefab preloadedSpriteSplat;

        [Space(5.0f)]
        public SpriteRenderer contentSpriteRenderer;
        public SpriteRenderer backgroundSpriteRenderer;

        #endregion

        #region Private Variables

        private bool m_IsSpriteSplattingEnabled;

        private float m_MaxDistance;
        private Vector3 m_MaxBoundary;
        private Transform m_TransformReferenceForBackgroundSpriteRenderer;


        private Gradient m_SplatGradient;

        private List<SpriteSplatter> m_ActiveSplatter;

        #endregion

        #region Mono Behaviour

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {

                Destroy(gameObject);
            }

            m_TransformReferenceForBackgroundSpriteRenderer = backgroundSpriteRenderer.transform;
            m_MaxBoundary = new Vector3(
                    m_TransformReferenceForBackgroundSpriteRenderer.localScale.x,
                    m_TransformReferenceForBackgroundSpriteRenderer.localScale.y,
                    0
                );
            m_MaxDistance = Vector2.Distance(Vector2.zero, m_MaxBoundary) / 2.0f;
        }

        #endregion

        #region Configuretion

        private void CreateSplatter(Vector3 t_SplatPosition, Color t_SplatColor, int t_SortingOrder = -1)
        {

            if (IsSpriteSplattingEnabled())
            {
                //AudioController.Instance.PlaySoundFXForColorSplat();
                HapticFeedbackController.Instance.TapPeekVibrate();

                Vector3 t_RotationVarient = new Vector3(0f, 0f, Random.Range(0, rotationVarient) * 360);
                Vector3 t_ScaleVarient = Vector3.one * splatSize * Random.Range(1f - scaleVarient, 1);

                GameObject t_NewSplatter = preloadedSpriteSplat.PullPreloadedPrefab();
                t_NewSplatter.transform.position = t_SplatPosition;
                t_NewSplatter.transform.eulerAngles = t_RotationVarient;
                t_NewSplatter.transform.localScale = t_ScaleVarient;

                SpriteSplatter t_NewSpriteSplatter = t_NewSplatter.GetComponent<SpriteSplatter>();
                t_NewSpriteSplatter.ShowSplat(t_SplatColor, t_SortingOrder);
                m_ActiveSplatter.Add(t_NewSpriteSplatter);
            }
        }

        private IEnumerator ControllerForHidingAllSplater()
        {

            WaitForEndOfFrame t_CycleDelay = new WaitForEndOfFrame();

            int t_NumberOfActiveSplatter = m_ActiveSplatter.Count;
            for (int i = 0; i < t_NumberOfActiveSplatter; i++)
            {

                m_ActiveSplatter[i].HideSplat();
                yield return t_CycleDelay;
            }

            StopCoroutine(ControllerForHidingAllSplater());
        }

        #endregion

        #region Public Callback

        public bool IsSpriteSplattingEnabled()
        {
            return m_IsSpriteSplattingEnabled;
        }

        public void EnableSpriteSplatting(Sprite contentSprite, Sprite backgroundSprite = null)
        {

            contentSpriteRenderer.sprite = contentSprite;
            backgroundSpriteRenderer.sprite = backgroundSprite == null ? backgroundSpriteRenderer.sprite : backgroundSprite;

            m_ActiveSplatter = new List<SpriteSplatter>();

            m_IsSpriteSplattingEnabled = true;
        }

        public void DisableSpriteSplatting()
        {

            m_IsSpriteSplattingEnabled = false;
        }

        public void CreateSplatterWithColor(Vector3 t_SplatPosition, Color t_SplatColor, int t_SortingOrder = -1)
        {

            CreateSplatter(t_SplatPosition, t_SplatColor, t_SortingOrder);
        }

        public void CreateSplatterWtthGradient(Vector3 t_SplatPosition, Gradient t_SplatGradeint = null, int t_SortingOrder = -1)
        {

            if (t_SplatGradeint == null)
                m_SplatGradient = defaultSplatGradient;
            else
                m_SplatGradient = t_SplatGradeint;

            CreateSplatter(t_SplatPosition, m_SplatGradient.Evaluate(Random.Range(0f, 1f)), t_SortingOrder);
        }

        public void HideAllSplat()
        {

            StartCoroutine(ControllerForHidingAllSplater());
        }

        public float GetDistanceFromTarget(Vector2 t_ObjectPosition)
        {

            return (Vector2.Distance(t_ObjectPosition, m_TransformReferenceForBackgroundSpriteRenderer.position) / m_MaxDistance);
        }

        #endregion
    }
}


