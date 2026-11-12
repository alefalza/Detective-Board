using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArticleVariant
{
    [SerializeField] private ToneData tone;
    [Tooltip("Artículo completo en este tono. Admite rich text (ej. <b>keyword</b>).")]
    [TextArea(6, 20)] [SerializeField] private string text;

    public ToneData Tone => tone;
    public string Text => text;
}

[CreateAssetMenu(fileName = "NewArticle", menuName = "Detective/Article")]
public class ArticleData : ScriptableObject
{
    [Tooltip("Una variante por tono. La primera es la que se muestra al pasar el artículo en limpio.")]
    [SerializeField] private List<ArticleVariant> variants = new List<ArticleVariant>();

    public ToneData DefaultTone => variants[0].Tone;

    public string GetText(ToneData tone)
    {
        foreach (ArticleVariant variant in variants)
        {
            if (variant.Tone == tone) return variant.Text;
        }

        Debug.LogWarning($"[ArticleData] '{name}' no tiene variante para el tono '{tone.name}'.", this);
        return string.Empty;
    }
}
