using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour, IMovable
{
    [SerializeField] float Acceleration;
    [SerializeField] float Speed;

    public void Move()
    {
        Vector3 movingDirection = this.transform.forward;
        this.GetComponent<Rigidbody>().velocity = movingDirection.normalized * Speed;
    }
    public void SetSpeed(float speed) { Speed = speed; }
    void Update()
    {
        Move();
    }
}
