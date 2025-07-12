using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IHealth))]
public class LimitHealthByTag : MonoBehaviour, ILimit
{
    // Core that has a specific color has a lower health limit

    [SerializeField] string tagToLimit;
    [SerializeField] int healthLimit;
    public void CheckConditionAndGetLimit(ref int value)
    {
        if (this.gameObject.tag == tagToLimit) value = healthLimit;
    }
}
