using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DeviceActivator : MonoBehaviour
{
    [SerializeField] GameObject desktop, mobile, console;
    private void OnEnable()
    {
        if(desktop != null) desktop.gameObject.SetActive(DeviceManager.deviceType == DeviceType.Desktop);
        if(mobile != null) mobile.gameObject.SetActive(DeviceManager.deviceType == DeviceType.Handheld);
        if(console != null) console.gameObject.SetActive(DeviceManager.deviceType == DeviceType.Console);
    }
}