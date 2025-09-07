using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class LevelEntry
{
    public int levelIndex;
    public int starsEarned;
}

[System.Serializable]
public class GameSaveData
{
    public List<LevelEntry> levels = new List<LevelEntry>();

    public int lastLevelEnterd = 0; // השלב האחרון שהשחקן שיחק
    public int totalCoins = 0; // אוצרות
    public int totalGems = 0;  // יהלומים
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public static event Action OnCoinsChanged;
    public static event Action OnGemsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ChanglastLevelEnterd(int level)
    {
        GameSaveData data = LoadData();
        data.lastLevelEnterd = level;
        SaveData(data);
    }
    public int GetlastLevelEnterd()
    {
        string json = PlayerPrefs.GetString("SaveData", "{}");
        if (string.IsNullOrEmpty(json)) return 0;

        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        if (data == null) return 0;

        return data.lastLevelEnterd;
    }

    #region מערכת כוכבים

    public void SaveLevelStars(int levelIndex, int stars)
    {
        string json = PlayerPrefs.GetString("SaveData", "{}");
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json) ?? new GameSaveData();

        LevelEntry level = data.levels.Find(l => l.levelIndex == levelIndex);
        if (level != null)
        {
            level.starsEarned = Mathf.Max(level.starsEarned, stars);
        }
        else
        {
            data.levels.Add(new LevelEntry { levelIndex = levelIndex, starsEarned = stars });
        }

        string newJson = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SaveData", newJson);
        PlayerPrefs.Save();
    }

    public int GetStarsForLevel(int levelIndex)
    {
        string json = PlayerPrefs.GetString("SaveData", "{}");
        if (string.IsNullOrEmpty(json)) return 0;

        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        if (data == null || data.levels == null) return 0;

        LevelEntry level = data.levels.Find(l => l.levelIndex == levelIndex);
        return level != null ? level.starsEarned : 0;
    }

    public int GetStarsInTotal()
    {
        string json = PlayerPrefs.GetString("SaveData", "{}");
        if (string.IsNullOrEmpty(json)) return 0;

        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        if (data == null || data.levels == null) return 0;

        int totalStars = 0;
        foreach (LevelEntry level in data.levels)
        {
            totalStars += level.starsEarned;
        }

        return totalStars;
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("SaveData");
        PlayerPrefs.Save();
    }


    #endregion

    #region מערכת מטבעות

    private GameSaveData LoadData()
    {
        string json = PlayerPrefs.GetString("SaveData", "{}");
        return JsonUtility.FromJson<GameSaveData>(json) ?? new GameSaveData();
    }

    private void SaveData(GameSaveData data)
    {
        string newJson = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SaveData", newJson);
        PlayerPrefs.Save();
    }

    public void AddCoins(int amount)
    {
        GameSaveData data = LoadData();
        data.totalCoins += amount;
        SaveData(data);

        OnCoinsChanged?.Invoke();  // קריאה לאירוע של שינוי מטבעות
    }

    public void AddGems(int amount)
    {
        GameSaveData data = LoadData();
        data.totalGems += amount;
        SaveData(data);

        OnGemsChanged?.Invoke();  // קריאה לאירוע של שינוי אבנים
    }

    public int GetCoins()
    {
        return LoadData().totalCoins;
    }

    public int GetGems()
    {
        return LoadData().totalGems;
    }
    #endregion
}
