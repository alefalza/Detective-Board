using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Conecta un Toggle con un grupo del mixer. Un componente por Toggle.
/// </summary>
[RequireComponent(typeof(Toggle))]
public class AudioGroupToggle_UI : MonoBehaviour
{
    [Tooltip("Nombre del parámetro expuesto en el AudioMixer (ej. SFXVolume).")]
    [SerializeField] private string mixerParameterName;

    private Toggle _toggle;

    private void Awake() => _toggle = GetComponent<Toggle>();

    private void Start()
    {
        _toggle.SetIsOnWithoutNotify(AudioSettingsManager.Instance.IsGroupOn(mixerParameterName));
        _toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool isOn) => AudioSettingsManager.Instance.ToggleGroup(mixerParameterName, isOn);

    private void OnDestroy() => _toggle.onValueChanged.RemoveListener(OnValueChanged);
}
