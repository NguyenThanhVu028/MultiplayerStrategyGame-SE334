using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPower : MonoBehaviour, IPower
{
    [SerializeField] PowersManager.Power power = PowersManager.Power.Spawning;
    [SerializeField] float timeBetweenUnit = 0.2f;
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
        PlayersManager.GetInstance()?.GetLocalPlayer().SetTimeBetweenUnit(timeBetweenUnit);
        yield return new WaitForSeconds(second);
        PlayersManager.GetInstance()?.GetLocalPlayer().ResetTimeBetweenUnit();
    }
}
