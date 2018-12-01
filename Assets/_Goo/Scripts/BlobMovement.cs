using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobMovement : MonoBehaviour
{
    private GameObject blobl = null;  //Attachen zu Blobbls
    private GameObject outerBlobl = null;
    public Vector3 targetPos;
    public float movingDistance = 10;
    private MassSpring currentMassSpring = null;

    public float movingSpeed = 6;

    public Vector3 movementForce;
    private Vector3 currMoveTarget;

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {

            if (Physics.Raycast(ray, out hit))
            {

                //transform.position = targetPos;
                if (hit.transform.gameObject.GetComponent<Point>() != null && blobl == null)
                {
                    blobl = hit.transform.gameObject;
                    currentMassSpring = blobl.transform.parent.GetComponent<MassSpring>();
                }
            }
            if (blobl != null)
            {
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition, 0);
                currentMassSpring.MoveBlobl(blobl, targetPos, movingSpeed);
            }
        }

        if (Input.GetMouseButtonUp(0) && blobl != null)
        {
            currentMassSpring.UnpauseBlobls();
            blobl = null;
        }
        if (blobl != null && blobl.GetComponent<Point>().stickToWall)
        {
            if (Input.GetKeyDown("s"))
            {
                if (blobl.GetComponent<Point>().state == Point.StickyState.None)
                {
                    currentMassSpring.SetStickyState(blobl, Point.StickyState.Wall);
                }
                else if (blobl.GetComponent<Point>().state == Point.StickyState.Wall)
                {
                    currentMassSpring.SetStickyState(blobl, Point.StickyState.None);
                    blobl.transform.parent.GetComponent<MassSpring>().SetStickyState(blobl, Point.StickyState.None);

                }
            }
        }
        CheckForMerge();
        CheckForSplit();
        /*if (!Input.GetKeyUp("a"))
        {
            if (Input.GetKey("a"))
            {
                currentMassSpring.MoveBlobl(outerBlobl, new Vector3(outerBlobl.GetComponent<Point>().rb.transform.position.x - movingDistance, outerBlobl.GetComponent<Point>().rb.transform.position.y, 0), movingSpeed);
                currentMassSpring.UnpauseBlobls();
                outerBlobl.transform.GetChild(0).gameObject.SetActive(false);
            }
            if (outerBlobl == null && currentMassSpring != null)
            {
                outerBlobl = currentMassSpring.getOuterBlobl(true);
                currentMassSpring.UnpauseBlobls();
                outerBlobl.transform.GetChild(0).gameObject.SetActive(false);

            }
        }
        else if (outerBlobl != null && currentMassSpring != null && Input.GetKeyUp("a"))
        {
            currentMassSpring.UnpauseBlobls();
            outerBlobl.transform.GetChild(0).gameObject.SetActive(true);
            outerBlobl = null;
        }
        if (Input.GetKey("d"))
        {
            currentMassSpring.MoveBlobl(outerBlobl, new Vector3(outerBlobl.GetComponent<Point>().rb.transform.position.x + movingDistance, outerBlobl.GetComponent<Point>().rb.transform.position.y, 0), movingSpeed);
            currentMassSpring.UnpauseBlobls(); outerBlobl.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (outerBlobl == null && currentMassSpring != null && Input.GetKey("d"))
        {
            outerBlobl = currentMassSpring.getOuterBlobl(false);
            currentMassSpring.UnpauseBlobls();
            outerBlobl.transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (outerBlobl != null && currentMassSpring != null && Input.GetKeyUp("d"))
        {
            currentMassSpring.UnpauseBlobls();
            outerBlobl.transform.GetChild(0).gameObject.SetActive(true);
            outerBlobl = null;
        }*/
        Movement();
    }

    void Movement()
    {
        if (currentMassSpring == null)
            return;
        if (Input.GetKey("a"))
        {
            for (int i = 0; i < currentMassSpring.points.Count; i++)
            {
                if (currentMassSpring.points[i].transform.position.x < currentMassSpring.com.x)
                    currentMassSpring.points[i].rb.AddForce(-movementForce);
            }
        }
        if (Input.GetKey("d"))
        {
            for (int i = 0; i < currentMassSpring.points.Count; i++)
            {
                if (currentMassSpring.points[i].transform.position.x > currentMassSpring.com.x)
                {
                    currentMassSpring.points[i].rb.AddForce(movementForce);
                }
            }
        }
        currentMassSpring.CalcCom();
    }

    void CheckForMerge()
    {
        if (Input.GetKeyDown("e"))
        {
            for (int i = 0; i < BlobBL.instance.arrayCount; i++)
            {
                for (int j = 0; j < BlobBL.instance.massSprings[i].points.Count; j++)
                {
                    int k = BlobBL.instance.massSprings[i].points[j].stickToBlobl;
                    if (k >= 0 && k != i)
                    {
                        BlobBL.instance.Merge(i, k);
                        break;
                    }
                }
            }
        }
    }

    void CheckForSplit()
    {
        if (Input.GetKeyDown("w"))
        {
            if (currentMassSpring != null)
                currentMassSpring.Split();
        }
    }
}