using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawner
{
    Transform transform { get; }
    GameObject gameObject { get; }
    void SpawnArmy(GameObject target);
    void SpawnArmy(string objectId, GameObject target);
    void StopSpawningArmy();
    bool IsSpawning();
    void SetSpawnedObjectSpeed(float speed);
    float GetSpawnedObjectSpeed();
    void SetTimeBetweenUnit(float timeBetweenUnit);
}
