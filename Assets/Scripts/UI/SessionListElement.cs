using Unity.Netcode;
using UnityEngine;
using Unity.Multiplayer.Widgets;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Multiplayer;

public class SessionListElement : MonoBehaviour
{
    [SerializeField] Button sessionJoinButton;
    [SerializeField] IRef<IHookProvider<string>> sessionIDHook;
    [SerializeField] IRef<IHookProvider<int>> playerCountHook;
    private void OnEnable()
    {
        sessionJoinButton.onClick.AddListener(JoinSession);
    }
    private void OnDisable()
    {
        sessionJoinButton.onClick.AddListener(JoinSession);
    }
    ISessionInfo currentInfo;
    public void Set(ISessionInfo info)
    {
        currentInfo = info;
        sessionIDHook.I.SetValue(info.Id);
        playerCountHook.I.SetValue(info.MaxPlayers - info.AvailableSlots);
    }
    void JoinSession()
    {
        SessionManager.Instance.JoinSessionById(currentInfo.Id);
    }
}