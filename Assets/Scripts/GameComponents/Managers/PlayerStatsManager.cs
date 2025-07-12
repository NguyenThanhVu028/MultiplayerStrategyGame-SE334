using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    // This is used to save some player's information

    static PlayerStatsManager instance;

    [Tooltip("Name of save file")]
    [SerializeField] private string FileName = "Stats";
    private void Awake()
    {
        instance = this;
    }
    public static PlayerStatsManager GetInstance() { return instance; }
    public PlayerStats LoadPlayerStats()
    {
        string FullPath = Path.Combine(Application.persistentDataPath, FileName);
        if (!File.Exists(FullPath)) return new PlayerStats();

        try
        {
            string dataString;
            using (FileStream stream = new FileStream(FullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataString = reader.ReadToEnd();
                }
            }
            PlayerStats stats = JsonUtility.FromJson<PlayerStats>(dataString);
            return stats;
        }
        catch(Exception e){
            Debug.LogError(e);
            return new PlayerStats();
        }
    }
    public void SavePlayerStats(PlayerStats playerStats)
    {
        if(playerStats == null)
        {
            Debug.LogError("The data you provided has been corupted!");
            return;
        }
        try
        {
            string FullPath = Path.Combine(Application.persistentDataPath, FileName);
            Directory.CreateDirectory(Path.GetDirectoryName(FullPath));

            string dataString = JsonUtility.ToJson(playerStats, true);
            Debug.Log(dataString + " " + playerStats.GetCurrentLevel());

            using (FileStream stream = new FileStream(FullPath, FileMode.Create))
            {
                using (StreamWriter Writer = new StreamWriter(stream))
                {
                    Writer.Write(dataString);
                }
            }
        }
        catch (Exception e) { Debug.LogError(e); }
    }
}

[Serializable]
public class PlayerStats
{
    public int currentLevel;
    public string latestLogin;
    public PlayerStats()
    {
        currentLevel = 0;
        latestLogin = null;
    }
    public void SetCurrentLevel(int level) { currentLevel = level; }
    public int GetCurrentLevel() { return currentLevel; }
    public string GetLatestLogin() { return latestLogin; }
    public void SetLatestLogin(string login) { latestLogin = login; }
}
