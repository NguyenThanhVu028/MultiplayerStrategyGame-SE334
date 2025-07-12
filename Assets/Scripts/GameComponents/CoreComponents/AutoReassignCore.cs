using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// When a new master client is selected, the cores on that new master client side will add itself to cores manager
public class AutoReassignCore : MonoBehaviourPunCallbacks
{
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (GetComponent<ISpawner>() != null && PhotonNetwork.IsMasterClient)
        {
            CoresManager.GetInstance()?.AddCore(this.gameObject);
        }
    }
}
