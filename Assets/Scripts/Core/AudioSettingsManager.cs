/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: silencia o activa los grupos del AudioMixer y guarda esa elección entre sesiones.
 */

using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : Singleton<AudioSettingsManager>
{
    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private string[] savedGroups;

    private const string PrefsPrefix = "Audio_";

    private void Start()
    {
        foreach (string group in savedGroups)
            ApplyGroup(group, IsGroupOn(group));
    }

    public void SetVolume(string mixerParameterName, float sliderValue)
    {
        float db = sliderValue > 0 ? Mathf.Log10(sliderValue) * 20f : -80f;
        mainAudioMixer.SetFloat(mixerParameterName, db);
    }

    public bool IsGroupOn(string mixerParameterName) => PlayerPrefs.GetInt(PrefsPrefix + mixerParameterName, 1) == 1;

    public void ToggleGroup(string mixerParameterName, bool isOn)
    {
        ApplyGroup(mixerParameterName, isOn);
        PlayerPrefs.SetInt(PrefsPrefix + mixerParameterName, isOn ? 1 : 0);
    }

    private void ApplyGroup(string mixerParameterName, bool isOn)
    {
        float targetVolume = isOn ? 0f : -80f;
        mainAudioMixer.SetFloat(mixerParameterName, targetVolume);
    }
}
