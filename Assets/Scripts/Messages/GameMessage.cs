/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: título y cuerpo de un mensaje.
 */

using UnityEngine;

[CreateAssetMenu(fileName = "NewGameMessage", menuName = "Game/Message")]
public class GameMessage : ScriptableObject
{
    public string title;
    [TextArea] public string body;
}
