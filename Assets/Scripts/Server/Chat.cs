using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using WebSocketSharp;

public class Chat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messages;
    [SerializeField] private TMP_InputField inputText;
    [SerializeField] private GameObject chatArea;
    [SerializeField] private GameObject newChatIcon;
    private void Start()
    {
        messages.text = "";
    }
    private void Update()
    {
        if (chatArea.activeInHierarchy) newChatIcon.SetActive(false);
        if(Input.GetKeyDown(KeyCode.Return) && !inputText.text.IsNullOrEmpty() && chatArea.activeInHierarchy)
        {
            if (GetComponent<PhotonView>())
            {
                string mess = PhotonNetwork.NickName + ": " + inputText.text;
                inputText.text = "";
                GetComponent<PhotonView>().RPC("SendMess", RpcTarget.All, mess);
            }
        }
    }
    [PunRPC]
    public void SendMess(string mess)
    {
        messages.text += mess + "\n";
        if (!chatArea.activeInHierarchy) newChatIcon.SetActive(true);
    }
}
