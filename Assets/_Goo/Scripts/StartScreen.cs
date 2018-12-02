using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour {
    public float timeVisible;

	// Use this for initializationme
	void Start () {
        StartCoroutine(DisableObj());
	}
    IEnumerator DisableObj()
    {
        yield return new WaitForSeconds(timeVisible);
        Destroy(this.gameObject);
    }
}
