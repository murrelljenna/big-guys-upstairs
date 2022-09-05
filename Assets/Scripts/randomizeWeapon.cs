using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomizeWeapon : MonoBehaviour
{
	string[] weapons = new string[3];

    void Start()
    {

    	weapons[0] = "w_axe_A";
		weapons[1] = "w_dagger_A";
		weapons[2] = "w_pick";

    	int chosenWeapon  = Random.Range(0, weapons.Length);  

    	for (int i = 0; i < weapons.Length; i++) {
    		if (i != chosenWeapon) {
        		this.transform.Find(weapons[i]).gameObject.SetActive(false);
        	}
    	}
    }
}
