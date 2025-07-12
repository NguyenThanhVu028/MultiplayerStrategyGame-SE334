using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void ReceiveAttack(string tag, int damage);
}
