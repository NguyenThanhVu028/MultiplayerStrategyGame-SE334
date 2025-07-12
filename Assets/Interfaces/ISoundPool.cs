using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundPool
{
    string GetSoundId();
    void Play();
    void ForcePlay();
}
