using UnityEngine;
using UnityEngine.UI;
using TMPro; // בשביל TextMeshProUGUI

public class ImageSelector : MonoBehaviour
{
    public Image displayImage;        // התמונה המקורית
    public Sprite[] availableSprites; // רשימת התמונות האפשריות

    [Header("תוספות חדשות")]
    public TextMeshProUGUI selectionText; // גרור לכאן טקסט להצגת האינדקס
    public Image extraDisplayImage;       // גרור לכאן את ה-Image הנוסף (למשל תצוגה חיצונית)

    // פונקציה שתבחר תמונה לפי אינדקס
    public void SelectImage(int index)
    {
        if (index >= 0 && index < availableSprites.Length)
        {
            // מציג בתמונה המקורית
            displayImage.sprite = availableSprites[index];

            // מציג גם בתמונה הנוספת (אם הוגדר)
            if (extraDisplayImage != null)
                extraDisplayImage.sprite = availableSprites[index];

            // מעדכן טקסט חיצוני אם קיים
            if (selectionText != null)
                selectionText.text = index.ToString();
        }
        else
        {
            Debug.LogWarning("אינדקס מחוץ לטווח!");
        }
    }

    // פונקציה חדשה - רכישת פריט לפי אינדקס ומחיר
    public void BuyItem(int index, int price)
    {
        if (index < 0 || index >= availableSprites.Length)
        {
            Debug.LogWarning("אינדקס מחוץ לטווח!");
            return;
        }

        // בודק אם יש מספיק מטבעות
        if (SaveManager.Instance.GetCoins () >= price)
        {
            // מוריד מטבעות
            SaveManager.Instance.AddCoins(-price);

            // בוחר את הפריט שנרכש
            SelectImage(index);

            Debug.Log($"נרכש פריט מספר {index} במחיר {price} מטבעות.");
        }
        else
        {
            Debug.Log("אין מספיק מטבעות לרכישה!");
        }
    }
}
