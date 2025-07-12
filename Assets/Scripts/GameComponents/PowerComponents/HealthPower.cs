using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPower : MonoBehaviour, IPower
{
    [SerializeField] PowersManager.Power power = PowersManager.Power.Health;
    [SerializeField] float regenerateTime = 0.5f;
    [SerializeField] float duration = 10.0f;
    private Coroutine coroutine;
    public void ActivatePower(PowersManager.Power power)
    {
        if (power == this.power) Activate();
    }
    private void Activate()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = (StartCoroutine(PowerCoroutine(duration)));
    }
    IEnumerator PowerCoroutine(float second)
    {
        PlayersManager.GetInstance()?.GetLocalPlayer().SetHealthRegenerateTime(regenerateTime);
        yield return new WaitForSeconds(second);
        PlayersManager.GetInstance()?.GetLocalPlayer().ResetHealthRegenerateTime();
    }
}
