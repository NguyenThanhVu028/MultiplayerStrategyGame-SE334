using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // This is used as a pool for objects in a match

    static PoolManager instance;
    [SerializeField] List<GameObject> Pools = new List<GameObject>();
    private void Awake()
    {
        instance = this;
    }
    public static PoolManager GetInstance() { return instance; }
    public GameObject SpawnObjectLocal(string ID)
    {
        foreach(var item in Pools)
        {
            if(item.GetComponent<IPool>()?.GetID() == ID) return item.GetComponent<IPool>()?.SpawnLocal();
        }
        return null;
    }
    public GameObject SpawnObjectOnline(string ID)
    {
        foreach (var item in Pools)
        {
            if (item.GetComponent<IPool>()?.GetID() == ID) return item.GetComponent<IPool>()?.SpawnOnline();
        }
        return null;
    }
}
