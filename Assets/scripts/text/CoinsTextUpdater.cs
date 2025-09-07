using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsTextUpdater : MonoBehaviour
{
    public TMP_Text coinsText;  // טקסט מטבעות להציג

    private void OnEnable()
    {
        SaveManager.OnCoinsChanged += UpdateCoinsText;  // הירשם לאירוע של שינוי במטבעות
    }

    private void OnDisable()
    {
        SaveManager.OnCoinsChanged -= UpdateCoinsText;  // התנתק מהאירוע כאשר האובייקט נמחק
    }

    private void Start()
    {
        UpdateCoinsText();  // עדכון ההתחלתי של המטבעות
    }

    private void UpdateCoinsText()
    {
        int coins = SaveManager.Instance.GetCoins();  // קבלת כמות המטבעות
        coinsText.text = coins.ToString();  // עדכון הטקסט
    }
}