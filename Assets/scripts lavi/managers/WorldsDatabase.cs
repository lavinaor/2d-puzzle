using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class WorldManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static WorldManager Instance { get; private set; }

    public List<WorldData> worlds;

    [Header("Ads Settings")]
    [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] private string _iOSAdUnitId = "Interstitial_iOS";
    private string _adUnitId;
    private string _nextSceneToLoad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#else
        _adUnitId = _androidAdUnitId;
#endif
    }

    public void LoadNextLevel()
    {
        int currentLevel = getLevel();
        int nextLevel = currentLevel + 1;
        string nextScene = GetSceneNameForLevel(nextLevel);

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogWarning("�� ���� ��� ��� ������");
            return;
        }

        SaveManager.Instance.ChanglastLevelEnterd(nextLevel);
        _nextSceneToLoad = nextScene;

        Debug.Log("���� ����� ���� ����� ���� ���...");
        Advertisement.Load(_adUnitId, this);
    }

    // -------------------------
    // IUnityAdsLoadListener
    // -------------------------
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("����� ����� ������. ����...");
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"����� ������ �����: {error} - {message}");
        SceneManager.LoadScene(_nextSceneToLoad); // ����� ��� �����
    }

    // -------------------------
    // IUnityAdsShowListener
    // -------------------------
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"����� ����� �����: {error} - {message}");
        SceneManager.LoadScene(_nextSceneToLoad);
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("����� �������, ���� �� ���� ���...");
        SceneManager.LoadScene(_nextSceneToLoad);
    }

    // -------------------------
    // ��� ��������� ��� ������ ��� ����
    // -------------------------

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

    public int getLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string[] parts = sceneName.Split('-');

        if (parts.Length == 2 && int.TryParse(parts[1], out int stageNumber))
            return stageNumber;

        Debug.LogWarning("Scene name format is invalid! Expected format: areaName-stageNumber");
        return -1;
    }

    public string getWorld()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string[] parts = sceneName.Split('-');

        if (parts.Length == 2)
            return parts[0];

        Debug.LogWarning("Scene name format is invalid! Expected format: areaName-stageNumber");
        return null;
    }
}

[System.Serializable]
public class WorldData
{
    public string worldName;
    public int startLevel;
    public int endLevel;
    public string displayName;
    public string mainLevelSecen;
}
