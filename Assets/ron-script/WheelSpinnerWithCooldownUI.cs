using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class WheelSegment
{
    public string prizeName;
    public Sprite prizeImage;
}

public class WheelSpinnerWithCooldownUI : MonoBehaviour
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
    public Text prizeDisplayText;

    [Header("Cooldown")]
    public float spinCooldown = 5f; // זמן המתנה בין סיבובים בשניות
    public Text cooldownText;       // UI Text שמראה את הזמן שנותר

    private bool canSpin = true;
    private bool isSpinning = false;

    public void SpinWheel()
    {
        if (!isSpinning && canSpin)
        {
            StartCoroutine(SpinCoroutine());
        }
        else if (!canSpin)
        {
            Debug.Log("חכה עד שהגלגל יתחדש...");
        }
    }

    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;
        canSpin = false;

        // סאונד ואנימציה
        if (spinAudio != null) spinAudio.Play();
        if (buttonAnimator != null) buttonAnimator.SetTrigger("Pressed");

        // בוחר פלח רנדומלי
        int targetSegment = Random.Range(0, segments);
        float segmentAngle = 360f / segments;
        float totalRotation = 360f * Random.Range(minFullSpins, maxFullSpins) + targetSegment * segmentAngle;

        float elapsed = 0f;
        float startZ = transform.eulerAngles.z;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinDuration;
            float easedT = 1 - Mathf.Pow(1 - t, 3); // easing
            float currentRotation = Mathf.Lerp(0, totalRotation, easedT);
            transform.rotation = Quaternion.Euler(0, 0, startZ - currentRotation);
            yield return null;
        }

        // סיום מדויק על הפלח
        transform.rotation = Quaternion.Euler(0, 0, startZ - totalRotation);

        // הצגת פרס
        if (segmentPrizes != null && segmentPrizes.Length == segments)
        {
            WheelSegment prize = segmentPrizes[targetSegment];

            if (prizeDisplayImage != null)
                prizeDisplayImage.sprite = prize.prizeImage;

            if (prizeDisplayText != null)
                prizeDisplayText.text = prize.prizeName;

            Debug.Log($"גלגל עצר על פלח {targetSegment} - פרס: {prize.prizeName}");
        }

        isSpinning = false;

        // התחלת Cooldown
        float cooldownElapsed = 0f;
        while (cooldownElapsed < spinCooldown)
        {
            cooldownElapsed += Time.deltaTime;
            float remaining = Mathf.Clamp(spinCooldown - cooldownElapsed, 0, spinCooldown);

            if (cooldownText != null)
                cooldownText.text = $"חכה {remaining:F1} שניות";

            yield return null;
        }

        canSpin = true;

        if (cooldownText != null)
            cooldownText.text = "לחץ כדי לסובב!";
    }
}
