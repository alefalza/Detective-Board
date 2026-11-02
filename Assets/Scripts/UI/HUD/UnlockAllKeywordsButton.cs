using UnityEngine;

public class UnlockAllKeywordsButton : MonoBehaviour
{
    [SerializeField] private DeductionResolver deductions;

    // Asignar en el OnClick del botón
    public void OnClick()
    {
        TypewriterManager.Instance.AddKeywords(deductions.GetAllKeywords());
        Debug.Log("ALL KEYWORDS UNLOCKED");
    }
}
