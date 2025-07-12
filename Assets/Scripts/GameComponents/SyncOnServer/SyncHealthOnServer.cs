using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

[RequireComponent(typeof(IHealth))]
[RequireComponent(typeof(PhotonView))]
public class SyncHealthOnServer : MonoBehaviour
{
    // This is used to sync health with other clients

    enum SyncingAuthority { PhotonView, MasterClient, PlayerColor}

    [SerializeField] private SyncingAuthority syncingAuthority = SyncingAuthority.PlayerColor;
    [SerializeField] private bool isSyncing = true;
    [SerializeField] private int syncingRatePerSecond = 60;

    private Coroutine syncingCoroutine;
    private int previouslySyncedHealth;

    void Start()
    {
        previouslySyncedHealth = -1;
        syncingCoroutine = null;
    }

    void Update()
    {
        if (isSyncing)
        {
            if (CheckAuthority()) // If this object has the authority to sync health -> only it can regenerate
            {
                if (syncingCoroutine == null) syncingCoroutine = StartCoroutine(SyncingCoroutine(1.0f / syncingRatePerSecond));
                if (GetComponent<IHealth>().IsActive() && !GetComponent<IHealth>().IsRegerating()) GetComponent<IHealth>().StartRegerating(); 
            }
            else // Any objects that doesnt have the authority to sync health -> stop regenerating
            {
                if (syncingCoroutine != null)
                {
                    StopCoroutine(syncingCoroutine);
                    syncingCoroutine = null;
                }
                if (GetComponent<IHealth>().IsRegerating()) GetComponent<IHealth>().StopRegerating();
                previouslySyncedHealth = -1;
            }
        }
        else
        {
            if (syncingCoroutine != null) { 
                StopCoroutine(syncingCoroutine);
                syncingCoroutine = null;
            }
            previouslySyncedHealth = -1;
        }
    }

    public bool CheckAuthority() // Check if this client has the authority to sync health
    {
        switch (syncingAuthority)
        {
            case SyncingAuthority.MasterClient:
                if (PhotonNetwork.IsMasterClient) return true;
                return false;
            case SyncingAuthority.PhotonView:
                if (GetComponent<PhotonView>().IsMine) return true;
                return false;
            case SyncingAuthority.PlayerColor:
                if (gameObject.tag == CoresManager.GetInstance()?.GetNeutralColor() && PhotonNetwork.IsMasterClient) return true; // The masterClientOwnedColor in this game is "Gray"
                else if (this.gameObject.tag == PlayersManager.GetInstance()?.GetPlayerColor()) return true;
                return false;
            default:
                return false;
        }
    }
    public void StartSyncing() { isSyncing = true; }
    public void StopSyncing() { isSyncing = false; }
    public bool IsSyncing() { return isSyncing; }
    public void ForceSync() // Force other clients to sync health, even when this client doesnt have the permission
    {
        GetComponent<PhotonView>()?.RPC("AnnounceForceSyncHealth", RpcTarget.Others, this.GetComponent<IHealth>().GetHealth());
    }

    //Custom functions
    private IEnumerator SyncingCoroutine(float second)
    {
        while (isSyncing && PhotonNetwork.InRoom && CheckAuthority())
        {
            if(previouslySyncedHealth != this.GetComponent<IHealth>().GetHealth())
            {
                GetComponent<PhotonView>().RPC("AnnounceSyncHealth", RpcTarget.Others, this.GetComponent<IHealth>().GetHealth());
                previouslySyncedHealth = this.GetComponent<IHealth>().GetHealth();
            }

            yield return new WaitForSeconds(second);
        }
    }
    [PunRPC]
    private void AnnounceSyncHealth(int health)
    {
        if (CheckAuthority()) return; // Other clients but the client that has the authority wiil sync health
        this.GetComponent<IHealth>().SetHealth(health);
    }
    [PunRPC]
    private void AnnounceForceSyncHealth(int health)
    {
        this.GetComponent<IHealth>().SetHealth(health);
    }
}
