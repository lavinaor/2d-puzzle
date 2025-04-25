using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemsTextUpdater : MonoBehaviour
{
    public TMP_Text gemsText;  // ���� ����� �����

    private void OnEnable()
    {
        SaveManager.OnGemsChanged += UpdateGemsText;  // ����� ������ �� ����� ������
    }

    private void OnDisable()
    {
        SaveManager.OnGemsChanged -= UpdateGemsText;  // ����� ������� ���� �������� ����
    }

    private void Start()
    {
        UpdateGemsText();  // ����� ������� �� ������
    }

    private void UpdateGemsText()
    {
        int gems = SaveManager.Instance.GetGems();  // ���� ���� ������
        gemsText.text = "Gems: " + gems.ToString();  // ����� �����
    }
}