using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : Singleton<AudioSettingsManager>
{
    [SerializeField] private AudioMixer mainAudioMixer;

    // Método para conectar a los Sliders de la UI (valores de 0.001 a 1)
    public void SetVolume(string mixerParameterName, float sliderValue)
    {
        // Convertimos el valor lineal del slider a escala logarítmica de decibeles (-80dB a 0dB)
        float db = sliderValue > 0 ? Mathf.Log10(sliderValue) * 20f : -80f;
        mainAudioMixer.SetFloat(mixerParameterName, db);
    }

    // Método para conectar a los Toggles de muteo rápido
    public void ToggleGroup(string mixerParameterName, bool isOn)
    {
        float targetVolume = isOn ? 0f : -80f; // 0dB (activo) o -80dB (silenciado)
        mainAudioMixer.SetFloat(mixerParameterName, targetVolume);
    }
}
