using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class lelastLevelText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        int lastLevelEnterd = SaveManager.Instance.GetlastLevelEnterd();
        _textMeshPro.text = lastLevelEnterd.ToString();
    }
}
