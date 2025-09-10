using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class CountdownButtonWithPopup : MonoBehaviour
{
    public enum TimeUnit { Hours, Minutes }

    [Header("Settings")]
    public Button targetButton;          // ������ �������
    public TMP_Text timerText;           // ���� TMP ����� �� ����
    public TimeUnit timeUnit = TimeUnit.Hours; // ����� �� �� ���� �� ����
    public int amount = 1;               // ��� ����/���� �����
    public Image buttonImage;            // ����� ����� ���� �����
    public Sprite clickedSprite;         // ����� ���� ���� �����
    public GameObject popup;             // ����-�� ������ ���� ������

    private DateTime nextAvailableTime;
    private const string SaveKey = "CountdownButtonWithPopup_NextTime";

    private void Start()
    {
        // ���� �� ���� ����� �� ��
        if (PlayerPrefs.HasKey(SaveKey))
        {
            long savedBinary = Convert.ToInt64(PlayerPrefs.GetString(SaveKey));
            nextAvailableTime = DateTime.FromBinary(savedBinary);
        }
        else
        {
            nextAvailableTime = DateTime.Now;
        }

        targetButton.onClick.AddListener(OnButtonClick);

        // ���� �����-�� ���� ������
        if (popup != null)
            popup.SetActive(false);
    }

    private void Update()
    {
        if (DateTime.Now >= nextAvailableTime)
        {
            targetButton.interactable = true;
        }
        else
        {
            targetButton.interactable = false;

            TimeSpan remaining = nextAvailableTime - DateTime.Now;

            timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                (int)remaining.TotalHours,
                remaining.Minutes,
                remaining.Seconds);
        }
    }

    private void OnButtonClick()
    {
        Debug.Log("����� ����!");

        // ���� �� ���� ��� ��� �� ������
        if (timeUnit == TimeUnit.Hours)
            nextAvailableTime = DateTime.Now.AddHours(amount);
        else
            nextAvailableTime = DateTime.Now.AddMinutes(amount);

        // ����
        PlayerPrefs.SetString(SaveKey, nextAvailableTime.ToBinary().ToString());
        PlayerPrefs.Save();

        // ���� �� ������ �� ����
        if (buttonImage != null && clickedSprite != null)
        {
            buttonImage.sprite = clickedSprite;
        }

        // ����� ���� ���-�� ���� �����
        if (popup != null)
        {
            StartCoroutine(ShowPopupDelayed());
        }
    }

    private IEnumerator ShowPopupDelayed()
    {
        yield return new WaitForSeconds(1f);
        popup.SetActive(true);
    }
}
