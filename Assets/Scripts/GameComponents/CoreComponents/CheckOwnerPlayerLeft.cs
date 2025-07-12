using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheckOwnerPlayerLeft : MonoBehaviourPunCallbacks
{
    // Used by a core, that core will check if the player that owns it leave the game, then it will proceed to destroy itself or set its color to default color
    private enum Action { Destory, ChangeTagToDefaultTag }
    [SerializeField] private string defautTag = "Gray";
    [SerializeField] private Action action;
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        switch (action)
        {
            case Action.Destory:
                if (GetComponent<PhotonView>() != null && GetComponent<PhotonView>().IsMine)
                {
                    Dictionary<string, string> tempList = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties["playersColors"];
                    if (tempList != null)
                    {
                        if (!tempList.ContainsKey(otherPlayer.UserId))
                        {
                            return;
                        }
                        if (this.gameObject.tag == tempList[otherPlayer.UserId])
                        {
                            PhotonNetwork.Destroy(this.gameObject);
                        }
                    }
                }
                break;
            case Action.ChangeTagToDefaultTag:
                if (PhotonNetwork.IsMasterClient)
                {
                    Dictionary<string, string> tempList = (Dictionary<string, string>)PhotonNetwork.CurrentRoom.CustomProperties["playersColors"];
                    if (tempList != null)
                    {
                        if (!tempList.ContainsKey(otherPlayer.UserId))
                        {
                            return;
                        }
                        if (this.gameObject.tag == tempList[otherPlayer.UserId]) // If is the owner of this core
                        {
                            this.gameObject.tag = defautTag;
                            GetComponent<SyncTagOnServer>()?.ForceSync(); // Sync with other clients
                        }
                    }
                }
                break;
        }
    }
}
