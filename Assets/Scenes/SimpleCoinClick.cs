using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class SimpleCoinClick : MonoBehaviour, IPointerClickHandler
{
    public Image targetImage;       // ������ ������ ������
    public Sprite newSprite;        // ������ �����
    public AudioSource audioSource; // ���� ���
    public AudioClip clickSound;    // ����� ������
    public TMP_Text coinsText;      // ����� ����� �� �������
    public int addAmount = 25;      // ��� ������ ��� �����
    public float fadeDuration = 0.3f; // ��� ������

    private static int currentCoins = 0; // ���� ��� ����� ����� �� ���� ����
    private const string PLAYER_PREFS_KEY = "Coins"; // ���� ������

    private void Awake()
    {
        // ��� �� ����� ������ ����
        currentCoins = PlayerPrefs.GetInt(PLAYER_PREFS_KEY, 0);
        UpdateCoinsText();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ���� ���
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);

        // ������ �����
        if (targetImage && newSprite)
            targetImage.sprite = newSprite;

        // ������ �����
        currentCoins += addAmount;
        UpdateCoinsText();

        // ����� ��PlayerPrefs
        PlayerPrefs.SetInt(PLAYER_PREFS_KEY, currentCoins);
        PlayerPrefs.Save();

        // ������ �� ������ �� �������
        if (targetImage)
        {
            targetImage.transform.DOScale(0f, fadeDuration).SetEase(Ease.InBack);
            targetImage.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                targetImage.gameObject.SetActive(false);
            });
        }
    }

    void UpdateCoinsText()
    {
        if (coinsText)
            coinsText.text = currentCoins.ToString();
    }
}
