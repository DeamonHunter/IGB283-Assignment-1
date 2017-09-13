using UnityEngine;

public class Cube : Shape {

    public Cube(Vector3[] x) {
        Debug.Assert(x.Length == 8, "A square should only have four points.");
        Vertices = x;
        Setup();
    }

    public Cube(float scale, Vector3 center) {
        Vertices = new Vector3[] {
            new Vector3(0, 0, 0), new Vector3(scale, 0, 0), new Vector3(0, scale, 0),new Vector3(scale, scale, 0),
            new Vector3(0, 0, scale), new Vector3(scale,0,scale), new Vector3(0,scale,scale), new Vector3(scale,scale,scale)
        };
        Setup();
        ApplyTransformation(IGB283.IGB283Transform.Translate(center - new Vector3(0.5f, 0.5f, 0.5f)));
    }

    public override int[] GetTriangles(int offset) {
        return new int[] {
            0 + offset, 3 + offset, 1 + offset,
            0 + offset, 2 + offset, 3 + offset,
            0 + offset, 4 + offset, 6 + offset,
            0 + offset, 6 + offset, 2 + offset,
            0 + offset, 1 + offset, 5 + offset,
            0 + offset, 5 + offset, 4 + offset,
            7 + offset, 6 + offset, 4 + offset,
            7 + offset, 4 + offset, 5 + offset,
            7 + offset, 5 + offset, 1 + offset,
            7 + offset, 1 + offset, 3 + offset,
            7 + offset, 6 + offset, 2 + offset,
            7 + offset, 2 + offset, 3 + offset
        };
    }
}
