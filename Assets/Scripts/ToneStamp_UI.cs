using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ButtonScaler_UI))]
public class ToneStamp_UI : MonoBehaviour
{
    [SerializeField] private ToneData tone;
    [Tooltip("Opcional. Muestra el nombre del tono.")]
    [SerializeField] private TextMeshProUGUI label;

    private ButtonScaler_UI _scaler;

    public ToneData Tone => tone;

    public event Action<ToneStamp_UI> Clicked;

    private void Awake() => _scaler = GetComponent<ButtonScaler_UI>();

    private void Start()
    {
        if (label != null) label.text = tone.DisplayName;
    }

    // Asignar en el On Click Event del ButtonScaler_UI
    public void OnClick() => Clicked?.Invoke(this);

    public void SetSelected(bool selected) => _scaler.SetSelected(selected);
}
