using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// מנג'ר לחנות מטבעות – כולל חבילות למכירה.
/// Singleton מבטיח שתמיד יהיה מופע אחד בלבד.
/// </summary>
public class CoinShopManager : MonoBehaviour
{
    public static CoinShopManager Instance { get; private set; }

    [Header("All Coin Packages")]
    public List<CoinPackage> allPackages = new List<CoinPackage>();

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // בדיקה אם יש חבילות מוגדרות
        if (allPackages == null)
            allPackages = new List<CoinPackage>();
    }

    /// <summary>
    /// רכישת חבילת מטבעות
    /// </summary>
    /// <param name="package"></param>
    public void BuyPackage(CoinPackage package)
    {
        if (package == null)
        {
            Debug.LogWarning("CoinShopManager: Tried to buy a null package!");
            return;
        }

        // כרגע סימולציה של רכישה
        Debug.Log($"Bought {package.coinAmount} coins for ${package.priceUSD:F2}");

        // מוסיף לשחקן
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.AddCoins(package.coinAmount);
        }
        else
        {
            Debug.LogWarning("CoinShopManager: SaveManager instance is null!");
        }

        // כאן אפשר להוסיף אנימציה/פידבק UI
    }
}

/// <summary>
/// מחלקה שמייצגת חבילת מטבעות
/// </summary>
[System.Serializable]
public class CoinPackage
{
    public string id;          // מזהה ייחודי
    public int coinAmount;     // כמה מטבעות מקבלים
    public float priceUSD;     // מחיר במטבע אמיתי
    public Sprite icon;        // אייקון חבילת מטבע
}
