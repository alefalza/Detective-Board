using UnityEngine;

[RequireComponent(typeof(ButtonScaler_UI))]
public class TypewriterButton_HUD : MonoBehaviour
{
    [Header("Punch Settings")]
    [SerializeField] private Vector3 punchAmount = new Vector3(0.25f, 0.25f, 0f);
    [SerializeField] private float punchDuration = 0.35f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip receiveSound;

    private ButtonScaler_UI _scaler;

    private void Awake() => _scaler = GetComponent<ButtonScaler_UI>();

    public void OnKeywordReceived()
    {
        if (audioSource != null && receiveSound != null)
            audioSource.PlayOneShot(receiveSound);

        _scaler.Punch(punchAmount, punchDuration);
    }
}
