using UnityEngine;

[RequireComponent(typeof(OnCollisionEventListener))]
public abstract class OnCollisionEvent : MonoBehaviour
{
    #region Mono Behaviour

    protected virtual void Awake()
    {

        OnCollisionEventListener OnCollisionEventListenerReference = GetComponent<OnCollisionEventListener>();

        OnCollisionEventListenerReference.OnCollisionEnterEvent += OnCollisionEnterCallback;
        OnCollisionEventListenerReference.OnCollisionExitEvent  += OnCollisionExitCallback;
    }

    #endregion

    #region AbstructMethod

    protected abstract void OnCollisionEnterCallback(Collision collision);
    protected abstract void OnCollisionExitCallback(Collision collision);

    #endregion
}
