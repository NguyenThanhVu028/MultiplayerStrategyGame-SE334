using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckPlayerColor))]
public class SetColorByPlayerColor : MonoBehaviour
{
    // Set color based on player's color, used together with "CheckPlayerColor" component

    [SerializeField] private Color matchPlayerColor;
    [SerializeField] private Color notMatchPlayerColor;
    public void OnMatchPlayerColor()
    {
        if (GetComponent<ICustomColor>() == null) return;
        if (PlayersManager.GetInstance()?.GetPlayerColor() == null) return;
        GetComponent<ICustomColor>().SetColor(matchPlayerColor);
    }
    public void OnNotMatchPlayerColor()
    {
        if (GetComponent<ICustomColor>() == null) return;
        if (PlayersManager.GetInstance()?.GetPlayerColor() == null) return;
        GetComponent<ICustomColor>().SetColor(notMatchPlayerColor);
    }
}
