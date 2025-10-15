using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // ���� ����� ����� �����

public class ButtonUnlocker : MonoBehaviour
{
    [SerializeField] private Button targetButton; // ������ ���� ���� ������/�����
    [SerializeField] private TextMeshProUGUI starsText; // ��� ���� ����� �������
    [SerializeField] private int requiredStars = 20; // ���� ������� �����
    [SerializeField] private string sceneNameToLoad; // �� ����� ����� ������ ������

    void Start()
    {
        UpdateButtonAndText();

        // ������� ����� ������
        if (targetButton != null)
            targetButton.onClick.AddListener(OnButtonClicked);
    }

    public void UpdateButtonAndText()
    {
        int currentStars = SaveManager.Instance.GetStarsInTotal();

        // ����� ������
        targetButton.interactable = currentStars >= requiredStars;

        // ����� �����
        if (starsText != null)
            starsText.text = currentStars + " / " + requiredStars + " Stars";
    }

    private void OnButtonClicked()
    {
        if (!string.IsNullOrEmpty(sceneNameToLoad))
            SceneManager.LoadScene(sceneNameToLoad); // ���� �� ����� ������
    }
}
