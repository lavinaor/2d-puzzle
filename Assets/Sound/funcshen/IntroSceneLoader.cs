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

    private void Start()
    {
        // ���� ������
        SoundFXManager.Instance.PlaySoundFXClipNoSpawn(AudioClip, false);
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        // ���� ����� ��� �������� ������ �� ������ �������
        yield return new WaitForSeconds(waitBeforeLoad);

        // ����� ����� ����
        SceneManager.LoadScene(nextSceneName);
    }
}