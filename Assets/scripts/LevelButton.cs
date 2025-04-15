using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : SceneChanger
{
    [SerializeField]
    private List<Image> stars; // רשימת תמונות של כוכבים

    [SerializeField]
    private int level; // מספר שלב

    private void Start()
    {
        int starsNum = SaveManager.Instance.GetStarsForLevel(level);
        Debug.Log("כוכבים לשלב " + level + ": " + starsNum);
        UpdateStars(starsNum);
    }

    void UpdateStars(int starsNum)
    {
        // מכבה את כל הכוכבים
        foreach (var star in stars)
        {
            star.gameObject.SetActive(false);
        }

        // מדליק את הכמות המתאימה
        for (int i = 0; i < starsNum && i < stars.Count; i++)
        {
            Debug.Log(starsNum);
            stars[i].gameObject.SetActive(true);
        }
    }
}