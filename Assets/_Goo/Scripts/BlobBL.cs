using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlobBL : MonoBehaviour
{

    public List<MassSpring> massSprings;
    public int numBlobls;
    
    // depracated
    private List<Spring> intermedSprings;

    public static BlobBL instance;
        
    private void Start()
    {
        massSprings = new List<MassSpring>();
        if (massSprings == null)
            throw new System.InvalidOperationException("MassSpring reference has to be set!");
        massSprings.Add(GameObject.Find("Blob").GetComponent<MassSpring>());
        for (int i = 0; i < numBlobls; i++)
        {
            massSprings[0].AddBlobl(transform.position + new Vector3(i, i % 2, 0), Quaternion.identity);
        }
        massSprings[0].indexBL = 0;

        BlobMovement movement = gameObject.GetComponent<BlobMovement>();
        movement.setCurrentMassSpring(massSprings[0]);
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
        massSprings.Add(newBlob);
        massSprings[massSprings.Count-1].indexBL = massSprings.Count - 1;
    }
    /*
    private void RemoveMassSpring(MassSpring toRemove)
    {
        massSprings[toRemove.indexBL] = massSprings[arrayCount - 1];
        if(massSprings[toRemove.indexBL] != null)
            massSprings[toRemove.indexBL].indexBL = toRemove.indexBL;
    }*/

    public void Merge(int idx1, int idx2)
    {
        if (massSprings.Count > 1)
        {
            MassSpring ToMerge = massSprings[idx2];
            massSprings.RemoveAt(idx2);
            massSprings[idx1].Merge(ToMerge);
        }
    }
}
