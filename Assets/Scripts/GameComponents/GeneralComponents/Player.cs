using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] private string playerColor = "";
    [SerializeField] private string playerDino = "";

    [SerializeField] private float oriDinoSpeed = 1.0f;
    [SerializeField] private float oriTimeBetweenUnit = 0.5f;
    [SerializeField] private float oriHealthRegenerateTime = 1.0f;
    [SerializeField] private bool isShielded = false;

    [SerializeField] private float dinoSpeed;
    [SerializeField] private float timeBetweenUnit;
    [SerializeField] private float healthRegenerateTime;

    private void OnEnable()
    {
        dinoSpeed = oriDinoSpeed;
        timeBetweenUnit = oriTimeBetweenUnit;
        healthRegenerateTime = oriHealthRegenerateTime;
    }

    public void SetPlayerColor(string color) {  playerColor = color; }
    public string GetPlayerColor() { return playerColor; }
    public void SetPlayerDino(string dino) { playerDino = dino; }
    public string GetPlayerDino() { return playerDino; }
    public float GetDinoSpeed() {  return dinoSpeed; }
    public float GetHealthRegenerateTime() {  return healthRegenerateTime; }
    public void SetHealthRegenerateTime(float value) { healthRegenerateTime = value; }
    public void ResetHealthRegenerateTime() { healthRegenerateTime = oriHealthRegenerateTime; }
    public bool IsShielded() { return isShielded; }
    public void SetIsShielded(bool t) { isShielded = t; }
    public float GetTimeBetweenUnit() { return timeBetweenUnit; }
    public void SetTimeBetweenUnit(float value) { timeBetweenUnit = value; }
    public void ResetTimeBetweenUnit() { timeBetweenUnit = oriTimeBetweenUnit; }
}
