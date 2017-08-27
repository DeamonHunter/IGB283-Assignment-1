using System.Collections;
using System.Collections.Generic;
using IGB283;
using UnityEngine;

public class Controller : MonoBehaviour {
    private Mesh mesh;
    private List<Shape> shapes;

    // Use this for initialization
    private void Start() {
        //Get the mesh
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //Add in some shapes to test rendering
        shapes = new List<Shape>();
        shapes.Add(new Triangle(Vector3.left, Vector3.right, Vector3.up));
        shapes[0].Speed = 1;
        shapes.Add(new Triangle(new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(0, 2, 0)));
        shapes.Add(new Triangle(new Vector3(-1, 2, 0), new Vector3(1, 2, 0), new Vector3(0, 3, 0)));
        shapes.Add(new Triangle(new Vector3(-1, 3, 0), new Vector3(1, 3, 0), new Vector3(0, 4, 0)));
        shapes.Add(new Square(new Vector3(-1, -4, 0), new Vector3(1, -4, 0), new Vector3(1, -2, 0), new Vector3(-1, -2, 0)));

    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            //Stupidly complicated interact code
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
            Debug.Log(pos);
            Shape interactedShape = shapes[0];
            var smallestMag = (shapes[0].Center - pos).sqrMagnitude;
            for (int i = 1; i < shapes.Count; i++) {
                if ((shapes[i].Center - pos).sqrMagnitude < smallestMag) {
                    interactedShape = shapes[i];
                    smallestMag = (shapes[i].Center - pos).sqrMagnitude;
                }
            }

            if (smallestMag < interactedShape.InteractionRadius) {
                //Do Something
                Debug.Log("Interacted at: " + interactedShape.Center);
            }
        }

        RotateAndTranslate(shapes[0], 30, -1, 1);
        IGB283Transform.Rotate(shapes[1], 20 * Time.deltaTime);
        IGB283Transform.Rotate(shapes[2], 40 * Time.deltaTime);
        IGB283Transform.Rotate(shapes[3], 80 * Time.deltaTime);
        IGB283Transform.Rotate(shapes[4], 160 * Time.deltaTime);
        if (Mathf.Floor(Time.time) % 2 == 0)
            IGB283Transform.Scale(shapes[4], 1 + 0.5f * Time.deltaTime, new Vector3(0.2f, 0.1f, 0f));
        else
            IGB283Transform.Scale(shapes[4], 1 - 0.5f * Time.deltaTime, new Vector3(0.2f, 0.1f, 0f));


        UpdateMesh();
    }

    /// <summary>
    /// Updates the mesh to reflect new positions of shapes and even new shapes
    /// </summary>
    private void UpdateMesh() {
        int offset = 0;
        List<int> triangles = new List<int>();
        List<Vector3> points = new List<Vector3>();
        foreach (var triangle in shapes) {
            var trianglePoints = triangle.GetTriangles(offset);
            triangles.AddRange(trianglePoints);
            offset += trianglePoints.Length;
            points.AddRange(triangle.Vertices);
        }
        mesh.vertices = points.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    private void RotateAndTranslate(Shape shape, float angle, float point1, float point2) {
        if (!shape.MoveTowardsFirst) {
            if (shape.Center.x >= point2) {
                shape.MoveTowardsFirst = true;
            }
            IGB283Transform.Translate(shape, Vector3.right * shape.Speed * Time.deltaTime); //move towards point 2
        }
        else {
            if (shape.Center.x <= point1) {
                shape.MoveTowardsFirst = false;
            }
            IGB283Transform.Translate(shape, Vector3.left * shape.Speed * Time.deltaTime); //move towards point 1
        }
        IGB283Transform.Rotate(shape, angle * Time.deltaTime);

    }
}
