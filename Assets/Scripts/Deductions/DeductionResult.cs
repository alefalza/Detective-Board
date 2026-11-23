/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: resultado de evaluar un trío: lejos, cerca o correcto.
 */

using System.Collections.Generic;

public enum DeductionOutcome { Far, Close, Correct }

public readonly struct DeductionResult
{
    public DeductionOutcome Outcome { get; }
    public string Text { get; }
    public DeductionRule Rule { get; }
    public IReadOnlyList<EvidenceItem> Participants { get; }

    public bool IsNewSolve { get; }

    public DeductionResult(
        DeductionOutcome outcome,
        string text,
        DeductionRule rule = null,
        IReadOnlyList<EvidenceItem> participants = null,
        bool isNewSolve = false)
    {
        Outcome = outcome;
        Text = text;
        Rule = rule;
        Participants = participants;
        IsNewSolve = isNewSolve;
    }
}
