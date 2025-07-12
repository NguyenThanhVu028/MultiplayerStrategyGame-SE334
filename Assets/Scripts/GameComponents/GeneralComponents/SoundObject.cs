using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour, ISoundPool
{
    [SerializeField] string soundId;
    [SerializeField] AudioClip soundClip;
    [SerializeField] GameObject soundPlayer;
    [SerializeField] int maxNumberOfSound = 5;

    List<GameObject> soundPlayers = new List<GameObject>();

    public string GetSoundId() {  return soundId; }
    public void Play()
    {
        int count = 1;
        foreach(var obj in soundPlayers)
        {
            if (obj == null) continue;
            if(obj.GetComponent<AudioSource>() != null)
            {
                if (count >= maxNumberOfSound)
                {
                    return;
                }
                if (!obj.GetComponent<AudioSource>().isPlaying)
                {
                    obj.GetComponent<AudioSource>()?.PlayOneShot(soundClip);
                    return;
                }
                count++;
            }
        }
        var newPlayer = Instantiate(soundPlayer);
        newPlayer.GetComponent<AudioSource>()?.PlayOneShot(soundClip);
        soundPlayers.Add(newPlayer);
    }
    public void ForcePlay()
    {
        if(soundPlayers.Count == 0)
        {
            var newPlayer = Instantiate(soundPlayer);
            newPlayer.GetComponent<AudioSource>()?.PlayOneShot(soundClip);
            soundPlayers.Add(newPlayer);
            return;
        }
        foreach( var obj in soundPlayers)
        {
            if (obj == null) continue;
            obj.GetComponent<AudioSource>()?.Stop();
            obj.GetComponent<AudioSource>()?.PlayOneShot(soundClip);
            return;
        }
    }
}
