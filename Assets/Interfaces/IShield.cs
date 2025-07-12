using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShield
{
    void UpdateShieldState();
    void CheckShieldState();
    void DeactivateShield();
}
