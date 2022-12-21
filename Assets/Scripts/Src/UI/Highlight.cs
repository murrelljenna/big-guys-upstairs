using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    //we assign all the renderers here through the inspector
    private List<Renderer> renderers;

    [SerializeField]
    private Color color = Color.white;

    //helper list to cache all the materials ofd this object
    private List<Material> materials;

    public bool isEnabled = true;

    //Gets all the materials from each renderer
    private void Awake()
    {
        if (!isEnabled)
        {
            return;
        }
        materials = new List<Material>();
        renderers = new List<Renderer>(transform.Find("Model").GetComponentsInChildren<Renderer>());
        foreach (var renderer in renderers)
        {
            //A single child-object might have mutliple materials on it
            //that is why we need to all materials with "s"
            materials.AddRange(new List<Material>(renderer.materials));
        }

        // Code above is tanking the framerate because its holding on to thousands of renderers

        if (materials.Count > 5)
        {
            isEnabled = false;
            materials.Clear();
            renderers.Clear();
        }
    }

    public void ToggleHighlight(bool val)
    {
        if (!isEnabled)
        {
            return;
        }
        if (val)
        {
            foreach (var material in materials)
            {
                //We need to enable the EMISSION
                material.EnableKeyword("_EMISSION");
                //before we can set the color
                material.SetColor("_EmissionColor", color);
            }
        }
        else
        {
            foreach (var material in materials)
            {
                //we can just disable the EMISSION
                //if we don't use emission color anywhere else
                material.DisableKeyword("_EMISSION");
            }
        }

    }
}
