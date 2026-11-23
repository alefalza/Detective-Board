/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: resalta el botón de la vista activa con escala, ícono y color de fondo.
 */

using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonScaler_UI))]
public class ViewButtonSelector_HUD : MonoBehaviour
{
    [SerializeField] private GameView[] selectedViews;

    [Header("Icon")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite selectedIcon;

    [Header("Background")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color selectedBackgroundColor = Color.white;

    private ButtonScaler_UI _scaler;
    private Sprite _defaultIcon;
    private Color _defaultBackgroundColor;

    private void Awake()
    {
        _scaler = GetComponent<ButtonScaler_UI>();

        if (iconImage != null) _defaultIcon = iconImage.sprite;
        if (backgroundImage != null) _defaultBackgroundColor = backgroundImage.color;
    }

    private void Start()
    {
        GameViewManager view = GameViewManager.Instance;
        view.StateChanged += OnStateChanged;

        bool selected = IsSelected(view.CurrentView);
        _scaler.SetSelected(selected, animate: false);
        ApplyVisuals(selected);
    }

    private void OnStateChanged(GameView view)
    {
        bool selected = IsSelected(view);
        _scaler.SetSelected(selected);
        ApplyVisuals(selected);
    }

    private void ApplyVisuals(bool selected)
    {
        if (iconImage != null && selectedIcon != null)
            iconImage.sprite = selected ? selectedIcon : _defaultIcon;

        if (backgroundImage != null)
            backgroundImage.color = selected ? selectedBackgroundColor : _defaultBackgroundColor;
    }

    private bool IsSelected(GameView view) => Array.IndexOf(selectedViews, view) >= 0;

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
            GameViewManager.Instance.StateChanged -= OnStateChanged;
    }
}
