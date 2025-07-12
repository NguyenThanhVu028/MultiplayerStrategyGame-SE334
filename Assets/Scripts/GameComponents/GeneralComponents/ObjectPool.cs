using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour, IPool
{
    [Tooltip("This ID will be used to instantiate online")]
    [SerializeField] string ID;
    [SerializeField] GameObject ObjectToPool;
    List<GameObject> LocalObjects = new List<GameObject>();
    List<GameObject> OnlineObjects = new List<GameObject>();
    
    public string GetID() => ID;
    public GameObject SpawnLocal()
    {
        foreach (var obj in LocalObjects)
        {
            if (obj == null) continue;
            if (obj.gameObject.activeInHierarchy == false)
            {
                obj?.SetActive(true);
                return obj;
            }
        }
        if (ObjectToPool != null)
        {
            GameObject newObj = Instantiate(ObjectToPool);
            if (newObj != null) LocalObjects.Add(newObj); 
            return newObj;
        }
        else return null;
    }
    public GameObject SpawnOnline()
    {
        foreach (var obj in OnlineObjects)
        {
            if (obj == null) continue;
            if (obj.gameObject.activeInHierarchy == false)
            {
                obj.SetActive(true);
                GetComponent<PhotonView>()?.RPC("SetActiveObject", RpcTarget.OthersBuffered, obj.GetComponent<PhotonView>()?.ViewID);
                return obj;
            }
        }
        GameObject newObj = PhotonNetwork.Instantiate(ID, Vector3.zero, Quaternion.identity);
        if(newObj != null) OnlineObjects.Add(newObj); 
        return newObj;
    }
    [PunRPC]
    private void SetActiveObject(int viewId)
    {
        PhotonView obj = PhotonView.Find(viewId);
        obj?.gameObject.SetActive(true);
    }
}
