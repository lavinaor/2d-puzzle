using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemsTextUpdater : MonoBehaviour
{
    public TMP_Text gemsText;  // טקסט אבנים להציג

    private void OnEnable()
    {
        SaveManager.OnGemsChanged += UpdateGemsText;  // הירשם לאירוע של שינוי באבנים
    }

    private void OnDisable()
    {
        SaveManager.OnGemsChanged -= UpdateGemsText;  // התנתק מהאירוע כאשר האובייקט נמחק
    }

    private void Start()
    {
        UpdateGemsText();  // עדכון ההתחלתי של האבנים
    }

    private void UpdateGemsText()
    {
        int gems = SaveManager.Instance.GetGems();  // קבלת כמות האבנים
        if (gems < 10000)
            gemsText.text = gems.ToString();  // עדכון הטקסט
        else
            gemsText.text = (gems/1000).ToString() + "k";  // עדכון הטקסט

    }
}