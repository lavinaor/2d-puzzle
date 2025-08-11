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
    //הופך אותו לסטטי
    public static GameManager Instance;

    //הכמות שצריך כדי לנצח את השלב
    public int goal;
    // כמות הצעדים שיש לך 
    public int moves;
    //שומר את הניקוד
    public int points;

    //שומר האם המשחק נגמר
    public bool isGameEnded;

    // רשימה של מטרות לכל סוג ממתק
    [SerializeField]
    private List<Goal> goals = new List<Goal>();

    //השומר את הכיתובים על המסך
    public TMP_Text pointText;
    public TMP_Text movesText;
    public TMP_Text goalText;

    // נוסיף משתנה לשמירת ה-Slider
    public Slider scoreSlider;

    // הוספת רשימה לכוכבים
    public List<Image> stars;
    private int starCount = 0;

    // קובץ הסאונד להשמעה
    [SerializeField]
    private AudioClip starSound;

    // אפקט בפריפב
    [SerializeField]
    private GameObject vfx;

    [Header("End screen")]
    //מסך ניצחון
    public GameObject[] victoryPanel = new GameObject[0];
    //מסך הפסד
    public GameObject losePanel;

    public List<Image> starsEndScree;

    private string areaName;
    private int levelNum;

    // קובץ הסאונד להשמעה
    [SerializeField]
    private AudioClip winSound;
    [SerializeField]
    private AudioClip loseSound;

    // אפקטים לניצחון והפסד
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
        // עדכון ה-Slider שיתחיל מ-0 עם הגבלת ה-Max
        scoreSlider.maxValue = goal;
        scoreSlider.value = 0;
        ParseSceneName();
    }

    void ParseSceneName()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // פיצול לפי מקף
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
            // עבור כל ממתק ברשימה
            foreach (candy c in candiesToProcess)
            {
                // הפחת ממטרה תואמת לסוג הממתק
                Goal specificGoal = goals.Find(g => g.type == c.candyType && g.goalPerType > 0);
                if (specificGoal != null)
                {
                    specificGoal.goalPerType--;
                    points++;
                }

                // הפחת גם ממטרה רנדומלית אם קיימת
                Goal randomGoal = goals.Find(g => g.type == CandyType.random && g.goalPerType > 0);
                if (randomGoal != null)
                {
                    randomGoal.goalPerType--;
                    points++; // רק אם לא ספרנו כבר בנקודה קודם
                }
            }
        }
        else
        {
            points += candiesToProcess.Count;
        }

        // קריאה לעדכון הכוכבים
        UpdateStars();

        // מחשב את הערך של הסליידר בטווח 0-1
        if (goal > 0)
        {
            scoreSlider.value = (float)points/* / goal*/;
        }

        //מוריד מהלכים אם צריך
        if (_subtracMoves)
        {
            moves--;
        }





        // סיום המשחק
        if (points >= goal || (moves == 0 && starCount > 0))
        {
            isGameEnded = true;

            // אפקט של ניצחון
            Instantiate(winVFX, victoryPanel[0].transform.position, Quaternion.identity);

            //הפעל מצב ניצחון
            PopUpManger.Instance.ChangeUIState(4);
            foreach (var panel in victoryPanel)
            {
                panel.SetActive(true);
            }

            //סאונד של ניצחון
            SoundFXManager.Instance.PlaySoundFXClip(winSound, transform, 1f, false);
            return;
        }

        if (moves == 0 && starCount <= 0)
        {
            isGameEnded = true;

            // אפקט של הפסד
            Instantiate(loseVFX, losePanel.transform.position, Quaternion.identity);

            //הפעל מצב הפסד
            PopUpManger.Instance.ChangeUIState(4);
            losePanel.SetActive(true);

            //סאונד של הפסד
            SoundFXManager.Instance.PlaySoundFXClip(loseSound, transform, 1f, false);
            return;
        }
    }

    void UpdateStars()
    {
        float progress = (float)points / goal;

        if (progress >= 0.5f && starCount < 1)
        {
            // כאן הוספתי את ה-Transform של אובייקט ה-SceneChanger
            SoundFXManager.Instance.PlaySoundFXClip(starSound, transform, 1f, true);

            // אפקט ויזואלי לכוכב
            Instantiate(vfx, stars[0].transform.position, Quaternion.identity);

            stars[0].gameObject.SetActive(true);
            starsEndScree[0].gameObject.SetActive(true);
            starCount = 1;
        }
        if (progress >= 0.8f && starCount < 2)
        {
            // כאן הוספתי את ה-Transform של אובייקט ה-SceneChanger
            SoundFXManager.Instance.PlaySoundFXClip(starSound, transform, 1f, true);

            // אפקט ויזואלי לכוכב
            Instantiate(vfx, stars[1].transform.position, Quaternion.identity);

            stars[1].gameObject.SetActive(true);
            starsEndScree[1].gameObject.SetActive(true);
            starCount = 2;
        }
        if (progress >= 1.0f && starCount < 3)
        {
            // כאן הוספתי את ה-Transform של אובייקט ה-SceneChanger
            SoundFXManager.Instance.PlaySoundFXClip(starSound, transform, 1f, true);

            // אפקט ויזואלי לכוכב
            Instantiate(vfx, stars[2].transform.position, Quaternion.identity);

            stars[2].gameObject.SetActive(true);
            starsEndScree[2].gameObject.SetActive(true);
            starCount = 3;
        }

        // שמור כוכבים
        SaveManager.Instance.SaveLevelStars(levelNum, starCount); // שמור לשלב 3, 2 כוכבים
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
