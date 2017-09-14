using UnityEngine;

/// <summary>
/// Class that defines a Triangular shape.
/// </summary>
public class Triangle : Shape {

    public Triangle(Vector3 x1, Vector3 x2, Vector3 x3) {
        Vertices = new[] { x1, x2, x3 };
        Setup();
    }
    public Triangle(Vector3[] x) {
        Debug.Assert(x.Length == 3, "A triangle should only have three points.");
        Vertices = x;
        Setup();
    }

    public override int[] GetTriangles(int offset) {
        return new int[] { 0 + offset, 1 + offset, 2 + offset };
    }
}
