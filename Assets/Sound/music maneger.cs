using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicmaneger : MonoBehaviour
{
    public static musicmaneger Instance;

    [SerializeField] private AudioSource musicObject;

    private AudioSource currentMusic;

    [Header("Fade Settings")]
    public float fadeDuration = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusicClipNoSpawn(AudioClip audioClip)
    {
        //spawn in gameOBject
        AudioSource audioSource = Instantiate(musicObject, this.transform.position, Quaternion.identity);

        //assign the deathAudioClip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = 1;

        //play sound
        audioSource.Play();

        //get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayMusicWithFade(AudioClip newClip)
    {
        StopAllCoroutines();
        StartCoroutine(FadeToNewClip(newClip));
    }

    private IEnumerator FadeToNewClip(AudioClip newClip)
    {
        // Fade out
        float startVolume = currentMusic.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            currentMusic.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        currentMusic.volume = 0;
        currentMusic.Stop();

        // Change clip
        currentMusic.clip = newClip;
        currentMusic.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            currentMusic.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        currentMusic.volume = startVolume;
    }
}
