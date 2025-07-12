using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPool
{
    string GetID();
    GameObject SpawnLocal();
    GameObject SpawnOnline();
}
