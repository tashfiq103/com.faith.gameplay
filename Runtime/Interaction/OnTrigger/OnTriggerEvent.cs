using UnityEngine;

[RequireComponent(typeof(OnTriggerEventListener))]
public abstract class OnTriggerEvent : MonoBehaviour
{

    #region Mono Behaviour

    protected virtual void Awake() {

        OnTriggerEventListener OnTriggerEventListenerReference = GetComponent<OnTriggerEventListener>();

        OnTriggerEventListenerReference.OnTriggerEnterEvent += OnTriggerEnterCallback;
        OnTriggerEventListenerReference.OnTriggerExitEvent  += OnTriggerExitCallback;
    }

    #endregion

    #region AbstructMethod

    protected abstract void OnTriggerEnterCallback(Collider other);
    protected abstract void OnTriggerExitCallback(Collider other);

    #endregion
}
