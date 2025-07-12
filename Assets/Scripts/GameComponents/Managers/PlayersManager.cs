using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;
using static Dinosaur;

public class PlayersManager : MonoBehaviourPunCallbacks
{
    //This class is used to manage local player and communicate between players in server

    [Header("Configs")]
    [SerializeField] DinoInfos dinoInfosConfig;

    [Header("Notification")]
    [SerializeField] GameObject notification;
    [Header("Player")]

    //Local player info
    [SerializeField] Player player;
    [SerializeField] MultiImage dinoImage;
    [SerializeField] GameObject playerColor;
    [SerializeField] MultiText dinoInfo;

    //Players datas
    private Dictionary<string, string> playersDinos = new Dictionary<string, string>();
    private Dictionary<string, string> playersColors = new Dictionary<string, string>();

    private string selectedColor = null;
    private string selectedDino = null;

    //AI datas
    private Dictionary<string, string> aiDino = new Dictionary<string, string>();

    static PlayersManager instance;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if(GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online)
        {
            if (!GameManager.GetInstance().IsMatchStarted() && PhotonNetwork.IsMasterClient)
            {
                if (CountReadyPlayer() >= PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    GameManager.GetInstance()?.SetStartMatch();
                }
                else if (CountReadyPlayer() >= PhotonNetwork.CurrentRoom.PlayerCount * 0.5f)
                {
                    GameManager.GetInstance()?.StartMatchStartingTimer();
                }
            }
        }
    }
    public static PlayersManager GetInstance() { return instance; }

    // Local player details
    public Player GetLocalPlayer() { return player; }
    public string GetPlayerColor() { return player?.GetPlayerColor(); }
    public string GetPlayerDino() { return player?.GetPlayerDino(); }

    // Online players detail
    private int CountReadyPlayer()
    {
        //Count players who already chose both dino and color
        int count = 0;
        var players = PhotonNetwork.PlayerList;
        foreach(var player in players)
        {
            if(playersColors.ContainsKey(player.UserId) && playersDinos.ContainsKey(player.UserId))
            {
                count++;
            }
        }
        //foreach(var playersColors in playersColors)
        //{
        //    if (playersDinos.ContainsKey(playersColors.Key)) count++;
        //}
        return count;
    }
    public Dictionary<string, string> GetPlayersColor()
    {
        Dictionary<string, string> tempPlayersColors = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties["playersColors"];
        if (tempPlayersColors != null) playersColors = tempPlayersColors;
        else playersColors.Clear();

        return playersColors;
    }
    public Dictionary<string, string> GetPlayersDino()
    {
        Dictionary<string, string> tempPlayersDinos = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties["playersDinos"];
        if (tempPlayersDinos != null) playersDinos = tempPlayersDinos;
        else playersDinos.Clear();

        return playersDinos;
    }
    // AI detail
    public void SetAIDino(string color, string dino)
    {
        if(aiDino.ContainsKey(color)) aiDino[color] = dino;
        else aiDino.Add(color, dino);
    }
    public string GetAIDino(string color)
    {
        if(aiDino.ContainsKey(color)) return aiDino[color];
        return null;
    }

    // Room Functions
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if(PhotonNetwork.IsMasterClient) //If there is a change of master client and this client is the new replacement, then obtain the previously saved list from room custom properties
        {
            Dictionary<string, string> tempPlayersColors = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties["playersColors"];
            if (tempPlayersColors != null) playersColors = tempPlayersColors;
            else playersColors.Clear();

            Dictionary<string, string> tempPlayersDinos = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties["playersDinos"];
            if (tempPlayersDinos != null) playersDinos = tempPlayersDinos;
            else playersDinos.Clear();
        }
    }

    // RPC Functions
    public void SelectColor(string color)
    {
        if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online)
        {
            selectedColor = color;
        }
        else
        {
            player?.SetPlayerColor(color);
        }
        playerColor.GetComponent<ICustomColor>()?.SetColor(color); //Update color displayer in select dino menu
    }
    public void SelectDino(string dinoName)
    {
        if(GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online)
        {
            selectedDino = dinoName;
        }
        else
        {
            player?.SetPlayerDino(dinoName);
        }
        dinoImage?.SetImage(dinoName);
        dinoInfo?.SetText(dinoInfosConfig.GetInfo(dinoName));
    }
    public void ConfirmSelected()
    {
        if(GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online)
        {
            if(selectedColor.IsNullOrEmpty() || selectedDino.IsNullOrEmpty())
            {
                notification?.SetActive(true);
                notification?.GetComponent<MultiText>()?.SetText(0, "UNABLE TO CONFIRM!");
                notification?.GetComponent<MultiText>()?.SetText(1, "Mak sure that you have chosen both color and dino");
            }
            else
            {
                GetComponent<PhotonView>()?.RPC("AnnounceSelected", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.UserId, selectedColor, selectedDino);
            }
        }
        else
        {
            if (GetPlayerColor().IsNullOrEmpty() || GetPlayerDino().IsNullOrEmpty())
            {
                notification?.SetActive(true);
                notification?.GetComponent<MultiText>()?.SetText(0, "UNABLE TO START THE MATCH!");
                notification?.GetComponent<MultiText>()?.SetText(1, "Mak sure that you have chosen both color and dino");
            }
            else GameManager.GetInstance()?.SetStartMatch();
        }
    }
    [PunRPC]
    private void AnnounceSelected(string playerId, string color, string dinoName)
    {
        // Send to master client

        bool acceptColor = true;
        bool acceptDino = true;

        //Obtain the previously saved list from room custom properties
        Dictionary<string, string> tempColorList = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties["playersColors"];
        if (tempColorList != null) playersColors = tempColorList;
        else playersColors.Clear();

        Dictionary<string, string> tempDinoList = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties["playersDinos"];
        if (tempDinoList != null) playersDinos = tempDinoList;
        else playersDinos.Clear();

        foreach (var playerColor in playersColors)
        {
            if (playerColor.Value == color && playerColor.Key != playerId) // Some one has already chosen this color
            {
                acceptColor = false;
            }
        }

        if(acceptColor)
        {
            try
            {
                ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
                if (playersColors.ContainsKey(playerId))
                {
                    playersColors[playerId] = color;
                }
                else
                {
                    playersColors.Add(playerId, color);
                }
                //Update the new list to room custom properties
                hashtable.Add("playersColors", playersColors);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

                acceptColor = true;
            }
            catch
            {
                acceptColor = false;
            }
        }

        try
        {
            if (dinoInfosConfig != null && dinoInfosConfig.CheckExist(dinoName))
            {
                ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
                if (playersDinos.ContainsKey(playerId))
                {
                    playersDinos[playerId] = dinoName;
                }
                else
                {
                    playersDinos.Add(playerId, dinoName);
                }

                //Update the new list to room custom properties
                hashtable.Add("playersDinos", playersDinos);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
                
                acceptDino = true;
            }
            else
            {
                acceptDino = false;
            }
        }
        catch
        {
            acceptDino = false;
        }

        GetComponent<PhotonView>()?.RPC("AnnounceResult", RpcTarget.All, playerId, acceptColor, color, acceptDino, dinoName);
    }
    [PunRPC]
    private void AnnounceResult(string playerId, bool acceptColor, string color, bool acceptDino, string dinoName)
    {
        if(PhotonNetwork.LocalPlayer.UserId == playerId)
        {
            if(acceptColor) player.SetPlayerColor(color);
            if(acceptDino) player.SetPlayerDino(dinoName);

            string result = "";
            result += (acceptColor) ? "Accept color, " : "Deny color,";
            result += (acceptDino) ? "accept dino." : "deny dino";

            notification?.SetActive(true);
            notification?.GetComponent<MultiText>()?.SetText(0, "RESULT");
            notification?.GetComponent<MultiText>()?.SetText(1, result);
        }
    }
}
