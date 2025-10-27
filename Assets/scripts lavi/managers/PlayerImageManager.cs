using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerFrame
{
    public string frameName;
    public Sprite frameSprite;
    public int price;
}

[System.Serializable]
public class PlayerPortrait
{
    public string portraitName;
    public Sprite portraitSprite;
    public int price;
}

[System.Serializable]
public class PlayerImageSaveData
{
    public int selectedFrameIndex = 0;
    public int selectedPortraitIndex = 0;
    public List<int> unlockedFrames = new List<int>();
    public List<int> unlockedPortraits = new List<int>();
}

public static class PlayerImageEvents
{
    public static Action OnImageChanged;
}

public class PlayerImageManager : MonoBehaviour
{
    public static PlayerImageManager Instance;

    [Header("Lists")]
    public List<PlayerFrame> allFrames = new List<PlayerFrame>();
    public List<PlayerPortrait> allPortraits = new List<PlayerPortrait>();

    [Header("Current Save Data")]
    public PlayerImageSaveData saveData = new PlayerImageSaveData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadData();
            EnsureDefaultUnlocked(); //  נוספה השורה הזאת
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //  פונקציה חדשה — שומרת על ברירת המחדל
    private void EnsureDefaultUnlocked()
    {
        // ודא שהפריטים הראשונים קיימים ונעולים כברירת מחדל
        if (allFrames.Count > 0 && !saveData.unlockedFrames.Contains(0))
            saveData.unlockedFrames.Add(0);

        if (allPortraits.Count > 0 && !saveData.unlockedPortraits.Contains(0))
            saveData.unlockedPortraits.Add(0);

        // אם אין בחירה תקפה, קבע את הראשונה
        if (saveData.selectedFrameIndex < 0 || saveData.selectedFrameIndex >= allFrames.Count)
            saveData.selectedFrameIndex = 0;

        if (saveData.selectedPortraitIndex < 0 || saveData.selectedPortraitIndex >= allPortraits.Count)
            saveData.selectedPortraitIndex = 0;

        SaveData();
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("PlayerImageData", json);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("PlayerImageData"))
        {
            string json = PlayerPrefs.GetString("PlayerImageData");
            saveData = JsonUtility.FromJson<PlayerImageSaveData>(json);
        }
    }

    public bool IsFrameUnlocked(int index) => saveData.unlockedFrames.Contains(index);
    public bool IsPortraitUnlocked(int index) => saveData.unlockedPortraits.Contains(index);

    public void UnlockFrame(int index)
    {
        if (!saveData.unlockedFrames.Contains(index))
            saveData.unlockedFrames.Add(index);
        SaveData();
    }

    public void UnlockPortrait(int index)
    {
        if (!saveData.unlockedPortraits.Contains(index))
            saveData.unlockedPortraits.Add(index);
        SaveData();
    }

    public void SelectFrame(int index)
    {
        saveData.selectedFrameIndex = index;
        SaveData();
        PlayerImageEvents.OnImageChanged?.Invoke(); // ✅ התווסף
    }

    public void SelectPortrait(int index)
    {
        saveData.selectedPortraitIndex = index;
        SaveData();
        PlayerImageEvents.OnImageChanged?.Invoke(); // ✅ התווסף
    }


    public PlayerFrame GetSelectedFrame() =>
        (saveData.selectedFrameIndex >= 0 && saveData.selectedFrameIndex < allFrames.Count)
        ? allFrames[saveData.selectedFrameIndex]
        : (allFrames.Count > 0 ? allFrames[0] : null); // ✅ נוספה תמיכה בדיפולט

    public PlayerPortrait GetSelectedPortrait() =>
        (saveData.selectedPortraitIndex >= 0 && saveData.selectedPortraitIndex < allPortraits.Count)
        ? allPortraits[saveData.selectedPortraitIndex]
        : (allPortraits.Count > 0 ? allPortraits[0] : null); // ✅ נוספה תמיכה בדיפולט
}
