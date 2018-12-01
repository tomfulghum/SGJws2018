using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMouseMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, 2.0f * Time.deltaTime);
    }
}
