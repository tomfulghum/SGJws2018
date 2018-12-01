using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovingTest : MonoBehaviour
{
    public new Camera camera;
    public GameObject Cube;

    public float speed = 10f;
    public Vector3 targetPos;
    public bool isMoving;
    const int MOUSE = 0;




    void Start()
    {
        targetPos = transform.position;
        isMoving = false;
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            if (Input.GetMouseButtonUp(0))
            {
                isMoving = false;
            }
            if (Input.GetMouseButton(0))
            {
                Debug.Log("Hit");
                isMoving = true;
           

            }
            
        }
        if (isMoving)
        {
            targetPos = camera.ScreenToWorldPoint(Input.mousePosition, 0);
            Debug.Log("Release");
            MoveObject();
        }
    }


    void MoveObject()
    {

        transform.position = targetPos;
    }
}