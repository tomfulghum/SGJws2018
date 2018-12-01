using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobMovement : MonoBehaviour
{
    private GameObject blobl;  //Attachen zu Blobbls

    public Vector3 targetPos;

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Input.GetMouseButton(0))
        {

            if (Physics.Raycast(ray, out hit))
            {

                //transform.position = targetPos;
                if (hit.transform.gameObject.GetComponent<Point>() != null)
                {
                    blobl = hit.transform.gameObject;
                }
            }
            if (blobl != null)
            {
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition, 0);
                blobl.transform.parent.GetComponent<MassSpring>().MoveBlobl(blobl, targetPos);
            }
        }

        if (Input.GetMouseButtonUp(0) && blobl!= null)
        {
            blobl.transform.parent.GetComponent<MassSpring>().UnpauseBlobls();
            blobl = null;
        }
        if (blobl != null &&blobl.GetComponent<Point>().StickToWall) {
            if (Input.GetKeyDown("s"))
            {
                if(blobl.GetComponent<Point>().state == Point.StickyState.None){
                    blobl.transform.parent.GetComponent<MassSpring>().SetStickyState(blobl,Point.StickyState.Wall);
                }
                else if(blobl.GetComponent<Point>().state == Point.StickyState.Wall){
                    blobl.transform.parent.GetComponent<MassSpring>().SetStickyState(blobl,Point.StickyState.None);
                }
            }
        }
    }




}