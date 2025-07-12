using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject MenuToOpen;
    public void OnActivated()
    {
        MenuToOpen?.SetActive(true);
    }
}
