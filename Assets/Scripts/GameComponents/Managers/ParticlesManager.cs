using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    // This is use as a pool for particle systems

    static ParticlesManager instance;
    [SerializeField] List<GameObject> particleObjects = new();
    private void Awake()
    {
        instance = this;
    }
    public static ParticlesManager GetInstance() { return instance; }
    public GameObject PlayParticle(string id)
    {
        foreach(var obj in particleObjects)
        {
            if(obj.GetComponent<IParticlePool>()?.GetParticleId() == id)
            {
                return obj.GetComponent<IParticlePool>().Play();
            }
        }
        return null;
    }
}
