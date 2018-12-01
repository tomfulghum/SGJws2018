﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassSpring : MonoBehaviour
{
    public delegate void SplitBlob(MassSpring ms);
    public static event SplitBlob OnSplit;
    public delegate void MergeBlob(MassSpring ms);
    public static event MergeBlob OnMerge;

    public float damping;
    public float mass;
    public float stiffness;
    public float initalLength;

    public GameObject prefabBlobl;
    public GameObject prefabBlob;
    public bool externalForces;
    public bool lockZAxis;
    public List<Point> points;

    public const float FLT_EPSILON = 0.000001f;

    private List<List<Spring>> springs;
    private Vector3 externalForce;
    private float forceDuration;

    // Use this for initialization
    void Awake()
    {
        externalForces = false;
        points = new List<Point>();
        springs = new List<List<Spring>>();
        forceDuration = 0;
    }

    // force calculation + integration
    private void FixedUpdate()
    {
        AddSpringForces();
        for (int i = 0; i < points.Count; i++)
        {
            points[i].MidpointAdvect_1();
            points[i].force += Physics.gravity;
        }
        AddSpringForces();
        for (int i = 0; i < points.Count; i++)
        {
            points[i].MidpointAdvect_2();
            points[i].force += Physics.gravity;
        }
    }

    // split into two blobs
    public void Split()
    {
        if (points.Count < 5)
            throw new System.InvalidOperationException("Blob has to have at least 5 blobls");
        int subValue = Mathf.FloorToInt(points.Count / 2f);
        GameObject newBlob = GameObject.Instantiate(prefabBlob);
        for (int i = 0; i < subValue; i++)
        {
            newBlob.GetComponent<MassSpring>().AddBlobl(points[i].transform, newBlob.transform);
            this.RemoveBlobl(points[i]);
        }
        externalForce = new Vector3(50, 50, 0);
        externalForces = true;
        forceDuration = 0.25f;
        OnSplit(newBlob.GetComponent<MassSpring>());
    }

    // move single blobl (simple set position so far)
    public void MoveBlobl(GameObject blobl, Vector3 targetPosition)
    {
        int idx = 0;
        for (int i = 0; i < points.Count; i++)
        {
            // set all other blobs to unmovable 
            if (points[i].gameObject != blobl)
                points[i].unmovable = true;
            else
                idx = i;
        }
        points[idx].transform.position = targetPosition;
    }

    // unpause static blobls
    public void UnpauseBlobls()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].unmovable = false;
        }
    }

    // called when blobl should be stuck to or removed from wall 
    public void SetStickyState(GameObject blobl, Point.StickyState state)
    {
        points[points.IndexOf(blobl.GetComponent<Point>())].state = state;
    }

    public void Merge(MassSpring massSpring)
    {
        for (int i = 0; i < massSpring.points.Count; i++)
        {
            this.AddBlobl(massSpring.points[i].transform, this.transform);
        }
        Destroy(massSpring.gameObject);
    }

    public void ApplyExternalForce(Vector3 force, float duration)
    {
        externalForce = force;
        externalForces = true;
        forceDuration = duration;
    }

    public void AddBlobl(Transform transform, Transform parent = null)
    {
        points.Add(GameObject.Instantiate(prefabBlobl, transform).GetComponent<Point>());
        if (parent != null)
            points[points.Count - 1].transform.parent = parent;
        else
            points[points.Count - 1].transform.parent = this.transform;
        AddSprings();
    }
    public void AddBlobl(Vector3 pos, Quaternion quat, Transform parent = null)
    {
        points.Add(GameObject.Instantiate(prefabBlobl, pos, quat).GetComponent<Point>());
        if(parent != null)
            points[points.Count - 1].transform.parent = parent;
        else
            points[points.Count - 1].transform.parent = this.transform;
        AddSprings();
    }

    public void AddSprings()
    {
        int i = points.Count - 1;
        springs.Add(new List<Spring>());
        for (int j = 0; j < springs.Count; j++)
        {
            if (i != j)
            {
                springs[j].Add(new Spring(initalLength, stiffness));
                springs[j][springs[j].Count - 1].p1 = points[i];
                springs[j][springs[j].Count - 1].p2 = points[j];
            }
        }
    }

    public void RemoveBlobl(Point toRemove)
    {
        int idx = points.IndexOf(toRemove);
        if (idx >= points.Count)
            throw new System.InvalidOperationException("Cannot remove current Point. (Has to be included in the List)");
        for (int i = 0; i < springs.Count; i++)
        {
            if (i == idx)
            {
                springs.RemoveAt(i);
                break;
            }
            else
                springs[i].RemoveAt(idx - i - 1);
        }
        GameObject tmp = points[idx].gameObject;
        points.RemoveAt(idx);
        Destroy(tmp);
    }

    void AddDamping()
    {
        for (int i = 0; i < points.Count; i++)
        {
            // could also be points[i].damping instead of damping
            points[i].force -= points[i].rb.velocity * damping;
        }
    }

    void ApplyExternalForceAll(Vector3 externalForce)
    {
        for (int i = 0; i < points.Count; i++)
        {
            //	independent of point's mass
            points[i].force += (externalForce * points[i].mass);
        }
    }

    void AddSpringForces()
    {
        //	iterate over all springs
        for (int i = 0; i < springs.Count; i++)
        {
            for (int j = 0; j < springs[i].Count; j++)
            {
                springs[i][j].CalculateSpringForce();
            }
        }
        //	add damping
        AddDamping();

        //	apply external forces
        if (externalForces && forceDuration > 0)
        {
            forceDuration -= Time.fixedDeltaTime;
            ApplyExternalForceAll(externalForce);
            if (forceDuration <= 0)
                externalForces = false;
        }
    }
}
