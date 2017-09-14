using UnityEngine;

/// <summary>
/// Class that defines a Quadrilateral shape.
/// </summary>
public class Square : Shape {
    public Square(Vector3 x1, Vector3 x2, Vector3 x3, Vector3 x4) {
        Vertices = new[] { x1, x2, x3, x4 };
        Setup();
    }
    public Square(Vector3[] x) {
        Debug.Assert(x.Length == 4, "A square should only have four points.");
        Vertices = x;
        Setup();
    }

    public override int[] GetTriangles(int offset) {
        return new int[] { 0 + offset, 1 + offset, 2 + offset, 0 + offset, 2 + offset, 3 + offset };
    }
}
