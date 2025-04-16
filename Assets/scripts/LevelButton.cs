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

    public override void OnChangeSeneDeley(string sceneName)
    {
        if(level != 1)
        {
            if (0 < SaveManager.Instance.GetStarsForLevel(level - 1))
            {
                base.OnChangeSeneDeley(sceneName);
            }
        }
        else
        {
            base.OnChangeSeneDeley(sceneName);
        }
    }
}