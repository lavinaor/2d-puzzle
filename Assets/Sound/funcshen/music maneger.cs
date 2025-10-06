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

    [Header("Random Music Playlist")]
    public List<AudioClip> backgroundTracks = new List<AudioClip>(); // רשימת השירים
    public bool playRandomLoop = false; // אם true – יריץ אוטומטית רשימה באקראי
    public float delayBetweenSongs = 1f; // הפסקה קטנה בין שירים

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
            return;
        }

        currentMusic = musicObject;
    }

    private void Start()
    {
        // אם רוצים מצב רקע אוטומטי
        if (playRandomLoop && backgroundTracks.Count > 0)
        {
            StartCoroutine(PlayRandomLoopForever());
        }
    }

    public void PlayMusicClipNoSpawn(AudioClip audioClip)
    {
        AudioSource audioSource = Instantiate(musicObject, this.transform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = 1;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }

    public void PlayMusicWithFade(AudioClip newClip, float volume)
    {
        StopAllCoroutines();
        StartCoroutine(FadeToNewClip(newClip, volume));
    }

    private IEnumerator FadeToNewClip(AudioClip newClip, float volume)
    {
        float startVolume = currentMusic.volume;

        // Fade out
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
            currentMusic.volume = Mathf.Lerp(0, volume, t / fadeDuration);
            yield return null;
        }

        currentMusic.volume = volume;
    }

    // ----------- החלק החדש כאן 👇 -----------
    private IEnumerator PlayRandomLoopForever()
    {
        // בוחר שיר ראשון רנדומלי
        int currentIndex = Random.Range(0, backgroundTracks.Count);
        yield return StartCoroutine(FadeToNewClip(backgroundTracks[currentIndex], 1f));

        while (true)
        {
            // מחכה לסוף השיר הנוכחי + הפסקה
            yield return new WaitForSeconds(currentMusic.clip.length + delayBetweenSongs);

            // בוחר שיר חדש (שונה מהקודם)
            int nextIndex;
            do
            {
                nextIndex = Random.Range(0, backgroundTracks.Count);
            } while (nextIndex == currentIndex && backgroundTracks.Count > 1);

            currentIndex = nextIndex;

            // מעביר עם Fade
            yield return StartCoroutine(FadeToNewClip(backgroundTracks[currentIndex], 1f));
        }
    }
}
