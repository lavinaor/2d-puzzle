using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IslandLockButton : SceneChanger
{
    [SerializeField]
    private TextMeshProUGUI starLockText;

    [SerializeField]
    private int starLock = 0;

    [SerializeField] 
    private GameObject lockedVisual;

    [SerializeField] 
    private GameObject unlockedVisual;

    [SerializeField] 
    private Button button;

    [SerializeField]
    private List<Graphic> visualsToGray;

    private bool isEnough = false;

    // קובץ הסאונד להשמעה
    [SerializeField]
    private AudioClip musicCngeClip;
    [SerializeField]
    private float musicClipVolume = 1;

    private void Start()
    {
        int starsNum = SaveManager.Instance.GetStarsInTotal();
        starLockText.text = (starsNum + "/" + starLock);
        if (starsNum >= starLock)
        {
            isEnough = true;
            if (unlockedVisual != null) unlockedVisual.SetActive(true);
            if (lockedVisual != null) lockedVisual.SetActive(false);
            if (button != null) button.interactable = true;
        }
        else
        {
            isEnough = false;
            if (unlockedVisual != null) unlockedVisual.SetActive(false);
            if (lockedVisual != null) lockedVisual.SetActive(true);
            if (button != null) button.interactable = false;
        }
        UpdateGrayVisuals();
    }

    private void UpdateGrayVisuals()
    {
        Color targetColor = isEnough ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f);

        foreach (var g in visualsToGray)
        {
            if (g != null)
                g.color = targetColor;
        }
    }

    public override void OnChangeSeneDeley(string sceneName)
    {
        if (isEnough)
        {
            if (musicmaneger.Instance != null && musicCngeClip != null)
                musicmaneger.Instance.PlayMusicWithFade(musicCngeClip, musicClipVolume);
            base.OnChangeSeneDeley(sceneName);
        }
        else
        {
            Debug.Log("not enoth stars");
        }
    }
}
