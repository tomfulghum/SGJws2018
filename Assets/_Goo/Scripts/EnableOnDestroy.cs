using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnDestroy : MonoBehaviour {

    public GameObject toEnable;
    public GameObject[] toDestroys;
    	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < toDestroys.Length; i++)
        {
            if (toDestroys[i] != null)
                return;
        }
        toEnable.SetActive(true);
    }
}
