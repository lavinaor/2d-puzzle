using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //���� ���� �����
    public static GameManager Instance;

    //����� ����� ��� ���� �� ����
    public int goal;
    // ���� ������ ��� �� 
    public int moves;
    //���� �� ������
    public int points;

    //���� ��� ����� ����
    public bool isGameEnded;

    // ����� �� ����� ��� ��� ����
    [SerializeField]
    private List<Goal> goals = new List<Goal>();

    //����� �� �������� �� ����
    public TMP_Text pointText;
    public TMP_Text movesText;
    public TMP_Text goalText;

    // ����� ����� ������ �-Slider
    public Slider scoreSlider;

    // ����� ����� �������
    public List<Image> stars;
    private int starCount = 0;

    // ���� ������ ������
    [SerializeField]
    private AudioClip starSound;

    // ���� ������
    [SerializeField]
    private GameObject vfx;

    [Header("End screen")]
    //��� ������
    public GameObject[] victoryPanel = new GameObject[0];
    //��� ����
    public GameObject losePanel;

    public List<Image> starsEndScree;

    private string areaName;
    private int levelNum;

    // ���� ������ ������
    [SerializeField]
    private AudioClip winSound;
    [SerializeField]
    private AudioClip loseSound;

    // ������ ������� �����
    [SerializeField]
    private GameObject winVFX;
    [SerializeField]
    private GameObject loseVFX;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (goals.Count != 0)
        {
            goal = 0;
            foreach (Goal g in goals)
            {
                goal += g.goalPerType;
            }
        }
        // ����� �-Slider ������ �-0 �� ����� �-Max
        scoreSlider.maxValue = goal;
        scoreSlider.value = 0;
        ParseSceneName();
    }

    void ParseSceneName()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // ����� ��� ���
        string[] parts = sceneName.Split('-');

        if (parts.Length == 2)
        {
            areaName = parts[0];
            int.TryParse(parts[1], out levelNum);
        }
        else
        {
            Debug.LogWarning("Scene name format is invalid! Expected format: areaName-stageNumber");
        }
    }

    // Update is called once per frame
    void Update()
    {
        pointText.text = "Points: " + points.ToString();
        movesText.text = moves.ToString();
        goalText.text = "goals: " + goal.ToString();
    }

    public void ProcessTurn(List<candy> candiesToProcess, bool _subtracMoves)
    {

        if (goals.Count != 0)
        {
            // ���� �� ���� ������
            foreach (candy c in candiesToProcess)
            {
                // ���� ����� ����� ���� �����
                Goal specificGoal = goals.Find(g => g.type == c.candyType && g.goalPerType > 0);
                if (specificGoal != null)
                {
                    specificGoal.goalPerType--;
                    points++;
                }

                // ���� �� ����� �������� �� �����
                Goal randomGoal = goals.Find(g => g.type == CandyType.random && g.goalPerType > 0);
                if (randomGoal != null)
                {
                    randomGoal.goalPerType--;
                    points++; // �� �� �� ����� ��� ������ ����
                }
            }
        }
        else
        {
            points += candiesToProcess.Count;
        }

        // ����� ������ �������
        UpdateStars();

        // ���� �� ���� �� ������� ����� 0-1
        if (goal > 0)
        {
            scoreSlider.value = (float)points/* / goal*/;
        }

        //����� ������ �� ����
        if (_subtracMoves)
        {
            moves--;
        }





        // ���� �����
        if (points >= goal || (moves == 0 && starCount > 0))
        {
            isGameEnded = true;

            // ���� �� ������
            Instantiate(winVFX, victoryPanel[0].transform.position, Quaternion.identity);

            //���� ��� ������
            PopUpManger.Instance.ChangeUIState(4);
            foreach (var panel in victoryPanel)
            {
                panel.SetActive(true);
            }

            //����� �� ������
            SoundFXManager.Instance.PlaySoundFXClip(winSound, transform, 1f, false);
            return;
        }

        if (moves == 0 && starCount <= 0)
        {
            isGameEnded = true;

            // ���� �� ����
            Instantiate(loseVFX, losePanel.transform.position, Quaternion.identity);

            //���� ��� ����
            PopUpManger.Instance.ChangeUIState(4);
            losePanel.SetActive(true);

            //����� �� ����
            SoundFXManager.Instance.PlaySoundFXClip(loseSound, transform, 1f, false);
            return;
        }
    }

    void UpdateStars()
    {
        float progress = (float)points / goal;

        if (progress >= 0.5f && starCount < 1)
        {
            // ��� ������ �� �-Transform �� ������� �-SceneChanger
            SoundFXManager.Instance.PlaySoundFXClip(starSound, transform, 1f, true);

            // ���� ������� �����
            Instantiate(vfx, stars[0].transform.position, Quaternion.identity);

            stars[0].gameObject.SetActive(true);
            starsEndScree[0].gameObject.SetActive(true);
            starCount = 1;
        }
        if (progress >= 0.8f && starCount < 2)
        {
            // ��� ������ �� �-Transform �� ������� �-SceneChanger
            SoundFXManager.Instance.PlaySoundFXClip(starSound, transform, 1f, true);

            // ���� ������� �����
            Instantiate(vfx, stars[1].transform.position, Quaternion.identity);

            stars[1].gameObject.SetActive(true);
            starsEndScree[1].gameObject.SetActive(true);
            starCount = 2;
        }
        if (progress >= 1.0f && starCount < 3)
        {
            // ��� ������ �� �-Transform �� ������� �-SceneChanger
            SoundFXManager.Instance.PlaySoundFXClip(starSound, transform, 1f, true);

            // ���� ������� �����
            Instantiate(vfx, stars[2].transform.position, Quaternion.identity);

            stars[2].gameObject.SetActive(true);
            starsEndScree[2].gameObject.SetActive(true);
            starCount = 3;
        }

        // ���� ������
        SaveManager.Instance.SaveLevelStars(levelNum, starCount); // ���� ���� 3, 2 ������
    }

    public List<Goal> GetGoals()
    {
        return goals;
    }

    [Serializable]
    public class Goal
    {
        public CandyType type;
        public int goalPerType;
    }

}
