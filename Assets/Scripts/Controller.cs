using System.Collections.Generic;
using IGB283;
using UnityEngine;

/// <summary>
/// Main controller of the assignment.
/// </summary>
public class Controller : MonoBehaviour {
    #region Public Variables
    public Color[] Colours;
    #endregion

    #region Robot Parts
    private Shape head;
    public GameObject HeadCollider;
    private Shape upperArm;
    public GameObject UpperArmCollider;
    private Shape lowerArm;
    public GameObject LowerArmCollider;
    private Shape body;
    public GameObject BodyCollider;
    #endregion

    #region Private Variables
    //Components
    private Mesh mesh;

    //Constants
    private const float walkingSpeed = 3;
    private const float jumpingSpeed = 5.5f;
    private const float gravity = -18;

    //Shapes
    private List<Shape> shapes;

    //Animation Parameters
    private bool fallingDown;
    private bool gettingUp;
    private bool moveRight = true;
    private bool movingForward;
    private bool jumpingUp;
    private bool jumpingForward;
    private bool animating;
    private bool moveHeadUp;

    //Other Parameters
    private float verticalMomentum;
    private float gettingUpCooldown;
    private float jumpCooldown;
    #endregion

    /// <summary>
    /// Used to initialise all shapes.
    /// </summary>
    private void Start() {
        //Get the mesh and clear it
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //Create the robot arm and setup the tree structure needed.
        shapes = new List<Shape>();

        body = new Square(new Vector3(-1f, -1f, 0), new Vector3(-1f, 0f, 0), new Vector3(1f, 0f, 0), new Vector3(1, -1, 0));
        body.RotateCenter = new Vector3(0, -0.5f, 0);
        body.ShapeCollider = BodyCollider;
        body.Parent = true;
        shapes.Add(body);

        upperArm = new Square(new Vector3(-0.25f, 0f, 0), new Vector3(-0.25f, 2f, 0), new Vector3(0.25f, 2f, 0), new Vector3(0.25f, 0f, 0));
        upperArm.RotateCenter = new Vector3(0, 0f, 0);
        upperArm.ShapeCollider = UpperArmCollider;
        body.AddChild(upperArm);
        shapes.Add(upperArm);

        lowerArm = new Square(new Vector3(-0.25f, 2f, 0), new Vector3(-0.25f, 4f, 0), new Vector3(0.25f, 4f, 0), new Vector3(0.25f, 2f, 0));
        lowerArm.RotateCenter = new Vector3(0, 2f, 0);
        lowerArm.ShapeCollider = LowerArmCollider;
        upperArm.AddChild(lowerArm);
        shapes.Add(lowerArm);

        head = new Square(new Vector3(-0.1f, 4f, 0), new Vector3(-0.1f, 5f, 0), new Vector3(0.1f, 5f, 0), new Vector3(0.1f, 4f, 0));
        head.RotateCenter = new Vector3(0, 4f, 0);
        head.ShapeCollider = HeadCollider;
        lowerArm.AddChild(head);
        shapes.Add(head);
    }


    // Update is called once per frame
    private void Update()
    {
        if (!gettingUp && !fallingDown) {
            //Fall down on command if not already falling down.
            if (Input.GetKeyDown(KeyCode.Z))
                fallingDown = true;
        }

        //Apply the falling momentum. See if on ground.
        bool onGround = ApplyVerticalMomentum();


        if (fallingDown)
            FallDown();
        else if (gettingUp)
            GettingUp();
        else
        {
            //Perform normal animations if not falling down.
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
                //If not jumpingg. Allow the player to steer and perform jumps.
                if (Input.GetKey(KeyCode.A))
                    moveRight = false;
                else if (Input.GetKey(KeyCode.D))
                    moveRight = true;
                TranslateLeftAndRight(onGround);
                if (Input.GetKey(KeyCode.W)) {
                    JumpStraightUp();
                } else if (Input.GetKey(KeyCode.S)) {
                    JumpForward();
                }
            }

            //Lower jump cooldowns if on the ground.
            if (onGround) {
                jumpCooldown -= Time.deltaTime;
                if (jumpCooldown <= 0 && !animating) {
                    jumpingUp = false;
                    jumpingForward = false;
                }
            }
        }

        //Update the mesh to reflect changes
        UpdateMesh();
    }

    #region Movement Functions
    /// <summary>
    /// Move the base of the shape vertically based off <see cref="verticalMomentum"/>
    /// </summary>
    /// <returns>Whether the shape is on the ground.</returns>
    private bool ApplyVerticalMomentum()
    {
        // See if there is ground below.
        var hit = Physics2D.Raycast(new Vector2((body.Vertices[0].x + body.Vertices[2].x) / 2, body.Vertices[0].y),
            Vector2.down, 0.01f);
        if (hit && verticalMomentum <= 0)
        {
            //If there is ground and heading downwards, snap to it
            Matrix3x3 T = IGB283Transform.Translate(new Vector2(0, -hit.distance));
            body.ApplyTransformation(T);
            verticalMomentum = 0;
            return true;
        }
        else {
            //Otherwise continue as normal
            Matrix3x3 T = IGB283Transform.Translate(new Vector2(0, verticalMomentum * Time.deltaTime));
            body.ApplyTransformation(T);
            verticalMomentum += gravity * Time.deltaTime;
            return false;
        }
    }

    /// <summary>
    /// Move the base of the shape. Also turn around the shape if it runs into anything while on the ground.
    /// </summary>
    /// <param name="onGround">Whether or not the base is on the ground.</param>
    private void TranslateLeftAndRight(bool onGround) {
        //See if there is a wall in the direction we are travelling.
        float distance = onGround ? walkingSpeed * Time.deltaTime : jumpingSpeed * Time.deltaTime;
        Vector2 direction = moveRight ? Vector2.right * distance : Vector2.left * distance;
        Vector2 pos = new Vector2(moveRight ? body.Vertices[2].x : body.Vertices[0].x,
            (body.Vertices[0].y + body.Vertices[1].y) / 2);
        var hit = Physics2D.Raycast(pos, direction, distance, 1);
        //If on the ground, change direction. If there is no wall move as normal.
        if (hit) {
            if (onGround) {
                moveRight = !moveRight;
            }
            else
                return;
        }
        Matrix3x3 T = IGB283Transform.Translate(direction);
        body.ApplyTransformation(T);
    }

    /// <summary>
    /// Rotates the <see cref="Shape"/> by the angle provided. Rotations permute through children.
    /// </summary>
    /// <param name="shape">The shape to rotate.</param>
    /// <param name="angle">The clockwise angle to rotate by. (In degrees.)</param>
    private void RotateShape(Shape shape, float angle) {
        Matrix3x3 T = IGB283Transform.Translate(-(Vector2)shape.RotateCenter);
        Matrix3x3 R = IGB283Transform.Rotate(angle);
        Matrix3x3 TReverse = IGB283Transform.Translate((Vector2)shape.RotateCenter);
        shape.ApplyTransformation(TReverse * R * T, angle);
    }
    #endregion

    #region Animation Functions
    private void FallDown() {
        //Change animation direction depending on direction of movement.
        int direction = moveRight ? 1 : -1;
        if ((head.Angle < 0 && moveRight) || (head.Angle > 0 && !moveRight)) {
            RotateShape(head, Time.deltaTime * 160 * direction);
        }
        if ((lowerArm.Angle < 15 && moveRight) || (lowerArm.Angle > -15 && !moveRight)) {
            RotateShape(lowerArm, Time.deltaTime * 80 * direction);
        }

        if ((upperArm.Angle > -110 && moveRight) || (upperArm.Angle < 110 && !moveRight)) {
            RotateShape(upperArm, Time.deltaTime * -200 * direction);
        }
        else {
            fallingDown = false;
            gettingUp = true;
            gettingUpCooldown = 1f;
        }

    }

    private void GettingUp() {
        //Don't get up unless cooldown is met.
        if (gettingUpCooldown > 0) {
            gettingUpCooldown -= Time.deltaTime;
            return;
        }

        //Change animation direction depending on direction of movement.
        int direction = moveRight ? 1 : -1;

        if ((upperArm.Angle < -60 && moveRight) || (upperArm.Angle > 60 && !moveRight)) {
            RotateShape(upperArm, Time.deltaTime * 40 * direction);
            RotateShape(lowerArm, Time.deltaTime * 30 * direction);
            RotateShape(head, Time.deltaTime * 30 * direction);
        }
        else if ((upperArm.Angle < 0 && moveRight) || (upperArm.Angle > 0 && !moveRight)) {
            RotateShape(upperArm, Time.deltaTime * 100 * direction);
            if ((lowerArm.Angle > 0 && moveRight) || (lowerArm.Angle < 0 && !moveRight))
                RotateShape(lowerArm, Time.deltaTime * -100 * direction);
            if ((head.Angle > 0 && moveRight) || (head.Angle < 0 && !moveRight))
                RotateShape(head, Time.deltaTime * -50 * direction);
        }
        else {
            //Reset everything properly
            gettingUp = false;
            RotateShape(upperArm, -upperArm.Angle);
            RotateShape(lowerArm, -lowerArm.Angle);
            RotateShape(head, -head.Angle);
        }
    }

    private void JumpStraightUp() {
        //Setup if this is a "fresh" jump
        if (!jumpingUp && !animating && jumpCooldown <= 0) {
            verticalMomentum = 9;
            jumpCooldown = 0.5f;
            jumpingUp = true;
            animating = true;
            movingForward = true;
        }

        //Change animation direction depending on direction of movement.
        int direction = moveRight ? 1 : -1;

        if (((upperArm.Angle < 15 && moveRight) || (upperArm.Angle > -15 && !moveRight)) && movingForward) {
            RotateShape(upperArm, Time.deltaTime * 50 * direction);
            if ((upperArm.Angle >= 15 && moveRight) || (upperArm.Angle <= -15 && !moveRight))
                movingForward = false;
            RotateShape(lowerArm, Time.deltaTime * 40 * direction);
        }
        else if (((upperArm.Angle > 0 && moveRight) || (upperArm.Angle < 0 && !moveRight)) && !movingForward) {
            RotateShape(upperArm, Time.deltaTime * -50 * direction);
            if ((lowerArm.Angle > 0 && moveRight) || (lowerArm.Angle < 0 && !moveRight))
                RotateShape(lowerArm, Time.deltaTime * -40 * direction);
        }
        else {
            //Reset everything properly
            animating = false;
            RotateShape(upperArm, -upperArm.Angle);
            RotateShape(lowerArm, -lowerArm.Angle);
        }

    }

    private void JumpForward() {
        //Setup if this is a "fresh" jump
        if (!jumpingForward && !animating && jumpCooldown <= 0) {
            verticalMomentum = 6.5f;
            jumpCooldown = 0.5f;
            jumpingForward = true;
            animating = true;
            movingForward = true;
        }

        //Change animation direction depending on direction of movement.
        int direction = moveRight ? 1 : -1;

        if (((upperArm.Angle < 30 && moveRight) || (upperArm.Angle > -30 && !moveRight)) && movingForward) {
            RotateShape(upperArm, Time.deltaTime * 60 * direction);
            if ((upperArm.Angle >= 30 && moveRight) || (upperArm.Angle <= -30 && !moveRight))
                movingForward = false;
            RotateShape(lowerArm, Time.deltaTime * -20 * direction);
        }
        else if (((upperArm.Angle > 0 && moveRight) || (upperArm.Angle < 0 && !moveRight)) && !movingForward) {
            RotateShape(upperArm, Time.deltaTime * -60 * direction);
            if ((lowerArm.Angle < 0 && moveRight) || (lowerArm.Angle > 0 && !moveRight))
                RotateShape(lowerArm, Time.deltaTime * 20 * direction);
        }
        else {
            //Reset everything properly
            animating = false;
            RotateShape(upperArm, -upperArm.Angle);
            RotateShape(lowerArm, -lowerArm.Angle);
        }
    }

    private void Nodding() {
        //Change animation direction depending on direction of movement.
        int direction = moveRight ? 1 : -1;

        if (!moveHeadUp) {
            RotateShape(head, Time.deltaTime * -140 * direction);
            RotateShape(lowerArm, Time.deltaTime * -28 * direction);
            RotateShape(upperArm, Time.deltaTime * 14 * direction);
            if ((60 < head.Angle && !moveRight) || (-60 > head.Angle && moveRight)) {
                moveHeadUp = true;
            }
        }
        else {
            RotateShape(head, Time.deltaTime * 120 * direction);
            RotateShape(lowerArm, Time.deltaTime * 24 * direction);
            RotateShape(upperArm, Time.deltaTime * -12 * direction);
            if ((15 > head.Angle && !moveRight) || (-15 < head.Angle && moveRight)) {
                moveHeadUp = false;
            }

        }
    }
    #endregion

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
}



