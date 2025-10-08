using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Must match the exposed parameter names in your AudioMixer
    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM   = "SFXVolume";

    // PlayerPrefs keys
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY   = "SFXVolume";

    private const float DEFAULT_VOL = 0.8f;

    private void Start()
    {
        // Hook up sliders (UI → code)
        if (musicSlider) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider)   sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Load saved values (or defaults)
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, DEFAULT_VOL);
        float sfx   = PlayerPrefs.GetFloat(SFX_KEY,   DEFAULT_VOL);

        if (musicSlider) musicSlider.value = music;
        if (sfxSlider)   sfxSlider.value   = sfx;

        // Apply to mixer
        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    public void SetMusicVolume(float linear)
    {
        SetLinearToDB(MUSIC_PARAM, linear);
        PlayerPrefs.SetFloat(MUSIC_KEY, Mathf.Clamp01(linear));
    }

    public void SetSFXVolume(float linear)
    {
        SetLinearToDB(SFX_PARAM, linear);
        PlayerPrefs.SetFloat(SFX_KEY, Mathf.Clamp01(linear));
    }

    private void SetLinearToDB(string exposedParam, float linear)
    {
        if (!mixer) return;
        // avoid -Infinity dB when slider is 0
        float v  = Mathf.Clamp(linear, 0.0001f, 1f);
        float dB = Mathf.Log10(v) * 20f; // 1.0 => 0 dB, 0.5 => -6 dB
        mixer.SetFloat(exposedParam, dB);
    }

    private void OnDisable() => PlayerPrefs.Save();
}
