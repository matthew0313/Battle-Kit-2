using Unity.Netcode;
using UnityEngine;

public class LocalOnlyObject : NetworkBehaviour
{
    private void Start()
    {
        gameObject.SetActive(NetworkObject.OwnerClientId == NetworkManager.LocalClientId);
    }
}