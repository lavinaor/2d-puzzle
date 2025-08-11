using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField]
    private List<Image> stars; // ����� ������ �� ������

    [SerializeField]
    private int level; // ���� ���

    // ���� ������ ������
    [SerializeField]
    private AudioClip sceneTransitionSound;

    [SerializeField]
    private TextMeshProUGUI levelText;


    //[SerializeField] 
    private GameObject unlockedVisual; 

    //����� ������
    [SerializeField] 
    private GameObject lockedVisual;  
    [SerializeField]
    private List<Graphic> visualsToGray;

    //���� �� ������ �� ����
    private Button button;

    private bool isUnlocked = false;

    private void Start()
    {
        levelText.text = level.ToString();
        button = GetComponent<Button>();
        int starsNum = SaveManager.Instance.GetStarsForLevel(level);
        Debug.Log("starsNum ���� " + level + ": " + starsNum);
        CheckIfUnlocked();
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

    public void OnChangeSeneSound()
    {
        if (isUnlocked)
        {
            string levelName = WorldManager.Instance.GetSceneNameForLevel(level);

            // ����� ������ ����� SoundFXManager
            if (SoundFXManager.Instance != null)
            {
                // ��� ������ �� �-Transform �� ������� �-SceneChanger
                SoundFXManager.Instance.PlaySoundFXClip(sceneTransitionSound, transform, 1f, true);
            }

            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.Log("��� ����");
        }
    }
}