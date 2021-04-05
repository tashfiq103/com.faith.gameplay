using System;
using UnityEngine;

public class OnCollisionEventListener : MonoBehaviour
{
    #region Public Variables

    public event Action<Collision> OnCollisionEnterEvent;
    public event Action<Collision> OnCollisionExitEvent;

    #endregion

    #region Mono Behaviour

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnterEvent?.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        OnCollisionExitEvent?.Invoke(collision);
    }

    #endregion
}
