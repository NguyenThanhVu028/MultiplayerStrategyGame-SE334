using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPower : MonoBehaviour, IPower
{
    [SerializeField] PowersManager.Power power;
    [SerializeField] float duration = 10.0f;
    private Coroutine coroutine;
    public void ActivatePower(PowersManager.Power power)
    {
        if (power == this.power) ActivateShield();
    }
    private void ActivateShield()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = (StartCoroutine(ShieldCoroutine(duration)));
    }
    IEnumerator ShieldCoroutine(float second)
    {
        PlayersManager.GetInstance()?.GetLocalPlayer().SetIsShielded(true);
        yield return new WaitForSeconds(second);
        PlayersManager.GetInstance()?.GetLocalPlayer().SetIsShielded(false);
    }
}
