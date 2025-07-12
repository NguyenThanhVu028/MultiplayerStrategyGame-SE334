using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DinoInfos", menuName = "Configs/DinoInfos")]
public class DinoInfos : ScriptableObject
{
    [SerializeField] List<DinoInfo> infos = new List<DinoInfo>();
    public bool CheckExist(string dinoName)
    {
        foreach (var info in infos) if (info.GetDinoName() == dinoName) return true;
        return false;
    }
    public string GetInfo(string dinoName)
    {
        foreach(var item in infos)
        {
            if (item?.GetDinoName() == dinoName) return item.GetDinoInfo();
        }
        return null;
    }
    public string GetImageName(string dinoName)
    {
        foreach (var item in infos)
        {
            if (item?.GetDinoName() == dinoName) return item.GetImageName();
        }
        return null;
    }
    public string GetPrefabName(string dinoName)
    {
        foreach (var item in infos)
        {
            if (item?.GetDinoName() == dinoName) return item.GetPrefabName();
        }
        return null;
    }
    [Serializable]
    public class DinoInfo
    {
        [SerializeField] private string dinoName;
        [SerializeField] private string prefabName;
        [SerializeField] private string imageName;
        [TextArea]
        [SerializeField] private string dinoInfo;

        public string GetDinoName() {  return dinoName; }
        public string GetPrefabName() {  return prefabName; }
        public string GetImageName() { return imageName; }
        public string GetDinoInfo() { return dinoInfo; }
    }
}
