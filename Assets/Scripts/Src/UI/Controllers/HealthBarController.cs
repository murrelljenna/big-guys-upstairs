using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    private GameObject healthbarObject;
    private SimpleHealthBar healthbar;

    public int maxHP;
    public int hp;
    public int lastHP;

    void Awake()
    {
        this.healthbarObject = this.transform.Find("Healthbar").gameObject;
        this.healthbar = this.healthbarObject.transform.Find("Simple Bar").Find("Status Fill 01").GetComponent<SimpleHealthBar>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void Update()
    {
        if (this.hp != this.maxHP && this.hp > 0)
        {
            this.healthbarObject.SetActive(true);
        }
        else
        {
            this.healthbarObject.SetActive(false);
        }
    }
}
