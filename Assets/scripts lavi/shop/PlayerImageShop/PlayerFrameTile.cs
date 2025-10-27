using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerFrameTile : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Button button;
    public Image icon;
    public Image background;
    public GameObject lockIcon;
    public GameObject selectedIcon;
    public TMP_Text nameText;
    public TMP_Text priceText;

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

    private PlayerFrame frame;
    private int frameIndex;
    private int price;
    private Coroutine fadeCoroutine;

    public void Setup(PlayerFrame frameData, int index, bool unlocked)
    {
        frame = frameData;
        frameIndex = index;
        price = frameData.price;

        if (nameText)
            nameText.text = frame.frameName;

        if (icon)
        {
            icon.canvasRenderer.SetAlpha(1f);
            icon.sprite = frame.frameSprite;
        }

        ApplyStateVisuals();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnTileClick);
    }

    private void OnTileClick()
    {
        bool unlocked = PlayerImageManager.Instance.IsFrameUnlocked(frameIndex);

        if (!unlocked)
        {
            int coins = SaveManager.Instance.GetCoins();
            if (coins >= price)
            {
                SaveManager.Instance.AddCoins(-price);
                PlayerImageManager.Instance.UnlockFrame(frameIndex);
                FadeFlash();
                ApplyStateVisuals();
            }
            else
            {
                Debug.Log("אין מספיק מטבעות!");
            }
        }
        else
        {
            PlayerImageManager.Instance.saveData.selectedFrameIndex = frameIndex;
            PlayerImageManager.Instance.SaveData();
            FadeFlash();
            ApplyStateVisuals();
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
        Color highlight = new Color(baseColor.r + 0.2f, baseColor.g + 0.2f, baseColor.b + 0.2f);

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
        bool unlocked = PlayerImageManager.Instance.IsFrameUnlocked(frameIndex);
        bool selected = PlayerImageManager.Instance.saveData.selectedFrameIndex == frameIndex;

        if (!unlocked)
        {
            if (useBgSprites && bgLocked) background.sprite = bgLocked;
            else background.color = colLocked;

            lockIcon?.SetActive(true);
            selectedIcon?.SetActive(false);
            priceText.text = price.ToString();
        }
        else if (selected)
        {
            if (useBgSprites && bgSelected) background.sprite = bgSelected;
            else background.color = colSelected;

            lockIcon?.SetActive(false);
            selectedIcon?.SetActive(true);
            priceText.text = "selected";
        }
        else
        {
            if (useBgSprites && bgUnlocked) background.sprite = bgUnlocked;
            else background.color = colUnlocked;

            lockIcon?.SetActive(false);
            selectedIcon?.SetActive(false);
            priceText.text = "select";
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
