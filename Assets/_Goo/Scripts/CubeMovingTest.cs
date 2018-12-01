using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovingTest : MonoBehaviour
{
    public new Camera camera;
    public GameObject Cube;  //Attachen zu Blobbls

    public Vector3 targetPos;

    void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && Input.GetMouseButton(0))

        {
            targetPos = camera.ScreenToWorldPoint(Input.mousePosition, 0);
            transform.position = targetPos;

        }

    }
}