using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDisable
{
    void SetDisableTime(float second);
    void StartDisable();
    void Disable();
}
