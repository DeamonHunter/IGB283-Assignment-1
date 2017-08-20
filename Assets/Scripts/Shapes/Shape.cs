using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shape {
    public Vector3[] Points;
    protected Vector3 Center;

    protected void CalculateCenter() {
        Center = Vector3.zero;
        foreach (var point in Points) {
            Center += point;
        }
        Center /= Points.Length;
    }

    public abstract int[] GetTriangles(int offset);
}
