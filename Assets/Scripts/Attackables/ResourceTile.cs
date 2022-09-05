using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using game.assets.utilities; 
using game.assets.utilities.resources; 

public class ResourceTile : Attackable {    
    
    GameObject banner;
    ownership ownerInfo;
    public ResourceSet resourceSetYield;

    private GameObjectSearcher searcher;

    protected int maxUpgrade = 3;

    protected int upgradeCostStone = 0;
    protected int upgradeCostGold = 0;
    protected int upgradeCostWood = 0;
    protected int upgradeCostFood = 0;
    protected int upgradeCostIron = 0;

    public int maxWorkers = 3;
    public List<Militia> workers = new List<Militia>();

    public Depositor upstream;

    List<GameObject> nodes = new List<GameObject>();

    public bool addWorker(Militia militia) {
        if (workers.Count < maxWorkers) {
            militia.clearAssignment();
            workers.Add(militia);
            militia.assignResourceTile(this);
            militia.startCollectingResources(getNode(), resourceSetYield);
            updateWorkerUI();
            return true;
        }
        return false;
    }

    private void updateWorkerUI() {
        this.info.transform.Find("workerMax").Find("workerMaxText").GetComponent<Text>().text = this.workers.Count.ToString() + "/" + this.maxWorkers.ToString();
    }

    public void removeWorker(Militia militia) {
        workers.Remove(militia);
        updateWorkerUI();
    }

    public override void Update() {
        base.Update();
    }

    protected override void Start()
    {
        //updateWorkerUI();
        searcher = this.GetComponent<GameObjectSearcher>();
        this.woodCost = 15;
        this.foodCost = 15;
        ownerInfo = this.gameObject.GetComponent<ownership>();
        ownerInfo.owned = false;
        this.id = this.gameObject.GetComponent<PhotonView>().ViewID;
        this.gameObject.name = id.ToString();
        this.banner = this.transform.Find("Banner").gameObject;
        this.banner.SetActive(false);

        Transform infoTransform = this.gameObject.transform.Find("Info");
        if (infoTransform != null) {
            if (infoTransform.gameObject != null) {
                info = infoTransform.gameObject;
                info.SetActive(false);
            }
        }

        animator = banner.GetComponent<Animator>();

        base.Start();
    }

    public override void onCapture() {
        this.transform.Find("Info").Find("1_Normal").Find("Text").GetComponent<Text>().text = '\u2714'.ToString();
        this.transform.Find("Info").Find("1_Pressed").Find("Text").GetComponent<Text>().text = '\u2714'.ToString();
        this.transform.Find("Info").Find("Text").GetComponent<Text>().text = "Captured";
        StartCoroutine(checkOwnerAndCapture());
    }

    public override void onDeCapture() {
        destroyObject();
    }

    private IEnumerator checkOwnerAndCapture() {
        yield return new WaitUntil(() => {
            return (GameObject.Find(ownerInfo.owner.ToString()) != null);
        });

        game.assets.Player player = GameObject.Find(ownerInfo.owner.ToString()).GetComponent<game.assets.Player>();

        banner.SetActive(true);
        banner.transform.Find("TT_Flag_A").gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Banner_" + owner.getPlayer().colorName) as Texture));

        this.upstream = ObjFinder.findClosestCity(transform.position, player).GetComponent<Depositor>();
    }

    public override void destroyObject() {
        if (attackers.Count > 0) {
            for (int i = attackers.Count - 1; i >= 0; i--) {
                attackers[i].cancelOrders();
            };
        }

        playDestructionEffect();
        animator.SetTrigger("destroy");
        Invoke("clearBanner", 1.5f);

        this.transform.Find("Info").Find("1_Normal").Find("Text").GetComponent<Text>().text = "E";
        this.transform.Find("Info").Find("1_Pressed").Find("Text").GetComponent<Text>().text = "E";
        this.transform.Find("Info").Find("Text").GetComponent<Text>().text = "Capture";

        ownerInfo.owned = false;
        ownerInfo.owner = 0;
        this.hp = this.maxHP;
    }

    public override void takeDamage(int damage) {
        if (this.GetComponent<ownership>().owned) { // Unowned resource tiles should not take damage.
            base.takeDamage(damage);
        }
    }

    public void clearBanner() {
        banner.SetActive(false);
    }

    private void playDestructionEffect() {
        AudioSource[] sources = this.transform.Find("DestroySounds").GetComponents<AudioSource>();
        AudioSource source = sources[UnityEngine.Random.Range(0, sources.Length)];
        AudioSource.PlayClipAtPoint(source.clip, this.transform.position);
    }
    public virtual GameObject getNode() {
        int index = UnityEngine.Random.Range(0, searcher.actors.Count);
        return searcher.actors[index];
        
    }
/*

    private void getClosestCity() {

    }
*/
}
