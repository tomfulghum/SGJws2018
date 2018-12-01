using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassSpring : MonoBehaviour
{

    public int numPoints;
    public float damping;
    public float mass;
    public float stiffness;
    public float initalLength;
    
    public GameObject prefab;
    public Vector3 externalForce;
    public bool externalForces;
    public bool lockZAxis;

    public const float FLT_EPSILON = 0.000001f;

    private int numSprings;
    private Point[] points;
    private List<Spring> springs;

    // Use this for initialization
    void Awake()
    {
        externalForces = false;
        points = new Point[numPoints];
        springs = new List<Spring>();
        for (int i = 0; i < numPoints; i++)
        {
            points[i] = GameObject.Instantiate(prefab, new Vector3(i,i%2,i%3), Quaternion.identity).GetComponent<Point>();
        }
        for (int i = 0; i < numPoints; i++)
        {
            for (int j = i+1; j < numPoints; j++)
            {
                springs.Add(new Spring(initalLength, stiffness));
                springs[springs.Count - 1].p1 = points[i];
                springs[springs.Count - 1].p2 = points[j];
            }
        }
    }

    private void FixedUpdate()
    {
        AddSpringForces();
        for (int i = 0; i < points.Length; i++)
        {
            points[i].MidpointAdvect_1();
        }
        AddSpringForces();
        for (int i = 0; i < points.Length; i++)
        {
            points[i].MidpointAdvect_2();
        }
    }

    void AddDamping()
    {
        for (int i = 0; i < numPoints; i++)
        {
            // could also be points[i].damping instead of m_fDamping
            points[i].force -= points[i].rb.velocity * damping;
        }
    }

    void ApplyExternalForceAll(Vector3 externalForce)
    {
        for (int i = 0; i < numPoints; i++)
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
            springs[i].CalculateSpringForce();
        }
        //	add damping
        AddDamping();

        //	apply external forces
        if (externalForces)
            ApplyExternalForceAll(Physics.gravity);
    }


}
