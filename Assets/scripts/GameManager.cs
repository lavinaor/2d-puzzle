using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //הופך אותו לסטטי
    public static GameManager Instance;

    //הרקע ללוח
    public GameObject beckgroundPanel;
    //מסך ניצחון
    public GameObject victoryPanel;
    //מסך הפסד
    public GameObject losePanel;

    //הכמות שצריך כדי לנצח את השלב
    public int goal;
    // כמות הצעדים שיש לך 
    public int moves;
    //שומר את הניקוד
    public int points;

    //שומר האם המשחק נגמר
    public bool isGameEnded;

    //השומר את הכיתובים על המסך
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

    //מחובר לכפתור שמשנה סצנה במקרה של ניצחון
    public void WinGame()
    {
        SceneManager.LoadScene(0);
    }

    //מחובר לכפתור שמשנה סצנה במקרה של הפסד
    public void LoseGame()
    {
        SceneManager.LoadScene(0);
    }
}
