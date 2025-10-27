using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerPortraitTile : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button selectButton;

    private int portraitIndex;
    private PlayerPortrait portraitData;
    private bool isUnlocked;

    public void Setup(PlayerPortrait portrait, int index, bool unlocked)
    {
        portraitData = portrait;
        portraitIndex = index;
        isUnlocked = unlocked;

        iconImage.sprite = portrait.portraitSprite;
        nameText.text = portrait.portraitName;
        priceText.text = portrait.price.ToString();

        buyButton.gameObject.SetActive(!unlocked);
        selectButton.gameObject.SetActive(unlocked);

        buyButton.onClick.RemoveAllListeners();
        selectButton.onClick.RemoveAllListeners();

        buyButton.onClick.AddListener(BuyPortrait);
        selectButton.onClick.AddListener(SelectPortrait);
    }

    private void BuyPortrait()
    {
        int coins = SaveManager.Instance.GetCoins();

        if (coins >= portraitData.price)
        {
            SaveManager.Instance.AddCoins(-portraitData.price);
            PlayerImageManager.Instance.UnlockPortrait(portraitIndex);

            isUnlocked = true;
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("אין מספיק מטבעות!");
        }
    }

    private void SelectPortrait()
    {
        PlayerImageManager.Instance.saveData.selectedPortraitIndex = portraitIndex;
        PlayerImageManager.Instance.SaveData();
    }
}
