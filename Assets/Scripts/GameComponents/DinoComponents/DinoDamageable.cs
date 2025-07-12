using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoDamageable : MonoBehaviour, IDamageable
{
    public void ReceiveAttack(string tag, int damage)
    {
        if (tag == this.gameObject.tag) return;
        GetComponent<IDisable>()?.Disable();
    }
}
