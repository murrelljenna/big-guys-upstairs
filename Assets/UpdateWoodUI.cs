using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateWoodUI : MonoBehaviour
{
	res Player;
	public Text text;
    void Start()
    {
        Player = GameObject.Find("Player").GetComponent<res>();
    }

    void Update()
    {
    	text.text = Player.wood.ToString() + " (+" + Player.woodIt.ToString() + ")";
    }
}
