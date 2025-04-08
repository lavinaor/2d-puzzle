using UnityEngine;
using UnityEngine.UI;

public class ButtonCycler : MonoBehaviour
{
    public GameObject[] buttonsToShow;
    private int currentIndex = 0;

    public void ShowNextButton()
    {
        // ��� �� ����
        foreach (GameObject btn in buttonsToShow)
        {
            btn.SetActive(false);
        }

        // ���� �� �� ������
        if (buttonsToShow.Length > 0)
        {
            buttonsToShow[currentIndex].SetActive(true);
            currentIndex = (currentIndex + 1) % buttonsToShow.Length; // �����
        }
    }
}