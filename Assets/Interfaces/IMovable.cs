using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    bool enabled { get; set; }
    GameObject gameObject { get; }
    void Move();
    void SetSpeed(float speed);
}
