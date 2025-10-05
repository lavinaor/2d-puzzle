using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class SimpleCoinClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject vfxPrefab;    // VFX שיופעל בלחיצה
    public AudioSource audioSource;  // מקור קול
    public AudioClip clickSound;     // הצליל להשמעה
    public TMP_Text coinsText;       // הטקסט שמראה את המטבעות
    public int addAmount = 25;       // כמה להוסיף בכל לחיצה
    public float vfxDuration = 1f;   // כמה זמן האפקט יישאר

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

        // להפעיל VFX
        if (vfxPrefab != null)
        {
            GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, vfxDuration);
        }

        // להוסיף למונה
        SaveManager.Instance.AddCoins(addAmount);
        UpdateCoinsText();

        // שמירה ל־PlayerPrefs
        PlayerPrefs.SetInt(PLAYER_PREFS_KEY, currentCoins);
        PlayerPrefs.Save();

        // להשבית את הכפתור לגמרי
        gameObject.SetActive(false);
    }

    void UpdateCoinsText()
    {
        if (coinsText)
            coinsText.text = currentCoins.ToString();
    }
}
