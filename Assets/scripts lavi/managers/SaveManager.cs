using System.Collections.Generic;
using UnityEngine;
using System;
using static SaveManager;

[System.Serializable]
public class LevelEntry
{
    public int levelIndex;
    public string WorldName;
    public int starsEarned;
}

[System.Serializable]
public class GameSaveData
{
    public List<LevelEntry> levels = new List<LevelEntry>();
    public List<LevelEntry> speciallevels = new List<LevelEntry>();

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

    [System.Serializable]
    public class SpecialCandyEntry
    {
        public string candyId;
        public int amount;
    }



    #region מערכת כוכבים

    public void SaveLevelStars(int levelIndex, int stars)
    {
        string json = PlayerPrefs.GetString("SaveData", "{}");
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json) ?? new GameSaveData();

        // קבלת שם העולם הנוכחי
        string worldName = WorldManager.Instance.getWorld();

        // לבדוק אם העולם שייך לרשימת העולמות המשניים
        bool isSpecial = WorldManager.Instance.secondaryWorlds.Exists(w => w.worldName == worldName);

        List<LevelEntry> targetList = isSpecial ? data.speciallevels : data.levels;
        LevelEntry level = targetList.Find(l => l.levelIndex == levelIndex && l.WorldName == worldName);

        if (level != null)
        {
            level.starsEarned = Mathf.Max(level.starsEarned, stars);
        }
        else
        {
            targetList.Add(new LevelEntry
            {
                levelIndex = levelIndex,
                WorldName = worldName,
                starsEarned = stars
            });
        }

        SaveData(data);
    }


    public int GetStarsForLevel(int levelIndex, string worldName = null)
    {
        GameSaveData data = LoadData();
        if (data == null) return 0;

        // אם לא הועבר שם עולם, נזהה אותו אוטומטית מהסצנה
        if (string.IsNullOrEmpty(worldName))
            worldName = WorldManager.Instance.getWorld();

        bool isSpecial = WorldManager.Instance.secondaryWorlds.Exists(w => w.worldName == worldName);
        List<LevelEntry> targetList = isSpecial ? data.speciallevels : data.levels;

        LevelEntry level = targetList.Find(l => l.levelIndex == levelIndex && l.WorldName == worldName);
        return level != null ? level.starsEarned : 0;
    }


    public int GetTotalStars(bool includeSpecial = true)
    {
        GameSaveData data = LoadData();
        if (data == null) return 0;

        int total = 0;
        foreach (var l in data.levels)
            total += l.starsEarned;

        if (includeSpecial)
        {
            foreach (var l in data.speciallevels)
                total += l.starsEarned;
        }

        return total;
    }

    public int GetSpecialStars()
    {
        GameSaveData data = LoadData();
        if (data == null) return 0;

        int total = 0;
        foreach (var l in data.speciallevels)
            total += l.starsEarned;

        return total;
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
