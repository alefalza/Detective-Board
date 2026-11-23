/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: slot que recibe una ficha de keyword y sabe qué keyword espera.
 */

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeywordSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private string expectedKeyword;

    public event Action<KeywordSlot> Changed;

    public KeywordChip_UI CurrentChip { get; private set; }
    public bool IsOccupied => CurrentChip != null;
    public string Keyword => CurrentChip != null ? CurrentChip.Keyword : null;

    public bool HasExpectedKeyword => !string.IsNullOrWhiteSpace(expectedKeyword);

    public bool IsCorrect => IsOccupied && HasExpectedKeyword
        && string.Equals(Keyword.Trim(), expectedKeyword.Trim(), StringComparison.OrdinalIgnoreCase);

    public void OnDrop(PointerEventData eventData)
    {
        if (IsOccupied || eventData.pointerDrag == null) return;

        KeywordChip_UI chip = eventData.pointerDrag.GetComponent<KeywordChip_UI>();
        if (chip != null) chip.SnapToSlot(this);
    }

    public void Occupy(KeywordChip_UI chip)
    {
        CurrentChip = chip;
        Changed?.Invoke(this);
    }

    public void Vacate()
    {
        CurrentChip = null;
        Changed?.Invoke(this);
    }
}
