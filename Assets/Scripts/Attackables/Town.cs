 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;
using UnityEngine.Events;

public class Town : Building, IPunObservable
{
    private int yield = 1;
    private int lastNoEnemies = 0;

    private int noBuildings = 0;
    private int maxBuildings = 5;

    private bool underAttack = false;

    private int resourceMask = 1 << 9;
    private int unitMask = 1 << 12;

    private Text buildingMaxText;

    protected override void Start() {
        prefabName = "Town";

        buildingMaxText = this.transform.Find("Info").Find("buildingMax").Find("buildingMaxText").GetComponent<Text>();
        setBuildingCounts();

        this.hp = 600;
        this.woodCost = 50;
        this.foodCost = 50;
        this.canBeRecycled = false;

        this.upgradeCostStone = 25;
        this.upgradeCostGold = 0;
        this.upgradeCostWood = 25;
        this.upgradeCostFood = 0;
        this.upgradeCostIron = 0;

        this.transform.Find("Info").Find("Upgrade").Find("Upgrade_Stone_Cost").GetComponent<Text>().text = upgradeCostStone.ToString();
        this.transform.Find("Info").Find("Upgrade").Find("Upgrade_Wood_Cost").GetComponent<Text>().text = upgradeCostStone.ToString();

        if (!this.photonView.IsMine) {
            this.GetComponent<buildingGhost>().active = false;
        }

        InvokeRepeating("checkEnemiesInRadius", 2f, 2f);

        base.Start();
    }

    public override void onCapture() {
        game.assets.Player player = owner.getPlayer();
        this.gameObject.GetComponent<LineRenderer>().SetColors(owner.playerColor, owner.playerColor);
        this.gameObject.GetComponent<ownership>().getPlayer().upCityCount(1);

        base.onCapture();
    }

    private void checkEnemiesInRadius() {
        if (!canAttack) {
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Renderer>().bounds.center, 10f, unitMask);
        int attackers = 0;

        if (hitColliders.Length != lastNoEnemies) {
            for (int i = 0; i < hitColliders.Length; i++) {
                if (hitColliders[i] != null && hitColliders[i].tag != "buildingGhost" && hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner) {
                    if (underAttack == false) {
                        // Being attacked first time - play noise without being annoying.
                        this.gameObject.GetComponent<AudioSource>().Play(0);
                    }

                    attackers+=1;
                    underAttack = true;
                    callForHelp();
                    break;
                }
            }

            if (attackers < 1) {
                underAttack = false;
            }
        }
    }

    public bool maxedBuildings() {
        return (noBuildings >= maxBuildings);
    }

    private void setBuildingCounts() {
        buildingMaxText.text = noBuildings + "/" + maxBuildings;
    }

    public void addBuilding() {
        noBuildings++;
        setBuildingCounts();
    }

    public void removeBuilding() {
        noBuildings--;
        setBuildingCounts();
    }

    private void callForHelp() {
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Renderer>().bounds.center, 10f, unitMask);

        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i] != null && hitColliders[i].GetComponent<ownership>().owner == this.gameObject.GetComponent<ownership>().owner && hitColliders[i].GetComponent<Unit>().isAttacking == false) {
                hitColliders[i].GetComponent<Unit>().checkEnemiesInRange(10f);
            }
        }
    }

    public override void destroyObject() {
        if (this.photonView.IsMine) {
            game.assets.Player player = owner.getPlayer();

            Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Renderer>().bounds.center, 10f, resourceMask);

            if (hitColliders.Length != lastNoEnemies) {
            for (int i = 0; i < hitColliders.Length; i++) {
                    if (hitColliders[i].GetComponent<ownership>() != null && hitColliders[i].GetComponent<ownership>().owned == true) {
                        hitColliders[i].gameObject.GetComponent<ownership>().deCapture();
                    }
                }
            }

            this.gameObject.GetComponent<ownership>().getPlayer().loseCity();
        }

        base.destroyObject();
    }

    // Called every frame the player is looking at this gameobject.

    public override void interactionOptions(game.assets.Player player) {
        GameObject cityViewed = this.transform.Find("Info").gameObject;
        cityViewed.SetActive(true);

        if (!midAnimation) {
            cityViewed.transform.Find("1_Pressed").gameObject.SetActive(false);
        }

        if (!underConstruction) {
            if (Input.GetKeyDown(KeyCode.E)) {
                if (!player.maxedUnits()) {
                    int wood = 0; // Please replace with real values soon.
                    int food = 5;

                    up1 = cityViewed.transform.Find("1_Normal").gameObject;
                    down1 = cityViewed.transform.Find("1_Pressed").gameObject;

                    up1.SetActive(false);
                    down1.SetActive(true);
                    midAnimation = true;
                    Invoke("releaseButton1", 0.2f);

                    if (player.canAfford(wood, food)) {
                        player.makeTransaction(wood, food);

                        /* Instantiate new militia outside city */

                        Vector2 randomInCircle = RandomPointOnUnitCircle(1.2f);
                        Vector3 spawnLocation = new Vector3(randomInCircle.x+this.transform.position.x, this.transform.position.y, randomInCircle.y+this.transform.position.z);

                        GameObject militia = PhotonNetwork.Instantiate("Militia", spawnLocation, Quaternion.identity, 0);

                        militia.GetComponent<ownership>().capture(player);
                    } else {
                        tooltips.flashLackResources();
                    }
                } else {
                    
                }
            } 

            if (Input.GetKeyDown(KeyCode.U)) {
                up2 = cityViewed.transform.Find("Upgrade").Find("2_Normal").gameObject;
                down2 = cityViewed.transform.Find("Upgrade").Find("2_Pressed").gameObject;

                up2.SetActive(false);
                down2.SetActive(true);
                midAnimation = true;
                Invoke("releaseButton2", 0.2f);

                if (player.canAfford(upgradeCostWood, upgradeCostFood, upgradeCostGold, upgradeCostStone, upgradeCostIron) && this.upgradeLevel < this.maxUpgrade){
                    player.makeTransaction(upgradeCostWood, upgradeCostFood, upgradeCostGold, upgradeCostStone, upgradeCostIron);
                    photonView.RPC("upgrade", RpcTarget.AllBuffered);
                } else {
                    tooltips.flashLackResources();
                }
            }
        }

        base.interactionOptions(player);
    }

    [PunRPC]
    public override void upgrade() {
        game.assets.Player player = owner.getPlayer();
        this.transform.Find("Model").gameObject.SetActive(false);
        this.transform.Find("Model2").gameObject.SetActive(false);
        this.transform.Find("Model3").gameObject.SetActive(false);

        Transform model = this.transform.Find("Model" + (upgradeLevel + 1).ToString());
        if (model != null) {
            model.gameObject.SetActive(true);
        }
        maxBuildings+=3;
        maxHP+=(maxHP/2);
        hp=maxHP;

        buildingMaxText.text = maxBuildings.ToString();

        this.yield++;
        base.upgrade();
    }

    public override void Awake() {
        base.Awake();
    }
}
