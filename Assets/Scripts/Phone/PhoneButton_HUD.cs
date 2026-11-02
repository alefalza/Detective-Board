using UnityEngine;

[RequireComponent(typeof(ButtonScaler_UI))]
public class PhoneButton_HUD : MonoBehaviour
{
    [Header("Entrance Anim Settings")]
    [SerializeField] private ButtonPivot_HUD buttonPivot;

    private void Start()
    {
        CorkboardManager.Instance.CanConsultChanged += OnConsultChanged;
    }

    private void OnConsultChanged(bool obj)
    {
        buttonPivot.SetVisible(obj);
    }

    private void OnDestroy()
    {
        if (CorkboardManager.Instance != null)
        {
            CorkboardManager.Instance.CanConsultChanged -= OnConsultChanged;
        }
    }
}
