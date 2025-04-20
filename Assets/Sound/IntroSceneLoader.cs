using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneLoader : MonoBehaviour
{
    [SerializeField] private float waitBeforeLoad = 3f; // כמה זמן לחכות לפני מעבר

    [SerializeField] private string nextSceneName = "GameScene"; // השם של הסצנה הבאה

    [SerializeField]
    private AudioClip AudioClip;

    [SerializeField]
    private Slider scoreSlider;
    private void Start()
    {
        // שמור כוכבים
        SoundFXManager.Instance.PlaySoundFXClipNoSpawn(AudioClip);
        StartCoroutine(LoadNextScene());
        // עדכון ה-Slider שיתחיל מ-0 עם הגבלת ה-Max
        scoreSlider.maxValue = waitBeforeLoad;
        scoreSlider.value = 0;
    }

    private IEnumerator LoadNextScene()
    {
        // אפשר לחכות כאן שהמוזיקה תסתיים או שדברים יסתיימו
        yield return new WaitForSeconds(waitBeforeLoad);

        // טעינת הסצנה הבאה
        SceneManager.LoadScene(nextSceneName);
    }

    private void Update()
    {
        // עדכן את ה-Slider (אם יש צורך)
        scoreSlider.value += Time.deltaTime;
    }
}