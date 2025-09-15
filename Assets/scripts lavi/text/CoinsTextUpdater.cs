using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsTextUpdater : MonoBehaviour
{
    public TMP_Text coinsText;  // ���� ������ �����

    private void OnEnable()
    {
        SaveManager.OnCoinsChanged += UpdateCoinsText;  // ����� ������ �� ����� �������
    }

    private void OnDisable()
    {
        SaveManager.OnCoinsChanged -= UpdateCoinsText;  // ����� ������� ���� �������� ����
    }

    private void Start()
    {
        UpdateCoinsText();  // ����� ������� �� �������
    }

    private void UpdateCoinsText()
    {
        int coins = SaveManager.Instance.GetCoins();  // ���� ���� �������
        coinsText.text = coins.ToString();  // ����� �����
    }
}