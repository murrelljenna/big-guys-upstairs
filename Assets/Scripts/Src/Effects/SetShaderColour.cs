using game.assets;
using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projector))]
public class SetShaderColour : MonoBehaviour
{
    public Ownership ownership;
    public void Start()
    {
        if (ownership == null)
        {
            return;
        }
        var projector = GetComponent<Projector>();
        var color = ownership.owner.colour.color;
        projector.material.SetColor("_Color", color);
    }

    public void SetColour(Color color)
    {
        GetComponent<Projector>().material.SetColor("_Color", color);
    }
}
