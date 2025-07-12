using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class SyncTagOnServer : MonoBehaviour
{
    // This is used to sync tag with other clients
    
    enum SyncingAuthority { PhotonView, MasterClient}
    [SerializeField] private SyncingAuthority syncingAuthority = SyncingAuthority.MasterClient;
    [SerializeField] private bool isSyncing = true;
    [SerializeField] private int syncingRatePerSecond = 60;
    [SerializeField] string masterClientOwnedColor;

    private Coroutine syncingCoroutine;
    private string previouslySyncedTag;
    void Start()
    {
        previouslySyncedTag = "";
        syncingCoroutine = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSyncing)
        {
            if (CheckAuthority())
            {
                if (syncingCoroutine == null) syncingCoroutine = StartCoroutine(SyncingCoroutine(1.0f / syncingRatePerSecond));
            }
            else
            {
                if (syncingCoroutine != null) StopCoroutine(syncingCoroutine);
                previouslySyncedTag = "";
            }
        }
        else
        {
            if (syncingCoroutine != null) StopCoroutine(syncingCoroutine);
            previouslySyncedTag = "";
        }
    }

    //Implement interface
    public bool CheckAuthority() // Check if this client has the permission to sync
    {
        switch (syncingAuthority)
        {
            case SyncingAuthority.MasterClient:
                if (PhotonNetwork.IsMasterClient) return true;
                return false;
            case SyncingAuthority.PhotonView:
                if (GetComponent<PhotonView>().IsMine) return true;
                return false;
            default:
                return false;
        }
    }
    public void StartSyncing() { isSyncing = true; }
    public void StopSyncing() { isSyncing = false; }
    public bool IsSyncing() { return isSyncing; }
    public void ForceSync()
    {
        GetComponent<PhotonView>().RPC("AnnounceForceSyncTag", RpcTarget.Others, this.gameObject.tag);
    }

    //Custom functions
    private IEnumerator SyncingCoroutine(float second)
    {
        while (isSyncing && PhotonNetwork.InRoom && CheckAuthority())
        {
            if (previouslySyncedTag != this.gameObject.tag)
            {
                GetComponent<PhotonView>().RPC("AnnounceSyncTag", RpcTarget.Others, this.gameObject.tag);
                previouslySyncedTag = this.gameObject.tag;
            }

            yield return new WaitForSeconds(second);
        }
    }
    [PunRPC]
    private void AnnounceSyncTag(string tag)
    {
        if (CheckAuthority()) return; // Other clients but the client that has the authority wiil sync tag
        this.gameObject.tag = tag;
    }
    [PunRPC]
    private void AnnounceForceSyncTag(string tag)
    {
        this.gameObject.tag = tag;
    }
}
