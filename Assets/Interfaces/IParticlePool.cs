using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParticlePool
{
    string GetParticleId();
    GameObject Play();
}
