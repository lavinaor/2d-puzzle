using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneLoader : MonoBehaviour
{
    [SerializeField] private float waitBeforeLoad = 3f; // ��� ��� ����� ���� ����

    [SerializeField] private string nextSceneName = "GameScene"; // ��� �� ����� ����

    [SerializeField]
    private AudioClip AudioClip;

    [SerializeField]
    private Slider scoreSlider;
    private void Start()
    {
        // ���� ������
        SoundFXManager.Instance.PlaySoundFXClipNoSpawn(AudioClip);
        StartCoroutine(LoadNextScene());
        // ����� �-Slider ������ �-0 �� ����� �-Max
        scoreSlider.maxValue = waitBeforeLoad;
        scoreSlider.value = 0;
    }

    private IEnumerator LoadNextScene()
    {
        // ���� ����� ��� �������� ������ �� ������ �������
        yield return new WaitForSeconds(waitBeforeLoad);

        // ����� ����� ����
        SceneManager.LoadScene(nextSceneName);
    }

    private void Update()
    {
        // ���� �� �-Slider (�� �� ����)
        scoreSlider.value += Time.deltaTime;
    }
}