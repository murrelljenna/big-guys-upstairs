using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Attackable : MonoBehaviourPunCallbacks, IPunObservable
{
    public Animator animator;
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

	public int hp;
	public int id;
	public List<Unit> attackers;
	public PhotonView photonView;
	List<Color> colours = new List<Color>() { Color.black, Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow };

    // Start is called before the first frame update
    public virtual void Start()
    {
    	if (this.gameObject.tag == "buildingGhost") {
    		this.enabled = false;
    	}
    	this.maxHP = hp;
    }

    public virtual void OnEnable() {
        Transform model = this.transform.Find("Model");
        if (model != null) {
            animator = model.gameObject.GetComponent<Animator>();
        }
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
    		playerCamera = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>(); 
    	}

    	if (this.hp != this.maxHP) {
    		canvas.SetActive(true);
    	}

        if (this.hp != this.lastHP) {
            healthBar.UpdateBar(this.hp, this.maxHP);
        }

        if (hp <= 0) {
            destroyObject();
        }
    }

    public virtual void onCapture() {

    }

    public virtual void onDeCapture() {

    }

    public virtual void takeDamage(int damage) {
    	this.hp -= damage;

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
            stream.SendNext(hp);
        }
        else
        {
            hp = (int)stream.ReceiveNext();
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
}
