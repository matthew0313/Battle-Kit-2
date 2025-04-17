using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Multiplayer;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;

public class Lobby : MonoBehaviour
{
    [SerializeField] TMP_InputField sessionID;
    [SerializeField] Button joinOrStartButton, startSessionButton;

    [Header("Session List")]
    [SerializeField] Button sessionListRefreshButton;
    [SerializeField] Transform sessionListAnchor;
    [SerializeField] SessionListElement sessionListElement;
    List<SessionListElement> sessionListElements = new();

    private void Awake()
    {
        SessionManager.Instance.onSignIn += OnSignIn;
    }
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
    }
    private void OnDestroy()
    {
        SessionManager.Instance.onSignIn -= OnSignIn;
    }
    void OnSignIn()
    {
        RefreshSessionList();
    }
    void JoinOrStartSession()
    {
        if (sessionID.text == string.Empty || sessionID.text == null) return;
        SessionManager.Instance.JoinOrStartSession(sessionID.text);
    }
    void StartSession()
    {
        SessionManager.Instance.StartSession();
    }
    async void RefreshSessionList()
    {
        IList<ISessionInfo> infoList = await SessionManager.Instance.QuerySessions();
        int i;
        for(i = 0; i < infoList.Count; i++)
        {
            if(sessionListElements.Count <= i)
            {
                sessionListElements.Add(Instantiate(sessionListElement, sessionListAnchor));
            }
            sessionListElements[i].Set(infoList[i]);
            sessionListElements[i].gameObject.SetActive(true);
        }
        for(; i < sessionListElements.Count; i++)
        {
            sessionListElements[i].gameObject.SetActive(false);
        }
    }
}
