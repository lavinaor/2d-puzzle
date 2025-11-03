using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class WorldManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static WorldManager Instance { get; private set; }

    public List<WorldData> worlds;
    public List<WorldData> secondaryWorlds;  // עולמות משניים

    [Header("Ads Settings")]
    [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] private string _iOSAdUnitId = "Interstitial_iOS";
    [Range(0, 100)]
    [SerializeField] private int adChancePercent = 20; // סיכוי באחוזים להצגת פרסומת

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
        string nextScene = GetNextSceneName();
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogWarning(" לא נמצא שלב הבא לטעינה");
            return;
        }

        _nextSceneToLoad = nextScene;

        int roll = Random.Range(0, 100);
        if (roll < adChancePercent)
        {
            Debug.Log($" מציג מודעה ({roll}%)");
            Advertisement.Load(_adUnitId, this);
        }
        else
        {
            Debug.Log($" מדלג על מודעה הפעם ({roll}%)");
            SceneManager.LoadScene(_nextSceneToLoad);
        }
    }


    // -------------------------
    // IUnityAdsLoadListener
    // -------------------------
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("מודעה נטענה בהצלחה. מציג...");
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"שגיאה בטעינת מודעה: {error} - {message}");
        SceneManager.LoadScene(_nextSceneToLoad); // ממשיך בלי מודעה
    }

    // -------------------------
    // IUnityAdsShowListener
    // -------------------------
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"שגיאה בהצגת מודעה: {error} - {message}");
        SceneManager.LoadScene(_nextSceneToLoad);
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("מודעה הסתיימה, טוען את השלב הבא...");
        SceneManager.LoadScene(_nextSceneToLoad);
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
                return _world.mainLevelScene;
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

    public WorldData GetWorldByName(string worldName)
    {
        foreach (var world in worlds)
        {
            if (world.worldName == worldName)
                return world;
        }

        foreach (var world in secondaryWorlds)
        {
            if (world.worldName == worldName)
                return world;
        }

        return null;
    }

    public string GetNextSceneName()
    {
        string worldName = getWorld();
        int level = getLevel();

        WorldData world = GetWorldByName(worldName);
        if (world == null)
        {
            Debug.LogWarning($"לא נמצא עולם בשם {worldName}");
            return null;
        }

        // אם אין מספר שלב (שלב מיוחד או בוס)
        if (level == -1)
        {
            Debug.Log($" שלב מיוחד (ללא מספר) בעולם {worldName}");
            return world.mainLevelScene; // או משהו שתגדיר במיוחד
        }

        // יש מספר שלב — בודקים אם יש שלב הבא
        if (level < world.endLevel)
        {
            return $"{worldName}-{level + 1}";
        }
        else
        {
            Debug.Log($" השחקן סיים את כל השלבים בעולם {worldName}, עובר לסצנה הראשית של העולם.");
            return world.mainLevelScene;
        }
    }

    public void ReturnToMainWorldBase()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        WorldData found = FindWorldBySceneName(currentScene);

        if (found != null)
        {
            if (!string.IsNullOrEmpty(found.mainLevelScene))
            {
                SceneManager.LoadScene(found.mainLevelScene);
                return;
            }
            else
            {
                Debug.LogWarning($"World {found.worldName} נמצא אך אין לו mainLevelScene מוגדר.");
                return;
            }
        }

        Debug.LogWarning("לא נמצא עולם תואם לסצנה הנוכחית: " + currentScene);
    }

    public WorldData FindWorldBySceneName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return null;

        // ראשית: בדיקה ישירה האם זו סצנת main של אחד העולם
        foreach (var w in worlds)
            if (!string.IsNullOrEmpty(w.mainLevelScene) && w.mainLevelScene == sceneName)
                return w;

        foreach (var w in secondaryWorlds)
            if (!string.IsNullOrEmpty(w.mainLevelScene) && w.mainLevelScene == sceneName)
                return w;

        // שנית: בדיקת פורמט "WorldName-..."
        // נשתמש ב-StartsWith כדי לתפוס "Forest-1", "Forest-Secret" וכו'
        foreach (var w in worlds)
        {
            if (!string.IsNullOrEmpty(w.worldName) && sceneName.StartsWith(w.worldName))
                return w;
        }

        foreach (var w in secondaryWorlds)
        {
            if (!string.IsNullOrEmpty(w.worldName) && sceneName.StartsWith(w.worldName))
                return w;
        }

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
    public string mainLevelScene;
}
