using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMarker : MonoBehaviour, IShield
{
    [SerializeField] float shieldOpaque;
    [SerializeField] GameObject shield;

    public void UpdateShieldState()
    {
        // The owner will update the shield's state
        if (PlayersManager.GetInstance() != null && PlayersManager.GetInstance().GetLocalPlayer().IsShielded())
        {
            if (shield.tag != "Shielded")
            {
                shield.tag = "Shielded";
                if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online) shield.GetComponent<SyncTagOnServer>()?.ForceSync();
            }
        }
        else
        {
            if (shield.tag != "UnShielded")
            {
                shield.tag = "UnShielded";
                if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online) shield.GetComponent<SyncTagOnServer>()?.ForceSync();
            }
        }
    }
    public void CheckShieldState()
    {
        // Update visibility, all clients on server will update the shield's visibility based on its tag
        if (shield.gameObject.tag == "Shielded") shield.GetComponent<ICustomColor>().SetOpaque(shieldOpaque);
        else shield.GetComponent<ICustomColor>().SetOpaque(0.0f);
    }
    public void DeactivateShield()
    {
        // If it's in offline mode, AI can deactivate its shield
        if(GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Offline)
        {
            if (shield.tag != "UnShielded")
            {
                shield.tag = "UnShielded";
            }
        }
        // If it's in online mode, only master client can deactivate neutral-color shields
        else if(GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online)
        {
            if (this.gameObject.tag == CoresManager.GetInstance()?.GetNeutralColor() && PhotonNetwork.IsMasterClient)
            {
                if (shield.tag != "UnShielded")
                {
                    shield.tag = "UnShielded";
                    shield.GetComponent<SyncTagOnServer>()?.ForceSync();
                }
            }
        }

    }
}
