using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���'� ����� ������ � ���� ������ ������.
/// Singleton ����� ����� ���� ���� ��� ����.
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

        // ����� �� �� ������ �������
        if (allPackages == null)
            allPackages = new List<CoinPackage>();
    }

    /// <summary>
    /// ����� ����� ������
    /// </summary>
    /// <param name="package"></param>
    public void BuyPackage(CoinPackage package)
    {
        if (package == null)
        {
            Debug.LogWarning("CoinShopManager: Tried to buy a null package!");
            return;
        }

        // ���� �������� �� �����
        Debug.Log($"Bought {package.coinAmount} coins for ${package.priceUSD:F2}");

        // ����� �����
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.AddCoins(package.coinAmount);
        }
        else
        {
            Debug.LogWarning("CoinShopManager: SaveManager instance is null!");
        }

        // ��� ���� ������ �������/����� UI
    }
}

/// <summary>
/// ����� ������� ����� ������
/// </summary>
[System.Serializable]
public class CoinPackage
{
    public string id;          // ���� ������
    public int coinAmount;     // ��� ������ ������
    public float priceUSD;     // ���� ����� �����
    public Sprite icon;        // ������ ����� ����
}
