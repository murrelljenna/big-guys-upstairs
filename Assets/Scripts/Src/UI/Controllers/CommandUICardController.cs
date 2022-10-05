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
        if (this.unit != null)
        {
            this.unit.onLowerHP.AddListener(updateHealth);
            this.unit.onRaiseHP.AddListener(updateHealth);
        }
    }

    private void updateHealth(float current, float max) {
        healthBar.UpdateBar(current, max);
    }
}
