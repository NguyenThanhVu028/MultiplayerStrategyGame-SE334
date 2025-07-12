using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectDisable : MonoBehaviour, IDisable
{
    enum GameMode { Offline, Online };

    [Header("Properties")]
    [SerializeField] private GameMode gameMode = GameMode.Online;
    [SerializeField] private bool autoDisableOnEnable;
    [SerializeField] private bool destroy;
    [SerializeField] float disableTime = 0;
    [Header("Events")]
    public UnityEvent onDisableEvent;
    private Coroutine startDisableCoroutine;
    [Header("Used for online mode")]
    [SerializeField] private Vector3 containerPosition = Vector3.zero;
    void OnEnable()
    {
        if (!autoDisableOnEnable) return;
        if(startDisableCoroutine != null) StopCoroutine(startDisableCoroutine);
        startDisableCoroutine = StartCoroutine(StartDisableCoroutine(disableTime));
    }
    public void SetDisableTime(float seconds) { disableTime = seconds; }
    public void StartDisable()
    {
        if (startDisableCoroutine != null) StopCoroutine(startDisableCoroutine);
        startDisableCoroutine = StartCoroutine(StartDisableCoroutine(disableTime));
    }
    public void Disable()
    {
        onDisableEvent.Invoke();
        if(gameMode == GameMode.Online)
        {
            if (GetComponent<PhotonView>() == null) return;
            if (destroy)
            {
                if(GetComponent<PhotonView>().IsMine) PhotonNetwork.Destroy(gameObject);
                else
                {
                    
                    GetComponent<PhotonView>()?.RPC("AnnounceDisable", GetComponent<PhotonView>().Owner);
                }
            }
            else
            {
                gameObject.SetActive(false);
                GetComponent<PhotonView>()?.RPC("AnnounceDisable", RpcTarget.Others);
            }
        }
        else
        {
            if(destroy) Destroy(gameObject);
            else gameObject.SetActive(false);
        }
            
    }
    [PunRPC]
    private void AnnounceDisable()
    {
        onDisableEvent.Invoke();
        if (destroy)
        {
            if (GetComponent<PhotonView>().IsMine) PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            transform.position = containerPosition;
            gameObject.SetActive(false);
        }
    }
    private IEnumerator StartDisableCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (destroy) Destroy(gameObject);
        else gameObject.SetActive(false);
        onDisableEvent?.Invoke();
    }
}
