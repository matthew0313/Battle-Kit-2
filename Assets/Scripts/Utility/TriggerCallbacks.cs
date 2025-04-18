using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TriggerCallbacks : MonoBehaviour
{
    public Action<Collider2D> onTriggerEnter2D, onTriggerStay2D, onTriggerExit2D;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggerEnter2D?.Invoke(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        onTriggerStay2D?.Invoke(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        onTriggerExit2D?.Invoke(collision);
    }
}