using UnityEngine;
using System.Collections.Generic;

public class TypewriterPanels_UI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform leftPanelRect;
    [SerializeField] private RectTransform rightPanelRect;

    [Header("Keywords Settings")]
    [SerializeField] private Transform keywordsContainer;
    [SerializeField] private KeywordChipUI keywordChipUIPrefab;
    [SerializeField] private KeywordSlot keywordSlotPrefab;
    [Tooltip("Cantidad mínima de slots en el panel. Si hay más keywords que slots, se instancian a demanda.")]
    [Min(0)] [SerializeField] private int minSlots = 6;
    [Tooltip("Inclinación máxima en Z (grados, hacia ambos lados) de cada ficha.")]
    [SerializeField] private float chipTiltRange = 4f;

    [Header("Panels Anim Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private iTween.EaseType openEase = iTween.EaseType.easeOutElastic;
    [SerializeField] private iTween.EaseType closeEase = iTween.EaseType.easeInBack;

    private const string LeftTweenName = "TypewriterLeftPanelGrow";
    private const string RightTweenName = "TypewriterRightPanelGrow";

    private readonly Dictionary<string, KeywordChipUI> _chips = new Dictionary<string, KeywordChipUI>();
    private readonly List<KeywordSlot> _slots = new List<KeywordSlot>();

    public void Slide(bool open)
    {
        if (leftPanelRect == null || rightPanelRect == null) return;

        iTween.Stop(leftPanelRect.gameObject, LeftTweenName);
        iTween.Stop(rightPanelRect.gameObject, RightTweenName);

        int sign = open ? 1 : -1;

        float leftTargetX = leftPanelRect.localPosition.x + (leftPanelRect.sizeDelta.x * sign);
        float rightTargetX = rightPanelRect.localPosition.x - (rightPanelRect.sizeDelta.x * sign);

        iTween.MoveTo(leftPanelRect.gameObject, iTween.Hash(
            "name", LeftTweenName,
            "x", leftTargetX,
            "islocal", true,
            "time", animationDuration,
            "easetype", open ? openEase : closeEase
        ));

        iTween.MoveTo(rightPanelRect.gameObject, iTween.Hash(
            "name", RightTweenName,
            "x", rightTargetX,
            "islocal", true,
            "time", animationDuration,
            "easetype", open ? openEase : closeEase
        ));
    }

    public void RefreshUIKeywords()
    {
        if (keywordsContainer == null || keywordChipUIPrefab == null || keywordSlotPrefab == null) return;

        while (_slots.Count < minSlots) CreateSlot();

        foreach (string kw in TypewriterManager.Instance.AvailableKeywords)
        {
            if (_chips.ContainsKey(kw)) continue;

            KeywordSlot slot = GetFreeSlot();

            KeywordChipUI chip = Instantiate(keywordChipUIPrefab, slot.transform);
            chip.Setup(kw);
            chip.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-chipTiltRange, chipTiltRange));
            chip.SnapToSlot(slot);

            _chips.Add(kw, chip);
        }
    }

    // Primer slot libre; si no queda ninguno, se instancia uno nuevo
    private KeywordSlot GetFreeSlot()
    {
        foreach (KeywordSlot slot in _slots)
        {
            if (!slot.IsOccupied) return slot;
        }
        return CreateSlot();
    }

    private KeywordSlot CreateSlot()
    {
        KeywordSlot slot = Instantiate(keywordSlotPrefab, keywordsContainer);
        _slots.Add(slot);
        return slot;
    }
}
