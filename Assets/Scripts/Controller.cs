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
    public GameObject BodyCollider;

    private bool fallingDown;
    private float verticalMomentum;
    private const float gravity = -18;
    private float gettingUpCooldown;
    private bool gettingUp;
    private bool moveUp = false;
    private float jumpCooldown;
    private bool moveRight = true;
    private float walkingSpeed = 5;
    private bool jumpingUp;
    private bool jumpingForward;
    bool movingForward = true;

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
                if (jumpCooldown <= 0)
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
            Vector2.down, 0.05f);
        if (hit && verticalMomentum <= 0)
        {
            Matrix3x3 T = IGB283Transform.Translate(new Vector2(0, -hit.distance));
            AdjustBody(T);
            verticalMomentum = 0;
            return true;
        }
        else {
            Matrix3x3 T = IGB283Transform.Translate(new Vector2(0, verticalMomentum * Time.deltaTime));
            AdjustBody(T);
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

    private void TranslateLeftAndRight(bool onGround)
    {
        float distance = walkingSpeed * Time.deltaTime;
        Vector2 direction = moveRight ? Vector2.right * distance : Vector2.left * distance;
        Vector2 pos = new Vector2(moveRight ? Body.Vertices[2].x : Body.Vertices[0].x,
            (Body.Vertices[0].y + Body.Vertices[1].y) / 2);
        var hit = Physics2D.Raycast(pos, direction, distance, 1);
        if (hit) {
            if (onGround)
            {
                //Move Head Angle here
                moveRight = !moveRight;
            }
            else
                return;
        }
        Matrix3x3 T = IGB283Transform.Translate(direction);
        AdjustBody(T);
    }

    private void JumpStraightUp()
    {
        if (!jumpingUp && jumpCooldown <= 0)
        {
            verticalMomentum = 9;
            jumpCooldown = 0.5f;
            jumpingUp = true;
        }
        // Do animation here
        // Do animation here
        if (UpperArm.Angle < 15 && movingForward) {
            Debug.Log("first statement");
            RotateShape(UpperArm, Time.deltaTime * 50);
            RotateShape(LowerArm, Time.deltaTime * 40);
            RotateShape(Head, Time.deltaTime * 30);
        } else if (UpperArm.Angle >= 15 && movingForward) {
            Debug.Log("second statement");
            movingForward = false;
        } else if (UpperArm.Angle > 0 && !movingForward) {
            Debug.Log("third statement");
            Debug.Log(UpperArm.Angle);
            RotateShape(UpperArm, Time.deltaTime * -50);
            RotateShape(LowerArm, Time.deltaTime * -40);
            RotateShape(Head, Time.deltaTime * -30);
        }

    }

    private void JumpForward() {
        if (!jumpingForward && jumpCooldown <= 0) {
            verticalMomentum = 5;
            jumpCooldown = 0.5f;
            jumpingForward = true;
        }
        // Do animation here
        if (UpperArm.Angle < 30 && movingForward) {
            Debug.Log("first statement");
            RotateShape(UpperArm, Time.deltaTime * 60);
            RotateShape(LowerArm, Time.deltaTime * 50);
            RotateShape(Head, Time.deltaTime * 40);
        } else if (UpperArm.Angle >= 30 && movingForward) {
            Debug.Log("second statement");
            movingForward = false;
        } else if (UpperArm.Angle > 0 && !movingForward) {
            Debug.Log("third statement");
            Debug.Log(UpperArm.Angle);
            RotateShape(UpperArm, Time.deltaTime * -60);
            RotateShape(LowerArm, Time.deltaTime * -50);
            RotateShape(Head, Time.deltaTime * -40);
        }
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

    private void AdjustBody(Matrix3x3 m)
    {
        Body.ApplyTransformation(m);
        BodyCollider.transform.position = m * BodyCollider.transform.position;
        BodyCollider.transform.rotation = Quaternion.Euler(0,0,Body.Angle);
    }
}



