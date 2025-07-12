using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPlayerColor : MonoBehaviour
{
    // Used to call events if this core has the same or different color compared to player
    [SerializeField] private UnityEvent onMatchPlayerColor;
    [SerializeField] private UnityEvent onNotMatchPlayerColor;
    void Update()
    {
        if (this.gameObject.tag == PlayersManager.GetInstance()?.GetPlayerColor()) onMatchPlayerColor.Invoke();
        else onNotMatchPlayerColor.Invoke();
    }
}
