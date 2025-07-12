using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(IHealth))]
public class DinoSpawner : MonoBehaviour, ISpawner
{
    [Header("Properties")]
    [SerializeField] private int damage;
    [SerializeField] int unitPerSpawn;
    [SerializeField] float spawnDistance = 0.5f;

    private float dinoSpeed = 1.0f;
    float timeBetweenUnit;

    private Coroutine spawnArmyCoroutine;
    private void Update()
    {
        // Reset properties if this core is owned by player
        if (this.gameObject.tag == PlayersManager.GetInstance()?.GetPlayerColor()) dinoSpeed = PlayersManager.GetInstance().GetLocalPlayer().GetDinoSpeed();
        if (this.gameObject.tag == PlayersManager.GetInstance()?.GetPlayerColor()) timeBetweenUnit = PlayersManager.GetInstance().GetLocalPlayer().GetTimeBetweenUnit();
    }
    public bool IsSpawning() { /*return isSpawning;*/ return (spawnArmyCoroutine != null); }
    public void SetSpawnedObjectSpeed(float speed) { dinoSpeed = speed; }
    public float GetSpawnedObjectSpeed() { return dinoSpeed; }
    public void SetTimeBetweenUnit(float time) { timeBetweenUnit = time; }
    public void ForceRegisterSpawner()
    {
        // Used when player click on a core
        if (PlayersManager.GetInstance()?.GetPlayerColor() == null) return;
        if (this.tag != PlayersManager.GetInstance()?.GetPlayerColor()) return;
        GameManager.GetInstance()?.ClearRegisteredSpawnersAndTarget();
        GameManager.GetInstance()?.RegisterSpawner(this);
    }
    public void TryRegisterSpawner()
    {
        if (PlayersManager.GetInstance()?.GetPlayerColor() == null) return;
        if (this.tag != PlayersManager.GetInstance()?.GetPlayerColor()) return;
        if (GameManager.GetInstance() != null && !GameManager.GetInstance().DeregisterSpawner(this))
        {
            if(GameManager.GetInstance().GetRegisteredSpawnersCount() > 0) GameManager.GetInstance().RegisterSpawner(this);
        }
    }
    public void SpawnArmy(GameObject target)
    {
        // Used to spawn player's dino
        if(target == gameObject) return;
        if (spawnArmyCoroutine != null) return;
        if (PlayersManager.GetInstance()?.GetPlayerColor() == null) return;
        SoundsManager.GetInstance()?.PlaySound("CoreAttack");
        if (this.tag != PlayersManager.GetInstance()?.GetPlayerColor()) return;
        spawnArmyCoroutine = StartCoroutine(SpawnArmyCoroutine(target));
    }
    public void SpawnArmy(string objectId, GameObject target)
    {
        //Used to spawn AI's dino
        if (target == gameObject) return;
        if (spawnArmyCoroutine != null) return;
        SoundsManager.GetInstance()?.PlaySound("CoreAttack");
        spawnArmyCoroutine = StartCoroutine(SpawnArmyCoroutine(objectId, target));

    }
    public void StopSpawningArmy()
    {
        //isSpawning = false;
        if (spawnArmyCoroutine != null) StopCoroutine(spawnArmyCoroutine);
        spawnArmyCoroutine = null;
    }
    
    IEnumerator SpawnArmyCoroutine(GameObject target) //Used for player
    {
        if (PlayersManager.GetInstance()?.GetPlayerDino() == null) yield return null;
        Vector3 FromPos = this.transform.position;
        Vector3 ToPos = target.transform.position;
        Vector3 Distance = ToPos - FromPos;
        float TargetAngle = Mathf.Atan2(-Distance.z, Distance.x) * Mathf.Rad2Deg + 90;

        if (gameObject.GetComponent<IHealth>() != null)
        {
            IHealth thisHealth = gameObject.GetComponent<IHealth>();
            if (thisHealth.GetHealth() <= 0)
            {
                spawnArmyCoroutine = null;
                yield return null;
            }
            
            float StartAngle = TargetAngle - 90;
            float CurrentAngle = StartAngle;
            int SpawnCounter = 0;
            int SpawningCount = thisHealth.GetHealth();

            while (SpawningCount > 0 && thisHealth.GetHealth() > 0)
            {
                if (SpawnCounter >= unitPerSpawn)
                {
                    SpawnCounter = 0; CurrentAngle = StartAngle;
                    yield return new WaitForSeconds(timeBetweenUnit);
                }
                GameObject item = null;
                if(GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online) item = PoolManager.GetInstance()?.SpawnObjectOnline(PlayersManager.GetInstance()?.GetPlayerDino());
                else item = PoolManager.GetInstance()?.SpawnObjectLocal(PlayersManager.GetInstance()?.GetPlayerDino());
                if (item != null)
                {
                    item.transform.position = FromPos + Quaternion.Euler(0, CurrentAngle, 0) * Vector3.forward * spawnDistance;
                    if (item.GetComponent<IAttack>() != null)
                    {
                        item.GetComponent<IAttack>().enabled = true;
                        item.GetComponent<IAttack>().SetTarget(target);
                        item.GetComponent<IAttack>().SetDamage(damage);
                    }
                    if (item.GetComponent<IMovable>() != null)
                    {
                        item.GetComponent<IMovable>().enabled = true;
                        item.GetComponent<IMovable>().SetSpeed(dinoSpeed);
                    }
                    if (item.GetComponent<SphereCollider>() != null) item.GetComponent<SphereCollider>().enabled = true;
                    item.transform.rotation = Quaternion.Euler(0, TargetAngle, 0);
                    item.tag = this.tag;
                    if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online) item.GetComponent<SyncTagOnServer>()?.ForceSync();
                }
                CurrentAngle += 180.0f / (unitPerSpawn - 1);
                thisHealth.DecreaseHealth(1);
                SpawningCount--;
                SpawnCounter++;
            }
            spawnArmyCoroutine = null;
        }
    }
    IEnumerator SpawnArmyCoroutine(string objectId, GameObject target) // Used for AI
    {
        if (PlayersManager.GetInstance()?.GetPlayerDino() == null) yield return null;
        Vector3 FromPos = this.transform.position;
        Vector3 ToPos = target.transform.position;
        Vector3 Distance = ToPos - FromPos;
        float TargetAngle = Mathf.Atan2(-Distance.z, Distance.x) * Mathf.Rad2Deg + 90;

        if (gameObject.GetComponent<IHealth>() != null)
        {
            IHealth thisHealth = gameObject.GetComponent<IHealth>();
            if (thisHealth.GetHealth() <= 0)
            {
                spawnArmyCoroutine = null;
                yield return null;
            }
            float StartAngle = TargetAngle - 90;
            float CurrentAngle = StartAngle;
            int SpawnCounter = 0;
            int SpawningCount = thisHealth.GetHealth();

            while (SpawningCount > 0 && thisHealth.GetHealth() > 0)
            {
                if (SpawnCounter >= unitPerSpawn)
                {
                    SpawnCounter = 0; CurrentAngle = StartAngle;
                    yield return new WaitForSeconds(timeBetweenUnit);
                }
                GameObject item = null;
                if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online) item = PoolManager.GetInstance()?.SpawnObjectOnline(objectId);
                else item = PoolManager.GetInstance()?.SpawnObjectLocal(objectId);
                if (item != null)
                {
                    item.transform.position = FromPos + Quaternion.Euler(0, CurrentAngle, 0) * Vector3.forward * spawnDistance;
                    if (item.GetComponent<IAttack>() != null)
                    {
                        item.GetComponent<IAttack>().enabled = true;
                        item.GetComponent<IAttack>().SetTarget(target);
                        item.GetComponent<IAttack>().SetDamage(damage);
                    }
                    if (item.GetComponent<IMovable>() != null)
                    {
                        item.GetComponent<IMovable>().enabled = true;
                        item.GetComponent<IMovable>().SetSpeed(dinoSpeed);
                    }
                    if (item.GetComponent<SphereCollider>() != null) item.GetComponent<SphereCollider>().enabled = true;
                    item.transform.rotation = Quaternion.Euler(0, TargetAngle, 0);
                    item.tag = this.tag;
                    item.GetComponent<SyncTagOnServer>()?.ForceSync();
                }
                CurrentAngle += 180.0f / (unitPerSpawn - 1);
                thisHealth.DecreaseHealth(1);
                SpawningCount--;
                SpawnCounter++;
            }
            spawnArmyCoroutine = null;
        }
    }
}
