using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    public List<WorldData> worlds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // שומר את האובייקט בין סצנות
    }

    public WorldData GetWorldForLevel(int level)
    {
        foreach (var world in worlds)
        {
            if (level >= world.startLevel && level <= world.endLevel)
                return world;
        }
        return null;
    }

    public bool HasNextLevelInWorld(int level)
    {
        var world = GetWorldForLevel(level);
        return world != null && level < world.endLevel;
    }

    public int GetNextLevel(int level)
    {
        if (HasNextLevelInWorld(level))
            return level + 1;
        return -1;
    }

    public string GetSceneNameForLevel(int level)
    {
        var world = GetWorldForLevel(level);
        if (world == null) return "";
        return $"{world.worldName}-{level}";
    }

    public string GetMainlevelSceneName(int level)
    {
        foreach (var _world in worlds)
        {
            if (level >= _world.startLevel && level <= _world.endLevel)
                return _world.mainLevelSecen;
        }
        return null;
    }

    public void LoadNextLevel(int currentLevel)
    {
        if (HasNextLevelInWorld(currentLevel))
        {
            currentLevel++;
            string scene = GetSceneNameForLevel(currentLevel);
            SceneManager.LoadScene(scene);
        }
        else
        {
            // עולם נגמר, לך למסך מעבר/פתיחה
            Debug.Log("סוף העולם, פותחים עולם חדש!");
            SceneManager.LoadScene("WorldUnlockScene");
        }
    }
}

[System.Serializable]
public class WorldData
{
    public string worldName;      // לדוגמה: "sea"
    public int startLevel;        // לדוגמה: 1
    public int endLevel;          // לדוגמה: 9
    public string displayName;    // שם מוצג: "ים"
    public string mainLevelSecen;    // שם מוצג: "ים"
}
