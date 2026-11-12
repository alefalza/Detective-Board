using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : Singleton<AudioSettingsManager>
{
    [SerializeField] private AudioMixer mainAudioMixer;
    [Tooltip("Parámetros expuestos del mixer cuyo estado on/off se guarda entre sesiones (ej. SFXVolume, AmbienceVolume).")]
    [SerializeField] private string[] savedGroups;

    private const string PrefsPrefix = "Audio_";

    private void Start()
    {
        // AudioMixer.SetFloat no tiene efecto en Awake: se aplica acá
        foreach (string group in savedGroups)
            ApplyGroup(group, IsGroupOn(group));
    }

    // Método para conectar a los Sliders de la UI (valores de 0.001 a 1)
    public void SetVolume(string mixerParameterName, float sliderValue)
    {
        // Convertimos el valor lineal del slider a escala logarítmica de decibeles (-80dB a 0dB)
        float db = sliderValue > 0 ? Mathf.Log10(sliderValue) * 20f : -80f;
        mainAudioMixer.SetFloat(mixerParameterName, db);
    }

    public bool IsGroupOn(string mixerParameterName) => PlayerPrefs.GetInt(PrefsPrefix + mixerParameterName, 1) == 1;

    // Muteo on/off, guardado entre sesiones
    public void ToggleGroup(string mixerParameterName, bool isOn)
    {
        ApplyGroup(mixerParameterName, isOn);
        PlayerPrefs.SetInt(PrefsPrefix + mixerParameterName, isOn ? 1 : 0);
    }

    private void ApplyGroup(string mixerParameterName, bool isOn)
    {
        float targetVolume = isOn ? 0f : -80f; // 0dB (activo) o -80dB (silenciado)
        mainAudioMixer.SetFloat(mixerParameterName, targetVolume);
    }
}
