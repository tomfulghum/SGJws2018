using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDestroy : MonoBehaviour {

    public GameObject[] toDestroys;
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < toDestroys.Length; i++)
        {
            if (toDestroys[i] != null)
                return;
        }
        Destroy(this.gameObject);
    }
}
