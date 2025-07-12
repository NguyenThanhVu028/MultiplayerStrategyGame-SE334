using System.Collections;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using UnityEngine;

public class Username : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI UsernameText;
    void Start()
    {
        if(UsernameText != null) UsernameText.text = PhotonNetwork.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
