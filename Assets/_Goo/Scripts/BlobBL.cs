using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlobBL : MonoBehaviour
{

    public MassSpring[] massSprings;
    public int numBlobls;
    private int arrayCount;

    private void Start()
    {
        if (massSprings == null)
            throw new System.InvalidOperationException("MassSpring reference has to be set!");

        if (massSprings.Length == 0)
            throw new System.InvalidOperationException("Array size has to be larger than 0.");

        for (int i = 0; i < numBlobls; i++)
        {
            massSprings[0].AddBlobl(new Vector3(i, i % 2, 0), Quaternion.identity, this.transform);
        }
        arrayCount = 1;
    }

    private void OnEnable()
    {
        MassSpring.OnSplit += AddMassSpring;
    }

    private void AddMassSpring(MassSpring newBlob)
    {
        massSprings[arrayCount] = newBlob;
        arrayCount++;
    }
}
