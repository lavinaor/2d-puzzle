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
        DontDestroyOnLoad(gameObject); // ���� �� �������� ��� �����
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

    public void LoadNextLevel(int Level)
    {
        if (HasNextLevelInWorld(Level))
        {
            Level++;
            string scene = GetSceneNameForLevel(Level);
            SceneManager.LoadScene(scene);
        }
        else
        {
            // ���� ����, �� ���� ����/�����
            Debug.Log("��� �����, ������ ���� ���!");
            SceneManager.LoadScene("WorldUnlockScene");
        }
    }

    public int getLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;


        // ����� ��� ���
        string[] parts = sceneName.Split('-');

        if (parts.Length == 2)
        {
            int.TryParse(parts[1], out int stageNumber);
            return stageNumber;
        }
        else
        {
            Debug.LogWarning("Scene name format is invalid! Expected format: areaName-stageNumber");
        }
        return -1;
    }

    public string getWorld()
    {
        string sceneName = SceneManager.GetActiveScene().name;


        // ����� ��� ���
        string[] parts = sceneName.Split('-');

        if (parts.Length == 2)
        {

            return parts[0];
        }
        else
        {
            Debug.LogWarning("Scene name format is invalid! Expected format: areaName-stageNumber");
        }
        return null;
    }
}

[System.Serializable]
public class WorldData
{
    public string worldName;      // ������: "sea"
    public int startLevel;        // ������: 1
    public int endLevel;          // ������: 9
    public string displayName;    // �� ����: "��"
    public string mainLevelSecen;    // �� ����: "��"
}
