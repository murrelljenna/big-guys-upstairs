using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Attackable : MonoBehaviourPunCallbacks, IPunObservable
{
    public Animator animator;
    [SerializeField]
    protected ownership owner;
	protected GameObject info;
	public bool canAttack = true;

	public string prefabName;
	protected Camera playerCamera;

	public int woodCost = 0;
	public int foodCost = 0;

	public int maxHP;
	public int lastHP;

	private GameObject canvas;
    private SimpleHealthBar healthBar;

    public string color;
    [SerializeField]
	public int hp;
	public int id;
    public bool dead = false;
	public List<Unit> attackers;
	public PhotonView photonView;
    List<Color> colours = new List<Color>() { Color.black, Color.blue, Color.white, Color.green, Color.magenta, Color.red, Color.yellow };
    List<string> colourStrings = new List<string>() { "black", "blue", "white", "green", "pink", "red", "yellow" };

    protected TooltipController tooltips;
    
    // UI Animation
    protected bool midAnimation;
    protected GameObject up1;
    protected GameObject down1;
    protected GameObject up2;
    protected GameObject down2;

    // Start is called before the first frame update
    public virtual void Start()
    {
    	if (this.gameObject.tag == "buildingGhost") {
    		this.enabled = false;
    	}
    	this.maxHP = hp;

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

        Transform canvasTransform = this.gameObject.transform.Find("Canvas");

        if (canvasTransform != null) {
        	canvas = this.gameObject.transform.Find("Canvas").gameObject;
        	healthBar = canvas.transform.Find("Simple Bar").transform.Find("Status Fill 01").GetComponent<SimpleHealthBar>();
        	canvas.SetActive(false);
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
    	if (playerCamera != null) {
        	canvas.transform.LookAt(transform.position + playerCamera.transform.rotation * Vector3.forward, playerCamera.transform.rotation * Vector3.up);   
    	} else {
    		playerCamera = getLocalCamera();
    	}

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

    public Camera getLocalCamera() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++) {
            if (players[i].GetComponent<PhotonView>().IsMine) {
                return players[i].transform.Find("FPSController").transform.Find("FirstPersonCharacter").GetComponent<Camera>();
            }
        }
        return null;
    }

    /* Run every frame that the entity is being looked at */
    public virtual void interactionOptions(game.assets.Player player) {

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

}
