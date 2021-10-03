using System.Collections;
using System.Collections.Generic;
using game.assets.ai;
using UnityEngine;

public class CommandUICardController : MonoBehaviour
{
    public Health unit;
    private SimpleHealthBar healthBar;

    void OnEnable()
    {
        healthBar = this.transform.Find("Simple Bar").Find("Status Fill 01").gameObject.GetComponent<SimpleHealthBar>();
    }

    void Update()
    {
        if (this.unit != null)
        {
            healthBar.UpdateBar(this.unit.HP, this.unit.maxHP);
        }
    }
}
