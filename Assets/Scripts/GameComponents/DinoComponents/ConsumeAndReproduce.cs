using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConsumeAndReproduce: MonoBehaviour, IReproduce
{
    // Dino will consume specified objects to reproduce

    [SerializeField] bool isActive;
    [SerializeField] float spawnDistance;
    [SerializeField] List<ObjectDetail> consumableObjectDetails = new();
    private float accumulatedStack;
    private void OnEnable()
    {
        accumulatedStack = 1;
        isActive = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        foreach(ObjectDetail detail in consumableObjectDetails)
        {
            if(other.tag == detail.GetObjectTag())
            {
                float rand = UnityEngine.Random.Range(0f, 100f);
                if (rand <= detail.GetReproduceRate())
                    Reproduce();
                break;
            }
        }
    }
    public void Reproduce()
    {
        GameObject item = null;
        if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online && GetComponent<PhotonView>().IsMine) item = PoolManager.GetInstance()?.SpawnObjectOnline(PlayersManager.GetInstance()?.GetPlayerDino());
        else if(GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Offline) item = PoolManager.GetInstance()?.SpawnObjectLocal(PlayersManager.GetInstance()?.GetPlayerDino());
        if (item != null)
        {
            Debug.Log(PlayersManager.GetInstance()?.GetPlayerDino());
            item.transform.position = this.transform.position - this.transform.forward.normalized * spawnDistance * accumulatedStack;
            accumulatedStack++;
            if (item.GetComponent<IAttack>() != null)
            {
                item.GetComponent<IAttack>().enabled = true;
                item.GetComponent<IAttack>().SetTarget(GetComponent<Attack>().GetTarget());
                item.GetComponent<IAttack>().SetDamage(this.transform.GetComponent<IAttack>().GetDamage());
            }
            if (item.GetComponent<ConsumeAndReproduce>() != null) item.GetComponent<ConsumeAndReproduce>().isActive = false;
            if (item.GetComponent<IMovable>() != null) item.GetComponent<IMovable>().enabled = true;
            if (item.GetComponent<SphereCollider>() != null) item.GetComponent<SphereCollider>().enabled = true;
            item.transform.rotation = this.transform.rotation;
            item.tag = this.tag;
            if(GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online) item.GetComponent<SyncTagOnServer>()?.ForceSync();
        }
    }
    [Serializable]
    class ObjectDetail
    {
        [SerializeField] string objectTag;
        [SerializeField] float reproduceRate;

        public string GetObjectTag() { return objectTag; }
        public float GetReproduceRate() { return reproduceRate; }
    }
}
