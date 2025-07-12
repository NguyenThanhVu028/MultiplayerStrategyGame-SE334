using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{
    bool enabled { get; set; }
    void SetTarget(GameObject obj);
    GameObject GetTarget();
    void SetDamage(int damage);
    int GetDamage();
    void AttackTarget(GameObject target);
}
