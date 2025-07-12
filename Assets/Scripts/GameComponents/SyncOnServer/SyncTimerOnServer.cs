using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(PhotonView))]
public class SyncTimerOnServer : MonoBehaviour, ISyncOnServer
{
    //  This is used to sync timer with other clients

    enum SyncingAuthority { MasterClient, PhotonView}
    enum SyncingMode { Precise, Fast}
    [SerializeField] private SyncingAuthority syncingAuthority = SyncingAuthority.MasterClient;
    [SerializeField] private SyncingMode syncingMode = SyncingMode.Fast;
    [SerializeField] private bool isSyncing = true;
    [SerializeField] private int syncingRatePerSecond = 60;
    [SerializeField] bool isOwner = false;
    [SerializeField] bool isOwnerTimerRunning = false;

    private Coroutine syncingCoroutine;
    private float previouslySyncedPreciseTime = -1;
    private int previouslySyncedMinute= -1;
    private int previouslySyncedSecond = -1;
    private void Start()
    {
        syncingCoroutine = null;
    }
    void Update()
    {
        if(isSyncing)
        {
            if (CheckAuthority())
            {
                if (syncingCoroutine == null) syncingCoroutine = StartCoroutine(SyncingCoroutine(1.0f / syncingRatePerSecond));
                if (!isOwner) // Update owner status
                {
                    if (isOwnerTimerRunning && !GetComponent<Timer>().IsRunning()) GetComponent<Timer>().StartTimer(); // Sync with previous owner
                    isOwner = true;
                }
            }
            else
            {
                isOwner = false;
                if (syncingCoroutine != null)
                {
                    StopCoroutine(syncingCoroutine);
                    syncingCoroutine = null;
                }
                if (GetComponent<Timer>().IsRunning()) GetComponent<Timer>().StopTimer();
                previouslySyncedSecond = previouslySyncedMinute = -1;
                previouslySyncedPreciseTime = -1;
            }
        }
        else
        {
            if (syncingCoroutine != null)
            {
                StopCoroutine(syncingCoroutine);
                syncingCoroutine = null;
            }
            previouslySyncedSecond = previouslySyncedMinute = -1;
            previouslySyncedPreciseTime = -1;
        }
    }

    public bool CheckAuthority()
    {
        switch (syncingAuthority)
        {
            case SyncingAuthority.MasterClient:
                if (PhotonNetwork.IsMasterClient) return true;
                return false;
            case SyncingAuthority.PhotonView:
                if (GetComponent<PhotonView>() != null && GetComponent<PhotonView>().IsMine) return true;
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
        if (syncingMode == SyncingMode.Fast) ForceSyncFastTime();
        else if (syncingMode == SyncingMode.Precise) ForceSyncPreciseTime();
    }

    //Custom functions
    public bool IsOwnerTimerRunning() { return isOwnerTimerRunning; }
    public void SetIsOwnerTimerRunning(bool t) {  isOwnerTimerRunning = t; }
    public void ForceSyncPreciseTime()
    {
        GetComponent<PhotonView>().RPC("AnnounceForceSyncPreciseTime", RpcTarget.Others, GetComponent<Timer>().GetTime());
    }
    public void ForceSyncFastTime()
    {
        int min, sec;
        GetComponent<Timer>().GetTime(out min, out sec);
        GetComponent<PhotonView>().RPC("AnnounceForceSyncFastTime", RpcTarget.Others, min, sec);
    }
    private IEnumerator SyncingCoroutine(float second)
    {
        if (!CheckAuthority()) yield return null;

        while (isSyncing && PhotonNetwork.InRoom)
        {
            if (isOwner) // Only start syncing after synced with previous owner
            {
                if (!GetComponent<Timer>().IsRunning()) // If timer is not running -> deactivate other cilents' timers so that if authority change, other clients' timer will also stop
                {
                    switch (syncingMode) // Check if it hasn't synced the latest value
                    {
                        case SyncingMode.Precise:
                            if(previouslySyncedPreciseTime != GetComponent<Timer>().GetTime())
                            {
                                GetComponent<PhotonView>().RPC("AnnounceCurrentPreciseTime", RpcTarget.Others, GetComponent<Timer>().GetTime());
                                previouslySyncedPreciseTime = GetComponent<Timer>().GetTime();

                                isOwnerTimerRunning = false;
                                GetComponent<PhotonView>().RPC("AnnounceIsOwnerTimerRunning", RpcTarget.Others, false); // Announce other clients that the owner's timer has stopped running
                            }
                            break;
                        case SyncingMode.Fast:
                            int sec, min;
                            GetComponent<Timer>().GetTime(out min, out sec);
                            if(previouslySyncedMinute != min || previouslySyncedSecond != sec)
                            {
                                GetComponent<PhotonView>().RPC("AnnounceCurrentFastTime", RpcTarget.Others, min, sec);
                                previouslySyncedSecond = sec; previouslySyncedMinute = min;

                                isOwnerTimerRunning = false;
                                GetComponent<PhotonView>().RPC("AnnounceIsOwnerTimerRunning", RpcTarget.Others, false); // Announce other clients that the owner's timer has stopped running
                            }
                            break;
                    }
                }
                else
                {
                    if (!isOwnerTimerRunning)
                    {
                        isOwnerTimerRunning = true;
                        GetComponent<PhotonView>().RPC("AnnounceIsOwnerTimerRunning", RpcTarget.Others, true);
                    }
                    switch (syncingMode)
                    {
                        case SyncingMode.Precise:
                            if (previouslySyncedPreciseTime != GetComponent<Timer>().GetTime())
                            {
                                GetComponent<PhotonView>().RPC("AnnounceCurrentPreciseTime", RpcTarget.Others, GetComponent<Timer>().GetTime());
                                previouslySyncedPreciseTime = GetComponent<Timer>().GetTime();
                            }
                            break;
                        case SyncingMode.Fast:
                            int sec, min;
                            GetComponent<Timer>().GetTime(out min, out sec);
                            if (previouslySyncedMinute != min || previouslySyncedSecond != sec)
                            {
                                GetComponent<PhotonView>().RPC("AnnounceCurrentFastTime", RpcTarget.Others, min, sec);
                                previouslySyncedSecond = sec; previouslySyncedMinute = min;
                            }
                            break;
                    }
                }
            }
            yield return new WaitForSeconds(second);
        }
    }
    [PunRPC]
    private void AnnounceIsOwnerTimerRunning(bool t)
    {
        isOwnerTimerRunning = t;
    }
    [PunRPC]
    private void AnnounceCurrentPreciseTime(float time)
    {
        if (CheckAuthority()) return;
        GetComponent<Timer>().SetTimer(time);
    }
    [PunRPC]
    private void AnnounceCurrentFastTime(int min, int sec)
    {
        if(CheckAuthority()) return;
        GetComponent<Timer>().SetTimer(min, sec);
    }
    [PunRPC]
    private void AnnounceForceSyncPreciseTime(float time)
    {
        GetComponent<Timer>().SetTimer(time);
    }
    [PunRPC]
    private void AnnounceForceSyncFastTime(int min, int sec)
    {
        GetComponent<Timer>().SetTimer(min, sec);
    }
}
