using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    // This is used as a bool for audios in the game

    static SoundsManager instance;
    [SerializeField] List<GameObject> soundObjects = new();
    private void Awake()
    {
        instance = this;
    }
    public static SoundsManager GetInstance() {  return instance; }
    public void PlaySound(string soundId)
    {
        foreach (var soundObject in soundObjects)
        {
            if(soundObject.GetComponent<ISoundPool>()?.GetSoundId() == soundId)
            {
                soundObject.GetComponent<ISoundPool>()?.Play();
                return;
            }
        }
        Debug.Log("Sound not found");
    }
    public void ForcePlaySound(string soundId)
    {
        foreach (var soundObject in soundObjects)
        {
            if (soundObject.GetComponent<ISoundPool>()?.GetSoundId() == soundId)
            {
                soundObject.GetComponent<ISoundPool>()?.ForcePlay();
                return;
            }
        }
    }
}
