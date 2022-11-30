using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Ghost : MonoBehaviour
{
    private Color previousColor;
    private Renderer ren;
    public bool colliding = false;


    void Start()
    {
        Transform transrights = transform.Find("Model");

        if (transrights == null) {
            Debug.LogError("This ghost doesn't have a model, put one there idiot. Won't work until it does.");
            return;
        }

        ren = transrights.gameObject.GetComponent<MeshRenderer>();
        previousColor = ren.material.color;
    }

    public void setColliding(bool isColliding)
    {
        if (isColliding)
        {
            colliding = true;
            if (ren != null)
            {
                ren.material.color = Color.red;
            }
        }
        else
        {
            colliding = false;
            if (ren != null)
            {
                ren.material.color = previousColor;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        setColliding(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            setColliding(true);
        }
    }
}
