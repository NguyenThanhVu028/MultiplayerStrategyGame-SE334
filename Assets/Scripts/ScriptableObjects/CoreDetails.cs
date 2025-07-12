using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoreDetails", menuName = "Configs/CoreDetails")]
public class CoreDetails : ScriptableObject
{
    [SerializeField] private List<CoreDetail> coreDetails = new List<CoreDetail>();
    public List<CoreDetail> GetDetails() { return new List<CoreDetail>(coreDetails); }
    [Serializable]
    public class CoreDetail
    {
        [Header("Core detail")]
        [SerializeField] private Vector3 position;
        [SerializeField] private string color;
        [SerializeField] private GameObject offlinePrefab;
        [SerializeField] private string onlinePrefabName;
        [Header("Land detail")]
        [SerializeField] private Vector3 landGlobalPosition;
        [SerializeField] private Vector3 landGlobalScale;
        [SerializeField] private string landSpriteName;

        public Vector3 GetPosition() { return position; }
        public void SetPosition(Vector3 position) { this.position = position; }
        public string GetColor() { return color; }
        public void SetColor(string color) { this.color = color; }
        public GameObject GetOfflinePrefab() { return  offlinePrefab; }
        public string GetOnlinePrefabName() { return onlinePrefabName; }

        public Vector3 GetLandGlobalPosition() { return landGlobalPosition; }
        public void SetLandGlobalPosition(Vector3 position) {  landGlobalPosition = position; }
        public Vector3 GetLandGlobalScale() { return landGlobalScale; }
        public void SetLandGlobalScale(Vector3 scale) { landGlobalScale = scale; }
        public string GetLandSprite() { return landSpriteName; }
        public void SetLandSprite(string sprite) { landSpriteName = sprite; }
    }
}
