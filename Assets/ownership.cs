using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ownership : MonoBehaviour
{
	public bool owned;
    // Start is called before the first frame update
    void Start()
    {
        owned = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void capture() {
    	owned = true;
    }
}
