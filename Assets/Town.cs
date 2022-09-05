using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Town : Attackable, IPunObservable
{
    private int lastNoEnemies = 0;

    private bool underAttack = false;

    int resourceMask = 1 << 9;
    int unitMask = 1 << 12;

    void Start() {

        this.hp = 200;
        this.woodCost = 75;
        this.foodCost = 25;

        InvokeRepeating("checkEnemiesInRadius", 2f, 2f);

        base.Start();
    }

    private void checkEnemiesInRadius() {
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Renderer>().bounds.center, 10f, unitMask);
        int attackers = 0;

        if (hitColliders.Length != lastNoEnemies) {
            for (int i = 0; i < hitColliders.Length; i++) {
                if (hitColliders[i] != null && hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner) {
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

    private void callForHelp() {
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Renderer>().bounds.center, 10f, unitMask);

        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i] != null && hitColliders[i].GetComponent<ownership>().owner == this.gameObject.GetComponent<ownership>().owner && hitColliders[i].GetComponent<Unit>().isAttacking == false) {
                hitColliders[i].GetComponent<Unit>().checkEnemiesInRange(10f);
            }
        }
    }

    public override void destroyObject() {
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Renderer>().bounds.center, 10f, resourceMask);

        if (hitColliders.Length != lastNoEnemies) {
        for (int i = 0; i < hitColliders.Length; i++) {
                if (hitColliders[i].GetComponent<ownership>().owned == true) {
                    hitColliders[i].gameObject.GetComponent<ownership>().deCapture();
                }
            }
        }

        AudioSource[] sources = this.transform.Find("DestroySounds").GetComponents<AudioSource>();
        AudioSource source = sources[UnityEngine.Random.Range(0, sources.Length)];
        AudioSource.PlayClipAtPoint(source.clip, this.transform.position);

        base.destroyObject();
    }

    public override void takeDamage(int damage) {
        AudioSource[] sources = this.transform.Find("DamageSounds").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));

        base.takeDamage(damage);
    }

    // Start is called before the first frame update
    public override void Awake() {
        base.Awake();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(this.hp);
        }
        else
        {
            this.hp = (int)stream.ReceiveNext();
        }
    }
}
