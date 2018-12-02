using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttachmentDirection
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}

public class ConnectableBoxAttachment : MonoBehaviour
{
    public AttachmentDirection direction;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {

    }
}
