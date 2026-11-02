using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DropZone : MonoBehaviour
{
    private BoxCollider2D _boxCollider;

    public EvidenceItem CurrentItem { get; private set; }
    public bool IsOccupied => CurrentItem != null;

    private void Awake() => _boxCollider = GetComponent<BoxCollider2D>();

    private void Start() => _boxCollider.isTrigger = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        EvidenceItem item = other.GetComponent<EvidenceItem>();
        if (item != null && !IsOccupied)
        {
            item.SetHoveredZone(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        EvidenceItem item = other.GetComponent<EvidenceItem>();
        if (item != null)
        {
            item.ClearHoveredZone(this);
        }
    }

    public void Occupy(EvidenceItem item) => CurrentItem = item;
    public void Vacate() => CurrentItem = null;
}
