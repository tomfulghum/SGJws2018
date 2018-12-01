using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassSpring : MonoBehaviour
{

    public int numBlobls;
    public float damping;
    public float mass;
    public float stiffness;
    public float initalLength;

    public GameObject prefab;
    public Vector3 externalForce;
    public bool externalForces;
    public bool lockZAxis;
    public bool firstInit;
    public List<Point> points;

    public const float FLT_EPSILON = 0.000001f;

    private List<List<Spring>> springs;

    // Use this for initialization
    void Start()
    {
        if (numBlobls < 1)
            throw new System.InvalidOperationException("At least one blobl has to be present(numBlobls).");
        externalForces = false;
        points = new List<Point>();
        springs = new List<List<Spring>>();
        if (firstInit)
        {
            for (int i = 0; i < numBlobls; i++)
            {
                AddBlobl(new Vector3(i, i % 2, 0), Quaternion.identity, this.transform);
            }
        }
    }

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

    public void Split()
    {
        if (points.Count < 5)
            throw new System.InvalidOperationException("Blob has to have at least 5 blobls");
        int subValue = Mathf.FloorToInt(points.Count / 2f);
        GameObject newBlob = GameObject.Instantiate(this.gameObject, transform);
        newBlob.GetComponent<MassSpring>().firstInit = false;
        for (int i = 0; i < subValue; i++)
        {
            this.RemoveBlobl(points[i]);
        }
        for (int i = subValue; i < points.Count; i++)
        {
            newBlob.GetComponent<MassSpring>().RemoveBlobl(points[i]);
        }
    }

    public void MoveBlobl(GameObject blobl, Vector3 targetPosition)
    {
        int idx = 0;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].gameObject != blobl)
                points[i].unmovable = true;
            else
                idx = i;
        }
        points[idx].transform.position = targetPosition;
        points[idx].stationary = true;
    }

    public void UnpauseBlobls()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].unmovable = false;
        }
    }

    public void Merge()
    {

    }

    public void AddBlobl(Transform transform, Transform parent)
    {
        points.Add(GameObject.Instantiate(prefab, transform).GetComponent<Point>());
        points[points.Count - 1].transform.parent = parent;
        AddSprings();
    }
    public void AddBlobl(Vector3 pos, Quaternion quat, Transform parent)
    {
        points.Add(GameObject.Instantiate(prefab, pos, quat).GetComponent<Point>());
        points[points.Count - 1].transform.parent = parent;
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
        if (externalForces)
            ApplyExternalForceAll(Physics.gravity);
    }
}
