using UnityEngine;

public class WheelSpinner : MonoBehaviour
{
    public int segments = 10;          // ���� �����
    public float spinTime = 3f;        // ��� ������
    private bool isSpinning = false;

    public void SpinWheel()
    {
        if (!isSpinning)
            StartCoroutine(SpinCoroutine());
    }

    private System.Collections.IEnumerator SpinCoroutine()
    {
        isSpinning = true;

        int targetSegment = Random.Range(0, segments);  // ��� �������
        float segmentAngle = 360f / segments;

        // ���� ������� ����� ���� �����
        float totalRotation = 360f * Random.Range(3, 6) + targetSegment * segmentAngle;

        float elapsed = 0f;
        float startRotation = transform.eulerAngles.z;

        while (elapsed < spinTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinTime;
            // easing: ����� ��� ���� ����� ����
            float currentRotation = Mathf.Lerp(0, totalRotation, 1 - Mathf.Pow(1 - t, 3));
            transform.rotation = Quaternion.Euler(0, 0, startRotation - currentRotation);
            yield return null;
        }

        // ���� ����� �� ����
        transform.rotation = Quaternion.Euler(0, 0, startRotation - totalRotation);
        Debug.Log("���� ��� �� ��� ����: " + targetSegment);

        isSpinning = false;
    }
}
