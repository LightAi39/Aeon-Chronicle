using System.Collections.Generic;
using UnityEngine;

public class GlowHighlight : MonoBehaviour
{
    private Dictionary<Renderer, Material[]> glowMaterialDictionary = new();
    private Dictionary<Renderer, Material[]> originalMaterialDictionary = new();
    private Dictionary<Color, Material> cachedGlowMaterials = new();

    private Renderer[] _renderers;

    public Material glowMaterial;

    private bool isGlowing = false;

    private Color validSpaceColor = Color.green;
    private Color originalGlowColor;
    
    private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");

    private void Awake()
    {
        PrepareMaterialDictionaries();
        originalGlowColor = glowMaterial.GetColor(GlowColor);
    }

    private void PrepareMaterialDictionaries()
    {
        _renderers = GetComponentsInChildren<Renderer>();

        foreach (var renderer in _renderers)
        {
            Material[] originalMaterials = renderer.sharedMaterials; // Avoid using renderer.materials here
            originalMaterialDictionary.Add(renderer, originalMaterials);
            
            Material[] glowMaterials = new Material[renderer.sharedMaterials.Length + 1];

            for (int i = 0; i < originalMaterials.Length; i++)
            {
                glowMaterials[i] = originalMaterials[i];
            }

            // Create a unique instance of the glow material for each renderer
            Material uniqueGlowMaterial = new Material(glowMaterial);
            glowMaterials[^1] = uniqueGlowMaterial;

            glowMaterialDictionary.Add(renderer, glowMaterials);
        }
    }

    public void ToggleGlow()
    {
        if (isGlowing == false)
        {
            ResetGlowHighlight();
            foreach (var renderer in originalMaterialDictionary.Keys)
            {
                renderer.materials = glowMaterialDictionary[renderer];
            }
        }
        else
        {
            foreach (var renderer in originalMaterialDictionary.Keys)
            {
                renderer.materials = originalMaterialDictionary[renderer];
            }
        }

        isGlowing = !isGlowing;
    }

    public void ToggleGlow(bool state)
    {
        if (isGlowing == state) return;
        isGlowing = !state;
        ToggleGlow();
    }

    public void ResetGlowHighlight()
    {
        foreach (var renderer in glowMaterialDictionary.Keys)
        {
            foreach (var item in glowMaterialDictionary[renderer])
            {
                if (item.HasProperty(GlowColor)) // Check if the material has the property
                {
                    item.SetColor(GlowColor, originalGlowColor);
                }
            }
        }
    }

    public void HighlightValidPath()
    {
        if (isGlowing == false)
            return;

        foreach (var renderer in glowMaterialDictionary.Keys)
        {
            foreach (var item in glowMaterialDictionary[renderer])
            {
                if (item.HasProperty(GlowColor)) // Check if the material has the property
                {
                    item.SetColor(GlowColor, validSpaceColor);
                }
            }
        }
    }
}
