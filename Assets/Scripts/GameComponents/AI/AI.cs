using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ISpawner))]
public class AI : MonoBehaviour
{
    [Header("Dino properties")]
    [SerializeField] private float dinoSpeed;
    [SerializeField] private float timeBetweenUnit = 0.5f;
    [SerializeField] private float healthRegenerateTime;
    [SerializeField] private float recoverTime;
    [Header("Priority")]
    [SerializeField] AIProrities priorities;

    private bool isOwningLock = false;
    private bool isUnderAttack = false;
    private Coroutine recoverCoroutine = null;
    void Update()
    {
        if (GameManager.GetInstance() != null && GameManager.GetInstance().IsMatchStarted() && !GameManager.GetInstance().IsMatchEnded())
        {
            if (CheckPermission())
            {
                Attack();
            }
        }

        // Return lock if it is not spawning
        if(!GetComponent<ISpawner>().IsSpawning() && isOwningLock) 
        {
            isOwningLock = false;
            AIManager.Instance.ReturnLock(this.gameObject.tag);
        }

        // Update spawner values if the AI owns this spawner
        if(this.gameObject.tag != PlayersManager.GetInstance()?.GetPlayerColor())
        {
            GetComponent<ISpawner>()?.SetSpawnedObjectSpeed(dinoSpeed);
            GetComponent<ISpawner>()?.SetTimeBetweenUnit(timeBetweenUnit);
            GetComponent<IHealth>()?.SetRegenerateTime(healthRegenerateTime);
        }

    }
    public void SetUnderAttack()
    {
        isUnderAttack = true;
        if(recoverCoroutine != null) StopCoroutine(recoverCoroutine);
        recoverCoroutine = StartCoroutine(RecoverCoroutine());
    }
    public bool IsUnderAttack() { return isUnderAttack; }
    private bool CheckPermission()
    {
        // Have perimission if this core's color is not neutral and not player's
        if (this.gameObject.tag != CoresManager.GetInstance()?.GetNeutralColor() && this.gameObject.tag != PlayersManager.GetInstance()?.GetPlayerColor()) return true;
        return false;
    }
    private float CalculateScore(ISpawner target)
    {
        if (target == null) return 0;
        if (target.gameObject.tag == gameObject.tag)
        {
            if(target.gameObject.GetComponent<AI>() != null && target.gameObject.GetComponent<AI>().IsUnderAttack()) // Prioritize land that is under attack
            {
                return -1;
            }
            return 0;
        }

        return priorities.GetScore(AIManager.Instance.GetMode(), GetComponent<ISpawner>(), target);
    }
    private GameObject SelectTarget()
    {
        var cores = CoresManager.GetInstance()?.GetCores();
        if(cores == null) return null;
        float maxScore = 0; GameObject target = null; List<GameObject> coresUnderAttack = new();

        foreach(var core in cores)
        {
            float tempScore = CalculateScore(core.GetComponent<ISpawner>());
            if(tempScore == -1)
            {
                coresUnderAttack.Add(core); // Prioritize core that is under attack
            }
            if (tempScore > maxScore)
            {
                maxScore = tempScore;
                target = core;
            }
        }
        if(coresUnderAttack.Count > 0)
        {
            int rand = Random.Range(0, coresUnderAttack.Count);
            if (rand >= coresUnderAttack.Count) rand = coresUnderAttack.Count - 1;
            if (rand >= 0) return coresUnderAttack[rand];
        }
        return target;
    }
    private void Attack()
    {
        if (AIManager.Instance.AcquireLock(this.gameObject.tag))
        {
            isOwningLock = true;

            GameObject target = SelectTarget();
            if (target != null)
            {
                string dino = PlayersManager.GetInstance()?.GetAIDino(this.gameObject.tag);
                GetComponent<ISpawner>()?.SpawnArmy(dino, target);
            }
            else
            {
                AIManager.Instance.ReturnLockImmediately(gameObject.tag);
            }
        }
    }
    private IEnumerator RecoverCoroutine()
    {
        yield return new WaitForSeconds(recoverTime);
        isUnderAttack = false;
        recoverCoroutine = null;
    }
}
