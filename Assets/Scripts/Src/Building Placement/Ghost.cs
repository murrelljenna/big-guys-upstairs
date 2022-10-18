using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Ghost : MonoBehaviour
{
    private Color previousColor;
    private Renderer renderer;
    public bool colliding = false;


    void Start()
    {
        Transform transrights = transform.Find("Model");

        if (transrights == null) {
            Debug.LogError("This ghost doesn't have a model, put one there idiot. Won't work until it does.");
            return;
        }

        renderer = transrights.gameObject.GetComponent<MeshRenderer>();
        previousColor = renderer.material.color;
    }

    public void setColliding(bool isColliding)
    {
        if (isColliding)
        {
            colliding = true;
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }
            else
            {
                Debug.LogError("This ghost has a model, but that model doesn't have a renderer, wtf are you doing?");
            }
        }
        else
        {
            colliding = false;
            if (renderer != null)
            {
                renderer.material.color = previousColor;
            }
            else
            {
                Debug.LogError("This ghost has a model, but that model doesn't have a renderer, wtf are you doing?");
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("Exiting collision");
        setColliding(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entering collision");
        if (!other.isTrigger)
        {
            setColliding(true);
        }
    }
}
