/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: avisa cuando todos los slots del artículo tienen la keyword correcta.
 */

using System;
using System.Text;
using UnityEngine;

public class ArticleValidator : MonoBehaviour
{
    [SerializeField] private KeywordSlot[] articleSlots;

    public event Action Solved;
    public event Action<int> Failed;
    public event Action PlacementChanged;

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
        if (signature == null) return;
        if (signature == _lastSignature) return;

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
