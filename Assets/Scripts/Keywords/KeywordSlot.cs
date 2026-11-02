using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeywordSlot : MonoBehaviour, IDropHandler
{
    [Tooltip("Keyword que debe ocupar este slot en el artículo final. Vacío = slot sin validación (ej. panel lateral).")]
    [SerializeField] private string expectedKeyword;

    // Se dispara cuando el slot se ocupa o se libera
    public event Action<KeywordSlot> Changed;

    public KeywordChipUI CurrentChip { get; private set; }
    public bool IsOccupied => CurrentChip != null;
    public string Keyword => CurrentChip != null ? CurrentChip.Keyword : null;

    public bool HasExpectedKeyword => !string.IsNullOrWhiteSpace(expectedKeyword);

    public bool IsCorrect => IsOccupied && HasExpectedKeyword
        && string.Equals(Keyword.Trim(), expectedKeyword.Trim(), StringComparison.OrdinalIgnoreCase);

    public void OnDrop(PointerEventData eventData)
    {
        if (IsOccupied || eventData.pointerDrag == null) return;

        KeywordChipUI chip = eventData.pointerDrag.GetComponent<KeywordChipUI>();
        if (chip != null) chip.SnapToSlot(this);
    }

    public void Occupy(KeywordChipUI chip)
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
