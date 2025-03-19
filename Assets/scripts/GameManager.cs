using System;
using System.Collections;
using System.Collections.Generic;
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

    //���� ����
    public GameObject beckgroundPanel;
    //��� ������
    public GameObject victoryPanel;
    //��� ����
    public GameObject losePanel;

    //����� �� �������� �� ����
    public TMP_Text pointText;
    public TMP_Text movesText;
    public TMP_Text goalText;

    // ����� ����� ������ �-Slider
    public Slider scoreSlider;

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
    }

    // Update is called once per frame
    void Update()
    {
        pointText.text = "Points: " + points.ToString();
        movesText.text = "Moves: " + moves.ToString();
        goalText.text = "goals: " + goal.ToString();
    }

    public void ProcessTurn(List<candy> candiesToProcess, bool _subtracMoves)
    {

        if (goals.Count != 0)
        {
            // ���� �� ���� ������
            foreach (candy c in candiesToProcess)
            {
                // ��� �� ����� ������� ���� �����
                Goal goalData = goals.Find(g => g.type == c.candyType);

                // �� ����� ���� ���� ���� �� �����
                if (goalData != null && goalData.goalPerType > 0)
                {
                    // ���� �� ����� ������� ��� ��� ����
                    goalData.goalPerType--;

                    points++;
                    // ���� �� �-Slider (�� �� ����)
                    scoreSlider.value = (float)points / goal;
                }
            }
        }
        else
        {
            points += candiesToProcess.Count;
        }


        // ���� �� ���� �� ������� ����� 0-1
        if (goal > 0)
        {
            scoreSlider.value = (float)points/* / goal*/;
        }

        if (_subtracMoves)
        {
            moves--;
        }

        if (points >= goal)
        {
            isGameEnded = true;
            beckgroundPanel.SetActive(true);
            victoryPanel.SetActive(true);
            return;
        }

        if (moves == 0)
        {
            isGameEnded = true;
            beckgroundPanel.SetActive(true);
            losePanel.SetActive(true);
            return;
        }
    }

    //����� ������ ����� ���� ����� �� ������
    public void WinGame()
    {
        SceneManager.LoadScene(0);
    }

    //����� ������ ����� ���� ����� �� ����
    public void LoseGame()
    {
        SceneManager.LoadScene(0);
    }

    [Serializable]
    class Goal
    {
        public candyType type;
        public int goalPerType;
    }

}
