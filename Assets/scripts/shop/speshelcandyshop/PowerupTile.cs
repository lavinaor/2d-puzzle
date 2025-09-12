using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerupTile : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Image iconImage;          // התמונה של הממתק
    [SerializeField] private TextMeshProUGUI nameText; // שם הממתק
    [SerializeField] private TextMeshProUGUI amountText; // כמה יש לי
    [SerializeField] private Button buyButton;         // כפתור קנייה

    private SpecialCandyData data; // הנתונים של הממתק

    /// <summary>
    /// אתחול הטייל עם המידע
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
    /// עדכון הטקסט שמראה כמה יש
    /// </summary>
    private void UpdateAmountUI()
    {
        int amount = SaveManager.Instance.GetCandyAmount(data.id);
        if (amountText) amountText.text = $"x{amount}";
    }

    /// <summary>
    /// פעולה של קנייה
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
