using System.Collections;
using System.Collections.Generic;
using IGB283;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Main controller of the assignment.
/// </summary>
public class Controller : MonoBehaviour {
    #region Public Variables
    public float DragCooldown = 0.1f;
    public bool ThreeDimensional;
    public Color[] Colours;
    public Slider[] Sliders;
    #endregion

    #region private Variables
    //Components
    private Mesh mesh;

    //Constants
    private const float maxSpeed = 10;

    //Shapes
    private List<Shape> shapes;
    #endregion


    private Shape Head;
    private Shape UpperArm;
    private Shape LowerArm;
    private Shape Body;

    private bool fallingDown;
    private float verticalMomentum;
    private const float gravity = -18;
    private float gettingUpCooldown;
    private bool gettingUp;

    /// <summary>
    /// Used to initialise all shapes.
    /// </summary>
    private void Start() {
        //Get the mesh and clear it
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        shapes = new List<Shape>();

        Body = new Square(new Vector3(-1f, -1f, 0), new Vector3(-1f, 0f, 0), new Vector3(1f, 0f, 0), new Vector3(1, -1, 0));
        Body.RotateCenter = new Vector3(0, -0.5f, 0);
        shapes.Add(Body);

        UpperArm = new Square(new Vector3(-0.25f, 0f, 0), new Vector3(-0.25f, 2f, 0), new Vector3(0.25f, 2f, 0), new Vector3(0.25f, 0f, 0));
        UpperArm.RotateCenter = new Vector3(0, 0f, 0);
        Body.AddChild(UpperArm);
        shapes.Add(UpperArm);

        LowerArm = new Square(new Vector3(-0.25f, 2f, 0), new Vector3(-0.25f, 4f, 0), new Vector3(0.25f, 4f, 0), new Vector3(0.25f, 2f, 0));
        LowerArm.RotateCenter = new Vector3(0, 2f, 0);
        UpperArm.AddChild(LowerArm);
        shapes.Add(LowerArm);

        Head = new Square(new Vector3(-0.1f, 4f, 0), new Vector3(-0.1f, 5f, 0), new Vector3(0.1f, 5f, 0), new Vector3(0.1f, 4f, 0));
        Head.RotateCenter = new Vector3(0, 4f, 0);
        LowerArm.AddChild(Head);
        shapes.Add(Head);
    }


    // Update is called once per frame
    private void Update()
    {
        if (!gettingUp && !fallingDown)
        {
            if (Input.GetKeyDown(KeyCode.G))
                fallingDown = true;
        }
        if (fallingDown)
            FallDown();
        if (gettingUp)
            GettingUp();

        Nodding();

        ApplyVerticalMomentum();
        //Update the mesh to reflect changes
        UpdateMesh();
    }

    private void ApplyVerticalMomentum()
    {
        var hit = Physics2D.Raycast(new Vector2((Body.Vertices[0].x + Body.Vertices[1].x) / 2, Body.Vertices[0].y),
            Vector2.down, 0.05f);
        if (hit)
        {
            Matrix3x3 T = IGB283Transform.Translate(new Vector2(0, -hit.distance));
            Body.ApplyTransformation(T);
            verticalMomentum = 0;
        }
        else {
            Matrix3x3 T = IGB283Transform.Translate(new Vector2(0, verticalMomentum * Time.deltaTime));
            Body.ApplyTransformation(T);
            verticalMomentum += gravity * Time.deltaTime;
        }
    }


    private void FallDown()
    {
        if (UpperArm.Angle > -110)
        {
            RotateShape(UpperArm, Time.deltaTime * -200);

            RotateShape(LowerArm, Time.deltaTime * 30);
        }
        else
        {
            fallingDown = false;
            gettingUp = true;
            gettingUpCooldown = 1f;
        }

    }


    private void GettingUp() {
        if (gettingUpCooldown > 0)
        {
            gettingUpCooldown -= Time.deltaTime;
            return;
        }

        if (UpperArm.Angle < -60) {
            RotateShape(UpperArm, Time.deltaTime * 40);
            RotateShape(LowerArm, Time.deltaTime * 30);
            RotateShape(Head, Time.deltaTime * 30);
        }
        else if (UpperArm.Angle < 0)
        {
            RotateShape(UpperArm, Time.deltaTime * 100);
            if (LowerArm.Angle > 0)
                RotateShape(LowerArm, Time.deltaTime * -100);
            if (Head.Angle > 0)
                RotateShape(Head, Time.deltaTime * -50);
        }
        else
        {
            gettingUp = false;
            RotateShape(UpperArm, -UpperArm.Angle);
            RotateShape(LowerArm, -LowerArm.Angle);
            RotateShape(Head, -Head.Angle);
        }
    }


    private void RotateShape(Shape shape, float angle) {
        Matrix3x3 T = IGB283Transform.Translate(-(Vector2)shape.RotateCenter);
        Matrix3x3 R = IGB283Transform.Rotate(angle);
        Matrix3x3 TReverse = IGB283Transform.Translate((Vector2)shape.RotateCenter);
        shape.ApplyTransformation(TReverse * R * T);
        shape.Angle += angle;

        TranslateLeftAndRight(Body, -50, 50);
    }

    /// <summary>
    /// Updates the mesh to reflect new positions of shapes and even new shapes
    /// </summary>
    private void UpdateMesh() {
        int offset = 0;
        List<int> triangles = new List<int>();
        List<Vector3> points = new List<Vector3>();

        //Add each shape to the above variables
        foreach (var shape in shapes) {
            var shapeTriangles = shape.GetTriangles(offset);
            triangles.AddRange(shapeTriangles);
            offset += shape.Vertices.Length;
            points.AddRange(shape.Vertices);
        }

        //Set Mesh vertex and triangle positions
        mesh.vertices = points.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.colors = new Color[] { Color.white, Color.white, Color.white, Color.white, Color.black, Color.black, Color.black, Color.black, Color.red, Color.red, Color.red, Color.red, Color.blue, Color.blue, Color.blue, Color.blue };
    }

    private void TranslateLeftAndRight(Shape shape, int point1, int point2) {

        //Determine which direction to move the shape in.
        Vector3 moveDir;
        if (!shape.MoveLeft) {
            if (shape.RotateCenter.x >= point1)
                shape.MoveLeft = true;
            moveDir = Vector3.right;
        } else {
            if (shape.RotateCenter.x <= point2)
                shape.MoveLeft = false;
            moveDir = Vector3.left;
        }

        Matrix3x3 T = IGB283Transform.Translate(Time.deltaTime * shape.Speed * new Vector2(moveDir.x, moveDir.y));
        shape.ApplyTransformation(T);
    }

    private void Nodding() {
        bool moveUp = false;
        if (!moveUp) {
            RotateShape(Head, Time.deltaTime * 20);
            if (Mathf.Approximately(60, Head.Angle)) {
                moveUp = true;
            }
        } else if (moveUp) {
            RotateShape(Head, Time.deltaTime * -20);
            if (Mathf.Approximately(0, Head.Angle)) {
                moveUp = true;
            }

        }

    }
}



