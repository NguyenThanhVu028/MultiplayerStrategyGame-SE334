using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class RoomLobby : MonoBehaviourPunCallbacks
{
    public enum Maps { RainForest, Desert, SnowHill }
    [Header("Room Elements")]
    [SerializeField] private GameObject notification;
    [SerializeField] private TextMeshProUGUI roomName;

    [Header("Timers")]
    [SerializeField] private Timer gameStartingTimer;

    [Header("Menus")]
    [SerializeField] private GameObject getReadyMenu;
    [SerializeField] List<GameObject> playersInRoomDisplayers = new List<GameObject>();
    [Space]
    [SerializeField] private GameObject selectMapMenu;
    [SerializeField] private GameObject mapNotification;

    //[Header("Configs")]
    //[SerializeField] private MapNames mapNamesConfig;
    //[SerializeField] private Images imagesConfig;

    private PhotonView ptView;
    [SerializeField] private Dictionary<string, int> chosenMaps = new(); //Set this as room custom property so that if the room changes master client, the new master client can obtain properties from its predecessor
    private bool isMapChosen;
    private int chosenMap;
    private Coroutine startGameCoroutine; //Use this to check, when there is a change of master client, the new master client will check if a map has already been chosen and restart a starting game coroutine

    private void Start()
    {
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.CustomProperties.Clear();

        if(roomName != null && PhotonNetwork.CurrentRoom != null)
        {
            roomName.text = "Room " + PhotonNetwork.CurrentRoom.Name;
        }
        getReadyMenu?.SetActive(true);
        selectMapMenu?.SetActive(false);
        ptView = GetComponent<PhotonView>();
        chosenMaps.Clear();
        chosenMap = -1;
        isMapChosen = false;
        LoadPlayersInRoom();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(!isMapChosen)
            {
                if(CountChosenMap() >= PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    StartGame();
                }
                else if(CountChosenMap() >= PhotonNetwork.CurrentRoom.PlayerCount * 0.5f && !gameStartingTimer.IsRunning())
                {
                    gameStartingTimer.StartTimer();
                }
            }
            else
            {
                //If there is already a chosen map but starting game coroutine has begun, restart the starting game coroutine
                if (startGameCoroutine == null && chosenMap != -1)
                {
                    startGameCoroutine = StartCoroutine(StartGameCoroutine(chosenMap));
                }
            }
        }
    }
    public void LeaveRoom()
    {
        if(PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
    public void SetRoomReady()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                notification?.SetActive(true);
                notification?.GetComponent<MultiText>()?.SetText(0, "NOT ENOUGH PLAYERS TO START THE GAME");
                notification?.GetComponent<MultiText>().SetText(1, "There must be at least two players to start the game!");
                return;
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
            ptView.RPC("AnnounceRoomReady", RpcTarget.AllBuffered);
        }
        else
        {
            if (notification != null)
            {
                notification.GetComponent<MultiText>()?.SetText(0, "THIS ACTION IS NOT ALLOW");
                notification.GetComponent<MultiText>()?.SetText(1, "Only room master can proceed this action!");
                notification.SetActive(true);
            }
        }
    }
    public void SelectMap(int map)
    {
        if(!Enum.IsDefined(typeof(Maps), map)) map = (int)Maps.RainForest;
        ptView?.RPC("AnnounceSelectMap", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.UserId, map);
    }
    public void FinishedGameStartingCountDown()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }
    public int CountChosenMap()
    {
        int count = 0;
        foreach(var detail in chosenMaps)
        {
            string id = detail.Key;
            var players = PhotonNetwork.PlayerList;
            foreach( var player in players)
            {
                if(player.UserId == id)
                {
                    count++; break;
                }
            }
        }
        return count;
    }
    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return; // Only master client can start the game
        int map = AnalyzeChosenMaps();

        if (map != -1)
        {
            isMapChosen = true;
            ptView.RPC("AnnounceChosenMap", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId, map);
        }

        startGameCoroutine = StartCoroutine(StartGameCoroutine(map));
    }

    [PunRPC]
    private void AnnounceRoomReady()
    {
        getReadyMenu?.SetActive(false);
        selectMapMenu?.SetActive(true);
    }
    [PunRPC]
    private void AnnounceSelectMap(string playerId, int map)
    {
        //Obtain the previously saved list from room custom properties
        Dictionary<string, int> tempList = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["chosenMaps"];
        if (tempList != null) chosenMaps = tempList;
        else chosenMaps.Clear();

        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        if (chosenMaps.ContainsKey(playerId))
        {
            chosenMaps[playerId] = map;
        }
        else
        {
            chosenMaps.Add(playerId, map);
        }

        //Update the new list to room custom properties
        hashtable.Add("chosenMaps", chosenMaps);
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
    }
    //[PunRPC]
    //private void AnnounceGameStartingCountDown()
    //{
    //    //Debug.Log("Start Count Down");
    //    if(gameStartingTimer != null)
    //    {
    //        gameStartingTimer.ResetTimer();
    //        gameStartingTimer.StartTimer();
    //    }
    //}
    //[PunRPC]
    //private void AnnounceFinishedGameStartingCountDown()
    //{
    //    playersFinshedGameStartingCountDown++;
    //}
    [PunRPC]
    private void AnnounceChosenMap(string userId, int map)
    {
        var players = PhotonNetwork.PlayerList;
        bool isVerified = false;
        foreach (var player in players)
        {
            if (player.IsMasterClient && player.UserId == userId)
            {
                isVerified = true; break;
            }
        }
        if (!isVerified) return;

        isMapChosen = true; chosenMap = map;
        gameStartingTimer.StopTimer();
        gameStartingTimer.SetTimer(0, 0);
        mapNotification?.SetActive(true);
        mapNotification?.GetComponent<MultiImage>()?.SetImage(((Maps)map).ToString());
        mapNotification?.GetComponent<MultiText>()?.SetText(0, ((Maps)map).ToString());
    }

    private int AnalyzeChosenMaps()
    {
        if (chosenMaps.Count == 0) return -1;
        //Obtain the previously saved list from room custom properties
        Dictionary<string, int> tempList = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["chosenMaps"];
        if (tempList != null) chosenMaps = tempList;

        Dictionary<int, int> countMap = new Dictionary<int, int>();
        int max = 0; int mostChosen = -1;

        foreach(var chosenMap in chosenMaps)
        {
            if (countMap.ContainsKey(chosenMap.Value))
            {
                countMap[chosenMap.Value]++;
                if (countMap[chosenMap.Value] > max)
                {
                    max = countMap[chosenMap.Value];
                    mostChosen = chosenMap.Value;
                }
            }
            else
            {
                countMap.Add(chosenMap.Value, 1);
                if (countMap[chosenMap.Value] > max)
                {
                    max = countMap[chosenMap.Value];
                    mostChosen = chosenMap.Value;
                }
            }
        }
        if(max > 1) return (mostChosen);
        else
        {
            System.Random rand = new System.Random();
            int randomNum = rand.Next(0, chosenMaps.Count - 1);
            return (chosenMaps.ElementAt(randomNum).Value);
        }
    }
    private void LoadPlayersInRoom()
    {
        var playersInRoom = PhotonNetwork.PlayerList;
        for(int i = 0; i < playersInRoomDisplayers.Count; i++)
        {
            if(i >= playersInRoom.Length)
            {
                playersInRoomDisplayers[i].SetActive(false);
                continue;
            }
            playersInRoomDisplayers[i]?.SetActive(true);
            playersInRoomDisplayers[i]?.GetComponent<MultiText>()?.SetText(playersInRoom[i].NickName);
        }
        //foreach(var item in playerInRoomDisplayers)
        //{
        //    item.SetActive(false);
        //}
        
        //for(int i = 0; i< players.Count; i++)
        //{
        //    if (i >= playerInRoomDisplayers.Count) return;
        //    playerInRoomDisplayers[i].SetActive(true);
        //    playerInRoomDisplayers[i].GetComponent<CustomText>()?.SetText(players[i].NickName);
        //}
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        LoadPlayersInRoom();
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        LoadPlayersInRoom();
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient) //If there is a change of master client and this client is the new replacement, then obtain the previously saved list from room custom properties
        {
            Dictionary<string, int> tempList = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["chosenMaps"];
            if (tempList != null) chosenMaps = tempList;
            else chosenMaps.Clear();
        }
    }
    private IEnumerator StartGameCoroutine(int map)
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.RemoveBufferedRPCs();
        yield return new WaitForSeconds(5);
        //string sceneName = mapNamesConfig?.GetSceneName(mapName);
        string sceneName = ((Maps)map).ToString();
        PhotonNetwork.LoadLevel(sceneName);
    }
    //[Serializable]
    //class ChosenMapDetail
    //{
    //    string playerId;
    //    string mapName;

    //    public ChosenMapDetail(string playerId, string mapName) { this.playerId = playerId; this.mapName = mapName; }
    //    public void SetPlayerId(string playerId) { this.playerId = playerId; }
    //    public string GetPlayerId() { return playerId; }
    //    public void SetMapName(string mapName) { this.mapName = mapName; }
    //    public string GetMapName() { return mapName; }
    //}
}
