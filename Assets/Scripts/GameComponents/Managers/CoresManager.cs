using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CoresManager : MonoBehaviourPunCallbacks
{
    // This is used to manage all the cores in the game

    static CoresManager instance;
    [Header("Configs")]
    [SerializeField] private List<CoreDetails> coreDetailsLayouts = new List<CoreDetails>(); // There can be more than one layout for cores, each layout has a list of coreDetail which contain information about its position, land image
    [SerializeField] private Images landImages;
    [SerializeField] private string neutralColor = "Gray";
    [SerializeField] private List<GameObject> cores = new();

    [Header("Used for offline")]
    [SerializeField] private List<string> allColors = new();
    [SerializeField] private List<AICoreDetail> aiCores = new();
    [SerializeField] private GameObject playerCore;
    void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (cores.Count == 0) return;
        //If the match has started but has yet ended
        if (GameManager.GetInstance() != null && GameManager.GetInstance().IsMatchStarted() && !GameManager.GetInstance().IsMatchEnded())
        {
            if (GameManager.GetInstance().GetGameMode() == GameManager.GameMode.Online && PhotonNetwork.IsMasterClient)
            {
                CheckWinnerOnline();
            }
            else if (GameManager.GetInstance().GetGameMode() == GameManager.GameMode.Offline)
            {
                CheckWinnerOffline();
            }
        }
    }
    public static CoresManager GetInstance() {  return instance; }
    public Images GetLandImages() { return landImages; }
    public string GetNeutralColor() { return  neutralColor; }
    public void LoadCores()
    {
        // Spawn cores based on specified details

        int coreLayoutIndex = UnityEngine.Random.Range(0, coreDetailsLayouts.Count);
        if(coreLayoutIndex >= coreDetailsLayouts.Count) coreLayoutIndex = coreDetailsLayouts.Count - 1;

        if (coreDetailsLayouts[coreLayoutIndex] == null) return;
        foreach (var coreDetails in coreDetailsLayouts[coreLayoutIndex].GetDetails())
        {
            Vector3 pos = coreDetails.GetPosition();
            string color = coreDetails.GetColor();
            string onlinePrefabName = coreDetails.GetOnlinePrefabName();
            GameObject offlinePrefab = coreDetails.GetOfflinePrefab();

            GameObject core = null;
            if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online) core = PhotonNetwork.Instantiate(onlinePrefabName, pos, Quaternion.Euler(90, 0, 0));
            else core = Instantiate(offlinePrefab, pos, Quaternion.Euler(90, 0, 0));
            
            if (core != null)
            {
                cores.Add(core);

                Vector3 landPos = coreDetails.GetLandGlobalPosition();
                Vector3 landScale = coreDetails.GetLandGlobalScale();
                string landSpriteName = coreDetails.GetLandSprite();

                core.GetComponent<HaveLand>()?.SetLandGlobalPosition(landPos);
                core.GetComponent<HaveLand>()?.SetLandLocalScale(new Vector3(landScale.x / core.transform.lossyScale.x, landScale.y / core.transform.lossyScale.y, landScale.z / core.transform.lossyScale.z));
                core.GetComponent<HaveLand>()?.SetLandSprite(landSpriteName);
                core.gameObject.tag = color;
            }
        }
    }
    public void DistributeCores()
    {
        if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online)
        {
            DistributeCoresOnline();
        }
        else
        {
            DistributeCoresOffline();
        }
    }
    private void DistributeCoresOnline()
    {
        var players = PhotonNetwork.PlayerList;
        var playersColors = PlayersManager.GetInstance()?.GetPlayersColor();
        var playersDinos = PlayersManager.GetInstance()?.GetPlayersDino();

        var cloneCores = new List<GameObject>();
        foreach (var item in cores) cloneCores.Add(item.gameObject);
        if (cloneCores.Count == 0) return;

        foreach (var player in players)
        {
            if (playersColors.ContainsKey(player.UserId) && playersDinos.ContainsKey(player.UserId))
            {
                int randomIndex = UnityEngine.Random.Range(0, cloneCores.Count);
                randomIndex = (randomIndex >= cloneCores.Count) ? cloneCores.Count - 1 : randomIndex;
                cloneCores[randomIndex].gameObject.tag = playersColors[player.UserId];
                cloneCores.RemoveAt(randomIndex);
            }
        }
        foreach (var core in cores)
        {
            core.GetComponent<SyncTagOnServer>().ForceSync(); // Sync tag after assigning cores
        }
    }
    private void DistributeCoresOffline()
    {
        if(cores.Count == 0 || aiCores.Count == 0 || playerCore == null)
        {
            GameManager.GetInstance()?.AnnounceGameOver();
            return;
        }

        string playerColor = PlayersManager.GetInstance()?.GetPlayerColor();
        playerCore.gameObject.tag = playerColor;

        List<string> colors = new List<string>(allColors);
        foreach (var item in colors)
        {
            if(item == playerColor)
            {
                colors.Remove(item);
                break;
            }
        }

        if(colors.Count == 0)
        {
            GameManager.GetInstance()?.AnnounceGameOver();
            return;
        }

        foreach(var aiCore in aiCores)
        {
            int colorIndex = UnityEngine.Random.Range(0, colors.Count);
            aiCore.GetCoreObject().gameObject.tag = colors[colorIndex];
            PlayersManager.GetInstance()?.SetAIDino(colors[colorIndex], aiCore.GetCoreDino());
            colors.RemoveAt(colorIndex);

            if (colors.Count == 0)
            {
                return;
            }
        }
        
    }
    public List<GameObject> GetCores() { return cores; }
    public void ClearCores() 
    {
        foreach (var core in cores) Destroy(core);
        cores.Clear(); 
    }
    public void ClearCoresOnline()
    {
        foreach(var core in cores) PhotonNetwork.Destroy(core);
        cores.Clear();
    }
    public void AddCore(GameObject core) { cores.Add(core); }

    private void CheckWinnerOnline()
    {
        string winTag = null;
        foreach (var core in cores)
        {
            if (core.tag == "Gray") continue;
            if (winTag == null) winTag = core.tag;
            else
            {
                if (winTag != core.tag) return; // If there are two cores that have different colors beside "Gray"
            }
        }
        if (winTag != null)
        {
            var dinos = FindObjectsOfType<MonoBehaviour>().OfType<IMovable>();
            foreach (var dino in dinos)
            {
                if (dino.gameObject.tag != winTag) return;
            }
            GameManager.GetInstance()?.SetEndMatch(winTag);
        }
    }
    private void CheckWinnerOffline()
    {
        bool playerWon = true;
        bool playerFailed = true;

        foreach(var core in cores)
        {
            if(core.gameObject.tag == neutralColor) continue;
            if (core.gameObject.tag != PlayersManager.GetInstance()?.GetPlayerColor()) playerWon = false; // If there are still other players -> not won
            if (core.gameObject.tag == PlayersManager.GetInstance()?.GetPlayerColor()) playerFailed = false; // If player still owns a core -> not failed
        }
        if (!playerWon && !playerFailed) return; // If player neither win or fail
        var dinos = FindObjectsOfType<MonoBehaviour>().OfType<IMovable>();
        foreach(var dino in dinos)
        {
            if (dino.gameObject.tag != PlayersManager.GetInstance()?.GetPlayerColor()) playerWon = false; // If there are still other players' dinos -> not won
            if (dino.gameObject.tag == PlayersManager.GetInstance()?.GetPlayerColor()) playerFailed = false; // If players still have some dinos left -> not fail
        }
        if (playerWon) GameManager.GetInstance()?.AnnouncePlayerWin();
        else if(playerFailed) GameManager.GetInstance()?.AnnouncePlayerFail();
    }

    [Serializable]
    class AICoreDetail
    {
        [SerializeField] private GameObject core;
        [SerializeField] private string dino;

        public GameObject GetCoreObject() {  return core; }
        public string GetCoreDino() {  return dino; }
    }
}
