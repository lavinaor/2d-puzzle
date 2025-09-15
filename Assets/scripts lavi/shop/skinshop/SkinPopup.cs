using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinPopup : MonoBehaviour
{
    public static SkinPopup Instance;

    [Header("UI Refs")]
    [SerializeField] private Image background;
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI skinNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private GameObject lockIcon;

    [Header("Background (choose sprites OR colors)")]
    [SerializeField] private bool useBgSprites = false;
    [SerializeField] private Sprite bgLocked;
    [SerializeField] private Sprite bgUnlocked;
    [SerializeField] private Sprite bgSelected;
    [SerializeField] private Color colLocked = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color colUnlocked = Color.white;
    [SerializeField] private Color colSelected = new Color(0.9f, 0.9f, 1f, 1f);

    [Header("Animation")]
    [SerializeField] private float frameDuration = 1.0f;
    [SerializeField] private float fadeDuration = 0.15f;

    private int skinIndex;
    private CandySkin skin;
    private int price;

    private int frame = 0;
    private float timer = 0f;
    private bool fading = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Prepare(int index, CandySkin data, int cost)
    {
        skinIndex = index;
        skin = data;
        price = cost;

        if (skinNameText) skinNameText.text = skin != null ? skin.skinName : string.Empty;

        // אתחול אנימציה
        frame = 0;
        timer = 0f;

        if (previewImage != null && skin != null && skin.candyPairs != null && skin.candyPairs.Count > 0)
        {
            previewImage.sprite = skin.candyPairs[0].sprite;
            previewImage.color = Color.white; // חשוב: צבע מלא כדי לא להיתקע חצי מטושטש
        }

        ApplyStateVisuals();
    }

    private void OnEnable()
    {
        // אתחול מחדש של האנימציה בכל כניסה לפופאפ
        frame = 0;
        timer = 0f;
        if (previewImage != null && skin != null && skin.candyPairs != null && skin.candyPairs.Count > 0)
        {
            previewImage.sprite = skin.candyPairs[0].sprite;
            previewImage.color = Color.white; // צבע מלא
        }
    }

    private void Update()
    {
        if (skin == null || previewImage == null || skin.candyPairs == null || skin.candyPairs.Count == 0)
            return;

        timer += Time.unscaledDeltaTime;
        if (timer >= frameDuration && !fading)
        {
            timer = 0f;
            StartCoroutine(FadeToNextSprite());
        }
    }

    private IEnumerator FadeToNextSprite()
    {
        if (skin == null || previewImage == null || skin.candyPairs == null || skin.candyPairs.Count == 0)
            yield break;

        fading = true;

        frame = (frame + 1) % skin.candyPairs.Count;
        Sprite nextSprite = skin.candyPairs[frame].sprite;

        if (nextSprite == null)
        {
            fading = false;
            yield break;
        }

        float t = 0f;
        Color start = previewImage.color;
        Color transparent = new Color(start.r, start.g, start.b, 0f);

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            previewImage.color = Color.Lerp(start, transparent, t / fadeDuration);
            yield return null;
        }

        previewImage.sprite = nextSprite;

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            previewImage.color = Color.Lerp(transparent, start, t / fadeDuration);
            yield return null;
        }

        previewImage.color = start;
        fading = false;
    }

    private void ApplyStateVisuals()
    {
        if (skin == null || CandySkinManager.Instance == null)
            return;

        bool unlocked = CandySkinManager.Instance.IsSkinUnlocked(skinIndex);
        bool selected = CandySkinManager.Instance.selectedSkinIndex == skinIndex;

        if (background != null)
        {
            if (useBgSprites)
            {
                if (!unlocked && bgLocked) background.sprite = bgLocked;
                else if (selected && bgSelected) background.sprite = bgSelected;
                else if (bgUnlocked) background.sprite = bgUnlocked;
            }
            else
            {
                if (!unlocked) background.color = colLocked;
                else if (selected) background.color = colSelected;
                else background.color = colUnlocked;
            }
        }

        if (lockIcon != null) lockIcon.SetActive(!unlocked);

        if (actionButton == null || actionButtonText == null) return;

        actionButton.onClick.RemoveAllListeners();

        if (!unlocked)
        {
            if (priceText)
            {
                priceText.gameObject.SetActive(true);
                priceText.text = $"price: {price}";
            }

            actionButtonText.text = "buy";
            actionButton.interactable = SaveManager.Instance != null && SaveManager.Instance.GetCoins() >= price;

            actionButton.onClick.AddListener(() =>
            {
                if (SaveManager.Instance != null && SaveManager.Instance.GetCoins() >= price)
                {
                    SaveManager.Instance.AddCoins(-price);
                    CandySkinManager.Instance.UnlockSkin(skinIndex);
                    CandySkinManager.Instance.SelectSkin(skinIndex);
                    SkinShopUI.Instance?.RefreshGrid();
                    ApplyStateVisuals();
                }
            });
        }
        else
        {
            if (priceText) priceText.gameObject.SetActive(false);

            if (selected)
            {
                actionButtonText.text = "selected";
                actionButton.interactable = false;
            }
            else
            {
                actionButtonText.text = "select";
                actionButton.interactable = true;

                actionButton.onClick.AddListener(() =>
                {
                    CandySkinManager.Instance.SelectSkin(skinIndex);
                    SkinShopUI.Instance?.RefreshGrid();
                    ApplyStateVisuals();
                });
            }
        }
    }

    public void Close()
    {
        PopUpManger.Instance?.ChangeUIState((int)PopUpType.NoneUI);
    }
}
