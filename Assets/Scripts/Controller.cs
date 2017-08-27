﻿using System.Collections;
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

        IGB283Transform.Rotate(shapes[0], 10 * Time.deltaTime);
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

    private void RotateAndTranslate(Shape shape, float angle, Vector3 point1, Vector3 point2) {
        IGB283Transform.Translate(shape, point1); //Start at point 1
        if () {
            IGB283Transform.Translate(shape, point2 * shape.Speed * Time.deltaTime); //move towards point 2
            IGB283Transform.Rotate(shape, angle * Time.deltaTime); //
        } else {
            IGB283Transform.Translate(shape, point1 * shape.Speed * Time.deltaTime); //move towards point 1
            IGB283Transform.Rotate(shape, angle * Time.deltaTime);
        }


        
        
    }
}
