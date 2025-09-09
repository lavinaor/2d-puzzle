using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Gameplay Settings")]
    public int goal;          // כמות נקודות לניצחון
    public int moves;         // צעדים שנותרו
    public int points;        // ניקוד נוכחי
    public bool isGameEnded;  // האם המשחק הסתיים

    [SerializeField] private List<Goal> goals = new List<Goal>();

    [Header("UI")]
    public TMP_Text pointText;
    public TMP_Text movesText;
    public TMP_Text goalText;

    public Slider scoreSlider;

    [Header("Stars")]
    public List<Image> stars; // כוכבים במהלך המשחק
    public List<Image> starsEndScreen; // כוכבים במסך הסיום
    [SerializeField] private Color emptyStarColor = new Color(0.2f, 0.2f, 0.2f, 1f); // צבע כוכב ריק
    [SerializeField] private Color filledStarColor = Color.white;                     // צבע כוכב מלא
    private int starCount = 0;

    [Header("Audio & VFX")]
    [SerializeField] private AudioClip starSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private GameObject vfx;
    [SerializeField] private GameObject winVFX;
    [SerializeField] private GameObject loseVFX;

    [Header("End Screen")]
    public GameObject[] victoryPanel = new GameObject[0];
    public GameObject losePanel;

    private string areaName;
    private int levelNum;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // חישוב מטרה כוללת לפי כל היעדים
        if (goals.Count != 0)
        {
            goal = 0;
            foreach (Goal g in goals)
            {
                goal += g.goalPerType;
            }
        }

        // הגדרת הסליידר
        scoreSlider.maxValue = goal;
        scoreSlider.value = 0;

        foreach (var star in stars)
            star.color = emptyStarColor;

        foreach (var star in starsEndScreen)
            star.color = emptyStarColor;


        ParseSceneName();
    }

    private void Update()
    {
        pointText.text = "Points: " + points;
        movesText.text = moves.ToString();
        goalText.text = "Goals: " + goal;
    }

    void ParseSceneName()
    {
        string sceneName = SceneManager.GetActiveScene().name;
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

    // עיבוד תור
    public void ProcessTurn(List<candy> candiesToProcess, bool _subtractMoves)
    {
        if (goals.Count != 0)
        {
            foreach (candy c in candiesToProcess)
            {
                // יעד לפי סוג ממתק
                Goal specificGoal = goals.Find(g => g.type == c.candyType && g.goalPerType > 0);
                if (specificGoal != null)
                {
                    specificGoal.goalPerType--;
                    points++;
                }

                // יעד רנדומלי
                Goal randomGoal = goals.Find(g => g.type == CandyType.random && g.goalPerType > 0);
                if (randomGoal != null)
                {
                    randomGoal.goalPerType--;
                    points++;
                }
            }
        }
        else
        {
            points += candiesToProcess.Count;
        }

        UpdateStars();

        // עדכון הסליידר
        if (goal > 0)
        {
            scoreSlider.value = points;
        }

        if (_subtractMoves)
            moves--;

        // תנאי סיום
        if (points >= goal || (moves == 0 && starCount > 0))
        {
            EndGame(true);
            return;
        }

        if (moves == 0 && starCount <= 0)
        {
            EndGame(false);
            return;
        }
    }

    // סיום משחק
    void EndGame(bool isWin)
    {
        isGameEnded = true;

        if (isWin)
        {
            Instantiate(winVFX, victoryPanel[0].transform.position, Quaternion.identity);
            PopUpManger.Instance.ChangeUIState(4);

            foreach (var panel in victoryPanel)
                panel.SetActive(true);

            SoundFXManager.Instance.PlaySoundFXClip(winSound, transform, 1f, false);

            // אנימציה של כוכבי מסך סיום
            AnimateEndScreenStars(starsEndScreen, starCount);
        }
        else
        {
            Instantiate(loseVFX, losePanel.transform.position, Quaternion.identity);
            PopUpManger.Instance.ChangeUIState(4);
            losePanel.SetActive(true);
            SoundFXManager.Instance.PlaySoundFXClip(loseSound, transform, 1f, false);
        }
    }

    // עדכון כוכבים במשחק
    void UpdateStars()
    {
        float progress = (float)points / goal;

        if (progress >= 0.5f && starCount < 1)
        {
            GainStar(0);
        }
        if (progress >= 0.8f && starCount < 2)
        {
            GainStar(1);
        }
        if (progress >= 1.0f && starCount < 3)
        {
            GainStar(2);
        }

        // שמירת כוכבים
        SaveManager.Instance.SaveLevelStars(levelNum, starCount);
    }

    void GainStar(int index)
    {
        SoundFXManager.Instance.PlaySoundFXClip(starSound, transform, 1f, true);

        // הכוכב המקורי נצבע בצהוב ונשאר במקום גם כשהזמן עצר
        stars[index].DOColor(filledStarColor, 0.3f).SetUpdate(true);

        // שכפול זמני של הכוכב לאנימציה
        CreateStarEffect(stars[index]);

        // עדכון מונה הכוכבים
        starCount = index + 1;
    }


    void CreateStarEffect(Image originalStar)
    {
        // שכפול הכוכב
        GameObject tempStar = Instantiate(originalStar.gameObject, originalStar.transform.parent);
        tempStar.transform.SetSiblingIndex(originalStar.transform.GetSiblingIndex() + 1);

        Image tempImage = tempStar.GetComponent<Image>();
        tempImage.color = filledStarColor;

        tempStar.transform.DOScale(1.08f, 0.25f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true) // רץ גם כש-Time.timeScale = 0
            .OnComplete(() =>
            {
                tempStar.transform.DOScale(1f, 0.15f)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(true);
            });

        tempStar.transform.DOShakePosition(0.3f, new Vector3(4f, 4f, 0f), 10)
            .SetUpdate(true);

        tempImage.DOFade(0f, 1f)
            .SetDelay(0.1f)
            .SetUpdate(true)
            .OnComplete(() => Destroy(tempStar));

    }

    // אנימציה למסך הסיום כמו באפקט בזמן המשחק
    void AnimateEndScreenStars(List<Image> starsList, int count)
    {
        StartCoroutine(AnimateEndStarsCoroutine(starsList, count));
    }

    private IEnumerator AnimateEndStarsCoroutine(List<Image> starsList, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int index = i;

            // הכוכב המקורי הופך לצהוב
            starsList[index].DOColor(filledStarColor, 0.3f).SetUpdate(true);

            // אפקט שכפול מותאם למסך הסיום
            CreateEndScreenStarEffect(starsList[index]);

            // סאונד
            SoundFXManager.Instance.PlaySoundFXClip(starSound, transform, 1f, true);

            // חצי שניה בין כוכב לכוכב
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }


    void CreateEndScreenStarEffect(Image originalStar)
    {
        GameObject tempStar = Instantiate(originalStar.gameObject, originalStar.transform.parent);
        tempStar.transform.SetSiblingIndex(originalStar.transform.GetSiblingIndex() + 1);

        Image tempImage = tempStar.GetComponent<Image>();
        tempImage.color = filledStarColor;

        // נשארים בגודל המקורי ואז מגדילים מזערית
        Vector3 originalScale = originalStar.transform.localScale;
        tempStar.transform.localScale = originalScale;

        tempStar.transform.DOScale(originalScale * 1.05f, 0.25f) // הגדלה מזערית ב-5%
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                tempStar.transform.DOScale(originalScale, 0.15f) // חוזר לגודל המקורי
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(true);
            });

        // רטט קטן מאוד
        tempStar.transform.DOShakePosition(0.3f, new Vector3(0.2f, 0.2f, 0f), 8)
            .SetUpdate(true);

        // Fade הדרגתי
        tempImage.DOFade(0f, 1f)
            .SetDelay(0.1f)
            .SetUpdate(true)
            .OnComplete(() => Destroy(tempStar));
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
