using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Puente entre el tablero y el teléfono: guarda el resultado pendiente, lleva la cámara
/// al teléfono, entrega el texto a PhoneViewManager y aplica los efectos al volver al tablero.
/// </summary>
public class DeductionPhoneBridge : MonoBehaviour
{
    [SerializeField] private DeductionResolver deductions;
    [Tooltip("Segundos que se espera, ya en el tablero, antes de aplicar los cambios de la deducción. 0 = instantáneo.")]
    [Min(0f)] [SerializeField] private float applyDelay = 0.5f;

    private DeductionResult? _pending;
    private bool _textTaken;

    // true desde que se presenta una deducción hasta que termina la animación de las pistas validadas
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

    // Lo llama PhoneViewManager al entrar al teléfono. false = no hay resultado pendiente.
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
        // Mientras se espera y se anima no se puede consultar ni cambiar de vista
        GameViewManager view = GameViewManager.Instance;
        view.BeginSequence();

        if (applyDelay > 0f) yield return new WaitForSeconds(applyDelay);

        deductions.Apply(result);

        // El punch de las pistas ignora el timeScale, la espera también
        if (result.IsNewSolve) yield return new WaitForSecondsRealtime(deductions.ValidatedAnimDuration);

        view.EndSequence();
        IsResolving = false;
        ResolveFinished?.Invoke();
    }
}
