using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PowersManager;

[CreateAssetMenu(fileName ="PowerInfos", menuName ="PowerInfos")]
public class PowerInfos : ScriptableObject
{
    [SerializeField] List<PowerInfo> powerInfos = new List<PowerInfo>();
    //public List<PowerInfo> GetPowerInfos() {  return powerInfos; }
    public PowerInfo GetPowerInfo(PowersManager.Power power)
    {
        PowerInfo info = null;
        foreach (var powerInfo in powerInfos)
        {
            if (powerInfo.GetPower() == power)
            {
                info = powerInfo; break;
            }
        }
        return info;
    }
    [Serializable]
    public class PowerInfo
    {
        [SerializeField] PowersManager.Power power;
        [SerializeField] Sprite powerImage;
        [SerializeField] float duration;
        private Coroutine coroutine = null;

        public PowersManager.Power GetPower() {  return power; }
        public Sprite GetPowerImage() { return powerImage; }
        public float GetDuration() { return duration; }
        public Coroutine GetCoroutine() { return coroutine; }
        public void SetCoroutine(Coroutine coroutine) { this.coroutine = coroutine; }
    }
}
