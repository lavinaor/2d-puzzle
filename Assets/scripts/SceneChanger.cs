using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private float deley;

    // קובץ הסאונד להשמעה
    [SerializeField]
    private AudioClip sceneTransitionSound;

    public virtual void OnChangeSeneDeley(string sceneName)
    {
        StartCoroutine(DeleyAndThenLode(sceneName));
    }

    IEnumerator DeleyAndThenLode(string sceneName)
    {
        // השמעת הסאונד בעזרת SoundFXManager
        if (SoundFXManager.Instance != null)
        {
            // כאן הוספתי את ה-Transform של אובייקט ה-SceneChanger
            SoundFXManager.Instance.PlaySoundFXClip(sceneTransitionSound, transform, 1f);
        }

        yield return new WaitForSeconds(deley);

        SceneManager.LoadScene(sceneName);
    }
}
