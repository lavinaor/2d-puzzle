using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textObject;

    private string areaName;
    private int stageNumber;

    void Start()
    {
        ParseSceneName();
        if (textObject != null)
        {
            textObject.text = /*areaName + ": " +*/ stageNumber.ToString();
        }
        else
        {
            Debug.LogWarning("לא הוגדר TextMeshProUGUI");
        }
    }

    void ParseSceneName()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // פיצול לפי מקף
        string[] parts = sceneName.Split('-');

        if (parts.Length == 2)
        {
            areaName = parts[0];
            int.TryParse(parts[1], out stageNumber);
        }
        else
        {
            Debug.LogWarning("Scene name format is invalid! Expected format: areaName-stageNumber");
        }
    }
}
