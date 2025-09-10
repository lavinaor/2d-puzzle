using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class CountdownButtonWithPopup : MonoBehaviour
{
    public enum TimeUnit { Hours, Minutes }

    [Header("Settings")]
    public Button targetButton;          // הכפתור ביוניטי
    public TMP_Text timerText;           // טקסט TMP שמציג את הזמן
    public TimeUnit timeUnit = TimeUnit.Hours; // לבחור אם זה שעות או דקות
    public int amount = 1;               // כמה שעות/דקות לספור
    public Image buttonImage;            // תמונה לשנות אחרי לחיצה
    public Sprite clickedSprite;         // תמונה חדשה אחרי לחיצה
    public GameObject popup;             // הפופ-אפ שיופיע אחרי ההמתנה

    private DateTime nextAvailableTime;
    private const string SaveKey = "CountdownButtonWithPopup_NextTime";

    private void Start()
    {
        // טוען את הזמן השמור אם יש
        if (PlayerPrefs.HasKey(SaveKey))
        {
            long savedBinary = Convert.ToInt64(PlayerPrefs.GetString(SaveKey));
            nextAvailableTime = DateTime.FromBinary(savedBinary);
        }
        else
        {
            nextAvailableTime = DateTime.Now;
        }

        targetButton.onClick.AddListener(OnButtonClick);

        // וודא שהפופ-אפ כבוי בהתחלה
        if (popup != null)
            popup.SetActive(false);
    }

    private void Update()
    {
        if (DateTime.Now >= nextAvailableTime)
        {
            targetButton.interactable = true;
        }
        else
        {
            targetButton.interactable = false;

            TimeSpan remaining = nextAvailableTime - DateTime.Now;

            timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                (int)remaining.TotalHours,
                remaining.Minutes,
                remaining.Seconds);
        }
    }

    private void OnButtonClick()
    {
        Debug.Log("כפתור נלחץ!");

        // מחשב את הזמן הבא לפי מה שהגדרת
        if (timeUnit == TimeUnit.Hours)
            nextAvailableTime = DateTime.Now.AddHours(amount);
        else
            nextAvailableTime = DateTime.Now.AddMinutes(amount);

        // שומר
        PlayerPrefs.SetString(SaveKey, nextAvailableTime.ToBinary().ToString());
        PlayerPrefs.Save();

        // משנה את התמונה אם קיים
        if (buttonImage != null && clickedSprite != null)
        {
            buttonImage.sprite = clickedSprite;
        }

        // מתחיל קבלת פופ-אפ אחרי שנייה
        if (popup != null)
        {
            StartCoroutine(ShowPopupDelayed());
        }
    }

    private IEnumerator ShowPopupDelayed()
    {
        yield return new WaitForSeconds(1f);
        popup.SetActive(true);
    }
}
