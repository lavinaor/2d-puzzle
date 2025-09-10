using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class SimpleCoinClick : MonoBehaviour, IPointerClickHandler
{
    public Image targetImage;       // התמונה שתחליף ותיעלם
    public Sprite newSprite;        // התמונה החדשה
    public AudioSource audioSource; // מקור קול
    public AudioClip clickSound;    // הצליל להשמעה
    public TMP_Text coinsText;      // הטקסט שמראה את המטבעות
    public int addAmount = 25;      // כמה להוסיף בכל לחיצה
    public float fadeDuration = 0.3f; // זמן ההעלמה

    private static int currentCoins = 0; // סטטי כדי שכולם ישתפו את אותו מונה
    private const string PLAYER_PREFS_KEY = "Coins"; // מפתח לשמירה

    private void Awake()
    {
        // טען את הכמות ששמרנו קודם
        currentCoins = PlayerPrefs.GetInt(PLAYER_PREFS_KEY, 0);
        UpdateCoinsText();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // לנגן קול
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);

        // להחליף תמונה
        if (targetImage && newSprite)
            targetImage.sprite = newSprite;

        // להוסיף למונה
        currentCoins += addAmount;
        UpdateCoinsText();

        // שמירה ל־PlayerPrefs
        PlayerPrefs.SetInt(PLAYER_PREFS_KEY, currentCoins);
        PlayerPrefs.Save();

        // להעלים את התמונה עם אנימציה
        if (targetImage)
        {
            targetImage.transform.DOScale(0f, fadeDuration).SetEase(Ease.InBack);
            targetImage.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                targetImage.gameObject.SetActive(false);
            });
        }
    }

    void UpdateCoinsText()
    {
        if (coinsText)
            coinsText.text = currentCoins.ToString();
    }
}
