using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobBL : MonoBehaviour
{

    public MassSpring massSpring;
    public int numBlobls;
    private void Start()
    {
        if (massSpring == null)
            throw new System.InvalidOperationException("MassSpring reference has to be set!");

        for (int i = 0; i < numBlobls; i++)
        {
            massSpring.AddBlobl(new Vector3(i, i % 2, 0), Quaternion.identity, this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
