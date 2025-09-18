using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    public Image displayImage;       // �-Image ��� ���� ������
    public Sprite[] availableSprites; // ����� ������� ��������

    // ������� ����� ����� ��� ������
    public void SelectImage(int index)
    {
        if (index >= 0 && index < availableSprites.Length)
        {
            displayImage.sprite = availableSprites[index];
        }
        else
        {
            Debug.LogWarning("������ ���� �����!");
        }
    }
}
