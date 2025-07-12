using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using WebSocketSharp;

public class GameManager:MonoBehaviourPunCallbacks
{
    // This is used to manage and operate the game

    public enum GameMode { Offline, Online}
    [Header("GameMode")]
    [SerializeField] private GameMode gameMode;

    [Header("Timers")]
    [SerializeField] private Timer matchStartingTimer;
    [SerializeField] private Timer matchEndTimer;
    [SerializeField] private Timer nextMatchTimer;

    [Header("Menus")]
    [SerializeField] GameObject selectDinoMenu;
    [SerializeField] GameObject drawMenu;
    [SerializeField] GameObject victoryMenu;
    [SerializeField] GameObject defeatedMenu;

    [Header("Notification")]
    [SerializeField] GameObject notification;

    [Header("Player displayers")]
    [SerializeField] List<MultiImage> playersDinoImage;
    [SerializeField] List<GameObject> playersColor;
    [SerializeField] List<MultiText> playersName;

    [Header("Used for offline mode")]
    [SerializeField] private NameList levels;

    static GameManager instance;

    [Header("Used to spawn dinos")]
    private List<SpawnerDetail> registeredSpawnerList = new List<SpawnerDetail>();
    [SerializeField] private GameObject target = null;

    //Used for the match
    [SerializeField]private bool isMatchStarted = false;
    [SerializeField]private bool isMatchEnded = false;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        selectDinoMenu.SetActive(true);
        victoryMenu.SetActive(false);
        defeatedMenu.SetActive(false);
        notification.SetActive(false);
        drawMenu.SetActive(false);

        matchStartingTimer.StopTimer();
        matchEndTimer.StopTimer();
        nextMatchTimer.StopTimer();

        //isMatchStartingTimerStarted = false;
        isMatchStarted = false;
        isMatchEnded = false;
    }
    private void Update()
    {
        if(isMatchStarted)
        {
            if(GetComponent<ProcessMouseInput>() != null) GetComponent<ProcessMouseInput>().enabled = true; // Enable mouse input
            if (!isMatchEnded && !matchEndTimer.IsRunning()) matchEndTimer.StartTimer();
        }
        else
        {
            if (GetComponent<ProcessMouseInput>() != null) GetComponent<ProcessMouseInput>().enabled = false; // Disable mouse input

            if(matchEndTimer.IsRunning() || nextMatchTimer.IsRunning()) isMatchStarted = true;
        }
    }
    public static GameManager GetInstance()
    {
        return instance;
    }

    //Used for spawning army
    public void StartSpawningArmy()
    {
        if (IsMouseOverUI()) return; // Don't spawn if the mouse is blocked by an UI

        if (target == null) // If there is no target
        {
            registeredSpawnerList.Clear();
            return;
        }

        foreach (var spawner in registeredSpawnerList)
        {
            if (spawner != null) spawner.spawner.SpawnArmy(target);
            if (spawner.arrow != null) spawner.arrow.gameObject.SetActive(false);
        }
        registeredSpawnerList.Clear();
        target = null;
    }
    public void RegisterSpawner(ISpawner obj)
    {
        if (IsMouseOverUI()) return;
        SoundsManager.GetInstance()?.PlaySound("CoreSelect");
        registeredSpawnerList.Add(new SpawnerDetail(obj));
    }
    public bool CheckRegisteredSpawner(ISpawner obj)
    {
        foreach(var item in registeredSpawnerList)
        {
            if(item.spawner == obj) return true;
        }
        return false;
    }
    public bool DeregisterSpawner(ISpawner obj) 
    {
        if (IsMouseOverUI()) return false;

        bool deleted = false;
        foreach(var item in registeredSpawnerList.ToList())
        {
            if (item.spawner == obj)
            {
                if(item.arrow != null) item.arrow.gameObject.SetActive(false);
                registeredSpawnerList.Remove(item); deleted = true;
                break;
            }
        }
        if (registeredSpawnerList.Count == 0) target = null;
        return deleted;
    }
    public void ClearRegisteredSpawnersAndTarget()
    {
        foreach (var item in registeredSpawnerList) if (item.arrow != null) item.arrow.gameObject.SetActive(false);
        registeredSpawnerList.Clear(); 
        target = null;
    }
    public int GetRegisteredSpawnersCount() { return registeredSpawnerList.Count; }
    public void RegisterTarget(GameObject target)
    {
        if (IsMouseOverUI()) return;
        this.target = target;
    }
    public void DeregisterTarget()
    {
        if (IsMouseOverUI()) return;
        target = null;
    }
    private bool IsMouseOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = (Vector2)Input.mousePosition;
        var rayCastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, rayCastResults);
        foreach(var item in rayCastResults)
        {
            if (item.gameObject.tag == "BlockUI") return true;
        }
        return false;
    }

    //Match functions
    //public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        if (isMatchStarted)
    //        {
    //            matchStartingTimer.StopTimer();
    //            if (!isMatchEnded)
    //            {
    //                matchEndTimer.StartTimer();
    //                nextMatchTimer.StopTimer();
    //            }
    //            else
    //            {
    //                matchEndTimer.StopTimer();
    //                nextMatchTimer.StartTimer();
    //            }
    //        }
    //        else
    //        {
    //            if (isMatchStartingTimerStarted)
    //            {
    //                matchStartingTimer.StartTimer();
    //            }
    //            else matchStartingTimer.StopTimer();
    //            matchEndTimer.StopTimer();
    //            nextMatchTimer.StopTimer();
    //        }
    //    }
    //    else
    //    {
    //        matchStartingTimer.StopTimer();
    //        matchEndTimer.StopTimer();
    //        nextMatchTimer.StopTimer();
    //    }
    //}
    public GameMode GetGameMode() { return gameMode; }
    public void StartMatchStartingTimer() 
    { 
        matchStartingTimer.StartTimer();
    }
    public void SetStartMatch()
    {
        if (isMatchStarted) return;
        
        if (gameMode == GameMode.Online)
        {
            StartMatch();
            GetComponent<PhotonView>()?.RPC("AnnounceStartMatch", RpcTarget.Others, PhotonNetwork.LocalPlayer.UserId);
        }
        else
        {
            StartMatch();
        }
    }
    public void StartMatch()
    {
        if (isMatchStarted) return;

        isMatchStarted = true;

        selectDinoMenu.SetActive(false);

        if(gameMode == GameMode.Online)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                matchStartingTimer.StopTimer();
                matchEndTimer.StartTimer();
                nextMatchTimer.StopTimer();
                CoresManager.GetInstance()?.LoadCores();
                CoresManager.GetInstance()?.DistributeCores();
            }
            else
            {
                // In case other client has already recevie a match started message but the timers has not synced yet -> force sync
                matchStartingTimer.GetComponent<SyncTimerOnServer>()?.SetIsOwnerTimerRunning(false);
                matchEndTimer.GetComponent<SyncTimerOnServer>()?.SetIsOwnerTimerRunning(true);
                nextMatchTimer.GetComponent<SyncTimerOnServer>()?.SetIsOwnerTimerRunning(false);
            }
        }
        else
        {
            matchEndTimer.StartTimer();
            nextMatchTimer.StopTimer();
            CoresManager.GetInstance()?.DistributeCores();
        }

    }
    public void LoadPlayersInfos() // Used for online
    {
        var players = PhotonNetwork.PlayerList;
        Dictionary<string, string> playersDinos = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties["playersDinos"];
        Dictionary<string, string> playersColors = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties["playersColors"];

        for(int i=0; i<playersName.Count; i++)
        {
            if (i >= players.Length)
            {
                playersName[i].SetText("");
                continue;
            }
            playersName[i].SetText(players[i]?.NickName);
        }

        for(int i=0; i< playersDinoImage.Count; i++)
        {
            if (playersDinos == null) break;
            if (i >= players.Length)
            {
                playersDinoImage[i].SetImage("");
                continue;
            }
            if (playersDinos.ContainsKey(players[i].UserId)) playersDinoImage[i].SetImage(playersDinos[players[i].UserId]);
        }

        for(int i=0; i<playersColor.Count; i++)
        {
            if (playersColors == null) break;
            if (i >= players.Length)
            {
                playersColor[i].GetComponent<ICustomColor>()?.SetColor(UnityEngine.Color.black);
                continue;
            }
            if (playersColors.ContainsKey(players[i].UserId)) playersColor[i].GetComponent<ICustomColor>()?.SetColor(playersColors[players[i].UserId]);
        }
    }
    public bool IsMatchStarted() { return isMatchStarted; }
    public bool IsMatchEnded() { return  isMatchEnded; }
    public void TimerOut()
    {
        if (gameMode == GameMode.Online)
        {
            CheckWinnerOnline();
        }
        else
        {
            CheckWinnerOffline();
        }

    }
    private void CheckWinnerOnline()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (isMatchEnded) return;
        isMatchEnded = true;

        string winner = SelectWinner();
        if (winner != null)
        {
            GetComponent<PhotonView>()?.RPC("AnnounceEndMatch", RpcTarget.All, winner);
        }
        else
        {
            GetComponent<PhotonView>()?.RPC("AnnounceDrawMatch", RpcTarget.All);
        }
    }
    private void CheckWinnerOffline() // Used when timeout
    {
        var cores = CoresManager.GetInstance()?.GetCores();
        if (cores == null) return;
        foreach(var core in cores)
        {
            if (core.gameObject.tag == CoresManager.GetInstance()?.GetNeutralColor()) continue;
            if(core.gameObject.tag != PlayersManager.GetInstance()?.GetPlayerColor())
            {
                AnnouncePlayerFail();
                return;
            }
        }
        AnnouncePlayerWin();
    }
    public void SetEndMatch(string winPlayerColor) // Used for online
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if(isMatchEnded) return;

        isMatchEnded = true;

        GetComponent<PhotonView>()?.RPC("AnnounceEndMatch", RpcTarget.All, winPlayerColor);
    }
    public void StartNextMatch()
    {
        if (gameMode == GameMode.Online)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CoresManager.GetInstance()?.ClearCoresOnline();
                PhotonNetwork.LoadLevel("RoomLobby");
            }
        }
        else SceneManager.LoadSceneAsync("MainMenu");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
    private string SelectWinner()
    {
        Dictionary<string, int> coresCounter = new Dictionary<string, int>();
        string winner = null;

        var cores = CoresManager.GetInstance()?.GetCores();
            
        foreach(var core in cores )
        {
            int maxValue = -1;
            if (core.tag == "Gray") continue;

            if (!coresCounter.ContainsKey(core.tag))
            {
                coresCounter.Add(core.tag, 0);
            }

            coresCounter[core.tag]++;
            if (coresCounter[core.tag] > maxValue)
            {
                maxValue = coresCounter[core.tag]; winner = core.tag;
            }
            else if (coresCounter[core.tag] == maxValue) //There are more than two players that possess the same number of cores
            {
                winner = null;
            }
        }

        return winner;
    }

    // Used for offline
    public void AnnouncePlayerFail()
    {
        if(isMatchEnded) return;
        isMatchEnded = true;

        matchEndTimer.StopTimer();
        nextMatchTimer.StartTimer();

        victoryMenu?.SetActive(false);
        defeatedMenu?.SetActive(true);
    }
    public void AnnouncePlayerWin()
    {
        if (isMatchEnded) return;
        isMatchEnded = true;

        isMatchEnded = true;
        matchEndTimer.StopTimer();
        nextMatchTimer.StartTimer();

        victoryMenu?.SetActive(true);
        defeatedMenu?.SetActive(false);

        PlayerStats stats = PlayerStatsManager.GetInstance()?.LoadPlayerStats();
        Debug.Log(SceneManager.GetActiveScene().name);
        int level = levels.Find(SceneManager.GetActiveScene().name);
        if(stats?.GetCurrentLevel() <= level) stats?.SetCurrentLevel(level + 1);
        PlayerStatsManager.GetInstance()?.SavePlayerStats(stats);
    }
    public void AnnounceGameOver()
    {
        if (isMatchEnded) return;
        isMatchEnded = true;
        matchEndTimer.StopTimer();
        nextMatchTimer.StartTimer();

        drawMenu?.SetActive(true);
    }
    
    // Used for online
    [PunRPC]
    private void AnnounceStartMatch(string userId)
    {
        // Check if this message is sent by master client
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

        //Send to other clients but master client
        GameManager.GetInstance()?.StartMatch();
    }

    [PunRPC]
    private void AnnounceDrawMatch()
    {
        isMatchEnded = true;

        if (PhotonNetwork.IsMasterClient)
        {
            matchEndTimer.StopTimer();
            nextMatchTimer.StartTimer();
        }
        else
        {
            matchEndTimer.GetComponent<SyncTimerOnServer>()?.SetIsOwnerTimerRunning(false);
            nextMatchTimer.GetComponent<SyncTimerOnServer>()?.SetIsOwnerTimerRunning(true);
        }

        drawMenu?.SetActive(true);
        victoryMenu?.SetActive(false);
        defeatedMenu?.SetActive(false);
    }
    [PunRPC]
    private void AnnounceEndMatch(string winPlayerColor)
    {
        isMatchEnded = true;

        if (PhotonNetwork.IsMasterClient)
        {
            matchEndTimer.StopTimer();
            nextMatchTimer.StartTimer();
        }
        else
        {
            matchEndTimer.GetComponent<SyncTimerOnServer>()?.SetIsOwnerTimerRunning(false);
            nextMatchTimer.GetComponent<SyncTimerOnServer>()?.SetIsOwnerTimerRunning(true);
        }

        if (PlayersManager.GetInstance()?.GetPlayerColor() ==  winPlayerColor)
        {
            victoryMenu?.SetActive(true);
            drawMenu?.SetActive(false);
            defeatedMenu?.SetActive(false);
        }
        else
        {
            victoryMenu?.SetActive(false);
            var playersColors = PlayersManager.GetInstance()?.GetPlayersColor();
            var playersDinos = PlayersManager.GetInstance()?.GetPlayersDino();

            if(!playersDinos.ContainsKey(PhotonNetwork.LocalPlayer.UserId) || !playersColors.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
            {
                drawMenu?.SetActive(true);
                defeatedMenu?.SetActive(false);
            }
            else
            {
                drawMenu?.SetActive(false);
                defeatedMenu?.SetActive(true);
            }
        }
    }

    class SpawnerDetail
    {
        public ISpawner spawner;
        public GroundArrow arrow;
        public SpawnerDetail(ISpawner spawner)
        {
            this.spawner = spawner;
            this.arrow = PoolManager.GetInstance()?.SpawnObjectLocal("Arrow").GetComponent<GroundArrow>();
            if (this.arrow != null)
            {
                arrow.gameObject.SetActive(true);
                arrow.SetFromPos(spawner.transform.position);
            }
            else Debug.Log("null arrow");
        }
    }
}

