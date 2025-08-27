using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinPopup : MonoBehaviour
{
    public static SkinPopup Instance;

    [Header("UI Refs")]
    [SerializeField] private Image background;           // ��� ������
    [SerializeField] private Image previewImage;         // ������ ������
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
    [SerializeField] private float frameDuration = 1.0f; // ��� ��� �� ������ �� ����
    [SerializeField] private float fadeDuration = 0.15f; // ��� ���� ��

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
    /// ����� ������ ���� ����� ������� (�� ����� ����!)
    /// </summary>
    public void Prepare(int index, CandySkin data, int cost)
    {
        skinIndex = index;
        skin = data;
        price = cost;

        if (skinNameText) skinNameText.text = skin != null ? skin.skinName : string.Empty;

        // ����� ����� ������ ������
        frame = 0;
        timer = 0f;
        if (previewImage && skin != null && skin.candyPairs != null && skin.candyPairs.Count > 0)
            previewImage.sprite = skin.candyPairs[0].sprite;

        ApplyStateVisuals();
    }

    private void OnEnable()
    {
        // ������ �"� ����� � ����� ��� ��� ������ ����� ����
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

        timer += Time.unscaledDeltaTime; // ��� "�����" �� ����timeScale=0
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

        // ������� ���
        frame = (frame + 1) % skin.candyPairs.Count;
        Sprite nextSprite = skin.candyPairs[frame].sprite;
        if (nextSprite == null)
        {
            fading = false;
            yield break;
        }

        // Fade Out ����
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

        // ����� ������
        previewImage.sprite = nextSprite;

        // Fade In ����
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
    /// ��� + ������ + ����� ����� ��� ��� (����/����/����)
    /// </summary>
    private void ApplyStateVisuals()
    {
        if (skin == null) return;

        bool unlocked = CandySkinManager.Instance.IsSkinUnlocked(skinIndex);
        bool selected = (CandySkinManager.Instance.selectedSkinIndex == skinIndex);

        // ��� ��� �������� �� �����
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

        // �����
        if (lockIcon) lockIcon.SetActive(!unlocked);

        // ����/�����
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
                    CandySkinManager.Instance.SelectSkin(skinIndex); // �����: ���� ����� ���� ����
                    SkinShopUI.Instance.RefreshGrid();
                    ApplyStateVisuals();
                }
            });
        }
        else
        {
            // ��� ����
            if (priceText) priceText.gameObject.SetActive(false);

            if (selected)
            {
                // ����� ����
                if (actionButtonText) actionButtonText.text = "select";
                if (actionButton) actionButton.interactable = false;
            }
            else
            {
                // ���� �����
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
        // ���� ��� ����� (��� ����� ������������)
        PopUpManger.Instance.ChangeUIState((int)PopUpType.NoneUI);
    }
}
