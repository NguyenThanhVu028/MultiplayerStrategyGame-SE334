using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DieInObstacles : MonoBehaviour
{
    // Dino will die if get in contact with specified objects

    [SerializeField] List<ObstacleDetail> obstacleDetails = new List<ObstacleDetail>();
    private void OnTriggerEnter(Collider other)
    {
        foreach(var detail in obstacleDetails)
        {
            if (other.tag == detail.GetObstacleTag())
            {
                float rand = UnityEngine.Random.Range(0f, 100f);
                if (rand <= detail.GetDeathRate())
                {
                    GetComponent<IDisable>().Disable();
                }
                break;
            }
        }
    }
    [Serializable]
    private class ObstacleDetail
    {
        [SerializeField] string obstacleTag;
        [SerializeField] float deathRate;

        public string GetObstacleTag() { return obstacleTag; }
        public float GetDeathRate() { return deathRate; }
    }



}
