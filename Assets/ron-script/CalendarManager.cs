using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DayReward
{
    public int dayNumber;
    public Sprite rewardImage; // תמונת הפרס
}

public class CalendarManager : MonoBehaviour
{
    public GameObject dayButtonPrefab; // Prefab של יום (כפתור)
    public Transform gridParent;       // Panel עם Grid Layout
    public int daysInMonth = 30;       // מספר ימים

    public Color claimedColor = Color.gray;
    public Color currentDayColor = Color.yellow;
    public Color futureDayColor = Color.black;

    public int currentDay = 1; // היום הנוכחי בחודש

    public DayReward[] dayRewards; // כאן אפשר לשים תמונה לכל יום

    public GameObject checkmarkPrefab; // prefab קטן עם ✔ עבור ימים שכבר תבעו

    void Start()
    {
        CreateCalendar();
    }

    void CreateCalendar()
    {
        for (int i = 1; i <= daysInMonth; i++)
        {
            GameObject day = Instantiate(dayButtonPrefab, gridParent);
            day.name = "Day" + i;

            Text buttonText = day.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = i.ToString();

            Image rewardImg = day.transform.Find("RewardImage")?.GetComponent<Image>();
            if (rewardImg != null)
            {
                DayReward reward = System.Array.Find(dayRewards, r => r.dayNumber == i);
                if (reward != null && reward.rewardImage != null)
                {
                    rewardImg.sprite = reward.rewardImage;
                    rewardImg.enabled = true;
                }
                else
                {
                    rewardImg.enabled = false;
                }
            }

            Button button = day.GetComponent<Button>();
            int dayIndex = i; // משתנה מקומי כדי להימנע מבעיות closure
            bool claimed = PlayerPrefs.GetInt("Day" + dayIndex, 0) == 1;

            if (claimed)
            {
                SetDayColor(day, claimedColor);
                AddCheckmark(day);
                button.interactable = false; // כבר תבעו, לא ניתן ללחוץ
            }
            else
            {
                button.interactable = true; // ניתן ללחוץ
                button.onClick.AddListener(() => ClaimDay(dayIndex, day));

                // צבע התצוגה הראשוני
                if (dayIndex == currentDay)
                    SetDayColor(day, currentDayColor);
                else
                    SetDayColor(day, futureDayColor);
            }
        }
    }

    void SetDayColor(GameObject day, Color color)
    {
        Image img = day.GetComponent<Image>();
        if (img != null) img.color = color;
    }

    void AddCheckmark(GameObject day)
    {
        if (checkmarkPrefab == null) return;
        GameObject check = Instantiate(checkmarkPrefab, day.transform);
        RectTransform rt = check.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-10, -10); // פינה עליונה ימנית
    }

    void ClaimDay(int dayIndex, GameObject day)
    {
        Debug.Log("Day " + dayIndex + " claimed!");
        PlayerPrefs.SetInt("Day" + dayIndex, 1);

        // צבע ווי רק ביום הזה
        SetDayColor(day, claimedColor);
        AddCheckmark(day);

        // פרס
        DayReward reward = System.Array.Find(dayRewards, r => r.dayNumber == dayIndex);
        if (reward != null && reward.rewardImage != null)
        {
            Debug.Log($"You claimed reward for day {dayIndex}!");
            // כאן אפשר להוסיף לוגיקה להוסיף את הפרס למלאי השחקן
        }

        // כפתור כבר לא לחיץ
        Button button = day.GetComponent<Button>();
        if (button != null) button.interactable = false;
    }
}
