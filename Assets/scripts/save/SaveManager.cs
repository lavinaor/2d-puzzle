using System.Collections.Generic;
using UnityEngine;

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
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

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
}
