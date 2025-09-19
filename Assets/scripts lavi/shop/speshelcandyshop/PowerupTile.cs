using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerupTile : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Button buyButton;

    private SpecialCandy currentCandy;

    public void Setup(SpecialCandy candyData, int playerAmount)
    {
        currentCandy = candyData;

        iconImage.sprite = candyData.icon;
        nameText.text = candyData.displayName;
        priceText.text = candyData.price.ToString();
        amountText.text = playerAmount.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(BuyCandy);
    }

    public void SetBuyButtonVisible(bool visible)
    {
        if (buyButton != null)
            buyButton.gameObject.SetActive(visible);
    }

    private void BuyCandy()
    {
        bool success = SpecialCandyManager.Instance.BuyCandy(currentCandy.candyId);

        if (success)
        {
            int newAmount = SpecialCandyManager.Instance.GetCandyAmount(currentCandy.candyId);
            amountText.text = newAmount.ToString();
        }
    }
}
