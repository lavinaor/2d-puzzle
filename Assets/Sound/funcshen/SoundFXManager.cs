using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;
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

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, bool doNotDestroy)
    {
        //spawn in gameOBject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the deathAudioClip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        if (doNotDestroy)
        {
            // save bethwen scens
            DontDestroyOnLoad(audioSource.gameObject);
        }

        //get length of sound FX clip
       // float clipLength = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, 2f);
    }

    public void PlaySoundFXClipNoSpawn(AudioClip audioClip, bool doNotDestroy)
    {
        //spawn in gameOBject
        AudioSource audioSource = Instantiate(soundFXObject, this.transform.position, Quaternion.identity);

        //assign the deathAudioClip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = 1;

        //play sound
        audioSource.Play();

        if (doNotDestroy)
        {
            // save bethwen scens
            DontDestroyOnLoad(audioSource.gameObject);
        }

        //get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume, bool doNotDestroy)
    {
        //spawn in gameOBject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign a random index
        int rand = Random.Range(0, audioClip.Length);

        //assign the deathAudioClip
        audioSource.clip = audioClip[rand];

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        if (doNotDestroy)
        {
            // save bethwen scens
            DontDestroyOnLoad(audioSource.gameObject);
        }

        //get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
