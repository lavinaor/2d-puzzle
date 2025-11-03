using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AllStars : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        int starsNum = SaveManager.Instance.GetTotalStars();
        _textMeshPro.text = starsNum.ToString();
    }
}
