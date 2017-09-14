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
    private LineRenderer lr;

    //Constants
    private const float maxSpeed = 10;

    //Shapes
    private List<Shape> shapes;

    //Dragging
    private Shape draggedShape;
    private float curDragCooldown;

    //Bounds
    private Vector2 XBounds;
    private Vector2 YBounds;
    #endregion


    /// <summary>
    /// Used to initialise all shapes.
    /// </summary>
    private void Start() {
        //Get the mesh and clear it
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //Get the linerenderer and set up bounds display
        lr = GetComponent<LineRenderer>();
        UpdateBounds();

        //Add shapes to render along with movement variables.
        shapes = new List<Shape>();
        shapes.Add(new Circle(Vector3.zero, 0.25f, 100));
        shapes[0].Speed = 3;
        shapes[0].RotationSpeed = new Vector3(60, 10, 20);

        shapes.Add(new Cube(0.5f, new Vector3(-1, 1, 0)));
        shapes[1].Speed = 5;
        shapes[1].RotationSpeed = new Vector3(10, 60, 20);

        shapes.Add(new Cube(0.5f, new Vector3(-1, -1, 0)));
        shapes[2].Speed = 2;
        shapes[2].RotationSpeed = new Vector3(30, 10, 50);

        shapes.Add(new Cube(0.5f, new Vector3(1, 1, 0)));
        shapes[3].Speed = 4;
        shapes[3].RotationSpeed = new Vector3(40, 30, 20);
        shapes[3].MoveLeft = false;

        shapes.Add(new Cube(0.5f, new Vector3(1, -1, 0)));
        shapes[4].Speed = 6;
        shapes[4].RotationSpeed = new Vector3(60, 20, 60);
        shapes[4].MoveLeft = false;
    }


    // Update is called once per frame
    private void Update() {
        //Check for left click down, but ignore any clicks when mouse is over the UI.
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0)) {
            //Get the position that the mouse is clicking at.
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);

            //Get the closest shape, if avaliable, and update the speed.
            Shape interactedShape;
            if (TryGetClosestShape(pos, out interactedShape)) {
                if (interactedShape.Speed < maxSpeed)
                    interactedShape.Speed += 1;
                draggedShape = interactedShape;
            }
        }
        //Check if the mouse is being held down as well as a shape to drag.
        if (Input.GetMouseButton(0) && draggedShape != null) {
            //Check to see if the object should be draggable.
            if (curDragCooldown > 0) {
                curDragCooldown -= Time.deltaTime;
            }
            else {
                float distance = Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).y, YBounds.x, YBounds.y) - draggedShape.Center.y;
                Matrix3x3 T = IGB283Transform.Translate(new Vector2(0, distance));
                draggedShape.ApplyTransformation(T);
            }
        }

        //Check for mouse up. Remove drag shape if occurs.
        if (Input.GetMouseButtonUp(0)) {
            draggedShape = null;
            curDragCooldown = DragCooldown;
        }

        //Check for right click down, but ignore any clicks when mouse is over the UI.
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(1)) {
            //Get the position that the mouse is clicking at.
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);

            //Get the closest shape, if avaliable, and update the speed.
            Shape interactedShape;
            if (TryGetClosestShape(pos, out interactedShape)) {
                if (interactedShape.Speed > 0)
                    interactedShape.Speed -= 1;
            }
        }

        //Move all shapes
        foreach (var shape in shapes) {
            RotateAndTranslate(shape);
        }

        //Update the mesh to reflect changes
        UpdateMesh();
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

        //Change colours to reflect new positions
        Color[] colors = new Color[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++) {
            colors[i] = Color.Lerp(Colours[0], Colours[1], (mesh.vertices[i].x - XBounds.x) / (XBounds.y - XBounds.x));
        }
        mesh.colors = colors;
    }

    /// <summary>
    /// Tries to get the closest shape to the point. Fails if the shape is outside the interaction range for it.
    /// </summary>
    /// <param name="pos">The point to check nearby.</param>
    /// <param name="interactedShape">If successful, will return the closest shape.</param>
    /// <returns>Whether or not the function succeeded.</returns>
    private bool TryGetClosestShape(Vector3 pos, out Shape interactedShape) {
        //Initialise with values from the first shape
        float smallestMag = (shapes[0].Center - pos).magnitude;
        interactedShape = shapes[0];

        //Search for all shapes and compare values to find closest.
        for (int i = 1; i < shapes.Count; i++) {
            if ((shapes[i].Center - pos).magnitude < smallestMag) {
                interactedShape = shapes[i];
                smallestMag = (shapes[i].Center - pos).magnitude;
            }
        }

        //Return if it was successful
        return smallestMag < interactedShape.InteractionRadius;
    }

    /// <summary>
    /// Main function which implements rotation and translation for all shapes.
    /// </summary>
    /// <param name="shape">Shape to move.</param>
    private void RotateAndTranslate(Shape shape) {

        //Determine which direction to move the shape in.
        Vector3 moveDir;
        if (!shape.MoveLeft) {
            if (shape.Center.x >= XBounds.y)
                shape.MoveLeft = true;
            moveDir = Vector3.right;
        }
        else {
            if (shape.Center.x <= XBounds.x)
                shape.MoveLeft = false;
            moveDir = Vector3.left;
        }

        //Change movement depending on whether or not 3D was selected.
        if (!ThreeDimensional) {
            var center = new Vector2(shape.Center.x, shape.Center.y);
            Matrix3x3 T = IGB283Transform.Translate(-center);
            Matrix3x3 R = IGB283Transform.Rotate(shape.RotationSpeed.z * Time.deltaTime);
            Matrix3x3 TReverse = IGB283Transform.Translate(center + Time.deltaTime * shape.Speed * new Vector2(moveDir.x, moveDir.y));
            shape.ApplyTransformation(TReverse * R * T);
        }
        else {
            IGB283.Matrix4x4 T = IGB283Transform.Translate(-shape.Center);
            IGB283.Matrix4x4 R = IGB283Transform.Rotate(shape.RotationSpeed * Time.deltaTime);
            IGB283.Matrix4x4 TReverse = IGB283Transform.Translate(shape.Center + Time.deltaTime * shape.Speed * moveDir);
            shape.ApplyTransformation(TReverse * R * T);
        }
    }


    #region UI ELEMENTS
    /// <summary>
    /// Change the 3D mode on transformations.
    /// </summary>
    /// <param name="toggle">Value to use.</param>
    public void Toggle3D(bool toggle) {
        ThreeDimensional = toggle;
    }


    /// <summary>
    /// Update the maximum bounds of shapes, as well as the linerenderer.
    /// </summary>
    public void UpdateBounds() {
        XBounds.x = Sliders[0].value;
        XBounds.y = Sliders[1].value;
        YBounds.x = Sliders[2].value;
        YBounds.y = Sliders[3].value;
        lr.positionCount = 4;
        lr.SetPositions(new[] { new Vector3(XBounds.x, YBounds.x), new Vector3(XBounds.x, YBounds.y), new Vector3(XBounds.y, YBounds.y), new Vector3(XBounds.y, YBounds.x) });
    }
    #endregion
}



