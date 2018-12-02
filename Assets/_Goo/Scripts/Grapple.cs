using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [Range(0.0f, 7.2f)]
    public float extension = 7.2f;
    public GameObject grappleBase;
    public HingeJoint2D[] links;
    public GameObject arm;
    public bool open = true;

    private float initialY;
    private float linkSize = 7.2f / 10.0f;

    // Use this for initialization
    void Start()
    {
        initialY = grappleBase.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        grappleBase.transform.position = new Vector3(grappleBase.transform.position.x, initialY + (7.2f - extension), grappleBase.transform.position.z);
        int upperLink = Mathf.FloorToInt((7.2f - extension) / linkSize);
        if (upperLink < 9)
        {
            if (upperLink > 0)
            {
                links[upperLink - 1].gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                links[upperLink - 1].transform.parent = grappleBase.transform;
            }

            links[upperLink].gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
            links[upperLink].transform.parent = transform;
        }
    }
}
