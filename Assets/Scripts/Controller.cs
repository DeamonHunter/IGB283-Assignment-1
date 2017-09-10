using System.Collections;
using System.Collections.Generic;
using IGB283;
using UnityEngine;

public class Controller : MonoBehaviour {
    private Mesh mesh;
    private List<Shape> shapes;
    float maxSpeed = 10;
    float minSpeed = 0;


    private Shape draggedShape;
    public float DragCooldown = 0.1f;
    private float curDragCooldown;

    public bool ThreeDimensional;

    private List<Color> c;

    // Use this for initialization
    private void Start() {
        //Get the mesh
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //Add in some shapes to test rendering
        shapes = new List<Shape>();
        shapes.Add(new Triangle(Vector3.left, Vector3.right, Vector3.up));
        shapes[0].Speed = 5;
        shapes.Add(new Triangle(new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(0, 2, 0)));
        shapes.Add(new Triangle(new Vector3(-1, 2, 0), new Vector3(1, 2, 0), new Vector3(0, 3, 0)));
        shapes.Add(new Triangle(new Vector3(-1, 3, 0), new Vector3(1, 3, 0), new Vector3(0, 4, 0)));
        shapes.Add(new Square(new Vector3(-1, -4, 0), new Vector3(1, -4, 0), new Vector3(1, -2, 0), new Vector3(-1, -2, 0)));
        shapes.Add(new Cube(0.5f));
    }

    // Update is called once per frame
    private void Update() {
        //Left Click
        if (Input.GetMouseButtonDown(0)) {
            //Stupidly complicated interact code
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
            Debug.Log(pos);
            Shape interactedShape;
            if (TryGetClosestShape(pos, out interactedShape)) {
                if (interactedShape.Speed < maxSpeed)
                    interactedShape.Speed += 1;
                Debug.Log("Interacted at: " + interactedShape.Center);
                draggedShape = interactedShape;
            }
        }

        if (Input.GetMouseButton(0) && draggedShape != null) {
            if (curDragCooldown > 0) {
                curDragCooldown -= Time.deltaTime;
            }
            else {
                float distance = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - draggedShape.Center.y;
                Debug.Log(distance);
                Matrix3x3 T = IGB283Transform.Translate(new Vector2(0, distance));
                draggedShape.ApplyTransformation(T);
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            draggedShape = null;
            curDragCooldown = DragCooldown;
        }

        //Right Click
        if (Input.GetMouseButtonDown(1)) {
            //Stupidly complicated interact code
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
            Debug.Log(pos);
            Shape interactedShape;
            if (TryGetClosestShape(pos, out interactedShape)) {
                if (interactedShape.Speed > 0)
                    interactedShape.Speed -= 1;
                Debug.Log("Interacted at: " + interactedShape.Center);
            }
        }

        RotateAndTranslate(shapes[0], 30, -1, 1);

        UpdateMesh();
    }

    private bool TryGetClosestShape(Vector3 pos, out Shape interactedShape) {
        float smallestMag = (shapes[0].Center - pos).sqrMagnitude;
        interactedShape = shapes[0];
        for (int i = 1; i < shapes.Count; i++) {
            if ((shapes[i].Center - pos).sqrMagnitude < smallestMag) {
                interactedShape = shapes[i];
                smallestMag = (shapes[i].Center - pos).sqrMagnitude;
            }
        }
        return smallestMag < interactedShape.InteractionRadius;
    }

    /// <summary>
    /// Updates the mesh to reflect new positions of shapes and even new shapes
    /// </summary>
    private void UpdateMesh() {
        int offset = 0;
        List<int> triangles = new List<int>();
        List<Vector3> points = new List<Vector3>();
        foreach (var shape in shapes) {
            var shapeTriangles = shape.GetTriangles(offset);
            triangles.AddRange(shapeTriangles);
            offset += shape.Vertices.Length;
            points.AddRange(shape.Vertices);
        }
        mesh.vertices = points.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    private void RotateAndTranslate(Shape shape, float angle, float point1, float point2) {
        Vector3 moveDir;
        if (!shape.MoveTowardsFirst) {
            if (shape.Center.x >= point2) {
                shape.MoveTowardsFirst = true;
            }
            moveDir = Vector3.right;
        }
        else {
            if (shape.Center.x <= point1) {
                shape.MoveTowardsFirst = false;
            }
            moveDir = Vector3.left;
        }

        if (!ThreeDimensional) {
            var center = new Vector2(shape.Center.x, shape.Center.y);
            Matrix3x3 T = IGB283Transform.Translate(-center);
            Matrix3x3 R = IGB283Transform.Rotate(angle * Time.deltaTime);
            Matrix3x3 TReverse = IGB283Transform.Translate(center + Time.deltaTime * shape.Speed * new Vector2(moveDir.x, moveDir.y));
            shape.ApplyTransformation(TReverse * R * T);
        }
        else {
            IGB283.Matrix4x4 T = IGB283Transform.Translate(-shape.Center);
            IGB283.Matrix4x4 R = IGB283Transform.Rotate(angle * Time.deltaTime, IGB283Transform.Axis.X);
            IGB283.Matrix4x4 TReverse = IGB283Transform.Translate(shape.Center + Time.deltaTime * shape.Speed * moveDir);
            shape.ApplyTransformation(TReverse * R * T);
        }
    }
}



