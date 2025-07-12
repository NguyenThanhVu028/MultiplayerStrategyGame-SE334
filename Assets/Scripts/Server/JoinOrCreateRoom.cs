using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class JoinOrCreateRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField RoomCode;
    [SerializeField] GameObject RoomsListUI;
    [SerializeField] GameObject Notification;

    [Header("Room properties")]
    [SerializeField] int maxPlayer = 4;
    [SerializeField] bool isVisible = true;
    [SerializeField] bool isOpen = true;
    [SerializeField] bool publishUserId = true;
    [SerializeField] bool cleanUpCacheOnLeave = false;

    private List<GameObject> Rooms = new List<GameObject>();
    public void LogOut()
    {
        if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
    }
    public void CreateRoom()
    {
        string name = RoomCode.text;
        if (name.IsNullOrEmpty())
        {
            Notification.GetComponent<MultiText>()?.SetText(0, "CAN'T CREATE ROOM");
            Notification.GetComponent<MultiText>()?.SetText(1, "Please enter a room code!");
            Notification.SetActive(true);
            return;
        }
        PhotonNetwork.CreateRoom(name, new RoomOptions() { MaxPlayers = maxPlayer, IsVisible = isVisible, IsOpen = isOpen, PublishUserId = publishUserId, CleanupCacheOnLeave = cleanUpCacheOnLeave }, TypedLobby.Default, null);  
    }
    public void JoinRoom()
    {
        string name = RoomCode.text;
        if (name.IsNullOrEmpty())
        {
            Notification.GetComponent<MultiText>()?.SetText(0, "CAN'T JOIN ROOM");
            Notification.GetComponent<MultiText>()?.SetText(1, "Please enter a room code!");
            Notification.SetActive(true);
            return;
        }
        PhotonNetwork.JoinRoom(name);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadSceneAsync("ConnectToServer");
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("RoomLobby");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //OnCreateRoomFailedEvent.Invoke();
        if (Notification != null)
        {
            Notification.GetComponent<MultiText>()?.SetText(0, "FAILED");
            Notification.GetComponent<MultiText>()?.SetText(1, message);
            Notification.SetActive(true);
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //OnJoineRoomFailedEvent.Invoke();
        if (Notification != null)
        {
            Notification.GetComponent<MultiText>().SetText(0, "FAILED");
            Notification.GetComponent<MultiText>().SetText(1, message);
            Notification.SetActive(true);
        }
    }
    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{
    //    Debug.Log("Updated");
    //    foreach (var item in roomList) 
    //    {
    //        if (item.RemovedFromList)
    //        {
    //            RemoveRoom(item.Name);
    //        }
    //        else
    //        {
    //            AddRoom(item);
    //        }
    //    }
    //}
    //private void RemoveRoom(string roomCode)
    //{
    //    foreach (var item in Rooms) 
    //    {
    //        if (item.GetComponent<RoomButton>() != null)
    //        {
    //            if (item.GetComponent<RoomButton>().GetRoomCode() == roomCode)
    //            {
    //                item.gameObject.SetActive(false);
    //                Rooms.Remove(item);
    //                return;
    //            }
    //        }

    //    }
    //}
    //private void AddRoom(RoomInfo info)
    //{
    //    foreach(var room in Rooms)
    //    {
    //        if(room.GetComponent<RoomButton>() != null)
    //        {
    //            if(room.GetComponent <RoomButton>().GetRoomCode() == info.Name)
    //            {
    //                room.GetComponent<RoomButton>().SetPlayersInRoom(info.PlayerCount);
    //                return;
    //            }
    //        }
    //    }

    //    var item = PoolManager.Instance.SpawnObjectLocal("RoomButton");
    //    if(item.GetComponent<RoomButton>() != null)
    //    {
    //        item.SetActive(true);
    //        item.GetComponent<RoomButton>().SetRoomCode(info.Name);
    //        item.transform.SetParent(RoomsListUI.transform);
    //        item.transform.localScale = Vector3.one;
    //        item.GetComponent<RoomButton>().SetPlayersInRoom(info.PlayerCount);
    //        Rooms.Add(item.gameObject);
    //    }
    //}
}
