using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // for IPointerClickHandler

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

    // small flag so we know if Setup was called
    private bool isSetup = false;

    private void Awake()
    {
        Debug.Log($"[PlayerPortraitTile] Awake on '{name}'");
        // try to auto-find button if not assigned
        if (button == null)
        {
            button = GetComponentInChildren<Button>(true);
            if (button != null)
                Debug.Log($"[PlayerPortraitTile] Auto-found Button on '{name}' -> {button.name}");
            else
                Debug.LogWarning($"[PlayerPortraitTile] Button NOT assigned and not found in children of '{name}'");
        }

        // attach safety listener if button exists (this ensures the listener exists even if Setup is forgotten)
        if (button != null)
        {
            // ensure interactable
            button.interactable = true;

            // ensure clicking the button triggers the onboard handler (PublicOnClick)
            button.onClick.RemoveListener(PublicOnClick); // remove only this listener to avoid wiping others
            button.onClick.AddListener(PublicOnClick);
        }

        // quick runtime checks
        if (EventSystem.current == null)
        {
            Debug.LogWarning("[PlayerPortraitTile] NO EventSystem found in scene! UI clicks will NOT work without an EventSystem.");
        }

        // Check for GraphicRaycaster on canvas root
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogWarning($"[PlayerPortraitTile] No parent Canvas found for '{name}'.");
        else
        {
            var gr = canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (gr == null) Debug.LogWarning($"[PlayerPortraitTile] Canvas '{canvas.name}' missing GraphicRaycaster.");
        }
    }

    private void OnEnable()
    {
        // Useful to see when object becomes active
        Debug.Log($"[PlayerPortraitTile] OnEnable '{name}' - isSetup={isSetup}");
    }

    /// <summary>
    /// Call this to initialize the tile. (Same signature as before)
    /// </summary>
    public void Setup(PlayerPortrait portraitData, int index, bool unlocked)
    {
        Debug.Log($"[PlayerPortraitTile] Setup called for '{name}' index={index} unlocked={unlocked}");
        // ensure button exists
        if (button == null)
        {
            button = GetComponentInChildren<Button>(true);
            if (button == null)
            {
                Debug.LogError($"[PlayerPortraitTile] Setup: No Button found in '{name}'. Aborting Setup.");
                return;
            }
            else
            {
                Debug.Log($"[PlayerPortraitTile] Setup: Auto-found Button '{button.name}'");
            }
        }

        // attach listener safely: remove only our PublicOnClick then re-add
        button.onClick.RemoveListener(PublicOnClick);
        button.onClick.AddListener(PublicOnClick);
        button.interactable = true;

        // store data
        portrait = portraitData;
        portraitIndex = index;
        price = portraitData != null ? portraitData.price : 0;

        // fill visuals
        if (nameText && portrait != null)
            nameText.text = portrait.portraitName;

        if (icon && portrait != null)
        {
            icon.canvasRenderer.SetAlpha(1f);
            icon.sprite = portrait.portraitSprite;
        }

        // apply visuals and mark setup
        ApplyStateVisuals();
        isSetup = true;

        // debug
        Debug.Log($"[PlayerPortraitTile] Setup finished for '{name}' (portrait {(portrait != null ? portrait.portraitName : "null")})");
    }

    // This is the central click handler called by button and by pointer handler
    private void PublicOnClick()
    {
        // call the same logic as before
        HandleClick();
    }

    // IPointerClickHandler fallback: will be called if EventSystem receives pointer events
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[PlayerPortraitTile] OnPointerClick on '{name}', pointer button {eventData.button}");
        HandleClick();
    }

    // centralized handling logic
    private void HandleClick()
    {
        if (!isSetup)
        {
            Debug.LogWarning($"[PlayerPortraitTile] HandleClick called but tile NOT setup yet for '{name}'");
            // still attempt minimal behavior so we can see logs
        }

        // safety checks for managers
        if (PlayerImageManager.Instance == null)
        {
            Debug.LogError("[PlayerPortraitTile] PlayerImageManager.Instance is NULL!");
            return;
        }
        if (SaveManager.Instance == null)
        {
            Debug.LogError("[PlayerPortraitTile] SaveManager.Instance is NULL!");
            return;
        }

        Debug.Log($"[PlayerPortraitTile] Clicked portraitIndex={portraitIndex}");

        bool unlocked = PlayerImageManager.Instance.IsPortraitUnlocked(portraitIndex);

        if (!unlocked)
        {
            int coins = SaveManager.Instance.GetCoins();
            if (coins >= price)
            {
                SaveManager.Instance.AddCoins(-price);
                PlayerImageManager.Instance.UnlockPortrait(portraitIndex);
                FadeFlash();
                ApplyStateVisuals();
                Debug.Log($"[PlayerPortraitTile] Bought portrait {portraitIndex}");
            }
            else
            {
                Debug.Log($"[PlayerPortraitTile] Not enough coins to buy portrait {portraitIndex}. Have {coins}, price {price}");
            }
        }
        else
        {
            PlayerImageManager.Instance.saveData.selectedPortraitIndex = portraitIndex;
            PlayerImageManager.Instance.SaveData();
            FadeFlash();
            ApplyStateVisuals();
            Debug.Log($"[PlayerPortraitTile] Selected portrait {portraitIndex}");
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
        bool unlocked = PlayerImageManager.Instance != null && PlayerImageManager.Instance.IsPortraitUnlocked(portraitIndex);
        bool selected = PlayerImageManager.Instance != null && PlayerImageManager.Instance.saveData.selectedPortraitIndex == portraitIndex;

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

    private void OnDisable()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (icon != null)
        {
            Color c = icon.color;
            c.a = 1f;
            icon.color = c;
        }
    }
}
