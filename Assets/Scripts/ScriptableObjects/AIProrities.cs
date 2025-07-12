using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "AIProrities", menuName ="AIPriorities")]
public class AIProrities : ScriptableObject
{
    [SerializeField] private float isNeutral;
    [SerializeField] private float minDifference;
    [SerializeField] private float maxDistance;
    [SerializeField] private float proximity;
    [SerializeField] private float isSpawning;
    [SerializeField] private float isWeaker;

    public float GetScore(AIManager.AIMode mode, ISpawner me, ISpawner target)
    {
        float score = 0;

        float isNeutralScore = 0;
        float proximityScore = 0;
        float isSpawningScore = 0;
        float isPossibleToAttackScore = 0;
        float isWeakerScore = 0;

        if(mode == AIManager.AIMode.Hard) if (target.IsSpawning()) isSpawningScore = GetIsSpawningScore();

        isNeutralScore = GetIsNeutralScore(target.gameObject.tag);

        float distance = Vector3.Distance(me.transform.position, target.transform.position);
        proximityScore = GetProximityScore(distance);

        if(mode == AIManager.AIMode.Hard) isWeakerScore = GetIsWeakerScore(me, target);

        if (mode == AIManager.AIMode.Easy) isPossibleToAttackScore = GetIsPossibleToAttackScoreEasy(me, target);
        else if (mode == AIManager.AIMode.Hard) isPossibleToAttackScore = GetIsPossibleToAttackScoreHard(me, target);
        if (isPossibleToAttackScore == 0) return 0;
        else score = isNeutralScore + proximityScore + isSpawningScore + isPossibleToAttackScore + isWeakerScore;

        return score;
    }
    public float GetIsNeutralScore(string tag) 
    {
        if (tag == CoresManager.GetInstance()?.GetNeutralColor())
        {
            return isNeutral;
        }
        else return 0; }
    public float GetProximityScore(float distance)
    {
        if (distance >= maxDistance) return 0;
        return (1.0f*(maxDistance - distance) / maxDistance) * 1.0f * proximity;
    }
    public float GetIsSpawningScore() {  return isSpawning; }
    public float GetIsPossibleToAttackScoreEasy(ISpawner me, ISpawner target)
    {
        if(me == null || target == null) return 0;
        if (me.gameObject.GetComponent<IHealth>().GetHealth() >= target.gameObject.GetComponent<IHealth>().GetHealth() + minDifference) return 1;
        else return 0;
    }
    public float GetIsWeakerScore(ISpawner me, ISpawner target)
    {
        int diff = me.gameObject.GetComponent<IHealth>().GetHealth() - target.gameObject.GetComponent<IHealth>().GetHealth();
        if(diff <= 0 ) return 0;
        if(diff >= isWeaker) return isWeaker;
        return diff;
    }
    public float GetIsPossibleToAttackScoreHard(ISpawner me, ISpawner target)
    {
        if (me == null || target == null) return 0;
        //me
        int myHealth = me.gameObject.GetComponent<IHealth>().GetHealth();
        bool isMyHealthMax = me.gameObject.GetComponent<IHealth>().IsAtMax();
        float myDinoSpeed = me.GetSpawnedObjectSpeed();

        //target
        int targetHealth = target.gameObject.GetComponent<IHealth>().GetHealth();
        bool isTargetHealthMax = target.gameObject.GetComponent<IHealth>().IsAtMax();
        float targetRegenerateTime = target.gameObject.GetComponent<IHealth>().GetRegenerateTime();

        float distance = Vector3.Distance(me.transform.position, target.transform.position);

        if (myHealth < targetHealth) return 0;
        if(myHealth == targetHealth)
            if (isTargetHealthMax && isMyHealthMax) return 1;

        int estimatedTargetHealthRegenerated = (int)((distance / myDinoSpeed) / targetRegenerateTime);
        if (myHealth <= estimatedTargetHealthRegenerated + targetHealth) return 0;
        else return 1;
    }
}
