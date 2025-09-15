using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinPackageTile : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    private CoinPackage package;

    public void Setup(CoinPackage coinPackage)
    {
        package = coinPackage;

        if (icon) icon.sprite = package.icon;
        if (amountText) amountText.text = package.coinAmount.ToString() + " Coins";
        if (priceText) priceText.text = "$" + package.priceUSD.ToString("F2");

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            CoinShopManager.Instance.BuyPackage(package);
        });
    }
}
