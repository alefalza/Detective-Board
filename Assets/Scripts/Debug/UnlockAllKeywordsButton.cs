/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: cheat que desbloquea todas las keywords de las deducciones.
 */

using UnityEngine;

public class UnlockAllKeywordsButton : MonoBehaviour
{
    [SerializeField] private DeductionResolver deductions;

    public void OnClick()
    {
        TypewriterManager.Instance.AddKeywords(deductions.GetAllKeywords());
        Debug.Log("ALL KEYWORDS UNLOCKED");
    }
}
