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
    private void Update() {
        IGB283.Matrix3x3 T = IGB283Transform.Translate(-(Vector2)UpperArm.RotateCenter);
        IGB283.Matrix3x3 R = IGB283Transform.Rotate(50f * Time.deltaTime);
        IGB283.Matrix3x3 TReverse = IGB283Transform.Translate((Vector2)UpperArm.RotateCenter);

        UpperArm.ApplyTransformation(TReverse * R * T);

        T = IGB283Transform.Translate(-(Vector2)LowerArm.RotateCenter);
        R = IGB283Transform.Rotate(-50f * Time.deltaTime);
        TReverse = IGB283Transform.Translate((Vector2)LowerArm.RotateCenter);

        LowerArm.ApplyTransformation(TReverse * R * T);


        T = IGB283Transform.Translate(-(Vector2)Head.RotateCenter);
        R = IGB283Transform.Rotate(50f * Time.deltaTime);
        TReverse = IGB283Transform.Translate((Vector2)Head.RotateCenter);

        Head.ApplyTransformation(TReverse * R * T);
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

        mesh.colors = new Color[] { Color.white, Color.white, Color.white, Color.white, Color.black, Color.black, Color.black, Color.black, Color.red, Color.red, Color.red, Color.red, Color.blue, Color.blue, Color.blue, Color.blue };
    }
}



