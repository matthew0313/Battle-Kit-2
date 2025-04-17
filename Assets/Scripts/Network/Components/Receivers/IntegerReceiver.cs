using System;
using Unity.Netcode;
using UnityEngine;

public class IntegerReceiver : NetworkBehaviour
{
    public Action<int> onResponseReceive;
    [Rpc(SendTo.Owner)]
    public void ResponseRpc(int data)
    {
        onResponseReceive?.Invoke(data);
    }
}