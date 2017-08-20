using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    private Mesh mesh;
    private List<Shape> shapes;

    // Use this for initialization
    private void Start() {
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        shapes = new List<Shape>();
        shapes.Add(new Triangle(Vector3.left, Vector3.right, Vector3.up));
        shapes.Add(new Triangle(new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(0, 2, 0)));
        shapes.Add(new Triangle(new Vector3(-1, 2, 0), new Vector3(1, 2, 0), new Vector3(0, 3, 0)));
        shapes.Add(new Triangle(new Vector3(-1, 3, 0), new Vector3(1, 3, 0), new Vector3(0, 4, 0)));
        shapes.Add(new Square(new Vector3(-1, -4, 0), new Vector3(1, -4, 0), new Vector3(1, -2, 0), new Vector3(-1, -2, 0)));

        UpdateMesh();
    }

    // Update is called once per frame
    private void Update() {
        UpdateMesh();
    }

    private void UpdateMesh() {
        int offset = 0;
        List<int> triangles = new List<int>();
        List<Vector3> points = new List<Vector3>();
        foreach (var triangle in shapes) {
            var trianglePoints = triangle.GetTriangles(offset);
            triangles.AddRange(trianglePoints);
            offset += trianglePoints.Length;
            points.AddRange(triangle.Points);
        }
        mesh.vertices = points.ToArray();
        mesh.triangles = triangles.ToArray();
    }
}
