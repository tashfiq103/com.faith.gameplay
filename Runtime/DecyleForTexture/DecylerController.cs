namespace com.faith.gameplay
{
    using UnityEngine;
    using System.Collections;

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DecylerController : MonoBehaviour
    {
        #region Public Variables

        public bool isUseEmbededTouchControl;

        [Space(5.0f)]
        public DecyleForTexture decyleForTexture;
        [Range(0f, 1f)]
        public float delayOfRandomDecyle = 1f;
        public Gradient decyleColor;

        [Space(5.0f)]
        public Camera cameraRefeference;

        [Space(5.0f)]
        public Sprite defaultSpriteToDecyle;

        #endregion

        #region Private Variables

        private bool m_IsDecyleEnabled;
        private bool m_IsTouchPressed = false;

        private bool m_IsRandomDecyleControllerRunning;

        private Transform m_TransformReference;
        private Transform m_CameraTransformReference;

        private SpriteRenderer m_SpriteRendererReference;

        private RaycastHit2D m_RayCastHit;

        private Vector3 m_TouchDownPosition;
        private Vector3 m_TouchPosition;
        private Vector3 m_TouchUpPosition;

        #endregion

        #region Mono Behaviour

        private void Awake()
        {
            m_TransformReference = transform;
            m_CameraTransformReference = cameraRefeference.transform;

            m_SpriteRendererReference = gameObject.GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (isUseEmbededTouchControl)
            {

                TouchController();
            }
        }

        private void FixedUpdate()
        {
            if (m_IsDecyleEnabled && m_IsTouchPressed)
            {

                m_RayCastHit = Physics2D.Raycast(m_CameraTransformReference.position, (m_TransformReference.position - m_CameraTransformReference.position));
                if (m_RayCastHit.collider != null)
                {

                }
            }
        }

        #endregion

        #region Configuretion   :   Touch Controller

        private void TouchController()
        {

#if UNITY_EDITOR

            if (Input.GetMouseButtonDown(0))
            {

                OnTouchDown(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {

                OnTouch(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {

                OnTouchUp(Input.mousePosition);
            }

#elif UNITY_ANDROID || UNITY_IOS
    switch (Input.GetTouch(0).phase) {

            case TouchPhase.Began:
                OnTouchDown(Input.GetTouch(0).position);
                break;
            case TouchPhase.Stationary:
                break;
            case TouchPhase.Moved:
                OnTouch(Input.GetTouch(0).position);
                break;
            case TouchPhase.Ended:
                OnTouchUp(Input.GetTouch(0).position);
                break;
            case TouchPhase.Canceled:
                OnTouchUp(Input.GetTouch(0).position);
                break;
        }
#endif

        }

        private void OnTouchDown(Vector2 t_TouchPosition)
        {

            m_IsTouchPressed = true;

            m_TouchDownPosition = t_TouchPosition;
        }

        private void OnTouch(Vector2 t_TouchPosition)
        {
            m_TouchPosition = t_TouchPosition;
        }

        private void OnTouchUp(Vector2 t_TouchPosition)
        {
            m_TouchUpPosition = t_TouchPosition;

            m_IsTouchPressed = false;
        }

        #endregion

        #region Configuretion

        private IEnumerator ControllerForRandomDecyle()
        {

            float t_CycleLength = delayOfRandomDecyle;
            WaitForSeconds t_CycleDelay = new WaitForSeconds(t_CycleLength);

            Texture2D t_CopyOfDefaultTextureForDecyle = new Texture2D(defaultSpriteToDecyle.texture.width, defaultSpriteToDecyle.texture.height);
            t_CopyOfDefaultTextureForDecyle.SetPixels(defaultSpriteToDecyle.texture.GetPixels());
            m_SpriteRendererReference.sprite = Sprite.Create(
                        t_CopyOfDefaultTextureForDecyle,
                        new Rect(0, 0, defaultSpriteToDecyle.texture.width, defaultSpriteToDecyle.texture.height), new Vector2(0.5f, 0.5f), (defaultSpriteToDecyle.texture.width + defaultSpriteToDecyle.texture.height) / 2.0f
                    );

            while (m_IsRandomDecyleControllerRunning)
            {

                Texture2D t_ModifiedTexture = decyleForTexture.GetDecyleTexture(
                        0,
                        m_SpriteRendererReference.sprite.texture,
                        new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)),
                        decyleColor.Evaluate(Random.Range(0f, 1f))
                    );

                int t_WidthOfTexture = t_ModifiedTexture.width;
                int t_HeightOfTexture = t_ModifiedTexture.height;

                Sprite t_ModifiedSprite = Sprite.Create(
                        t_ModifiedTexture,
                        new Rect(0, 0, t_WidthOfTexture, t_HeightOfTexture), new Vector2(0.5f, 0.5f), (t_WidthOfTexture + t_HeightOfTexture) / 2.0f
                    );

                m_SpriteRendererReference.sprite = t_ModifiedSprite;

                yield return t_CycleDelay;
            }

            StopCoroutine(ControllerForRandomDecyle());
        }

        #endregion

        #region Public Callback

        public void EnableDecycle()
        {

            m_IsDecyleEnabled = true;
        }

        public void DisableDecycle()
        {
            m_IsDecyleEnabled = false;
        }

        public void StartRandomDecyle()
        {
            if (!m_IsRandomDecyleControllerRunning)
            {

                m_IsRandomDecyleControllerRunning = true;
                StartCoroutine(ControllerForRandomDecyle());
            }
        }

        public void StopRandomDecyle()
        {

            m_IsRandomDecyleControllerRunning = false;
        }

        #endregion
    }
}


