using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : Shape {

    public Square(Vector3 x1, Vector3 x2, Vector3 x3, Vector3 x4) {
        Points = new[] { x1, x2, x3, x4 };
        CalculateCenter();
    }
    public Square(Vector3[] x) : base() {
        Debug.Assert(x.Length == 4, "A square should only have four points.");
        Points = x;
    }

    public override int[] GetTriangles(int offset) {
        return new int[] { 0 + offset, 1 + offset, 2 + offset, 0 + offset, 2 + offset, 3 + offset };
    }
}
