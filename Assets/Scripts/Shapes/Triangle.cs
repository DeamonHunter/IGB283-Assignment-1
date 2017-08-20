using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : Shape {

    public Triangle(Vector3 x1, Vector3 x2, Vector3 x3) {
        Vertices = new[] { x1, x2, x3 };
        CalculateCenter();
    }
    public Triangle(Vector3[] x) : base() {
        Debug.Assert(x.Length == 3, "A triangle should only have three points.");
        Vertices = x;
    }

    public override int[] GetTriangles(int offset) {
        return new int[] { 0 + offset, 1 + offset, 2 + offset };
    }
}
