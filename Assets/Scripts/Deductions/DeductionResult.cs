using System.Collections.Generic;

public enum DeductionOutcome { Far, Close, Correct }

public readonly struct DeductionResult
{
    public DeductionOutcome Outcome { get; }
    public string Text { get; }
    public DeductionRule Rule { get; }
    public IReadOnlyList<EvidenceItem> Participants { get; }

    // true solo la primera vez que se resuelve la regla (aplica efectos y emite keywords)
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
