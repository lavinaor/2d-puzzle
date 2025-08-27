using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinTile : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Button button; // הכפתור שמפעיל את הפופאפ
    public Image icon;                 // התמונה הקטנה של הסקין
    public Image background;           // רקע הריבוע
    public GameObject lockIcon;        // אייקון מנעול
    public GameObject selectedIcon;    // אייקון “נבחר”
    public TMP_Text nameText;          // שם
    public TMP_Text priceText;         // מחיר

    [Header("Background (choose sprites OR colors)")]
    public bool useBgSprites = false;
    public Sprite bgLocked;
    public Sprite bgUnlocked;
    public Sprite bgSelected;
    public Color colLocked = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color colUnlocked = Color.white;
    public Color colSelected = new Color(0.9f, 0.9f, 1f, 1f);

    [Header("Animation")]
    public float frameDuration = 1.0f; // כמה זמן כל ספרייט מוצג
    public float fadeDuration = 0.15f; // כמה זמן לוקח מעבר רך

    private CandySkin skin;
    private int skinIndex;
    private int price;
    private int frame = 0;
    private float timer = 0f;
    private bool fading = false;

    public void Setup(CandySkin skinData, int index)
    {
        skin = skinData;
        skinIndex = index;
        price = skinData.price;

        if (nameText)
            nameText.text = skin.skinName;

        // ספרייט ראשון
        if (icon)
        {
            icon.canvasRenderer.SetAlpha(1f);
            if (skin.candyPairs != null && skin.candyPairs.Count > 0 && skin.candyPairs[0].sprite != null)
                icon.sprite = skin.candyPairs[0].sprite;
            else
                icon.sprite = null;
        }

        // מצב ויזואלי התחלתי
        ApplyStateVisuals();

        // כפתור לחיצה לפתיחת פופאפ
        if (button != null)
        {
            button.onClick.AddListener(() => { SkinShopUI.Instance.OpenPopup(skinIndex); });
        }
    }

    private void Update()
    {
        if (skin == null || icon == null || skin.candyPairs.Count == 0) return;

        timer += Time.unscaledDeltaTime; // חשוב! לא Time.deltaTime
        if (timer >= frameDuration && !fading)
        {
            timer = 0f;
            StartCoroutine(FadeToNextSprite());
        }
    }





    private IEnumerator FadeToNextSprite()
    {
        if (skin == null || icon == null || skin.candyPairs.Count == 0) yield break;

        fading = true;

        // בוחר את הספרייט הבא
        frame = (frame + 1) % skin.candyPairs.Count;
        Sprite nextSprite = skin.candyPairs[frame].sprite;

        if (nextSprite == null)
        {
            fading = false;
            yield break;
        }

        float elapsed = 0f;
        Color originalColor = icon.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Fade out
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            icon.color = Color.Lerp(originalColor, transparentColor, elapsed / fadeDuration);
            yield return null;
        }

        // מחליף ספרייט
        icon.sprite = nextSprite;

        // Fade in
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            icon.color = Color.Lerp(transparentColor, originalColor, elapsed / fadeDuration);
            yield return null;
        }

        icon.color = originalColor;
        fading = false;
    }




    /// <summary>
    /// עדכון מצב ויזואלי לפי אם הסקין נעול/קנוי/נבחר
    /// </summary>
    public void ApplyStateVisuals()
    {
        bool unlocked = CandySkinManager.Instance.IsSkinUnlocked(skinIndex);
        bool selected = CandySkinManager.Instance.selectedSkinIndex == skinIndex;

        if (!unlocked)
        {
            if (useBgSprites && bgLocked) background.sprite = bgLocked;
            else background.color = colLocked;

            if (lockIcon) lockIcon.SetActive(true);
            if (selectedIcon) selectedIcon.SetActive(false);
            if (priceText) priceText.text = price.ToString();
        }
        else if (selected)
        {
            if (useBgSprites && bgSelected) background.sprite = bgSelected;
            else background.color = colSelected;

            if (lockIcon) lockIcon.SetActive(false);
            if (selectedIcon) selectedIcon.SetActive(true);
            if (priceText) priceText.text = "selected";
        }
        else
        {
            if (useBgSprites && bgUnlocked) background.sprite = bgUnlocked;
            else background.color = colUnlocked;

            if (lockIcon) lockIcon.SetActive(false);
            if (selectedIcon) selectedIcon.SetActive(false);
            if (priceText) priceText.text = "select";
        }
    }
}
