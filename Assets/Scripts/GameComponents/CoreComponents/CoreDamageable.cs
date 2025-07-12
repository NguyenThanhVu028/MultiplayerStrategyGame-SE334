using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IHealth))]
public class CoreDamageable: MonoBehaviour, IDamageable
{
    public void ReceiveAttack(string tag, int damage) // The core on the client that spawned dino side will process this function
    {
        if (GameManager.GetInstance()?.GetGameMode() == GameManager.GameMode.Offline)
        {
            ProcessAttackReceiveOffline(tag, damage);
        }
        else
        {
            if (GetComponent<SyncHealthOnServer>() != null)
            {
                if (GetComponent<SyncHealthOnServer>().CheckAuthority()) // Only the player that have the authority to sync this core's health can proceed
                {
                    ProcessAttackReceivedOnline(tag, damage);
                }
                else
                {
                    GetComponent<PhotonView>()?.RPC("AnnounceReceiveAttack", RpcTarget.Others, tag, damage);
                }
            }
        }
    }
    private void ProcessAttackReceiveOffline(string tag, int damage)
    {
        if (this.gameObject.tag == tag)
        {
            GetComponent<IHealth>()?.AddHealth(damage);
            var particle = ParticlesManager.GetInstance()?.PlayParticle("HealthPlus");
            if (particle != null) particle.transform.position = transform.position;
        }
        else
        {
            if (PlayersManager.GetInstance() != null && PlayersManager.GetInstance().GetLocalPlayer().IsShielded() && this.gameObject.tag == PlayersManager.GetInstance().GetPlayerColor()) return;
            GetComponent<IHealth>()?.DecreaseHealth(damage);

            GetComponent<AI>()?.SetUnderAttack(); // Announce the AI that this core is under attack

            var particle = ParticlesManager.GetInstance()?.PlayParticle("HealthDecrease");
            if (particle != null) particle.transform.position = transform.position;

            if (GetComponent<IHealth>()?.GetHealth() < 0)
            {
                GetComponent<IHealth>()?.SetHealth(-(GetComponent<IHealth>().GetHealth()));
                GetComponent<ISpawner>()?.StopSpawningArmy();
                this.gameObject.tag = tag;
                if(tag == PlayersManager.GetInstance()?.GetPlayerColor())
                {
                    GameObject powerBox = PoolManager.GetInstance()?.SpawnObjectLocal("PowerBox");
                    if (powerBox != null)
                    {
                        powerBox.transform.position = this.transform.position + new Vector3(0, 0.5f, 0);
                        Vector3 upVector = Vector3.up * Random.Range(2.0f, 3.0f);
                        Vector3 directionVector = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * Vector3.right * Random.Range(2.0f, 3.0f);
                        if (powerBox.GetComponent<Rigidbody>() != null) powerBox.GetComponent<Rigidbody>().velocity = upVector + directionVector;
                    }
                }
            }
        }
    }
    private void ProcessAttackReceivedOnline(string tag, int damage) //This function is used in online mode
    {
        if (this.gameObject.tag == tag)
        {
            GetComponent<IHealth>()?.AddHealth(damage);
            var particle = ParticlesManager.GetInstance()?.PlayParticle("HealthPlus");
            if (particle != null) particle.transform.position = transform.position;
        }
        else
        {
            if (this.gameObject.tag != "Gray" && PlayersManager.GetInstance() != null && PlayersManager.GetInstance().GetLocalPlayer().IsShielded()) return;
            GetComponent<IHealth>()?.DecreaseHealth(damage);

            var particle = ParticlesManager.GetInstance()?.PlayParticle("HealthDecrease");
            if(particle != null) particle.transform.position = transform.position;

            if (GetComponent<IHealth>()?.GetHealth() < 0)
            {
                GetComponent<IHealth>()?.SetHealth(-(GetComponent<IHealth>().GetHealth()));
                this.gameObject.tag = tag;
                GetComponent<PhotonView>()?.RPC("IsTakenOver", RpcTarget.All, tag);
                GetComponent<SyncHealthOnServer>()?.ForceSync();
                GetComponent<SyncTagOnServer>().ForceSync();
            }
        }
    }
    [PunRPC]
    private void AnnounceReceiveAttack(string tag, int damage)
    {
        if (GetComponent<SyncHealthOnServer>() != null)
        {
            if (GetComponent<SyncHealthOnServer>().CheckAuthority())
            {
                ProcessAttackReceivedOnline(tag, damage);
            }
        }
    }
    [PunRPC]
    private void IsTakenOver(string color)
    {
        if(PlayersManager.GetInstance()?.GetPlayerColor() == color) // Only the player that owns this core can proceed
        {
            GameObject powerBox = PoolManager.GetInstance()?.SpawnObjectLocal("PowerBox");
            if(powerBox != null)
            {
                powerBox.transform.position = this.transform.position + new Vector3(0, 0.5f, 0);
                Vector3 upVector = Vector3.up * Random.Range(2.0f, 3.0f);
                Vector3 directionVector = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * Vector3.right * Random.Range(2.0f, 3.0f);
                if(powerBox.GetComponent<Rigidbody>() != null) powerBox.GetComponent<Rigidbody>().velocity = upVector + directionVector;
            }
        }
    }
    public void SetAsTarget()
    {
        GameManager.GetInstance()?.RegisterTarget(this.gameObject);
    }
    public void CancelSetAsTarget()
    {
        GameManager.GetInstance()?.DeregisterTarget();
    }
}
