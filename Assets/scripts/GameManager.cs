using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //���� ���� �����
    public static GameManager Instance;

    //���� ����
    public GameObject beckgroundPanel;
    //��� ������
    public GameObject victoryPanel;
    //��� ����
    public GameObject losePanel;

    //����� ����� ��� ���� �� ����
    public int goal;
    // ���� ������ ��� �� 
    public int moves;
    //���� �� ������
    public int points;

    //���� ��� ����� ����
    public bool isGameEnded;

    //����� �� �������� �� ����
    public TMP_Text pointText;
    public TMP_Text movesText;
    public TMP_Text goalText;

    private void Awake()
    {
        Instance = this;
    }

    public void Initilize(int _moves, int _goal)
    {
        moves = _moves;
        goal = _goal;
    }

    // Update is called once per frame
    void Update()
    {
        pointText.text = "Points: " + points.ToString();
        movesText.text = "Moves: " + moves.ToString();
        goalText.text = "goals: " + goal.ToString();
    }

    public void ProcessTurn(int _pointToGain, bool _subtracMoves)
    {
        points += _pointToGain;
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
}
