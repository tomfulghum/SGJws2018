using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public delegate void Deactivate(GameObject gm);
    public static event Deactivate OnDeactivate;

    public enum StickyState
    {
        None,
        Wall,
        Blobls
    }

    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;
    public float mass = 1f;
    public StickyState state; // when sticking a selected blobl to a wall
    public bool unmovable; // when selecting another blobl to move
    public bool lockZAxis = true;
    public Rigidbody rb;
    public bool stickToWall;
    public int stickToBlobl;
    private Vector3 tempPos;
    private Vector3 tempVel;
    private Vector3 unMovVel;

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            throw new System.InvalidOperationException("Rigidbody has to be attached to object!");
        if (mass == 0)
            throw new System.InvalidOperationException("Mass has to be greater than 0!");
        stickToBlobl = -1;
        unMovVel = new Vector3();
    }

    //WallStick Possible?

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            OnDeactivate(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 9)
            Destroy(collision.gameObject);
        else if (collision.gameObject.layer == 10)
        {
            transform.parent.GetComponent<MassSpring>().AddBlobl(collision.gameObject.transform);
            Destroy(collision.gameObject);
        }
        else 
            stickToWall = true;
    }

    private void OnTriggerExit(Collider collision)
    {
        stickToWall = false;
        if (state != StickyState.Blobls)
            state = StickyState.None;
    }

    private void FixedUpdate()
    {
        stickToBlobl = -1;
        for (int i = 0; i < BlobBL.instance.massSprings.Count; i++)
        {
            MassSpring ms = transform.parent.GetComponent<MassSpring>();
            if (i != ms.indexBL)
            {
                for (int j = 0; j < BlobBL.instance.massSprings[i].points.Count; j++)
                {
                    if (Vector3.Distance(BlobBL.instance.massSprings[i].points[j].transform.position, this.transform.position) < GetComponent<SphereCollider>().radius * 2.5f)
                    {
                        stickToBlobl = i;
                    }
                }
            }
        }
    }

    public void MidpointAdvect_1()
    {
        if (unMovVel.z < 5)
            rb.velocity = unMovVel;
        if (state == StickyState.Wall || unmovable)
        {
            if (rb.velocity.z < 5)
                unMovVel = rb.velocity;
            rb.velocity = new Vector3(0, 0, 10);
            rb.MovePosition(new Vector3(rb.position.x, rb.position.y, 0f));
            return;
        }
        else
        {
            unMovVel.z = 10;
        }
        // calculate posAfterHalfStep = x(t) + h/2 * v(t, x(t))
        Vector3 posAfterHalfStep = rb.position + (Time.fixedDeltaTime / 2 * rb.velocity);
        // calculate velAfterHalfStep = v(t) + h/2 * a(t, x(t), v(t))
        Vector3 velAfterHalfStep = rb.velocity + (Time.fixedDeltaTime / 2 * force / mass);

        force = Vector3.zero;

        //	already calculates x(t+h) = x(t) + h*vtemp
        tempPos = rb.position; // rb.position + (Time.fixedDeltaTime * velAfterHalfStep);
        tempVel = rb.velocity;

        //	set current pos and vel to temporary values for calculation of forces
        rb.MovePosition(posAfterHalfStep);
        rb.velocity = velAfterHalfStep;
    }

    public void MidpointAdvect_2()
    {
        if (unMovVel.z < 5)
            rb.velocity = unMovVel;
        if (state == StickyState.Wall || unmovable)
        {
            if (rb.velocity.z < 5)
                unMovVel = rb.velocity;
            rb.velocity = new Vector3(0, 0, 10);
            rb.MovePosition(new Vector3(rb.position.x, rb.position.y, 0f));
            return;
        }
        else
        {
            unMovVel.z = 10;
            if (lockZAxis)
            {
                tempVel.z = 0;
                force.z = 0;
                tempPos.z = 0;
            }
            //	v(t+h) = v(t) + h*a(t+h/2, xtemp, vtemp)
            rb.velocity = tempVel + Time.fixedDeltaTime * force / mass;
            //	set new pos to stored position x(t+h)
            rb.MovePosition(tempPos);

            force = Vector3.zero;
        }
    }
}
