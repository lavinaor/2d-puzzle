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
    /// ����� �� ����� �� ���� ��� �-ID ���
    /// </summary>
    public bool HasCandy(string candyId)
    {
        return saveData.ownedCandies.Exists(c => c.candyId == candyId && c.amount > 0);
    }

    /// <summary>
    /// ����� ���� ������ ����� ���� �� ����� ��� ��� �� ���
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
            // ����� ����� ���� ������ ������
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

        // ���� ��� ����� ����� ������
        if (SaveManager.Instance.GetCoins() >= candy.price)
        {
            SaveManager.Instance.AddCoins(-candy.price);
            AddCandy(candyId, 1); // ����� 1 �����
            return true;
        }
        else
        {
            Debug.Log("Not enough coins to buy " + candy.displayName);
            return false;
        }
    }

    /// <summary>
    /// ����� ���� ������, �� ���� ���� - ���� ������ �� 0
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
    /// ���� ����� ��� ����� ����� �����
    /// </summary>
    public int GetCandyAmount(string candyId)
    {
        var candy = saveData.ownedCandies.Find(c => c.candyId == candyId);
        return candy != null ? candy.amount : 0;
    }

    /// <summary>
    /// ����� ������� �����
    /// </summary>
    private void SaveData()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Special candies saved to " + saveFilePath);
    }

    /// <summary>
    /// ����� ������� ������
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
    /// ���� ������ ��� ��� ����� (CandyType)
    /// </summary>
    public Sprite GetCandySpriteByType(CandyType type)
    {
        // ���� ��� �� �� ����� ��� ����
        foreach (var candy in saveData.ownedCandies)
        {
            if (candy.candyType == type && candy.amount > 0)
            {
                return candy.icon;
            }
        }

        // �� �� ����, ��� ������ ������
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
    public string candyId;      // ���� ������
    public string displayName;  // �� ����� �����
    public Sprite icon;         // ����� ����� �����
    public CandyType candyType; // ��� ����� �����
    public int amount;          // ��� �� ����� ����
    public int price;           // ���� ������
}

[System.Serializable]
public class SpecialCandySaveData
{
    public List<SpecialCandy> ownedCandies = new List<SpecialCandy>();
}
