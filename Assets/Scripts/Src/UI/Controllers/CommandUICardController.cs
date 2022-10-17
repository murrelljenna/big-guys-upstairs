using System.Collections;
using System.Collections.Generic;
using game.assets.ai;
using UnityEngine;

public class CommandUICardController : MonoBehaviour
{
    private Health unit;
    private SimpleHealthBar healthBar;

    void OnEnable()
    {
        healthBar = this.transform.Find("Simple Bar").Find("Status Fill 01").gameObject.GetComponent<SimpleHealthBar>();
        if (this.unit != null)
        {
            this.unit.onLowerHP.AddListener(updateHealth);
            this.unit.onRaiseHP.AddListener(updateHealth);
            updateHealth(unit.HP, unit.maxHP);
        }
    }

    public void setUnit(Health unit)
    {
        if (this.unit != null)
        {
            this.unit.onLowerHP.RemoveListener(updateHealth);
            this.unit.onRaiseHP.RemoveListener(updateHealth);
        }
        this.unit = unit;

        updateHealth(unit.HP, unit.maxHP);
    }

    public Health getUnit()
    {
        return unit;
    }

    private void updateHealth(float current, float max) {
        Debug.Log("Unit " + gameObject.name + " has taken damage, now at " + current);
        healthBar.UpdateBar(current, max);
    }
}
