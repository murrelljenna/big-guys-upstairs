using game.assets;
using game.assets.ai;
using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Health))]
public class Construction : MonoBehaviour
{
    [Tooltip("Prefab created when construction finished")]
    public GameObject onceBuilt;

    [Tooltip("Invoked when construction finished")]
    public UnityEvent built;

    private Health health;
    void Start()
    {
        health = GetComponent<Health>();
        health.onMaxHP.AddListener(finish);

        setToFirstModel();
    }

    public void build(int amt)
    {
        health.raiseHP(amt);

        if (health.HP > (health.maxHP / 2))
        {
            setToSecondModel();
        }
    }

    private void finish() {
        IInstantiator instantiator = InstantiatorFactory.getInstantiator(false);
        instantiator.InstantiateAsPlayer(onceBuilt, transform.position, transform.rotation, GetComponent<Ownership>().owner);
        built.Invoke();
        Destroy(this);
    }

    private void setToFirstModel()
    {
        transform.Find("Construction_0").gameObject.SetActive(true);
        transform.Find("Construction_1").gameObject.SetActive(false);
    }

    private void setToSecondModel()
    {
        transform.Find("Construction_0").gameObject.SetActive(false);
        transform.Find("Construction_1").gameObject.SetActive(true);
    }
}
