using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    // Start is called before the first frame update
	private Animator animator;
	// Use this for initialization
	void Start()
	{
	    animator = GetComponent<Animator>();
	    if (!animator)
	    {
	        Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
	    }
	}

    // Update is called once per frame
	void Update()
	{
	    if (!animator)
	    {
	        return;
	    }

	    float h = Input.GetAxis("Horizontal");
	    float v = Input.GetAxis("Vertical");

	    if (v < 0)
	    {
	        v = 0;
	    }
	    
	    animator.SetFloat("Speed", h * h + v * v);
	}
}
