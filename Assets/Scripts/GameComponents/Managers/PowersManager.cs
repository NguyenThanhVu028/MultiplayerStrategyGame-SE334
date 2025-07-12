using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PowersManager : MonoBehaviour
{
    // This is used to manage players' powers

    static PowersManager instance;
    public enum Power { Shield, Spawning, Health}
    [Header("Properrties")]
    [SerializeField] int maxPowerCount;
    [SerializeField] GameObject powerBar;
    [Header("Configs")]
    [SerializeField] Images powerImagesConfig;
    [SerializeField] List<GameObject> powersList = new();
    //[SerializeField] UnityEvent<Power> onActivatePower;
    void Awake()
    {
        instance = this;
    }
    public static PowersManager GetInstance() {  return instance; }
    public void GiveRandomPower()
    {
        int condition = UnityEngine.Random.Range(0, 100);
        if (condition % 2 != 0) return;

        int count = 0;
        for(int i=0; i< powerBar.transform.childCount; i++)
        {
            if (powerBar.transform.GetChild(i).gameObject.activeInHierarchy) count++;
        }
        if (count >= maxPowerCount) return;

        var powers = Enum.GetValues(typeof(Power));
        int rand = UnityEngine.Random.Range(0, 100);
        rand %= powers.Length;

        var powerButton = PoolManager.GetInstance()?.SpawnObjectLocal("PowerButton");
        if (powerButton == null) return;
        powerButton.GetComponent<Button>()?.onClick.RemoveAllListeners();
        powerButton.GetComponent<Button>()?.onClick.AddListener(delegate { ActivatePower(rand); });
        if(powerButton.GetComponent<IDisable>() != null)
        {
            powerButton.GetComponent<Button>()?.onClick.AddListener(powerButton.GetComponent<IDisable>().Disable);
        }
        powerButton.GetComponent<MultiImage>()?.SetImage(powerImagesConfig.GetImage(((Power)rand).ToString()));
        powerButton.transform.SetParent(powerBar.transform);
        powerButton.transform.localScale = Vector3.one;
    }
    public void ActivatePower(int power)
    {
        //Find all powers on this object
        foreach (var p in powersList) p.GetComponent<IPower>()?.ActivatePower((Power)power);
    }
}
