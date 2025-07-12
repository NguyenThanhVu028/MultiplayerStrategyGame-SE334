using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public void Play(string soundId)
    {
        SoundsManager.GetInstance()?.PlaySound(soundId);
    }
    public void ForcePlay(string soundId)
    {
        SoundsManager.GetInstance()?.ForcePlaySound(soundId);
    }
}
