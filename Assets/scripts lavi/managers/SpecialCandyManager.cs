using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpecialCandyManager : MonoBehaviour
{
    public static SpecialCandyManager Instance;

    [Header("All Available Candies")]
    public List<SpecialCandy> allCandies = new List<SpecialCandy>();

    [Header("Player Owned Candies")]
    public SpecialCandySaveData saveData = new SpecialCandySaveData();

    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "specialcandy_save.json");
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// בדיקה אם לשחקן יש ממתק לפי ה-ID שלו
    /// </summary>
    public bool HasCandy(string candyId)
    {
        return saveData.ownedCandies.Exists(c => c.candyId == candyId && c.amount > 0);
    }

    /// <summary>
    /// הוספת כמות מסוימת לממתק קיים או יצירת אחד חדש אם אין
    /// </summary>
    public void AddCandy(string candyId, int amountToAdd)
    {
        var candy = saveData.ownedCandies.Find(c => c.candyId == candyId);
        if (candy != null)
        {
            candy.amount += amountToAdd;
        }
        else
        {
            // חיפוש הממתק מתוך הרשימה הראשית
            var baseCandy = allCandies.Find(c => c.candyId == candyId);
            if (baseCandy != null)
            {
                SpecialCandy newCandy = new SpecialCandy
                {
                    candyId = baseCandy.candyId,
                    displayName = baseCandy.displayName,
                    icon = baseCandy.icon,
                    candyType = baseCandy.candyType,
                    price = baseCandy.price,
                    amount = amountToAdd
                };
                saveData.ownedCandies.Add(newCandy);
            }
            else
            {
                Debug.LogWarning("Candy ID not found in allCandies: " + candyId);
            }
        }
        SaveData();
    }

    public bool BuyCandy(string candyId)
    {
        var candy = allCandies.Find(c => c.candyId == candyId);
        if (candy == null)
        {
            Debug.LogWarning("Candy not found: " + candyId);
            return false;
        }

        // בודק שיש לשחקן מספיק מטבעות
        if (SaveManager.Instance.GetCoins() >= candy.price)
        {
            SaveManager.Instance.AddCoins(-candy.price);
            AddCandy(candyId, 1); // מוסיף 1 לשחקן
            return true;
        }
        else
        {
            Debug.Log("Not enough coins to buy " + candy.displayName);
            return false;
        }
    }

    /// <summary>
    /// הורדת כמות מהממתק, אם מגיע לאפס - נשאר ברשימה עם 0
    /// </summary>
    public void RemoveCandy(string candyId, int amountToRemove)
    {
        var candy = saveData.ownedCandies.Find(c => c.candyId == candyId);
        if (candy != null)
        {
            candy.amount = Mathf.Max(0, candy.amount - amountToRemove);
            SaveData();
        }
    }

    /// <summary>
    /// קבלת הכמות שיש לשחקן מממתק מסוים
    /// </summary>
    public int GetCandyAmount(string candyId)
    {
        var candy = saveData.ownedCandies.Find(c => c.candyId == candyId);
        return candy != null ? candy.amount : 0;
    }

    /// <summary>
    /// שמירת הנתונים לקובץ
    /// </summary>
    private void SaveData()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Special candies saved to " + saveFilePath);
    }

    /// <summary>
    /// טעינת הנתונים מהקובץ
    /// </summary>
    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<SpecialCandySaveData>(json);
        }
        else
        {
            saveData = new SpecialCandySaveData();
            SaveData();
        }
    }

    /// <summary>
    /// קבלת ספרייט לפי סוג הממתק (CandyType)
    /// </summary>
    public Sprite GetCandySpriteByType(CandyType type)
    {
        // קודם חפש אם יש לשחקן כזה ממתק
        foreach (var candy in saveData.ownedCandies)
        {
            if (candy.candyType == type && candy.amount > 0)
            {
                return candy.icon;
            }
        }

        // אם לא נמצא, חפש ברשימה הראשית
        foreach (var candy in allCandies)
        {
            if (candy.candyType == type)
            {
                return candy.icon;
            }
        }
        return null;
    }
}

[System.Serializable]
public class SpecialCandy
{
    public string candyId;      // מזהה ייחודי
    public string displayName;  // שם להצגה בחנות
    public Sprite icon;         // תמונה להצגה בחנות
    public CandyType candyType; // סוג הממתק במשחק
    public int amount;          // כמה יש לשחקן ממנו
    public int price;           // מחיר לרכישה
}

[System.Serializable]
public class SpecialCandySaveData
{
    public List<SpecialCandy> ownedCandies = new List<SpecialCandy>();
}
