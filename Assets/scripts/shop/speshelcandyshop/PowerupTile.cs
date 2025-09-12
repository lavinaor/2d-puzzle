using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerupTile : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Image iconImage;          // ������ �� �����
    [SerializeField] private TextMeshProUGUI nameText; // �� �����
    [SerializeField] private TextMeshProUGUI amountText; // ��� �� ��
    [SerializeField] private Button buyButton;         // ����� �����

    private SpecialCandyData data; // ������� �� �����

    /// <summary>
    /// ����� ����� �� �����
    /// </summary>
    public void Setup(SpecialCandyData candyData)
    {
        data = candyData;

        if (iconImage) iconImage.sprite = data.icon;
        if (nameText) nameText.text = data.displayName;
        UpdateAmountUI();

        if (buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(BuyOne);
        }
    }

    /// <summary>
    /// ����� ����� ����� ��� ��
    /// </summary>
    private void UpdateAmountUI()
    {
        int amount = SaveManager.Instance.GetCandyAmount(data.id);
        if (amountText) amountText.text = $"x{amount}";
    }

    /// <summary>
    /// ����� �� �����
    /// </summary>
    private void BuyOne()
    {
        if (SaveManager.Instance.GetCoins() >= data.price)
        {
            SaveManager.Instance.AddCoins(-data.price);
            SaveManager.Instance.AddCandy(data.id, 1);
            UpdateAmountUI();
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }
}
