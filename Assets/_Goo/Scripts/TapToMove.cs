using UnityEngine;
using System.Collections;
 
public class TapToMove : MonoBehaviour {
 //flag to check if the user has tapped / clicked. 
 //Set to true on click. Reset to false on reaching destination
 private bool flag = false;
 //destination point
 private Vector3 endPoint;
 //alter this to change the speed of the movement of player / gameobject
 public float duration = 50.0f;
 //vertical position of the gameobject
 private float yAxis;

 private GameObject blobl;

 
 void Start(){
  //save the y axis value of gameobject
 }
  
 // Update is called once per frame
 void Update () {
  //check if the screen is touched / clicked   
  if(Input.GetMouseButtonDown(0))
  {
   //declare a variable of RaycastHit struct
   RaycastHit hit;
   //Create a Ray on the tapped / clicked position
   Ray ray;
   //for unity editor
   ray = Camera.main.ScreenPointToRay(Input.mousePosition);
   
 
   //Check if the ray hits any collider
   if(Physics.Raycast(ray,out hit))
   {
    //set a flag to indicate to move the gameobject
    flag = true;
    //save the click / tap position
    endPoint = hit.point;
    //as we do not want to change the y axis value based on touch position, reset it to original y axis value
    blobl = hit.transform.gameObject;
   }
    
  }
  else if(blobl!= null && Input.GetMouseButtonUp(0)){
    flag=false;
    blobl.transform.parent.GetComponent<MassSpring>().UnpauseBlobls();
  }
  if(Input.GetMouseButtonDown(1))
  {
    blobl.transform.parent.GetComponent<MassSpring>().SetStickyState(blobl,Point.StickyState.Wall);
  }
  //check if the flag for movement is true and the current gameobject position is not same as the clicked / tapped position
  if(blobl!=null && flag){
   //move the gameobject to the desired position
        Vector3 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        blobl.transform.parent.GetComponent<MassSpring>().MoveBlobl(blobl, new Vector3(destination.x,destination.y,0));
      }
 }
}