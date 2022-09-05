using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class syncHealth : MonoBehaviour
{
    public Unit unit;
    private SimpleHealthBar healthBar;

    // Start is called before the first frame update
    void OnEnable() {
        healthBar = this.transform.Find("Simple Bar").Find("Status Fill 01").gameObject.GetComponent<SimpleHealthBar>();
    }

    // Update is called once per frame
    void Update() {
        if (this.unit != null) {
            healthBar.UpdateBar(this.unit.hp, this.unit.maxHP);
        }
    }
}
