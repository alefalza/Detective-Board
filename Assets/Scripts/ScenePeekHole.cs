// Abre un agujero transparente alrededor del cursor en una imagen que tapa inspectables,
// mientras el cursor está sobre alguno de ellos. Usa el shader Detective/UI/PeekHole.
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScenePeekHole : MonoBehaviour
{
    [SerializeField] private SceneryViewerSettings settings;
    [Tooltip("Zonas de los inspectables que esta imagen tapa.")]
    [SerializeField] private RectTransform[] coveredZones;

    private static readonly int HolePosId = Shader.PropertyToID("_HolePos");
    private static readonly int HoleRadiusId = Shader.PropertyToID("_HoleRadius");
    private static readonly int HoleSoftnessId = Shader.PropertyToID("_HoleSoftness");
    private static readonly int HoleMinAlphaId = Shader.PropertyToID("_HoleMinAlpha");
    private static readonly int HoleStrengthId = Shader.PropertyToID("_HoleStrength");

    private Image _image;
    private Material _material;
    private Camera _camera;
    private float _strength;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.raycastTarget = false; // no debe tapar a los inspectables de abajo

        // Instancia propia: cada imagen abre su agujero sin afectar a las demás
        _material = new Material(_image.material);
        _image.material = _material;

        _material.SetFloat(HoleRadiusId, settings.PeekRadius);
        _material.SetFloat(HoleSoftnessId, settings.PeekSoftness);
        _material.SetFloat(HoleMinAlphaId, settings.PeekMinAlpha);
        _material.SetFloat(HoleStrengthId, 0f);
    }

    private void Start() => _camera = GetComponentInParent<Canvas>().worldCamera;

    private void Update()
    {
        Vector2 pointer = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        bool active = CanPeek() && IsOverCoveredZone(pointer);

        float step = Time.unscaledDeltaTime / settings.PeekFadeDuration;
        _strength = Mathf.MoveTowards(_strength, active ? 1f : 0f, step);
        _material.SetFloat(HoleStrengthId, _strength);

        // Al desactivarse, el agujero se cierra en su última posición
        if (active) _material.SetVector(HolePosId, _camera.ScreenToWorldPoint(pointer));
    }

    private static bool CanPeek()
    {
        GameViewManager view = GameViewManager.Instance;
        return view.CurrentView == GameView.Scenery
               && !view.IsTransitioning
               && !InspectionWindow.Instance.IsOpen;
    }

    private bool IsOverCoveredZone(Vector2 pointer)
    {
        foreach (RectTransform zone in coveredZones)
        {
            if (zone != null && zone.gameObject.activeInHierarchy
                && RectTransformUtility.RectangleContainsScreenPoint(zone, pointer, _camera))
                return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        if (_material != null) Destroy(_material);
    }
}
