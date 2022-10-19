using game.assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projector))]
public class SetShaderColour : MonoBehaviour
{
    public void Start()
    {
        var projector = GetComponent<Projector>();
        var color = LocalGameManager.Get().getLocalPlayer().colour.color;
        projector.material.SetColor("Tint Color", color);
    }
}
