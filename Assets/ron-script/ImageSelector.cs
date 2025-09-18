using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    public Image displayImage;       // ה-Image שבו תוצג הבחירה
    public Sprite[] availableSprites; // רשימת התמונות האפשריות

    // פונקציה שתבחר תמונה לפי אינדקס
    public void SelectImage(int index)
    {
        if (index >= 0 && index < availableSprites.Length)
        {
            displayImage.sprite = availableSprites[index];
        }
        else
        {
            Debug.LogWarning("אינדקס מחוץ לטווח!");
        }
    }
}
