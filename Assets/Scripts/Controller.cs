﻿using System.Collections;
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
    private bool moveUp = false;
    private float jumpCooldown;

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
        bool onGround = ApplyVerticalMomentum();

        if (fallingDown)
            FallDown();
        else if (gettingUp)
            GettingUp();
        else
        {
            Nodding();
            if (onGround)
            {
                if (jumpCooldown <= 0)
                    JumpStraightUp();
                else
                    jumpCooldown -= Time.deltaTime;
            }
            //TranslateLeftAndRight(Body, 2, 0);
        }

        //Update the mesh to reflect changes
        UpdateMesh();
    }

    private bool ApplyVerticalMomentum()
    {
        
        var hit = Physics2D.Raycast(new Vector2((Body.Vertices[0].x + Body.Vertices[1].x) / 2, Body.Vertices[0].y),
            Vector2.down, 0.05f);
        if (hit && verticalMomentum <= 0)
        {
            Matrix3x3 T = IGB283Transform.Translate(new Vector2(0, -hit.distance));
            Body.ApplyTransformation(T);
            verticalMomentum = 0;
            return true;
        }
        else {
            Matrix3x3 T = IGB283Transform.Translate(new Vector2(0, verticalMomentum * Time.deltaTime));
            Body.ApplyTransformation(T);
            verticalMomentum += gravity * Time.deltaTime;
            return false;
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
        int currentPoint = point1;
        if (currentPoint == point1) {
            Debug.Log("if statement entered");
            if (shape.RotateCenter.x >= point1) {
                Debug.Log("second if statement entered");
                Debug.Log(shape.RotateCenter.x);
                currentPoint = point2;
            }
        } else if (currentPoint == point2) {
            Debug.Log("else if statement entered");
            if (shape.RotateCenter.x <= point2) {
                currentPoint = point1;
                Debug.Log("second else if statement entered");
                Debug.Log(shape.RotateCenter.x);
            }
        }

        Matrix3x3 T = IGB283Transform.Translate(new Vector2(currentPoint * Time.deltaTime, 0));
        shape.ApplyTransformation(T);

    }

    private void JumpStraightUp()
    {
        verticalMomentum = 8;
        jumpCooldown = 0.5f;
    }

    private void Nodding() {
        if (!moveUp) {
            RotateShape(Head, Time.deltaTime * 140);
            if (90 < Head.Angle) {
                moveUp = true;
            }
        } else {
            RotateShape(Head, Time.deltaTime * -120);
            if (30 > Head.Angle) {
                moveUp = false;
            }

        }

    }
}



