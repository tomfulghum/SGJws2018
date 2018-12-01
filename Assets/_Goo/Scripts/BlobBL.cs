using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlobBL : MonoBehaviour
{

    public MassSpring[] massSprings;
    public int numBlobls;
    public int arrayCount;
    
    // depracated
    private List<Spring> intermedSprings;

    public static BlobBL instance;
        
    private void Start()
    {
        if (massSprings == null)
            throw new System.InvalidOperationException("MassSpring reference has to be set!");

        if (massSprings.Length == 0)
            throw new System.InvalidOperationException("Array size has to be larger than 0.");

        for (int i = 0; i < numBlobls; i++)
        {
            massSprings[0].AddBlobl(new Vector3(i, i % 2, 0), Quaternion.identity);
        }
        massSprings[0].indexBL = 0;
        arrayCount = 1;
        intermedSprings = new List<Spring>();
        if (instance == null)
            instance = this;
        else
            throw new System.InvalidOperationException("Only one BlobBL can be activate at a time!");
    }

    private void OnEnable()
    {
        MassSpring.OnSplit += AddMassSpring;
    }

    private void AddMassSpring(MassSpring newBlob)
    {
        massSprings[arrayCount] = newBlob;
        massSprings[arrayCount].indexBL = arrayCount;
        arrayCount++;
    }

    private void RemoveMassSpring(MassSpring toRemove)
    {
        massSprings[toRemove.indexBL] = massSprings[arrayCount - 1];
        if(massSprings[toRemove.indexBL] != null)
            massSprings[toRemove.indexBL].indexBL = toRemove.indexBL;
    }

    public void Merge(int idx1,int idx2)
    {
        if (arrayCount > 1)
        {
            MassSpring ToMerge = massSprings[idx2];
            RemoveMassSpring(ToMerge);
            massSprings[idx1].Merge(ToMerge);
            arrayCount--;
        }
    }
}
