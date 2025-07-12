using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaveLand : MonoBehaviour
{
    // Used to adjust the land that a core owns

    [SerializeField] GameObject land;
    public GameObject GetLand() { return land; }
    public void SetLand(GameObject land) { this.land = land; }
    public void SetLandGlobalPosition(Vector3 position)
    {
        if(GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online) GetComponent<PhotonView>()?.RPC("AnnounceSetLandGlobalPosition", RpcTarget.Others, position);
        if (land != null)
        {
            land.transform.position = position;
        }
    }
    public void SetLandLocalScale(Vector3 scale)
    {
        if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online) GetComponent<PhotonView>()?.RPC("AnnounceSetLandLocalScale", RpcTarget.Others, scale);
        if (land != null)
        {
            land.transform.localScale = scale;
        }
    }
    public void SetLandSprite(string sprite)
    {
        if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Online) GetComponent<PhotonView>()?.RPC("AnnounceSetLandSprite", RpcTarget.Others, sprite);
        if (land != null)
        {
            if (CoresManager.GetInstance()?.GetLandImages() == null) return;
            Sprite newSprite = CoresManager.GetInstance()?.GetLandImages().GetImage(sprite);
            if (land.GetComponent<SpriteRenderer>() != null) land.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
    }
    [PunRPC]
    private void AnnounceSetLandGlobalPosition(Vector3 position)
    {
        if(land != null)
        {
            land.transform.position = position;
        }
    }
    [PunRPC]
    private void AnnounceSetLandLocalScale(Vector3 scale)
    {
        if(land != null)
        {
            land.transform.localScale = scale;
        }
    }
    [PunRPC]
    private void AnnounceSetLandSprite(string sprite)
    {
        if(land != null)
        {
            if (CoresManager.GetInstance()?.GetLandImages() == null) return;
            Sprite newSprite = CoresManager.GetInstance()?.GetLandImages().GetImage(sprite);
            if (land.GetComponent<SpriteRenderer>() != null) land.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
    }
}
