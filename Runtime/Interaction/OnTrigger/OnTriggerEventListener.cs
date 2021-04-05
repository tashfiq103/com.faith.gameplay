using System;
using UnityEngine;

public class OnTriggerEventListener : MonoBehaviour
{
    #region Public Variables

    public event Action<Collider> OnTriggerEnterEvent;
    public event Action<Collider> OnTriggerExitEvent;

    #endregion

    #region Mono Behaviour

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }

    #endregion
}
