using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerFrameTile : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button selectButton;

    private int frameIndex;
    private PlayerFrame frameData;
    private bool isUnlocked;

    public void Setup(PlayerFrame frame, int index, bool unlocked)
    {
        frameData = frame;
        frameIndex = index;
        isUnlocked = unlocked;

        iconImage.sprite = frame.frameSprite;
        nameText.text = frame.frameName;
        priceText.text = frame.price.ToString();

        buyButton.gameObject.SetActive(!unlocked);
        selectButton.gameObject.SetActive(unlocked);

        buyButton.onClick.RemoveAllListeners();
        selectButton.onClick.RemoveAllListeners();

        buyButton.onClick.AddListener(BuyFrame);
        selectButton.onClick.AddListener(SelectFrame);
    }

    private void BuyFrame()
    {
        int coins = SaveManager.Instance.GetCoins();

        if (coins >= frameData.price)
        {
            // מחסיר את המחיר
            SaveManager.Instance.AddCoins(-frameData.price);

            // מוסיף לרשימת הפתוחים
            PlayerImageManager.Instance.UnlockFrame(frameIndex);

            isUnlocked = true;
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("אין מספיק מטבעות!");
        }
    }

    private void SelectFrame()
    {
        PlayerImageManager.Instance.saveData.selectedFrameIndex = frameIndex;
        PlayerImageManager.Instance.SaveData();
    }
}
