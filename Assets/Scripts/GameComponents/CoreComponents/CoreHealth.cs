using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CoreHealth : MonoBehaviour, IHealth
{
    [Header("Properties")]
    [SerializeField] int health;
    [SerializeField] int initialHealth;
    [SerializeField] int defaultMaxHealth = -1;
    [SerializeField] float defaultRegenerateTime = 1;
    [SerializeField] float stopRegenerateTime = 0.5f;
    [SerializeField] bool isActive = true;
    [SerializeField] private bool isRegerating = false;
    [Header("Displayers")]
    [SerializeField] List<TextMeshPro> displayers;
    [Header("Event")]
    [SerializeField] UnityEvent onDeadEvent;
   
    private float regenerateCounter;
    private float stopRegenerateCounter;
    private float regenerateTime = 1;
    private int maxHealth;
    public bool IsActive() { return isActive; }
    public void SetIsActive(bool t) { isActive = t; }
    public void StartRegerating() { isRegerating = true; }
    public void StopRegerating() { isRegerating = false; }
    public bool IsRegerating() {  return isRegerating; }
    public bool IsAtMax()
    {
        if (health >= maxHealth) return true;
        return false;
    }
    public float GetRegenerateTime() {  return regenerateTime; }
    public void SetRegenerateTime(float time) {  regenerateTime = time; }
    public int GetHealth() { return health; }
    public void SetHealth(int value) {  health = value; }
    public void AddHealth(int value) { health += value; }
    public void DecreaseHealth(int value) { health -= value; stopRegenerateCounter = 0; }
    public void SetMaxHealth(int maxHealth) { this.defaultMaxHealth = maxHealth; }
    public int GetMaxHealth() { return maxHealth; }
    private void OnEnable()
    {
        health = initialHealth;
        regenerateCounter = 0;
    }
    private void Update()
    {
        //Update regenerate time
        if (this.gameObject.tag == CoresManager.GetInstance()?.GetNeutralColor()) regenerateTime = defaultRegenerateTime;
        else if(this.gameObject.tag == PlayersManager.GetInstance()?.GetPlayerColor()) regenerateTime = PlayersManager.GetInstance().GetLocalPlayer().GetHealthRegenerateTime();

        if (stopRegenerateCounter < stopRegenerateTime)
        {
            stopRegenerateCounter += Time.deltaTime;
        }
        else
        {
            if (isRegerating && isActive)
            {
                regenerateCounter += Time.deltaTime;
                if (regenerateCounter > regenerateTime)
                {
                    health++; regenerateCounter = 0;
                }
            }
        }

        //Check for limits
        CheckForLimits();
        
        foreach(var item in displayers) if(item != null) item.text = health.ToString();
    }
    private void CheckForLimits()
    {
        var limits = GetComponents<ILimit>();
        if (limits.Length == 0) return;

        maxHealth = defaultMaxHealth;
        foreach (var limit in limits)
        {
            int tempMaxHealth = -1;
            limit.CheckConditionAndGetLimit(ref maxHealth);
            if (tempMaxHealth != -1 && tempMaxHealth < maxHealth) maxHealth = tempMaxHealth;
        }
        if (health > maxHealth) health = maxHealth;
    }
}
