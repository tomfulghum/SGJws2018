﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;
    public float mass = 1f;
    public bool stationary = false;
    public bool lockZAxis = true;
    public Rigidbody rb;

    private Vector3 tempPos;
    private Vector3 tempVel;

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            throw new System.InvalidOperationException("Rigidbody has to be attached to object!");
        if (mass == 0)
            throw new System.InvalidOperationException("Mass has to be greater than 0!");
    }
    
    public void MidpointAdvect_1()
    {
        // calculate posAfterHalfStep = x(t) + h/2 * v(t, x(t))
        Vector3 posAfterHalfStep = rb.position + (Time.fixedDeltaTime / 2 * rb.velocity);
        // calculate velAfterHalfStep = v(t) + h/2 * a(t, x(t), v(t))
        Vector3 velAfterHalfStep = rb.velocity + (Time.fixedDeltaTime / 2 * force / mass);

        force = Vector3.zero;

        //	already calculates x(t+h) = x(t) + h*vtemp
        tempPos = rb.position; // rb.position + (Time.fixedDeltaTime * velAfterHalfStep);
        tempVel = rb.velocity;

        //	set current pos and vel to temporary values for calculation of forces
        rb.position = posAfterHalfStep;
        rb.velocity = velAfterHalfStep;
    }

    public void MidpointAdvect_2()
    {
        if (stationary)
            return;

        if (lockZAxis)
        {
            tempVel.z = 0;
            force.z = 0;
            tempPos.z = 0;
        }
        //	v(t+h) = v(t) + h*a(t+h/2, xtemp, vtemp)
        rb.velocity = tempVel + Time.fixedDeltaTime * force / mass;
        rb.position = tempPos;

        force = Vector3.zero;

        //	set new pos to stored position x(t+h)
    }
}
