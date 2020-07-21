////////////////////////////////////////////////////////////////////////////////
//  
// @author Benoît Freslon @benoitfreslon
// https://github.com/BenoitFreslon/Vibration
// https://benoitfreslon.com
//
////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VibrationExample : MonoBehaviour
{
    public Text inputTime;
    public Text inputPattern;
    public Text inputRepeat;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void TapVibrate ()
    {
        Vibration.Vibrate ();
    }

    public void TapVibrateCustom()
    {
        Debug.Log(inputTime.text);
#if UNITY_ANDROID && !UNITY_EDITOR
        Vibration.Vibrate ( int.Parse ( inputTime.text ) );
#endif

    }

    public void TapVibratePattern()
    {
        long[] longs = inputPattern.text.Select(item => (long)item).ToArray();
        Debug.Log(longs + " " + int.Parse(inputRepeat.text));

#if UNITY_ANDROID && !UNITY_EDITOR
        Vibration.Vibrate ( longs, int.Parse ( inputRepeat.text ) );
#endif

    }

    public void TapCancelVibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Vibration.Cancel ();
#endif

    }

    public void TapPopVibrate()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Vibration.VibratePop ();
#endif

    }

    public void TapPeekVibrate()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Vibration.VibratePeek ();
#endif

    }

    public void TapNopeVibrate()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Vibration.VibrateNope ();
#endif

    }
}
