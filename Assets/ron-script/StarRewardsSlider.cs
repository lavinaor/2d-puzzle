using UnityEngine;
using UnityEngine.UI;

public class SimpleStarRewards : MonoBehaviour
{
    [Header("סליידר להצגת ההתקדמות")]
    [SerializeField] private Slider progressSlider;   // סליידר להצגת ההתקדמות
    [SerializeField] private int maxStars = 50;       // מספר הכוכבים המקסימלי

    [Header("פרס")]
    [SerializeField] private int rewardThreshold = 10; // מספר הכוכבים לקבלת פרס
    private bool rewardGiven = false;                 // מונע מתן הפרס פעמיים

    void Start()
    {
        // מונע מהשחקן להזיז את הסליידר אך שומר על כל התמונות שלו
        if (progressSlider != null)
        {
            progressSlider.interactable = false;
        }

        UpdateProgress();
    }

    // קריאה כל פעם שהכוכבים משתנים
    public void UpdateProgress()
    {
        int totalStars = SaveManager.Instance.GetTotalStars(); // מספר הכוכבים שלך

        // עדכון הסליידר
        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = maxStars;
            progressSlider.value = totalStars;
        }

        // בדיקה אם הגיע הזמן לתת פרס
        if (!rewardGiven && totalStars >= rewardThreshold)
        {
            GiveReward();
            rewardGiven = true; // מונע מתן שוב
        }

        // אופציונלי: כמה כוכבים עד הפרס הבא
        int starsLeft = Mathf.Max(rewardThreshold - totalStars, 0);
        Debug.Log("כוכבים עד הפרס הבא: " + starsLeft);
    }

    private void GiveReward()
    {
        Debug.Log("קיבלת פרס! ");
        // כאן אפשר לשים את הקוד האמיתי למתן פרס
        // לדוגמה Instantiate של פרס, להפעיל כפתור או להוסיף מטבעות
    }
}
