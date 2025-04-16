using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneLoader : MonoBehaviour
{
    [SerializeField] private float waitBeforeLoad = 3f; // כמה זמן לחכות לפני מעבר

    [SerializeField] private string nextSceneName = "GameScene"; // השם של הסצנה הבאה

    [SerializeField]
    private AudioClip AudioClip;
    private void Start()
    {
        // שמור כוכבים
        SoundFXManager.Instance.PlaySoundFXClipNoSpawn(AudioClip);
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        // אפשר לחכות כאן שהמוזיקה תסתיים או שדברים יסתיימו
        yield return new WaitForSeconds(waitBeforeLoad);

        // טעינת הסצנה הבאה
        SceneManager.LoadScene(nextSceneName);
    }
}