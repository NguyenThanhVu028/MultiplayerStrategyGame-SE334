using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTagWithOtherObjects : MonoBehaviour
{
    // Sync tag locally with other objects
    [SerializeField] private List<GameObject> objectsToSyncTag = new List<GameObject>();
    void Update()
    {
        foreach (var obj in objectsToSyncTag)
        {
            if (obj?.tag != gameObject.tag) obj.tag = gameObject.tag;
        }
    }
}
