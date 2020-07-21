
using UnityEngine;

public class HapticFeedbackController : MonoBehaviour
{
    #region Public Variables

    public static HapticFeedbackController Instance;

    #endregion

    #region Private Variables

    private string IS_HAPTIC_FEEDBACK_ENABLED = "IS_HAPTIC_FEEDBACK_ENABLED";

    #endregion

    #region Mono Behaviour

    private void Awake()
    {
        if (Instance == null)
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {

            Destroy(gameObject);
        }
    }

    #endregion

    #region Public Callback

    public bool IsHapticFeedbackEnabled() {

        if (PlayerPrefs.GetInt(IS_HAPTIC_FEEDBACK_ENABLED, 1) == 1)
            return true;

        return false;
    }

    public void EnableHapticFeedback()
    {
        PlayerPrefs.SetInt(IS_HAPTIC_FEEDBACK_ENABLED, 1);
    }

    public void DisableHapticFeedback()
    {
        PlayerPrefs.SetInt(IS_HAPTIC_FEEDBACK_ENABLED, 0);
    }

    public void TapVibrate()
    {
        if (IsHapticFeedbackEnabled())
        {
            Vibration.Vibrate();
        }
    }

    public void TapVibrateCustom()
    {
        if (IsHapticFeedbackEnabled())
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        //Vibration.Vibrate ( int.Parse ( inputTime.text ) );
#endif
        }
    }

    public void TapVibratePattern()
    {
        if (IsHapticFeedbackEnabled())
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        //Vibration.Vibrate ( longs, int.Parse ( inputRepeat.text ) );
#endif
        }
    }

    public void TapCancelVibrate()
    {
        if (IsHapticFeedbackEnabled())
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        //Vibration.Cancel ();
#endif
        }
    }

    //Pop vibration: weak boom (only available with the haptic engine: iPhone 6s minimum)
    public void TapPopVibrate()
    {
        if (IsHapticFeedbackEnabled())
        {
#if UNITY_IOS && !UNITY_EDITOR
        Vibration.VibratePop ();
#endif
        }
    }

    //Peek vibration: strong boom (only available with the haptic engine: iPhone 6s minimum)
    public void TapPeekVibrate()
    {
        if (IsHapticFeedbackEnabled())
        {
#if UNITY_IOS && !UNITY_EDITOR
        Vibration.VibratePeek ();
#endif
        }
    }

    //Nope vibration: series of three weak booms (only available with the haptic engine: iPhone 6s minimum)
    public void TapNopeVibrate()
    {
        if (IsHapticFeedbackEnabled()) {

#if UNITY_IOS && !UNITY_EDITOR
        Vibration.VibrateNope ();
#endif
        }
    }

    #endregion
}
