using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StarReward
{
    public string rewardName;
    public GameObject rewardObject; // הכפתור או אובייקט UI של הפרס
    public int requiredStars;
    public AudioClip rewardSound;   // הצליל שישמיע כשמקבלים את הפרס
    [HideInInspector] public bool isGiven = false;
}

public class StarRewardController : MonoBehaviour
{
    [SerializeField] private List<StarReward> rewards;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private int maxStars = 50;
    [SerializeField] private AudioSource audioSource; // מקור קול כללי

    void Start()
    {
        UpdateRewards();
    }

    public void UpdateRewards()
    {
        int totalStars = SaveManager.Instance.GetStarsInTotal();

        // עדכון הסליידר – רק ויזואלי
        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = maxStars;
            progressSlider.value = totalStars;
        }

        // בדיקה איזה פרסים ניתן
        foreach (var reward in rewards)
        {
            if (!reward.isGiven && totalStars >= reward.requiredStars)
            {
                GiveReward(reward);
            }
        }
    }

    private void GiveReward(StarReward reward)
    {
        reward.isGiven = true;

        if (reward.rewardObject != null)
        {
            reward.rewardObject.SetActive(true);

            // מוסיפים Listener לכפתור אם יש
            Button btn = reward.rewardObject.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    StartCoroutine(PlayRewardAnimation(reward.rewardObject, reward.rewardSound));
                });
            }
        }

        Debug.Log($"פרס שוחרר: {reward.rewardName}");
    }

    private IEnumerator PlayRewardAnimation(GameObject obj, AudioClip clip)
    {
        // השמעת סאונד
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }

        Vector3 originalScale = obj.transform.localScale;
        Vector3 targetScale = originalScale * 1.3f;
        float duration = 0.2f;

        // הגדלה
        float t = 0f;
        while (t < duration)
        {
            obj.transform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = targetScale;

        // חזרה לגודל המקורי
        t = 0f;
        while (t < duration)
        {
            obj.transform.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = originalScale;
    }
}
