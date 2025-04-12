using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;

    [Header("Items")]
    [SerializeField] ItemDatabase m_itemDatabase;
    public ItemDatabase itemDatabase => m_itemDatabase;
}