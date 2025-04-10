using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Multiplayer;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class Lobby : MonoBehaviour
{
    [SerializeField] TMP_InputField sessionID;
    [SerializeField] Button joinOrStartButton, startSessionButton;

    [Header("Session List")]
    [SerializeField] Button sessionListRefreshButton;
    [SerializeField] Transform sessionListAnchor;
    [SerializeField] SessionListElement sessionListElement;
    List<SessionListElement> sessionListElements = new();
    
    private void Start()
    {
        sessionID.onEndEdit.AddListener(value =>
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter)) JoinOrStartSession();
        });
        sessionID.onValueChanged.AddListener(value =>
        {
            joinOrStartButton.interactable = value != string.Empty && value != null;
        });
        joinOrStartButton.interactable = false;
        joinOrStartButton.onClick.AddListener(JoinOrStartSession);
        startSessionButton.onClick.AddListener(StartSession);

        sessionListRefreshButton.onClick.AddListener(RefreshSessionList);
        RefreshSessionList();
    }
    void JoinOrStartSession()
    {
        if (sessionID.text == string.Empty || sessionID.text == null) return;
        SessionManager.Instance.JoinOrStartSessionAsHost(sessionID.text);
    }
    void StartSession()
    {
        SessionManager.Instance.StartSessionAsHost();
    }
    async void RefreshSessionList()
    {
        IList<ISessionInfo> infoList = await SessionManager.Instance.QuerySessions();
        for(int i = 0; i < infoList.Count; i++)
        {
            if(sessionListElements.Count <= i)
            {
                sessionListElements.Add(Instantiate(sessionListElement, sessionListAnchor));
            }
            sessionListElements[i].Set(infoList[i]);
        }
    }
}
