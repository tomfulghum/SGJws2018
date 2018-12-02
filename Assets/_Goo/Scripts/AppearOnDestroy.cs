using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearOnDestroy : MonoBehaviour {

    public GameObject [] toDestroys;
	public int originalLayer;
    void Awake()
    {
        this.GetComponent<Renderer>().enabled = false;
        originalLayer = gameObject.layer;
        gameObject.layer = 11;
    }

	// Update is called once per frame
	void Update () {
		for(int i = 0; i<toDestroys.Length; i++)
        {
            if (toDestroys[i] != null)
                return;
        }
        this.GetComponent<Renderer>().enabled = true;
        gameObject.layer = originalLayer;
    }
}
