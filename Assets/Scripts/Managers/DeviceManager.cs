using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class DeviceManager : MonoBehaviour
{
    static DeviceManager Instance;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Instance = Instantiate(Resources.Load<DeviceManager>("DeviceManager"));
        DontDestroyOnLoad(Instance.gameObject);
    }

    [SerializeField] bool debugOverride = false;
    [SerializeField] DeviceType overrideDeviceType = DeviceType.Desktop;
    public static DeviceType deviceType
    {
        get
        {
            if (Instance != null && Instance.debugOverride) return Instance.overrideDeviceType;
            return SystemInfo.deviceType;
        }
    }
}