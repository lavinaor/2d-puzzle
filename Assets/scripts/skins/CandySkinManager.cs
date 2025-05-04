using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CandySkinManager : MonoBehaviour
{
    public static CandySkinManager Instance;

    [Header("All Available Skins")]
    public List<CandySkin> allSkins = new List<CandySkin>();

    [Header("Current Selection")]
    public int selectedSkinIndex = 0;

    private CandySkinSaveData saveData = new CandySkinSaveData();

    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "candyskinsave.json");
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public CandySkin GetSelectedSkin()
    {
        if (selectedSkinIndex < allSkins.Count)
            return allSkins[selectedSkinIndex];
        else
            return null;
    }

    public void SelectSkin(int skinIndex)
    {
        if (IsSkinUnlocked(skinIndex))
        {
            selectedSkinIndex = skinIndex;
            SaveData();
        }
        else
        {
            Debug.LogWarning("Skin not unlocked yet!");
        }
    }

    public bool IsSkinUnlocked(int skinIndex)
    {
        if (skinIndex < 0 || skinIndex >= allSkins.Count) return false;

        return saveData.unlockedSkins.Contains(skinIndex);
    }

    public void UnlockSkin(int skinIndex)
    {
        if (!IsSkinUnlocked(skinIndex))
        {
            saveData.unlockedSkins.Add(skinIndex);
            SaveData();
        }
    }

    private void SaveData()
    {
        saveData.selectedSkinIndex = selectedSkinIndex;
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Candy skins saved to " + saveFilePath);
    }

    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<CandySkinSaveData>(json);
            selectedSkinIndex = saveData.selectedSkinIndex;
        }
        else
        {
            // ברירת מחדל: לפתוח את הסקין הראשון
            saveData = new CandySkinSaveData();
            saveData.unlockedSkins.Add(0);
            selectedSkinIndex = 0;
            SaveData();
        }
    }

    // פונקציה שמחזירה ספרייט לפי סוג ממתק
    public Sprite GetCandySpriteByType(CandyType type)
    {
        foreach (var pair in GetSelectedSkin().candyPairs)
        {
            if (pair.type == type)
                return pair.sprite;
        }
        return null; // לא נמצא
    }
}

[System.Serializable]
public class CandyTypeSpritePair
{
    public CandyType type;
    public Sprite sprite;
}

[System.Serializable]
public class CandySkinSaveData
{
    public int selectedSkinIndex = 0;
    public List<int> unlockedSkins = new List<int>();
}
