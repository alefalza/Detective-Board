/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: base genérica para managers con una única instancia, opcionalmente persistente entre escenas.
 */

using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private bool persist;
    
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            
            if (persist)
            {
              DontDestroyOnLoad(gameObject);
            }
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
