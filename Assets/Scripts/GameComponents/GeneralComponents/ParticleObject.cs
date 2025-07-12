using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour, IParticlePool
{
    [SerializeField] string particleId;
    [SerializeField] GameObject particlePlayerPrefab;

    List<GameObject> particlePlayers = new();

    public string GetParticleId()
    {
        return particleId;
    }

    public GameObject Play()
    {
        foreach(var obj in particlePlayers)
        {
            if(obj.GetComponent<ParticleSystem>()?.isPlaying == false)
            {
                obj.GetComponent<ParticleSystem>()?.Play();
                return obj;
            }
        }
        var newPlayer = Instantiate(particlePlayerPrefab);
        newPlayer.GetComponent<ParticleSystem>()?.Play();
        particlePlayers.Add(newPlayer);
        return newPlayer;
    }
}
