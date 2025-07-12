using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

//Can only attack objects that contain the "Damageable" component
[RequireComponent(typeof(Rigidbody))]
public class Attack : MonoBehaviour, IAttack
{
    [Tooltip("Used when attacking cores")]
    [SerializeField] int damage;
    [Tooltip("Used when attacking other dinos")]
    [SerializeField] float killRate = 40;
    [Tooltip("Used when attacking other dinos")]
    [SerializeField] float deathRate;
    [SerializeField] float attackZone;

    [SerializeField] UnityEvent onAttackEvent;
    [SerializeField] List<Dinosaur.DinoName> dinosToAttack;

    private GameObject Target = null;

    private void Update()
    {
        if(Target != null)
        {
            if(Vector3.Distance(this.transform.position, Target.transform.position) < attackZone) this.transform.LookAt(Target.transform.position);
            else
            {
                Vector3 distance = Target.transform.position - this.transform.position;
                // Adjust dinos direction if they are moving away from the target
                if (Vector3.Angle(distance, GetComponent<Rigidbody>().velocity) >= 90) this.transform.LookAt(Target.transform.position);
            }
        }
            
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Target != null)
        {
            if(other.gameObject == Target) // Attack if this core is the target
            {
                AttackTarget(Target);
                onAttackEvent?.Invoke();
                return;
            }
        }
        if (other.GetComponent<Dinosaur>() != null)
        {
            if(Random.Range(0.0f, 100.0f) <= killRate)
            {
                foreach (var dino in dinosToAttack)
                {
                    if (other.GetComponent<Dinosaur>().GetName() == dino) // Attack specified dinos
                    {
                        AttackTarget(other.gameObject);

                        if (Random.Range(0.0f, 100.0f) <= deathRate) GetComponent<IDisable>().Disable();

                        return;
                    }
                }
            }
        }
    }
    public void SetTarget(GameObject obj) { this.Target = obj; }
    public GameObject GetTarget() { return Target; }
    public void SetDamage(int damage) { this.damage = damage; }
    public int GetDamage() { return damage; }
    public void AttackTarget(GameObject target)
    {
        if (target == null) return;
        target.GetComponent<IDamageable>()?.ReceiveAttack(this.tag, damage);
    }
}
