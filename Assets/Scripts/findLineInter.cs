using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Drawing;


public class findLineInter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        bool lines_intersect; 
        bool segments_intersect;
        PointF intersection;
        PointF close_p1;
        PointF close_p2;

        bool lines_intersect2; 
        bool segments_intersect2;
        PointF intersection2;
        PointF close_p12;
        PointF close_p22;

        PointF midpoint;

        FindIntersection(
        new PointF(0.000300169f, -0.5549355f), 
        new PointF(-0.4674999f, -0.2251068f),
        new PointF(-0.4314998f, -0.5549355f),
        new PointF(0.03500012f, -0.2251068f),
        out lines_intersect,
        out segments_intersect,
        out intersection,
        out close_p1,
        out close_p2
        );

        FindIntersection(
        new PointF(0.000300169f, 0.1336145f), 
        new PointF(-0.4674999f, -0.2251068f),
        new PointF(-0.4314998f, 0.1336145f),
        new PointF(0.03500012f, -0.2251068f),
        out lines_intersect2,
        out segments_intersect2,
        out intersection2,
        out close_p12,
        out close_p22
        );

        Debug.Log("Intersection Point Front: \n");
        Debug.Log("X: " + intersection.X + "\n");
        Debug.Log("Y: " + intersection.Y + "\n");

        Debug.Log("Intersection Point Back: \n");
        Debug.Log("X: " + intersection2.X + "\n");
        Debug.Log("Y: " + intersection2.Y + "\n");

        CalculateMidPoint(intersection, intersection2, out midpoint);

        Debug.Log("Mid Point of both: \n");
        Debug.Log("X: " + midpoint.X + "\n");
        Debug.Log("Y: " + midpoint.Y + "\n");


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CalculateMidPoint(PointF p1, PointF p2, out PointF midpoint){
        midpoint = new PointF(
            (p1.X + p2.X) / 2,
            (p1.Y + p2.Y) / 2 
        );
    }

    private void FindIntersection(
    PointF p1, PointF p2, PointF p3, PointF p4,
    out bool lines_intersect, out bool segments_intersect,
    out PointF intersection,
    out PointF close_p1, out PointF close_p2)
{
    // Get the segments' parameters.
    float dx12 = p2.X - p1.X;
    float dy12 = p2.Y - p1.Y;
    float dx34 = p4.X - p3.X;
    float dy34 = p4.Y - p3.Y;

    // Solve for t1 and t2
    float denominator = (dy12 * dx34 - dx12 * dy34);

    float t1 =
        ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
            / denominator;
    if (float.IsInfinity(t1))
    {
        // The lines are parallel (or close enough to it).
        lines_intersect = false;
        segments_intersect = false;
        intersection = new PointF(float.NaN, float.NaN);
        close_p1 = new PointF(float.NaN, float.NaN);
        close_p2 = new PointF(float.NaN, float.NaN);
        return;
    }
    lines_intersect = true;

    float t2 =
        ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
            / -denominator;

    // Find the point of intersection.
    intersection = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);

    // The segments intersect if t1 and t2 are between 0 and 1.
    segments_intersect =
        ((t1 >= 0) && (t1 <= 1) &&
         (t2 >= 0) && (t2 <= 1));

    // Find the closest points on the segments.
    if (t1 < 0)
    {
        t1 = 0;
    }
    else if (t1 > 1)
    {
        t1 = 1;
    }

    if (t2 < 0)
    {
        t2 = 0;
    }
    else if (t2 > 1)
    {
        t2 = 1;
    }

    close_p1 = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);
    close_p2 = new PointF(p3.X + dx34 * t2, p3.Y + dy34 * t2);
}
}
