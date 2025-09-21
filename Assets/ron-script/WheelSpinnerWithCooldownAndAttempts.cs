using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class WheelSegment
{
    public string prizeName;
    public Sprite prizeImage;
}

public class WheelSpinnerWithCooldownAndAttempts : MonoBehaviour
{
    [Header("גלגל")]
    public int segments = 10;
    public float spinDuration = 3f;
    public int minFullSpins = 3;
    public int maxFullSpins = 6;

    [Header("סאונד")]
    public AudioSource spinAudio;

    [Header("אנימציה של לחיצה")]
    public Animator buttonAnimator;

    [Header("פלחים ופרסים")]
    public WheelSegment[] segmentPrizes;

    [Header("UI להצגת פרס")]
    public Image prizeDisplayImage;
    public TMP_Text prizeDisplayText;

    [Header("Cooldown")]
    public float spinCooldown = 1f;
    public CooldownUnit cooldownUnit = CooldownUnit.Seconds;
    public TMP_Text cooldownText;

    [Header("נסיונות לפני קולדאון")]
    public int maxSpinsBeforeCooldown = 3;
    public TMP_Text spinsLeftText;

    [Header("אנימציית פרס")]
    public float prizePopScale = 1.3f;
    public float prizePopDuration = 0.25f;

    private bool canSpin = true;
    private bool isSpinning = false;
    private int spinsLeft;

    // משתנה חדש למעקב אחרי הסיבוב הנוכחי
    private int currentSpinIndex = 0;

    public enum CooldownUnit
    {
        Seconds,
        Minutes,
        Hours
    }

    private void Start()
    {
        spinsLeft = maxSpinsBeforeCooldown;
        UpdateSpinsLeftText();
    }

    public void SpinWheel()
    {
        if (!isSpinning && canSpin)
        {
            StartCoroutine(SpinCoroutine());
        }
    }

    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;
        spinsLeft--;
        UpdateSpinsLeftText();

        if (spinAudio != null) spinAudio.Play();
        if (buttonAnimator != null) buttonAnimator.SetTrigger("Pressed");

        // שימוש בסיבוב הנוכחי במקום רנדום
        int targetSegment = currentSpinIndex % segments;

        float segmentAngle = 360f / segments;
        float totalRotation = 360f * Random.Range(minFullSpins, maxFullSpins) + targetSegment * segmentAngle;

        float elapsed = 0f;
        float startZ = transform.eulerAngles.z;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinDuration;
            float easedT = 1 - Mathf.Pow(1 - t, 3);
            float currentRotation = Mathf.Lerp(0, totalRotation, easedT);
            transform.rotation = Quaternion.Euler(0, 0, startZ - currentRotation);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, startZ - totalRotation);

        if (segmentPrizes != null && segmentPrizes.Length == segments)
        {
            WheelSegment prize = segmentPrizes[targetSegment];

            if (prizeDisplayImage != null)
                prizeDisplayImage.sprite = prize.prizeImage;

            if (prizeDisplayText != null)
            {
                prizeDisplayText.text = prize.prizeName;
                StartCoroutine(AnimatePrizeText(prizeDisplayText.transform));
            }
        }

        // עדכון הסיבוב הבא
        currentSpinIndex++;

        isSpinning = false;

        if (spinsLeft <= 0)
        {
            canSpin = false;
            float cooldownTime = GetCooldownInSeconds();
            if (cooldownText != null)
                cooldownText.gameObject.SetActive(true);

            yield return StartCoroutine(CooldownCoroutine(cooldownTime));

            canSpin = true;
            spinsLeft = maxSpinsBeforeCooldown;
            UpdateSpinsLeftText();

            if (cooldownText != null)
                cooldownText.gameObject.SetActive(false);
        }
    }

    private IEnumerator CooldownCoroutine(float duration)
    {
        float remaining = duration;

        while (remaining > 0)
        {
            if (cooldownText != null)
            {
                int displayTime = Mathf.CeilToInt(remaining);
                cooldownText.text = $"waiting {displayTime} seconds";
            }

            remaining -= Time.deltaTime;
            yield return null;
        }
    }

    private float GetCooldownInSeconds()
    {
        switch (cooldownUnit)
        {
            case CooldownUnit.Minutes:
                return spinCooldown * 60f;
            case CooldownUnit.Hours:
                return spinCooldown * 3600f;
            default:
                return spinCooldown;
        }
    }

    private void UpdateSpinsLeftText()
    {
        if (spinsLeftText != null)
        {
            spinsLeftText.text = $"  More  {spinsLeft} rounds";
        }
    }

    private IEnumerator AnimatePrizeText(Transform textTransform)
    {
        Vector3 originalScale = textTransform.localScale;
        Vector3 targetScale = originalScale * prizePopScale;

        float elapsed = 0f;

        while (elapsed < prizePopDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / prizePopDuration;
            textTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < prizePopDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / prizePopDuration;
            textTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        textTransform.localScale = originalScale;
    }
}
