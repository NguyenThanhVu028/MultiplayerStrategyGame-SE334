using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapNames", menuName = "Configs/MapNames")]
public class MapNames : ScriptableObject
{
    [SerializeField] private List<MapNameDetail> mapNames;
    public string GetSceneName(string mapName)
    {
        foreach (var item in mapNames) if (item?.GetMapName() == mapName) return item?.GetSceneName();
        return "";
    }
    [Serializable]
    public class MapNameDetail
    {
        [SerializeField] private string mapName;
        [SerializeField] private string sceneName;

        public string GetMapName() { return mapName; }
        public string GetSceneName() { return sceneName; }
    }
}
