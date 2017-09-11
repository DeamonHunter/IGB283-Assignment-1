using System.Collections;
using System.Collections.Generic;
using IGB283;
using UnityEngine;

public class Controller : MonoBehaviour {
    //Public variables
    public float DragCooldown = 0.1f;
    public bool ThreeDimensional;
    public Color[] Colours; 

    //Components
    private Mesh mesh;

    //Constants
    private const float maxSpeed = 10;
    private const float minSpeed = 0;

    //Shapes
    private List<Shape> shapes;

    //Dragging
    private Shape draggedShape;
    private float curDragCooldown;


    // Use this for initialization
    private void Start() {
        //Get the mesh
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //Add in some shapes to test rendering
        shapes = new List<Shape>();
        shapes.Add(new Circle(Vector3.zero, 0.25f, 100));
        shapes[0].Speed = 3;
        shapes[0].RotationSpeed = new Vector3(60, 10, 20);
        shapes.Add(new Cube(0.5f, new Vector3(-1,1,0)));
        shapes[1].Speed = 5;
        shapes[1].RotationSpeed = new Vector3(10, 60, 20);
        shapes.Add(new Cube(0.5f, new Vector3(-1,-1,0)));
        shapes[2].Speed = 2;
        shapes[2].RotationSpeed = new Vector3(30, 10, 50);
        shapes.Add(new Cube(0.5f, new Vector3(1,1,0)));
        shapes[3].Speed = 4;
        shapes[3].RotationSpeed = new Vector3(40, 30, 20);
        shapes[3].MoveTowardsFirst = false;
        shapes.Add(new Cube(0.5f, new Vector3(1,-1,0)));
        shapes[4].Speed = 6;
        shapes[4].RotationSpeed = new Vector3(60, 20, 60);
        shapes[4].MoveTowardsFirst = false;
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

        RotateAndTranslate(shapes[0], -1, 1);
        RotateAndTranslate(shapes[1], -1, 1);
        RotateAndTranslate(shapes[2], -1, 1);
        RotateAndTranslate(shapes[3], -1, 1);
        RotateAndTranslate(shapes[4], -1, 1);

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
        Color[] colors = new Color[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++) {
            colors[i] = Color.Lerp(Colours[0], Colours[1], (mesh.vertices[i].x + 1) / 2);
        }
        mesh.colors = colors;
    }

    private void RotateAndTranslate(Shape shape, float point1, float point2) {
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
            Matrix3x3 R = IGB283Transform.Rotate(shape.RotationSpeed.z * Time.deltaTime);
            Matrix3x3 TReverse = IGB283Transform.Translate(center + Time.deltaTime * shape.Speed * new Vector2(moveDir.x, moveDir.y));
            shape.ApplyTransformation(TReverse * R * T);
        }
        else {
            IGB283.Matrix4x4 T = IGB283Transform.Translate(-shape.Center);
            IGB283.Matrix4x4 R = IGB283Transform.Rotate(shape.RotationSpeed * Time.deltaTime );
            IGB283.Matrix4x4 TReverse = IGB283Transform.Translate(shape.Center + Time.deltaTime * shape.Speed * moveDir);
            shape.ApplyTransformation(TReverse * R * T);
        }
    }
}



