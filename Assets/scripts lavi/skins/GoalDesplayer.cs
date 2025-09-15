using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;

public class GoalDesplayer : MonoBehaviour
{
    [SerializeField]
    private Image Image;
    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    public void SetLook(Goal goal)
    {
        Sprite sprite = CandySkinManager.Instance.GetCandySpriteByType(goal.type);
        Image.sprite = sprite;
        textMeshPro.text = goal.goalPerType.ToString();
    }
}
