using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject MenuToTrigger;
    public void OnActivated()
    {
        MenuToTrigger?.SetActive((MenuToTrigger.activeInHierarchy)? false : true);
    }
}
