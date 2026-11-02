using System;
using System.Text;
using UnityEngine;

/// <summary>
/// Valida el artículo final: cuando todos los slots del papel están ocupados,
/// comprueba que cada keyword coincida con la esperada por su slot.
/// </summary>
public class ArticleValidator : MonoBehaviour
{
    [Tooltip("Slots del papel. Cada uno debe tener 'Expected Keyword' cargado.")]
    [SerializeField] private KeywordSlot[] articleSlots;

    public event Action Solved;
    public event Action<int> Failed; // cantidad de slots correctos
    public event Action PlacementChanged; // algún slot se ocupó o se liberó

    public int PlacedCount
    {
        get
        {
            int count = 0;
            foreach (KeywordSlot slot in articleSlots)
            {
                if (slot.IsOccupied) count++;
            }
            return count;
        }
    }

    private bool _solved;
    private string _lastSignature;

    private void Awake()
    {
        foreach (KeywordSlot slot in articleSlots)
        {
            if (!slot.HasExpectedKeyword)
                Debug.LogWarning($"[ArticleValidator] El slot '{slot.name}' no tiene Expected Keyword: el artículo nunca podrá resolverse.", slot);
        }
    }

    private void OnEnable()
    {
        foreach (KeywordSlot slot in articleSlots) slot.Changed += OnSlotChanged;
    }

    private void OnDisable()
    {
        foreach (KeywordSlot slot in articleSlots) slot.Changed -= OnSlotChanged;
    }

    private void OnSlotChanged(KeywordSlot changedSlot)
    {
        PlacementChanged?.Invoke();

        if (_solved) return;

        string signature = BuildSignature();
        if (signature == null) return;              // faltan slots por completar
        if (signature == _lastSignature) return;    // misma disposición ya evaluada (ej. un chip que vuelve a su slot)

        _lastSignature = signature;

        int correct = CountCorrect();
        if (correct == articleSlots.Length)
        {
            _solved = true;
            Solved?.Invoke();
        }
        else
        {
            Failed?.Invoke(correct);
        }
    }

    // null si algún slot está vacío; si no, la disposición actual de keywords
    private string BuildSignature()
    {
        var builder = new StringBuilder();
        foreach (KeywordSlot slot in articleSlots)
        {
            if (!slot.IsOccupied) return null;
            builder.Append(slot.Keyword).Append('|');
        }
        return builder.ToString();
    }

    private int CountCorrect()
    {
        int count = 0;
        foreach (KeywordSlot slot in articleSlots)
        {
            if (slot.IsCorrect) count++;
        }
        return count;
    }
}
