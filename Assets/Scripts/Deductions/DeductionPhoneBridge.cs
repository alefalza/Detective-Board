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

        if (applyDelay <= 0f)
            deductions.Apply(result);
        else
            StartCoroutine(ApplyAfterDelay(result));
    }

    private IEnumerator ApplyAfterDelay(DeductionResult result)
    {
        // Mientras se espera no se puede consultar ni cambiar de vista
        GameViewManager view = GameViewManager.Instance;
        view.BeginSequence();

        yield return new WaitForSeconds(applyDelay);

        view.EndSequence();
        deductions.Apply(result);
    }
}
