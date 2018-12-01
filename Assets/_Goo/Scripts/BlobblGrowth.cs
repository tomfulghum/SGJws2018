using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobblGrowth : MonoBehaviour {


    public int numBlobls;


	void Update () {
		
        if (Input.GetKeyDown("j")) //Changes to "touch food"
        {
            numBlobls++;
        }

        if (Input.GetKeyDown("k")) //changes to key for devide
        {
            numBlobls--;
        }

	}
}
