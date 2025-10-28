using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerPortraitTile : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Refs")]
    [SerializeField] private Button button;
    public Image icon;
    public Image background;
    public GameObject lockIcon;
    public GameObject selectedIcon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public TMP_Text buttonLabel;

    [Header("Background (choose sprites OR colors)")]
    public bool useBgSprites = false;
    public Sprite bgLocked;
    public Sprite bgUnlocked;
    public Sprite bgSelected;
    public Color colLocked = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color colUnlocked = Color.white;
    public Color colSelected = new Color(0.9f, 0.9f, 1f, 1f);

    [Header("Animation")]
    public float fadeDuration = 0.15f;

    private PlayerPortrait portrait;
    private int portraitIndex;
    private int price;
    private Coroutine fadeCoroutine;
    private bool isSetup = false;

    private void Awake()
    {
        if (button == null)
            button = GetComponentInChildren<Button>(true);

        if (button != null)
        {
            button.interactable = true;
            button.onClick.RemoveListener(PublicOnClick);
            button.onClick.AddListener(PublicOnClick);
        }
    }

    private void OnEnable()
    {
        PlayerImageEvents.OnImageChanged += ApplyStateVisuals;
    }

    private void OnDisable()
    {
        PlayerImageEvents.OnImageChanged -= ApplyStateVisuals;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    public void Setup(PlayerPortrait portraitData, int index, bool unlocked)
    {
        portrait = portraitData;
        portraitIndex = index;
        price = portraitData != null ? portraitData.price : 0;

        if (nameText && portrait != null)
            nameText.text = portrait.portraitName;

        if (icon && portrait != null)
        {
            icon.canvasRenderer.SetAlpha(1f);
            icon.sprite = portrait.portraitSprite;
        }

        ApplyStateVisuals();
        isSetup = true;
    }

    private void PublicOnClick()
    {
        HandleClick();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HandleClick();
    }

    private void HandleClick()
    {
        if (!isSetup || PlayerImageManager.Instance == null || SaveManager.Instance == null)
            return;

        bool unlocked = PlayerImageManager.Instance.IsPortraitUnlocked(portraitIndex);

        if (!unlocked)
        {
            int coins = SaveManager.Instance.GetCoins();
            if (coins >= price)
            {
                SaveManager.Instance.AddCoins(-price);
                PlayerImageManager.Instance.UnlockPortrait(portraitIndex);
                FadeFlash();
                PlayerImageManager.Instance.SelectPortrait(portraitIndex);
            }
            else
            {
                Debug.Log($"Not enough coins to buy portrait {portraitIndex}");
            }
        }
        else
        {
            PlayerImageManager.Instance.SelectPortrait(portraitIndex);
            FadeFlash();
        }
    }

    private void FadeFlash()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeEffect());
    }

    private IEnumerator FadeEffect()
    {
        if (icon == null) yield break;
        float elapsed = 0f;
        Color baseColor = icon.color;
        Color highlight = new Color(
            Mathf.Clamp01(baseColor.r + 0.2f),
            Mathf.Clamp01(baseColor.g + 0.2f),
            Mathf.Clamp01(baseColor.b + 0.2f)
        );

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            icon.color = Color.Lerp(baseColor, highlight, elapsed / fadeDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            icon.color = Color.Lerp(highlight, baseColor, elapsed / fadeDuration);
            yield return null;
        }

        icon.color = baseColor;
        fadeCoroutine = null;
    }

    public void ApplyStateVisuals()
    {
        if (PlayerImageManager.Instance == null)
            return;

        bool unlocked = PlayerImageManager.Instance.IsPortraitUnlocked(portraitIndex);
        bool selected = PlayerImageManager.Instance.saveData.selectedPortraitIndex == portraitIndex;

        if (!unlocked)
        {
            if (useBgSprites && bgLocked) background.sprite = bgLocked;
            else if (background) background.color = colLocked;

            lockIcon?.SetActive(true);
            selectedIcon?.SetActive(false);
            if (priceText) priceText.text = price.ToString();
            if (buttonLabel) buttonLabel.text = "Buy";
        }
        else if (selected)
        {
            if (useBgSprites && bgSelected) background.sprite = bgSelected;
            else if (background) background.color = colSelected;

            lockIcon?.SetActive(false);
            selectedIcon?.SetActive(true);
            if (priceText) priceText.text = "Selected";
            if (buttonLabel) buttonLabel.text = "Selected";
        }
        else
        {
            if (useBgSprites && bgUnlocked) background.sprite = bgUnlocked;
            else if (background) background.color = colUnlocked;

            lockIcon?.SetActive(false);
            selectedIcon?.SetActive(false);
            if (priceText) priceText.text = "Select";
            if (buttonLabel) buttonLabel.text = "Select";
        }
    }
}
