using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IslandLockButton : MonoBehaviour
{
    // קובץ הסאונד להשמעה
    [SerializeField]
    private AudioClip sceneTransitionSound;

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
        int starsNum = SaveManager.Instance.GetTotalStars();
        starLockText.text = ("stars: " + starsNum + "/" + starLock);
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

    public void OnChangeSeneSound(string sceneName)
    {
        if (isEnough)
        {
/*            if (musicmaneger.Instance != null && musicCngeClip != null)
                musicmaneger.Instance.PlayMusicWithFade(musicCngeClip, musicClipVolume);

            // השמעת הסאונד בעזרת SoundFXManager
            if (SoundFXManager.Instance != null)
            {
                // כאן הוספתי את ה-Transform של אובייקט ה-SceneChanger
                SoundFXManager.Instance.PlaySoundFXClip(sceneTransitionSound, transform, 1f, true);
            }*/

            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("not enoth stars");
        }
    }
}
