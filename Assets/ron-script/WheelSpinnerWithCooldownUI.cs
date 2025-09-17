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
    [Header("����")]
    public int segments = 10;
    public float spinDuration = 3f;
    public int minFullSpins = 3;
    public int maxFullSpins = 6;

    [Header("�����")]
    public AudioSource spinAudio;

    [Header("������� �� �����")]
    public Animator buttonAnimator;

    [Header("����� ������")]
    public WheelSegment[] segmentPrizes;

    [Header("UI ����� ���")]
    public Image prizeDisplayImage;
    public Text prizeDisplayText;

    [Header("Cooldown")]
    public float spinCooldown = 5f; // ��� ����� ��� ������� ������
    public Text cooldownText;       // UI Text ����� �� ���� �����

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
            Debug.Log("��� �� ������ �����...");
        }
    }

    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;
        canSpin = false;

        // ����� ��������
        if (spinAudio != null) spinAudio.Play();
        if (buttonAnimator != null) buttonAnimator.SetTrigger("Pressed");

        // ���� ��� �������
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

        // ���� ����� �� ����
        transform.rotation = Quaternion.Euler(0, 0, startZ - totalRotation);

        // ���� ���
        if (segmentPrizes != null && segmentPrizes.Length == segments)
        {
            WheelSegment prize = segmentPrizes[targetSegment];

            if (prizeDisplayImage != null)
                prizeDisplayImage.sprite = prize.prizeImage;

            if (prizeDisplayText != null)
                prizeDisplayText.text = prize.prizeName;

            Debug.Log($"���� ��� �� ��� {targetSegment} - ���: {prize.prizeName}");
        }

        isSpinning = false;

        // ����� Cooldown
        float cooldownElapsed = 0f;
        while (cooldownElapsed < spinCooldown)
        {
            cooldownElapsed += Time.deltaTime;
            float remaining = Mathf.Clamp(spinCooldown - cooldownElapsed, 0, spinCooldown);

            if (cooldownText != null)
                cooldownText.text = $"��� {remaining:F1} �����";

            yield return null;
        }

        canSpin = true;

        if (cooldownText != null)
            cooldownText.text = "��� ��� �����!";
    }
}
