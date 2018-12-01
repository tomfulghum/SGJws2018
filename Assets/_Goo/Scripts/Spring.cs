using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring
{

    public Point p1;
    public Point p2;

    public float initialLength;
    public float stiffness;

    public Spring(float l, float s)
    {
        initialLength = l;
        stiffness = s;
    }

    public void CalculateSpringForce()
    {
        //	calculate current distance between both points of spring  l_i
        double currentLength = Vector3.Distance(p1.rb.position, p2.rb.position);
        //	calculate - k_i * (l_i - L_i)
        double tmp = -stiffness * (currentLength - initialLength);
        //	calculate actual forces tmp * (x_1 - x_2) / currentLength (epsilon to prevent div by 0)
        Vector3 forceTmp = ((float)tmp) * (p1.rb.position - p2.rb.position) / ((float)currentLength + MassSpring.FLT_EPSILON);
        //	applies force to both points
        p1.force += forceTmp;
        p2.force -= forceTmp;
    }
}
