using UnityEngine;

public class WheelSpinner : MonoBehaviour
{
    public int segments = 10;          // מספר פלחים
    public float spinTime = 3f;        // זמן הסיבוב
    private bool isSpinning = false;

    public void SpinWheel()
    {
        if (!isSpinning)
            StartCoroutine(SpinCoroutine());
    }

    private System.Collections.IEnumerator SpinCoroutine()
    {
        isSpinning = true;

        int targetSegment = Random.Range(0, segments);  // פלח רנדומלי
        float segmentAngle = 360f / segments;

        // מספר סיבובים מלאים לפני עצירה
        float totalRotation = 360f * Random.Range(3, 6) + targetSegment * segmentAngle;

        float elapsed = 0f;
        float startRotation = transform.eulerAngles.z;

        while (elapsed < spinTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinTime;
            // easing: מתחיל מהר ומאט לקראת הסוף
            float currentRotation = Mathf.Lerp(0, totalRotation, 1 - Mathf.Pow(1 - t, 3));
            transform.rotation = Quaternion.Euler(0, 0, startRotation - currentRotation);
            yield return null;
        }

        // סיום מדויק על הפלח
        transform.rotation = Quaternion.Euler(0, 0, startRotation - totalRotation);
        Debug.Log("גלגל עצר על פלח מספר: " + targetSegment);

        isSpinning = false;
    }
}
