using System.Collections;
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

    private void Awake()
    {
        PrepareMaterialDictionaries();
    }

    private void PrepareMaterialDictionaries()
    {
        _renderers = GetComponentsInChildren<Renderer>();

        foreach (var renderer in _renderers)
        {
            Material[] originalMaterials = renderer.materials;
            originalMaterialDictionary.Add(renderer, originalMaterials);
            
            Material[] glowMaterials = new Material[renderer.materials.Length + 1];

            for (int i = 0; i < originalMaterials.Length; i++)
            {
                glowMaterials[i] = originalMaterials[i];
            }
            glowMaterials[^1] = glowMaterial;
            glowMaterialDictionary.Add(renderer, glowMaterials);
        }
    }

    public void ToggleGlow()
    {
        if (isGlowing == false)
        {
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
}
