/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: anima las keywords saliendo del texto y volando al botón de la máquina.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeywordSequenceController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private KeywordChip_UI keywordChipPrefab;
    [SerializeField] private RectTransform typewriterHUDTarget;

    [Header("World Camera")]
    [SerializeField] private Camera worldCamera;

    [Header("Anim")]
    [SerializeField] private float appearDuration = 0.35f;
    [SerializeField] private float flyToHUDDuration = 0.6f;
    [SerializeField] private float staggerDelay = 0.25f;

    public IEnumerator PlaySequence(TMP_Text sourceText, Transform chipParent, List<string> keywords, System.Action onBannerFadeRequested)
    {
        if (keywords == null || keywords.Count == 0) yield break;

        sourceText.ForceMeshUpdate();

        List<KeywordChip_UI> spawnedChips = new List<KeywordChip_UI>();

        foreach (string keyword in keywords)
        {
            Vector3 keywordWorldPos = GetKeywordWorldPosition(sourceText, keyword);

            KeywordChip_UI chip = Instantiate(keywordChipPrefab, chipParent);
            chip.RectTransform.position = ToChipPosition(sourceText, keywordWorldPos, chipParent);
            chip.Setup(keyword);

            spawnedChips.Add(chip);
        }

        onBannerFadeRequested?.Invoke();

        foreach (KeywordChip_UI chip in spawnedChips)
        {
            chip.PlayAppearAnimation(appearDuration);
        }

        yield return new WaitForSeconds(appearDuration);

        Vector3 targetHUDPos = typewriterHUDTarget != null ? typewriterHUDTarget.position : Vector3.zero;
        TypewriterButton_HUD hudReceiver = typewriterHUDTarget != null ? typewriterHUDTarget.GetComponent<TypewriterButton_HUD>() : null;

        for (int i = 0; i < spawnedChips.Count; i++)
        {
            KeywordChip_UI chip = spawnedChips[i];
            string keyword = keywords[i];

            chip.PlayFlyToHUDAnimation(targetHUDPos, flyToHUDDuration, () =>
            {
                TypewriterManager.Instance?.AddKeyword(keyword);

                hudReceiver?.OnKeywordReceived();
            });

            if (i < spawnedChips.Count - 1)
            {
                yield return new WaitForSeconds(staggerDelay);
            }
        }

        yield return new WaitForSeconds(flyToHUDDuration);
    }

    private Vector3 ToChipPosition(TMP_Text sourceText, Vector3 worldPos, Transform chipParent)
    {
        if (sourceText is TextMeshProUGUI) return worldPos;

        Camera sceneCamera = worldCamera != null ? worldCamera : Camera.main;
        Vector2 screenPoint = sceneCamera.WorldToScreenPoint(worldPos);

        RectTransform parentRect = (RectTransform)chipParent;
        Canvas rootCanvas = parentRect.GetComponentInParent<Canvas>().rootCanvas;
        Camera uiCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, screenPoint, uiCamera, out Vector3 chipWorldPos);
        return chipWorldPos;
    }

    private Vector3 GetKeywordWorldPosition(TMP_Text tmpText, string keyword)
    {
        TMP_TextInfo textInfo = tmpText.textInfo;
        string fullText = tmpText.text;
        int startIndex = fullText.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase);

        if (startIndex == -1 || textInfo.characterCount == 0)
            return tmpText.transform.position;

        int endIndex = Mathf.Min(startIndex + keyword.Length - 1, textInfo.characterCount - 1);

        TMP_CharacterInfo firstChar = textInfo.characterInfo[startIndex];
        TMP_CharacterInfo lastChar = textInfo.characterInfo[endIndex];

        Vector3 bottomLeft = tmpText.transform.TransformPoint(firstChar.bottomLeft);
        Vector3 topRight = tmpText.transform.TransformPoint(lastChar.topRight);

        return (bottomLeft + topRight) * 0.5f;
    }
}
