
using Unity.VisualScripting;
using UnityEngine;

public class TestAAR : MonoBehaviour
{
    // public void OnClickShowToast() {
    //     ExcuteVibration.Instance.ShowToast("Hi my name is lou.");
    // }
    //
    // public void OnClickGetRandomNumber() {
    //     ExcuteVibration.Instance.GetRandomNumber();
    // }

    public void OnClickVibrate()
    {
        ExcuteVibration.Instance.Perfect();
    }

    public void Fail()
    {
        ExcuteVibration.Instance.Fail();
    }

    public void Click()
    {
        ExcuteVibration.Instance.Touch();
    }
    
    
}
