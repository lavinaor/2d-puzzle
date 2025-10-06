using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider MasterVolumeslider;
    [SerializeField] private Slider MusicVolumeslider;
    [SerializeField] private Slider SoundFXVolumeslider;

    private bool isInitializing = true;

    private void Start()
    {
        // טוען ערכים מה־PlayerPrefs או ברירת מחדל (1f)
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // מגדיר את הערכים גם במיקסר וגם בסליידרים
        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);

        MasterVolumeslider.value = master;
        MusicVolumeslider.value = music;
        SoundFXVolumeslider.value = sfx;

        // מסמן שסיימנו את שלב האתחול
        isInitializing = false;

        // מאזין לשינויים
        MasterVolumeslider.onValueChanged.AddListener(OnMasterSliderChanged);
        MusicVolumeslider.onValueChanged.AddListener(OnMusicSliderChanged);
        SoundFXVolumeslider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    private void OnMasterSliderChanged(float value)
    {
        if (isInitializing) return;
        SetMasterVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    private void OnMusicSliderChanged(float value)
    {
        if (isInitializing) return;
        SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    private void OnSFXSliderChanged(float value)
    {
        if (isInitializing) return;
        SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(value, 0.001f, 1f)) * 20);
    }

    private void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(value, 0.001f, 1f)) * 20);
    }

    private void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(value, 0.001f, 1f)) * 20);
    }
}
