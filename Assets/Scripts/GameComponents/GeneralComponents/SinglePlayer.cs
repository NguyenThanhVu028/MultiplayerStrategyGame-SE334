using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class SinglePlayer : MonoBehaviour
{
    [SerializeField] private NameList levels;
    [SerializeField] GameObject notification;
    
    public void LoadLevel(int level)
    {
        if(levels == null)
        {
            Debug.LogError("Levels details not found!");
            return;
        }
        string scene = levels.GetAt(level);
        if(scene.IsNullOrEmpty())
        {
            Debug.LogError("Can't load the specified scene!");
        }

        PlayerStats stats = PlayerStatsManager.GetInstance()?.LoadPlayerStats();
        if(level > stats?.GetCurrentLevel())
        {
            notification?.SetActive(true);
            notification?.GetComponent<MultiText>()?.SetText(0, "CAN'T ENTER THIS LEVEL!");
            notification?.GetComponent<MultiText>()?.SetText(1, "You must finish all lower levels to enter this level!");
            return;
        }

        SceneManager.LoadSceneAsync(scene);
    }
}
