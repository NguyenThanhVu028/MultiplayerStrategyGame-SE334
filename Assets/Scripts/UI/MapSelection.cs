using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapSelection : MonoBehaviour
{
    [SerializeField] private string mapName;
    [SerializeField] private TextMeshProUGUI mapNameDisplayer;
    //[SerializeField] private TextMeshProUGUI votedMapNameDisplayer;

    private void Start()
    {
        mapNameDisplayer.text = mapName;
    }

    //public void OnSelectMap()
    //{
    //    votedMapNameDisplayer.text = mapName;
    //}
}
