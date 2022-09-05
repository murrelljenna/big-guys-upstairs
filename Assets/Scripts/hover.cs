using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hover : MonoBehaviour
{
    void Update() {
        this.transform.position = this.transform.parent.transform.position + (Vector3.up * Mathf.Cos(Time.time) / 10);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
    }
}
