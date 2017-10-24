﻿using System;
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
    public GameObject HeadCollider;
    private Shape UpperArm;
    public GameObject UpperArmCollider;
    private Shape LowerArm;
    public GameObject LowerArmCollider;
    private Shape Body;
    public GameObject BodyCollider;

    private bool fallingDown;
    private float verticalMomentum;
    private const float gravity = -18;
    private float gettingUpCooldown;
    private bool gettingUp;
    private bool moveUp = false;
    private float jumpCooldown;
    private bool moveRight = true;
    private float walkingSpeed = 3;
    private float jumpingSpeed = 5.5f;
    private bool jumpingUp;
    private bool jumpingForward;
    bool movingForward = true;
    private bool animating;

    /// <summary>
    /// Used to initialise all shapes.
    /// </summary>
    private void Start() {
        //Get the mesh and clear it
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        shapes = new List<Shape>();

        Body = new Square(new Vector3(-1f, -1f, 0), new Vector3(-1f, 0f, 0), 
            new Vector3(1f, 0f, 0), new Vector3(1, -1, 0));
        Body.RotateCenter = new Vector3(0, -0.5f, 0);
        Body.collider = BodyCollider;
        Body.Parent = true;
        shapes.Add(Body);

        UpperArm = new Square(new Vector3(-0.25f, 0f, 0), new Vector3(-0.25f, 2f, 0), 
            new Vector3(0.25f, 2f, 0), new Vector3(0.25f, 0f, 0));
        UpperArm.RotateCenter = new Vector3(0, 0f, 0);
        UpperArm.collider = UpperArmCollider;
        Body.AddChild(UpperArm);
        shapes.Add(UpperArm);

        LowerArm = new Square(new Vector3(-0.25f, 2f, 0), new Vector3(-0.25f, 4f, 0), 
            new Vector3(0.25f, 4f, 0), new Vector3(0.25f, 2f, 0));
        LowerArm.RotateCenter = new Vector3(0, 2f, 0);
        LowerArm.collider = LowerArmCollider;
        UpperArm.AddChild(LowerArm);
        shapes.Add(LowerArm);

        Head = new Square(new Vector3(-0.1f, 4f, 0), new Vector3(-0.1f, 5f, 0),
            new Vector3(0.1f, 5f, 0), new Vector3(0.1f, 4f, 0));
        Head.RotateCenter = new Vector3(0, 4f, 0);
        Head.collider = HeadCollider;
        LowerArm.AddChild(Head);
        shapes.Add(Head);
    }


    // Update is called once per frame
    private void Update()
    {
        if (!gettingUp && !fallingDown)
        {
            if (Input.GetKeyDown(KeyCode.Z))
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
            if (jumpingUp)
                JumpStraightUp();
            else if (jumpingForward)
            {
                JumpForward();
                if (!onGround)
                    TranslateLeftAndRight(onGround);
            }
            else {
                if (Input.GetKey(KeyCode.A))
                    moveRight = false;
                else if (Input.GetKey(KeyCode.D))
                    moveRight = true;
                TranslateLeftAndRight(onGround);
                if (Input.GetKey(KeyCode.W)) {
                    movingForward = true;
                    JumpStraightUp();
                } else if (Input.GetKey(KeyCode.S)) {
                    movingForward = true;
                    JumpForward();
                }
            }

            if (onGround)
            {
                jumpCooldown -= Time.deltaTime;
                if (jumpCooldown <= 0 && !animating)
                {
                    jumpingUp = false;
                    jumpingForward = false;
                }
            }
        }

        //Update the mesh to reflect changes
        UpdateMesh();
    }

    private bool ApplyVerticalMomentum()
    {

        var hit = Physics2D.Raycast(new Vector2((Body.Vertices[0].x + Body.Vertices[2].x) / 2, Body.Vertices[0].y),
            Vector2.down, 0.01f);
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
        int direction = moveRight? 1: -1;
        if ((Head.Angle < 0 && moveRight) || (Head.Angle > 0 && !moveRight)) {
            RotateShape(Head, Time.deltaTime * 160 * direction);
        }
        if ((LowerArm.Angle < 15 && moveRight) || (LowerArm.Angle > -15 && !moveRight)) {
            RotateShape(LowerArm, Time.deltaTime * 80 * direction);
        }

        if ((UpperArm.Angle > -110 && moveRight) || (UpperArm.Angle < 110 && !moveRight))
        {
            RotateShape(UpperArm, Time.deltaTime * -200 * direction);
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

        int direction = moveRight ? 1 : -1;

        if ((UpperArm.Angle < -60 && moveRight) || (UpperArm.Angle > 60 && !moveRight)) {
            RotateShape(UpperArm, Time.deltaTime * 40 * direction);
            RotateShape(LowerArm, Time.deltaTime * 30 * direction);
            RotateShape(Head, Time.deltaTime * 30 * direction);
        }
        else if ((UpperArm.Angle < 0 && moveRight) || (UpperArm.Angle > 0 && !moveRight))
        {
            RotateShape(UpperArm, Time.deltaTime * 100 * direction);
            if ((LowerArm.Angle > 0 && moveRight) || (LowerArm.Angle < 0 && !moveRight))
                RotateShape(LowerArm, Time.deltaTime * -100 * direction);
            if ((Head.Angle > 0 && moveRight) || (Head.Angle < 0 && !moveRight))
                RotateShape(Head, Time.deltaTime * -50 * direction);
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
        

        mesh.colors = Colours;
    }

    private void TranslateLeftAndRight(bool onGround)
    {
        float distance = onGround? walkingSpeed * Time.deltaTime : jumpingSpeed * Time.deltaTime;
        Vector2 direction = moveRight ? Vector2.right * distance : Vector2.left * distance;
        Vector2 pos = new Vector2(moveRight ? Body.Vertices[2].x : Body.Vertices[0].x,
            (Body.Vertices[0].y + Body.Vertices[1].y) / 2);
        var hit = Physics2D.Raycast(pos, direction, distance, 1);
        if (hit) {
            if (onGround)
            {
                moveRight = !moveRight;
            }
            else
                return;
        }
        Matrix3x3 T = IGB283Transform.Translate(direction);
        Body.ApplyTransformation(T);
    }

    private void JumpStraightUp()
    {
        if (!jumpingUp && !animating && jumpCooldown <= 0)
        {
            verticalMomentum = 9;
            jumpCooldown = 0.5f;
            jumpingUp = true;
            animating = true;
        }
        // Do animation here
        if (UpperArm.Angle < 15 && movingForward) {
            RotateShape(UpperArm, Time.deltaTime * 50);
            RotateShape(LowerArm, Time.deltaTime * 40);
            RotateShape(Head, Time.deltaTime * 30);
        } else if (UpperArm.Angle >= 15 && movingForward) {
            movingForward = false;
        } else if (UpperArm.Angle > 0 && !movingForward) {
            RotateShape(UpperArm, Time.deltaTime * -50);
            RotateShape(LowerArm, Time.deltaTime * -40);
            RotateShape(Head, Time.deltaTime * -30);
        }
        else {
            animating = false;
        }

    }

    private void JumpForward() {
        if (!jumpingForward && !animating && jumpCooldown <= 0) {
            verticalMomentum = 6.5f;
            jumpCooldown = 0.5f;
            jumpingForward = true;
            animating = true;
        }
        // Do animation here
        if (UpperArm.Angle < 30 && movingForward) {
            RotateShape(UpperArm, Time.deltaTime * 60);
            RotateShape(LowerArm, Time.deltaTime * 50);
            RotateShape(Head, Time.deltaTime * 40);
        } else if (UpperArm.Angle >= 30 && movingForward) {
            movingForward = false;
        } else if (UpperArm.Angle > 0 && !movingForward) {
            RotateShape(UpperArm, Time.deltaTime * -60);
            RotateShape(LowerArm, Time.deltaTime * -50);
            RotateShape(Head, Time.deltaTime * -40);
        }
        else {
            animating = false;
        }
    }

    private void Nodding() {
        int direction = moveRight ? 1 : -1;
        if (!moveUp) {
            RotateShape(Head, Time.deltaTime * -140 * direction);
            RotateShape(LowerArm, Time.deltaTime * -28 * direction);
            RotateShape(UpperArm, Time.deltaTime * 14 * direction);
            if ((60 < Head.Angle && !moveRight) || (-60 > Head.Angle && moveRight)) {
                moveUp = true;
            }
        } else {
            RotateShape(Head, Time.deltaTime * 120 * direction);
            RotateShape(LowerArm, Time.deltaTime * 24 * direction);
            RotateShape(UpperArm, Time.deltaTime * -12 * direction);
            if ((15 > Head.Angle && !moveRight) || (-15 < Head.Angle && moveRight)) {
                moveUp = false;
            }

        }

    }

    
}



