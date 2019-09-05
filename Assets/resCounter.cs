using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resCounter : MonoBehaviour
{
	GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
    	Player = GameObject.Find("Player");
        InvokeRepeating("iterateResources", 15f, 15f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void iterateResources() {
    	Player.GetComponent<res>().iterate();
    }
}
