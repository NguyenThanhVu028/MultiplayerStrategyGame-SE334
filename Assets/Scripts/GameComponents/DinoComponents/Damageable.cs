using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.Events;

public class Damageable : MonoBehaviour, IDamageable
{
    [SerializeField] UnityEvent<string, int> onReceiveAttack;

    public void SetAsTarget()
    {
        GameManager.GetInstance()?.RegisterTarget(this.gameObject);
    }
    public void CancelSetAsTarget()
    {
        GameManager.GetInstance()?.DeregisterTarget();
    }
    public void ReceiveAttack(string tag, int damage)
    {
        onReceiveAttack.Invoke(tag, damage);
    }

}
