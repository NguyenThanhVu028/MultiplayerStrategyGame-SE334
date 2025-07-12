using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject MenuToClose;
    public void OnActivated()
    {
        MenuToClose?.SetActive(false);
    }
}
