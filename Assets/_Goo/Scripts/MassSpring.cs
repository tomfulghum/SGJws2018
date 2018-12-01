using System.Collections;
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
    public int indexBL;
    public Vector3 com;
    public float maxMoveDistance;

    public const float FLT_EPSILON = 0.000001f;

    private List<List<Spring>> springs;
    private Vector3 externalForce;
    private float forceDuration;

    private List<Point> sticky;

    // Use this for initialization
    void Awake()
    {
        sticky = new List<Point>();
        externalForces = false;
        points = new List<Point>();
        springs = new List<List<Spring>>();
        forceDuration = 0;
        com = new Vector3();
    }

    // force calculation + integration
    private void FixedUpdate()
    {
        AddSpringForces();
        for (int i = 0; i < points.Count; i++)
        {
            points[i].MidpointAdvect_1();
            points[i].force += Physics.gravity * mass;


        }
        AddSpringForces();
        for (int i = 0; i < points.Count; i++)
        {
            points[i].MidpointAdvect_2();
            points[i].force += Physics.gravity * mass;

        }
    }

    // split into two blobs
    public void Split()
    {
        if (points.Count < 5)
            throw new System.InvalidOperationException("Blob has to have at least 5 blobls");
        int subValue = Mathf.FloorToInt(points.Count / 2f);
        GameObject newBlob = GameObject.Instantiate(prefabBlob);
        for (int i = newBlob.transform.childCount - 1; i >= 0; i--)
        {

            Destroy(newBlob.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < subValue; i++)
        {
            newBlob.GetComponent<MassSpring>().AddBlobl(points[i].transform);
            this.RemoveBlobl(points[i]);
        }
        externalForce = new Vector3(100, 100, 0);
        externalForces = true;
        forceDuration = 0.75f;
        OnSplit(newBlob.GetComponent<MassSpring>());
    }

    // move single blobl (simple set position so far)
    public void MoveBlobl(GameObject blobl, Vector3 targetPosition, float speed)
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
        com = Vector3.zero;
        for (int i = 0; i < points.Count; i++)
        {
            if(i!=idx)
                com += points[i].rb.position;
        }
        com /= (points.Count - 1);

        if (Vector3.Distance(com, targetPosition) < maxMoveDistance)
            points[idx].transform.position = Vector3.MoveTowards(points[idx].transform.position,targetPosition, speed*Time.deltaTime);
        else
            points[idx].transform.position = Vector3.MoveTowards(points[idx].transform.position, com + (targetPosition - com).normalized * maxMoveDistance, speed*Time.deltaTime);
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

        if (state == Point.StickyState.Wall)
        {

            if (sticky.Count < 2)
            {
                sticky.Add(blobl.GetComponent<Point>());
                points[points.IndexOf(blobl.GetComponent<Point>())].state = state;
            }
            else
            {
                if (sticky[0] == blobl.GetComponent<Point>())
                {
                    sticky[0].state = Point.StickyState.None;
                    sticky.RemoveAt(0);
                    sticky.Add(blobl.GetComponent<Point>());
                    points[points.IndexOf(blobl.GetComponent<Point>())].state = state;
                }
                else if (sticky[1] != blobl.GetComponent<Point>())
                {
                    sticky[0].state = Point.StickyState.None;
                    sticky.RemoveAt(0);
                    sticky.Add(blobl.GetComponent<Point>());
                    points[points.IndexOf(blobl.GetComponent<Point>())].state = state;
                }

            }
        }
        else if (state == Point.StickyState.None)
        {
            int index = sticky.IndexOf(blobl.GetComponent<Point>());
            if (index > -1)
            {
                blobl.GetComponent<Point>().state = Point.StickyState.None;
                sticky.RemoveAt(index);
            }
        }
        else if(state == Point.StickyState.Blobls)
        {

        }
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
        points[points.Count - 1].mass = mass;
        if (parent != null)
            points[points.Count - 1].transform.parent = parent;
        else
            points[points.Count - 1].transform.parent = this.transform;
        AddSprings();
    }
    public void AddBlobl(Vector3 pos, Quaternion quat, Transform parent = null)
    {
        points.Add(GameObject.Instantiate(prefabBlobl, pos, quat).GetComponent<Point>());
        points[points.Count - 1].mass = mass;
        if (parent != null)
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
    public void ApplyExternalForceOne(GameObject blobl, Vector3 force){
        blobl.GetComponent<Point>().force += (force * blobl.GetComponent<Point>().mass);
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
    public GameObject getOuterBlobl(bool Left){
        Point result = null;

        if(Left){
            foreach (Point p in points){
                if(result==null){
                    result = p;
                }
                else if(p.rb.transform.position.x < result.rb.transform.position.x){
                    result = p;
                }
            }  
        }
        else{
            foreach (Point p in points){
                if(result==null){
                    result = p;
                }
                else if(p.rb.transform.position.x > result.rb.transform.position.x){
                    result = p;
                }
            }  
        }
        return result.transform.gameObject;
    }
}
