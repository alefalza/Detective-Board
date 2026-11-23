/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: lleva el resultado de una deducción a la llamada y aplica sus efectos al volver al tablero.
 */

using System;
using System.Collections;
using UnityEngine;

public class DeductionPhoneBridge : MonoBehaviour
{
    [SerializeField] private DeductionResolver deductions;
    [Min(0f)] [SerializeField] private float applyDelay = 0.5f;

    private DeductionResult? _pending;
    private bool _textTaken;

    public bool IsResolving { get; private set; }

    public event Action ResolveFinished;

    private void Start()
    {
        GameViewManager.Instance.StateChanged += OnViewChanged;
    }

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
            GameViewManager.Instance.StateChanged -= OnViewChanged;
    }

    public bool Present(DeductionResult result)
    {
        GameViewManager view = GameViewManager.Instance;
        if (view.IsTransitioning || view.IsSequenceRunning || view.CurrentView != GameView.Corkboard) return false;

        _pending = result;
        _textTaken = false;
        IsResolving = true;
        view.SwitchToPhone();
        return true;
    }

    public bool TryTakeText(out string text)
    {
        if (!_pending.HasValue || _textTaken)
        {
            text = null;
            return false;
        }

        _textTaken = true;
        text = _pending.Value.Text;
        return true;
    }

    private void OnViewChanged(GameView view)
    {
        if (view != GameView.Corkboard || !_pending.HasValue || !_textTaken) return;

        DeductionResult result = _pending.Value;
        _pending = null;
        _textTaken = false;

        StartCoroutine(ApplyRoutine(result));
    }

    private IEnumerator ApplyRoutine(DeductionResult result)
    {
        GameViewManager view = GameViewManager.Instance;
        view.BeginSequence();

        if (applyDelay > 0f) yield return new WaitForSeconds(applyDelay);

        deductions.Apply(result);

        if (result.IsNewSolve) yield return new WaitForSecondsRealtime(deductions.ValidatedAnimDuration);

        view.EndSequence();
        IsResolving = false;
        ResolveFinished?.Invoke();
    }
}
