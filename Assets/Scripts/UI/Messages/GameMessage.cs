using UnityEngine;

[CreateAssetMenu(fileName = "NewGameMessage", menuName = "Game/Message")]
public class GameMessage : ScriptableObject
{
    public string title;
    [TextArea] public string body;
}
