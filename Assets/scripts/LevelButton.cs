using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : SceneChanger
{
    [SerializeField]
    private List<Image> stars; // ����� ������ �� ������

    [SerializeField]
    private int level; // ���� ���

    private void Start()
    {
        int starsNum = SaveManager.Instance.GetStarsForLevel(level);
        Debug.Log("������ ���� " + level + ": " + starsNum);
        UpdateStars(starsNum);
    }

    void UpdateStars(int starsNum)
    {
        // ���� �� �� �������
        foreach (var star in stars)
        {
            star.gameObject.SetActive(false);
        }

        // ����� �� ����� �������
        for (int i = 0; i < starsNum && i < stars.Count; i++)
        {
            Debug.Log(starsNum);
            stars[i].gameObject.SetActive(true);
        }
    }
}