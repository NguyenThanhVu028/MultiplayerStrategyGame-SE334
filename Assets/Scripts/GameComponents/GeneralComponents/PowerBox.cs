using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBox : MonoBehaviour
{
    public void OnActivate()
    {
        PowersManager.GetInstance()?.GiveRandomPower();
    }
}
