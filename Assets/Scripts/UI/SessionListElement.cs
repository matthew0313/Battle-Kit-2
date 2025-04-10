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
    [SerializeField] TMP_Text sessionID;
    [SerializeField] Button sessionJoinButton;
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
        sessionID.text = info.Id;
    }
    void JoinSession()
    {
        SessionManager.Instance.JoinSessionById(currentInfo.Id);
    }
}