using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : SceneChanger
{
    [SerializeField]
    private List<Image> stars; // רשימת תמונות של כוכבים

    [SerializeField]
    private int level; // מספר שלב

    [SerializeField] 
    private GameObject lockedVisual;  

    [SerializeField] 
    private GameObject unlockedVisual; 

    [SerializeField] 
    private Button button;

    [SerializeField]
    private List<Graphic> visualsToGray;

    private bool isUnlocked = false;

    private void Start()
    {
        int starsNum = SaveManager.Instance.GetStarsForLevel(level);
        Debug.Log("כוכבים לשלב " + level + ": " + starsNum);
        CheckIfUnlocked();
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
            stars[i].gameObject.SetActive(true);
        }
    }

    void CheckIfUnlocked()
    {
        if (level == 1 || SaveManager.Instance.GetStarsForLevel(level - 1) > 0)
        {
            isUnlocked = true;
            if (lockedVisual != null) lockedVisual.SetActive(false);
            if (unlockedVisual != null) unlockedVisual.SetActive(true);
            if (button != null) button.interactable = true;
        }
        else
        {
            isUnlocked = false;
            if (lockedVisual != null) lockedVisual.SetActive(true);
            if (unlockedVisual != null) unlockedVisual.SetActive(false);
            if (button != null) button.interactable = false;
        }

        UpdateGrayVisuals();
    }

    private void UpdateGrayVisuals()
    {
        Color targetColor = isUnlocked ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f);

        foreach (var g in visualsToGray)
        {
            if (g != null)
                g.color = targetColor;
        }
    }

    public override void OnChangeSeneDeley(string sceneName)
    {
        if (isUnlocked)
        {
            base.OnChangeSeneDeley(sceneName);
        }
        else
        {
            Debug.Log("שלב נעול");
        }
    }
}