using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ownership : MonoBehaviour
{
	public bool owned;
    [SerializeField]
    public game.assets.Player owner;
    // Start is called before the first frame update
    void Start()
    {
        owned = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void capture(game.assets.Player player) {
    	owned = true;
        this.owner = player;
    }
}
