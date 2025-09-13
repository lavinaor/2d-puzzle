using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StarReward
{
    public string rewardName;
    public GameObject rewardObject; // ������ �� ������� UI �� ����
    public int requiredStars;
    public AudioClip rewardSound;   // ����� ������ �������� �� ����
    [HideInInspector] public bool isGiven = false;
}

public class StarRewardController : MonoBehaviour
{
    [SerializeField] private List<StarReward> rewards;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private int maxStars = 50;
    [SerializeField] private AudioSource audioSource; // ���� ��� ����

    void Start()
    {
        UpdateRewards();
    }

    public void UpdateRewards()
    {
        int totalStars = SaveManager.Instance.GetStarsInTotal();

        // ����� ������� � �� �������
        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = maxStars;
            progressSlider.value = totalStars;
        }

        // ����� ���� ����� ����
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

            // ������� Listener ������ �� ��
            Button btn = reward.rewardObject.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    StartCoroutine(PlayRewardAnimation(reward.rewardObject, reward.rewardSound));
                });
            }
        }

        Debug.Log($"��� �����: {reward.rewardName}");
    }

    private IEnumerator PlayRewardAnimation(GameObject obj, AudioClip clip)
    {
        // ����� �����
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }

        Vector3 originalScale = obj.transform.localScale;
        Vector3 targetScale = originalScale * 1.3f;
        float duration = 0.2f;

        // �����
        float t = 0f;
        while (t < duration)
        {
            obj.transform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = targetScale;

        // ���� ����� ������
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
