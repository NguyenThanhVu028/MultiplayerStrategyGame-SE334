using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    bool IsActive();
    void SetIsActive(bool t);
    void StartRegerating();
    void StopRegerating();
    bool IsRegerating();
    bool IsAtMax();
    void SetRegenerateTime(float time);
    float GetRegenerateTime();
    int GetHealth();
    void SetHealth(int value);
    void AddHealth(int value);
    void DecreaseHealth(int value);
    void SetMaxHealth(int maxHealth);
    int GetMaxHealth();
}
