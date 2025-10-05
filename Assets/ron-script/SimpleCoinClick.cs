using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class SimpleCoinClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject vfxPrefab;    // VFX ������ ������
    public AudioSource audioSource;  // ���� ���
    public AudioClip clickSound;     // ����� ������
    public TMP_Text coinsText;       // ����� ����� �� �������
    public int addAmount = 25;       // ��� ������ ��� �����
    public float vfxDuration = 1f;   // ��� ��� ����� �����

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

        // ������ VFX
        if (vfxPrefab != null)
        {
            GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, vfxDuration);
        }

        // ������ �����
        SaveManager.Instance.AddCoins(addAmount);
        UpdateCoinsText();

        // ����� ��PlayerPrefs
        PlayerPrefs.SetInt(PLAYER_PREFS_KEY, currentCoins);
        PlayerPrefs.Save();

        // ������ �� ������ �����
        gameObject.SetActive(false);
    }

    void UpdateCoinsText()
    {
        if (coinsText)
            coinsText.text = currentCoins.ToString();
    }
}
