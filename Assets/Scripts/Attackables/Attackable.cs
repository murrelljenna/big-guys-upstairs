using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using game.assets.utilities.resources;

public class Attackable : MonoBehaviourPunCallbacks, IPunObservable
{
    public Animator animator;
    [SerializeField]
    protected ownership owner;
 
    public bool canAttack = true;

    public string prefabName;

    public ResourceSet cost = new ResourceSet();

    public int woodCost = 0;
    public int foodCost = 0;

    public int maxHP;
    public int lastHP;

    protected GameObject canvas;
    protected SimpleHealthBar healthBar;

    public string color;
    [SerializeField]
    public int hp;
    public int id;
    public bool dead = false;
    public List<Unit> attackers;
    public PhotonView photonView;
    protected Rigidbody rigidBody;
    public GameObject info;

    protected TooltipController tooltips;
    //protected bool underAttack;
    
    // UI Animation
    protected bool midAnimation;
    protected GameObject up1;
    protected GameObject down1;
    protected GameObject up2;
    protected GameObject down2;
    protected GameObject up3;
    protected GameObject down3;

    protected virtual void Start()
    {
        if (this.gameObject.tag == "buildingGhost") {
            this.enabled = false;
        }

        this.rigidBody = this.GetComponent<Rigidbody>();

        GameObject tooltipsGameObj = GameObject.Find("Tooltips");
        if (tooltipsGameObj != null) {
            tooltips = tooltipsGameObj.GetComponent<TooltipController>();
        }
    }

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
        Transform model = this.transform.Find("Model");
        if (model != null) {
            animator = model.gameObject.GetComponent<Animator>();
        }

        base.OnEnable();
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
        base.OnDisable();
    }

    public virtual void Awake() {
        owner = GetComponent<ownership>();
        photonView = PhotonView.Get(this);

        this.id = this.photonView.ViewID;
        this.gameObject.name = id.ToString();

        Transform canvasTransform = this.gameObject.transform.Find("Healthbar");

        if (canvasTransform != null) {
            Debug.Log(this.prefabName);
            canvas = canvasTransform.gameObject;
            healthBar = canvas.transform.Find("Simple Bar").transform.Find("Status Fill 01").GetComponent<SimpleHealthBar>();
            canvas.SetActive(false);
        }
    }

    [PunRPC]
    public void takeDamageRPC(int damage) {
        takeDamage(damage);
    }

    // Update is called once per frame
    public virtual void Update()
    {


        if (this.hp != this.maxHP && this.hp > 0) {
            canvas.SetActive(true);
        } else {
            canvas.SetActive(false);
        }
    }

    public virtual void onCapture() {}

    public virtual void onDeCapture() {}

    public virtual void takeDamage(int damage) {
        this.hp -= damage;

        healthBar.UpdateBar(this.hp, this.maxHP);

        if (hp <= 0 && !this.dead) {
            destroyObject();
        }

        modifySpeed(0.5f); // Half speed while receiving damage
        Invoke("doubleSpeed", 1f);
    }

    public virtual void destroyObject() {
        CancelInvoke();
        if (this.photonView.IsMine) {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
 
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
        }
        else
        {
        }
    }

    private void modifySpeed(float modifier) {
        UnityEngine.AI.NavMeshAgent navMeshAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (navMeshAgent != null) {
            navMeshAgent.speed = navMeshAgent.speed * modifier;
        }
    }

    private void doubleSpeed() { // Parameterless for invoke
        modifySpeed(2);
    }

    /* Run every frame that the entity is being looked at */
    public virtual void interactionOptions(game.assets.Player player) {

    } 

    protected bool townInRange(Vector3 location, float range) {       
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].tag == "town") {
                return true;
            }
        }

        return false;
    }

    protected bool townInRange(Vector3 location, float range, int ownerID) {
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].tag == "town" && hitColliders[i].gameObject.GetComponent<ownership>().owner == ownerID) { // If there is a town in range that belongs to the player.
                return true;
            }
        }

        return false;
    }

    void releaseButton1() {
        up1.SetActive(true);
        down1.SetActive(false);
        midAnimation = false;
    }

    void releaseButton2() {
        up2.SetActive(true);
        down2.SetActive(false);
        midAnimation = false;
    }

    void releaseButton3() {
        up3.SetActive(true);
        down3.SetActive(false);
        midAnimation = false;
    }

}
