using UnityEngine;
using TMPro;
using DG.Tweening;

public class SimpleCoinSpriteClick : MonoBehaviour
{
    public SpriteRenderer targetSpriteRenderer;
    public Sprite newSprite;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public TMP_Text coinsText;
    public int addAmount = 25;
    public float fadeDuration = 0.3f;
    public float newSpriteScale = 0.2f; // ��� ����� ������

    private static int currentCoins = 0;
    private const string PLAYER_PREFS_KEY = "Coins";
    private Vector3 originalScale; // ����� ����� ������

    private void Awake()
    {
        // ����� ����� �����
        originalScale = targetSpriteRenderer.transform.localScale;

        // ����� ������ ������
        currentCoins = PlayerPrefs.GetInt(PLAYER_PREFS_KEY, 0);
        UpdateCoinsText();
    }

    private void OnMouseDown()
    {
        // ���
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);

        // ����� ������ ������ ����� ����� �����
        if (targetSpriteRenderer && newSprite)
        {
            targetSpriteRenderer.sprite = newSprite;
            targetSpriteRenderer.transform.localScale = originalScale * newSpriteScale;
        }

        // ����� ����
        currentCoins += addAmount;
        UpdateCoinsText();

        PlayerPrefs.SetInt(PLAYER_PREFS_KEY, currentCoins);
        PlayerPrefs.Save();

        // ���� ���� ���
        if (targetSpriteRenderer)
        {
            targetSpriteRenderer
                .DOFade(0f, fadeDuration)
                .OnComplete(() =>
                {
                    targetSpriteRenderer.gameObject.SetActive(false);
                });
        }
    }

    void UpdateCoinsText()
    {
        if (coinsText)
            coinsText.text = currentCoins.ToString();
    }
}
