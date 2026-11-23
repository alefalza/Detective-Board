/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: conecta un toggle con un grupo del AudioMixer.
 */

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class AudioGroupToggle_UI : MonoBehaviour
{
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
