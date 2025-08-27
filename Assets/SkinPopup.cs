using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinPopup : MonoBehaviour
{
    public static SkinPopup Instance;

    [Header("UI Refs")]
    [SerializeField] private Image background;           // רקע הפופאפ
    [SerializeField] private Image previewImage;         // התצוגה הגדולה
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
    [SerializeField] private float frameDuration = 1.0f; // כמה זמן כל ספרייט על המסך
    [SerializeField] private float fadeDuration = 0.15f; // משך מעבר רך

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

    /// <summary>
    /// מזריק נתונים לפני פתיחת הפופאפּ (לא מדליק אותו!)
    /// </summary>
    public void Prepare(int index, CandySkin data, int cost)
    {
        skinIndex = index;
        skin = data;
        price = cost;

        if (skinNameText) skinNameText.text = skin != null ? skin.skinName : string.Empty;

        // לאתחל תצוגת ספרייט ראשונה
        frame = 0;
        timer = 0f;
        if (previewImage && skin != null && skin.candyPairs != null && skin.candyPairs.Count > 0)
            previewImage.sprite = skin.candyPairs[0].sprite;

        ApplyStateVisuals();
    }

    private void OnEnable()
    {
        // כשנפתח ע"י המנגר – לאתחל שוב כדי להבטיח סטייט תקין
        frame = 0;
        timer = 0f;
        if (previewImage && skin != null && skin.candyPairs != null && skin.candyPairs.Count > 0)
            previewImage.sprite = skin.candyPairs[0].sprite;
    }

    private void Update()
    {
        if (skin == null || previewImage == null) return;
        var list = skin.candyPairs;
        if (list == null || list.Count == 0) return;

        timer += Time.unscaledDeltaTime; // זמן "אמיתי" גם כשה־timeScale=0
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

        // הספרייט הבא
        frame = (frame + 1) % skin.candyPairs.Count;
        Sprite nextSprite = skin.candyPairs[frame].sprite;
        if (nextSprite == null)
        {
            fading = false;
            yield break;
        }

        // Fade Out ידני
        float t = 0f;
        Color start = previewImage.color;
        Color transparent = new Color(start.r, start.g, start.b, 0f);
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);
            previewImage.color = Color.Lerp(start, transparent, a);
            yield return null;
        }

        // החלפת ספרייט
        previewImage.sprite = nextSprite;

        // Fade In ידני
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);
            previewImage.color = Color.Lerp(transparent, start, a);
            yield return null;
        }
        previewImage.color = start;

        fading = false;
    }

    /// <summary>
    /// רקע + טקסטים + כפתור פעולה לפי מצב (נעול/נקנה/נבחר)
    /// </summary>
    private void ApplyStateVisuals()
    {
        if (skin == null) return;

        bool unlocked = CandySkinManager.Instance.IsSkinUnlocked(skinIndex);
        bool selected = (CandySkinManager.Instance.selectedSkinIndex == skinIndex);

        // רקע לפי ספרייטים או צבעים
        if (background)
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

        // מנעול
        if (lockIcon) lockIcon.SetActive(!unlocked);

        // מחיר/כפתור
        actionButton.onClick.RemoveAllListeners();

        if (!unlocked)
        {
            if (priceText)
            {
                priceText.gameObject.SetActive(true);
                priceText.text = $"price: {price}";
            }

            if (actionButtonText) actionButtonText.text = "buy";
            if (actionButton) actionButton.interactable = SaveManager.Instance.GetCoins() >= price;

            actionButton.onClick.AddListener(() =>
            {
                if (SaveManager.Instance.GetCoins() >= price)
                {
                    SaveManager.Instance.AddCoins(-price);
                    CandySkinManager.Instance.UnlockSkin(skinIndex);
                    CandySkinManager.Instance.SelectSkin(skinIndex); // אפשרי: מייד מצייד אחרי קניה
                    SkinShopUI.Instance.RefreshGrid();
                    ApplyStateVisuals();
                }
            });
        }
        else
        {
            // כבר נקנה
            if (priceText) priceText.gameObject.SetActive(false);

            if (selected)
            {
                // מצויד כרגע
                if (actionButtonText) actionButtonText.text = "select";
                if (actionButton) actionButton.interactable = false;
            }
            else
            {
                // אפשר לצייד
                if (actionButtonText) actionButtonText.text = "selected";
                if (actionButton) actionButton.interactable = true;

                actionButton.onClick.AddListener(() =>
                {
                    CandySkinManager.Instance.SelectSkin(skinIndex);
                    SkinShopUI.Instance.RefreshGrid();
                    ApplyStateVisuals();
                });
            }
        }
    }

    public void Close()
    {
        // סוגר דרך המנגר (כדי לשמור קונסיסטנטיות)
        PopUpManger.Instance.ChangeUIState((int)PopUpType.NoneUI);
    }
}
