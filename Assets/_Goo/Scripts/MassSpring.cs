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

    public const float FLT_EPSILON = 0.000001f;
    
    public List<Point> points;
    private List<List<Spring>> springs;

    // Use this for initialization
    void Start()
    {
        if (numBlobls < 1)
            throw new System.InvalidOperationException("At least one blobl has to be present(numBlobls).");
        externalForces = false;
        points = new List<Point>();
        springs = new List<List<Spring>>();
        for (int i = 0; i < numBlobls; i++)
        {
            if (i == 0)
            {
                points.Add(GameObject.Instantiate(prefab, new Vector3(i, 10, i % 3), Quaternion.identity).GetComponent<Point>());
                points[i].stationary = true;
            }
            else
            {
                points.Add(GameObject.Instantiate(prefab, new Vector3(i, (i % 2) * 4, i % 3), Quaternion.identity).GetComponent<Point>());
            }
        }
        for (int i = 0; i < numBlobls; i++)
        {
            springs.Add(new List<Spring>());
            for (int j = i + 1; j < numBlobls; j++)
            {
                springs[i].Add(new Spring(initalLength, stiffness));
                springs[i][springs[i].Count - 1].p1 = points[i];
                springs[i][springs[i].Count - 1].p2 = points[j];
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

    }

    public void Merge()
    {

    }

    public void RemoveBlobl()
    {

    }

    void AddDamping()
    {
        for (int i = 0; i < numBlobls; i++)
        {
            // could also be points[i].damping instead of m_fDamping
            points[i].force -= points[i].rb.velocity * damping;
        }
    }

    void ApplyExternalForceAll(Vector3 externalForce)
    {
        for (int i = 0; i < numBlobls; i++)
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
            for(int j = 0; j< springs[i].Count; j++)
                springs[i][j].CalculateSpringForce();
        }
        //	add damping
        AddDamping();

        //	apply external forces
        if (externalForces)
            ApplyExternalForceAll(Physics.gravity);
    }
}
