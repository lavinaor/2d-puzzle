using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerFrameTile : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject selectedIcon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;

    [Header("Background Style")]
    [SerializeField] private bool useBgSprites = false;
    [SerializeField] private Sprite bgLocked;
    [SerializeField] private Sprite bgUnlocked;
    [SerializeField] private Sprite bgSelected;
    [SerializeField] private Color colLocked = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color colUnlocked = Color.white;
    [SerializeField] private Color colSelected = new Color(0.9f, 0.9f, 1f, 1f);

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.15f;

    private PlayerFrame frame;
    private int frameIndex;
    private int price;
    private Coroutine fadeCoroutine;

    public void Setup(PlayerFrame frameData, int index, bool unlocked)
    {
        frame = frameData;
        frameIndex = index;
        price = frameData.price;

        if (nameText != null)
            nameText.text = frame.frameName;

        if (icon != null)
        {
            icon.sprite = frame.frameSprite;
            icon.canvasRenderer.SetAlpha(1f);
        }

        ApplyStateVisuals();

        if (button == null)
        {
            Debug.LogError($"[PlayerFrameTile] Missing Button reference on {gameObject.name}");
            return;
        }

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
   
            PlayerImageManager.Instance.SelectFrame(frameIndex);

            FadeFlash();
            ApplyStateVisuals();
            FrameShopUI.Instance.RefreshAllTiles();
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
        Color highlight = baseColor * 1.2f;

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
        if (PlayerImageManager.Instance == null) return;

        bool unlocked = PlayerImageManager.Instance.IsFrameUnlocked(frameIndex);
        bool selected = PlayerImageManager.Instance.saveData.selectedFrameIndex == frameIndex;

        if (!unlocked)
        {
            SetBackground(bgLocked, colLocked);
            lockIcon?.SetActive(true);
            selectedIcon?.SetActive(false);
            if (priceText) priceText.text = price.ToString();
        }
        else if (selected)
        {
            SetBackground(bgSelected, colSelected);
            lockIcon?.SetActive(false);
            selectedIcon?.SetActive(true);
            if (priceText) priceText.text = "selected";
        }
        else
        {
            SetBackground(bgUnlocked, colUnlocked);
            lockIcon?.SetActive(false);
            selectedIcon?.SetActive(false);
            if (priceText) priceText.text = "select";
        }
    }

    private void SetBackground(Sprite sprite, Color color)
    {
        if (background == null) return;
        if (useBgSprites && sprite != null)
            background.sprite = sprite;
        else
            background.color = color;
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
