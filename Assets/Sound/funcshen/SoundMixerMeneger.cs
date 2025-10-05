using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider MasterVolumeslider;
    [SerializeField] private Slider MusicVolumeslider;
    [SerializeField] private Slider SoundFXVolumeslider;

    private void Start()
    {
        // שלב 1: טען את ההגדרות הקיימות מה-PlayerPrefs
        float masterValue = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float musicValue = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxValue = PlayerPrefs.GetFloat("SoundFXVolume", 0.75f);

        // שלב 2: עדכן את הסליידרים לפי ההגדרות השמורות
        MasterVolumeslider.value = masterValue;
        MusicVolumeslider.value = musicValue;
        SoundFXVolumeslider.value = sfxValue;

        // שלב 3: החל את הערכים על ה-AudioMixer בפועל
        SetMasterVolume(masterValue);
        SetMusicVolume(musicValue);
        SetSoundFXVolume(sfxValue);
    }

    // פונקציה לשינוי ולשמירת עוצמת ה-Master
    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(level, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat("MasterVolume", level);
        PlayerPrefs.Save();
    }

    // פונקציה לשינוי ולשמירת עוצמת המוזיקה
    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(level, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat("MusicVolume", level);
        PlayerPrefs.Save();
    }

    // פונקציה לשינוי ולשמירת עוצמת אפקטים
    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(Mathf.Max(level, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat("SoundFXVolume", level);
        PlayerPrefs.Save();
    }
}
