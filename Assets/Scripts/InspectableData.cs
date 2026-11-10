using UnityEngine;

[CreateAssetMenu(fileName = "NewInspectable", menuName = "Detective/Inspectable")]
public class InspectableData : ScriptableObject
{
    [SerializeField] private string title;
    [TextArea(3, 8)] [SerializeField] private string description;
    [Tooltip("Opcional. Si está vacía, la ventana oculta la imagen.")]
    [SerializeField] private Sprite image;
    [Tooltip("Opcional. Pista que se agrega al tablero la primera vez que se inspecciona.")]
    [SerializeField] private EvidenceData evidence;

    public string Title => title;
    public string Description => description;
    public Sprite Image => image;
    public EvidenceData Evidence => evidence;
}
