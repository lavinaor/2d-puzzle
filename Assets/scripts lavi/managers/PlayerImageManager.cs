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
        }
        else
        {
            Destroy(gameObject);
        }
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
    }

    public void SelectPortrait(int index)
    {
        saveData.selectedPortraitIndex = index;
        SaveData();
    }

    public PlayerFrame GetSelectedFrame() =>
        (saveData.selectedFrameIndex >= 0 && saveData.selectedFrameIndex < allFrames.Count)
        ? allFrames[saveData.selectedFrameIndex]
        : null;

    public PlayerPortrait GetSelectedPortrait() =>
        (saveData.selectedPortraitIndex >= 0 && saveData.selectedPortraitIndex < allPortraits.Count)
        ? allPortraits[saveData.selectedPortraitIndex]
        : null;
}
