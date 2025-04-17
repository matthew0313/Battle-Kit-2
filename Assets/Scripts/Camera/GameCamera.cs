using Unity.Netcode;
using UnityEngine;

public class GameCamera : MonoBehaviour, ILocalPlayerSpawnCallbackReceiver
{
    [SerializeField] float lerpRate = 5.0f;
    Player player;

    public void OnLocalPlayerSpawn(Player player)
    {
        this.player = player;
        SetPos(player.transform.position);
    }

    private void FixedUpdate()
    {
        if(player != null)
        {
            SetPos(Vector2.Lerp(transform.position, player.transform.position, lerpRate * Time.fixedDeltaTime));
        }
    }
    void SetPos(Vector2 pos)
    {
        transform.position = pos;
        transform.position += new Vector3(0, 0, -10.0f);
    }
}