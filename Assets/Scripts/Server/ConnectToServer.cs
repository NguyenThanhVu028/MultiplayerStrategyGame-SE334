using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;
using WebSocketSharp;
using UnityEngine.Events;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [Header("Properties")]
    [SerializeField] TMP_InputField input;
    [SerializeField] GameObject Notification;
    [SerializeField] float loggingDelayTime = 1; // Used for animation
    [Header("Events")]
    [SerializeField] UnityEvent onLoggingEvent;
    [SerializeField] UnityEvent onLoggingFailedEvent;
    [SerializeField] UnityEvent onJoinedLobby;
    private void Start()
    {
        // Check if player has logged in before then automatically login with the latest username
        var stats = PlayerStatsManager.GetInstance()?.LoadPlayerStats();
        if (!stats.GetLatestLogin().IsNullOrEmpty())
        {
            onLoggingEvent.Invoke();
            StartCoroutine(ConnectingCoroutine(stats.GetLatestLogin()));
        }
    }
    public void Login()
    {
        if (input.text.IsNullOrEmpty())
        {
            Notification.GetComponent<MultiText>()?.SetText(0, "ERROR");
            Notification.GetComponent<MultiText>()?.SetText(1, "Please enter a name for your player!");
            Notification.SetActive(true);
            return;
        }

        // Save this username for later login
        var stats = PlayerStatsManager.GetInstance()?.LoadPlayerStats();
        if (stats != null) stats.SetLatestLogin(input.text);
        PlayerStatsManager.GetInstance()?.SavePlayerStats(stats);

        onLoggingEvent.Invoke();
        StartCoroutine(ConnectingCoroutine(input.text));
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        onLoggingFailedEvent.Invoke();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        onJoinedLobby.Invoke();
    }
    IEnumerator ConnectingCoroutine(string name)
    {
        yield return new WaitForSeconds(loggingDelayTime);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LocalPlayer.NickName = name;
        PhotonNetwork.ConnectUsingSettings();
    }
}
