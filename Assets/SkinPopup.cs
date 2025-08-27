using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinPopup : MonoBehaviour
{
    public Image previewImage; // Preview גדול
    public TextMeshProUGUI skinNameText;
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;
    public GameObject lockIcon;

    private int skinIndex;
    private CandySkin skin;
    private int price;

    private int currentFrame = 0;
    private float timer = 0f;
    private float frameDuration = 1f;

    public void Show(int index, CandySkin data, int cost)
    {
        skinIndex = index;
        skin = data;
        price = cost;

        skinNameText.text = skin.skinName;

        Refresh();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (skin == null || skin.candyPairs.Count == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameDuration)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % skin.candyPairs.Count;
            previewImage.sprite = skin.candyPairs[currentFrame].sprite;
        }
    }

    private void Refresh()
    {
        bool unlocked = CandySkinManager.Instance.IsSkinUnlocked(skinIndex);
        bool selected = (CandySkinManager.Instance.selectedSkinIndex == skinIndex);

        actionButton.onClick.RemoveAllListeners();

        if (unlocked)
        {
            lockIcon.SetActive(false);
            if (selected)
            {
                actionButton.interactable = false;
                actionButtonText.text = "Selected";
            }
            else
            {
                actionButton.interactable = true;
                actionButtonText.text = "Select";
                actionButton.onClick.AddListener(() =>
                {
                    CandySkinManager.Instance.SelectSkin(skinIndex);
                    SkinShopUI.Instance.RefreshGrid();
                    Refresh();
                });
            }
        }
        else
        {
            lockIcon.SetActive(true);
            actionButton.interactable = SaveManager.Instance.GetCoins() >= price;
            actionButtonText.text = "Buy (" + price + ")";
            actionButton.onClick.AddListener(() =>
            {
                if (SaveManager.Instance.GetCoins() >= price)
                {
                    SaveManager.Instance.AddCoins(-price);
                    CandySkinManager.Instance.UnlockSkin(skinIndex);
                    CandySkinManager.Instance.SelectSkin(skinIndex);
                    SkinShopUI.Instance.RefreshGrid();
                    Refresh();
                }
            });
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
