﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IGB283;
using UnityEngine;

/// <summary>
/// The base class of all shapes.
/// </summary>
public abstract class Shape {
    /// <summary>
    /// The list of all vertices that make up this shape
    /// </summary>
    public Vector3[] Vertices;

    /// <summary>
    /// The center point of the shape. For rotation reasons.
    /// </summary>
    public Vector3 Center;

    public float InteractionRadius;
    public bool MoveTowardsFirst;
    
    /// <summary>
    /// Get all triangles in order to render shape correctly.
    /// </summary>
    /// <param name="offset">The current rendering offset. As triangles are 3 numbers in a row.</param>
    /// <returns>An array of points that will render the shape correctly.</returns>
    public abstract int[] GetTriangles(int offset);

    /// <summary>
    /// The speed the shape moves. Used for translating between two points.
    /// </summary>
    public float Speed;
    public Vector3 RotationSpeed;

    public void Setup() {
        CalculateCenter();
        InteractionRadius = Vertices.Max(vert => (vert - Center).sqrMagnitude);
        //Debug.Log(InteractionRadius);
    }

    /// <summary>
    /// Calculate the center and store it to <see cref="Center"/>.
    /// </summary>
    public void CalculateCenter() {
        Center = Vector3.zero;
        foreach (var point in Vertices) {
            Center += point;
        }
        Center /= Vertices.Length;
    }

    public void ApplyTransformation(Matrix3x3 m) {

        for (int i = 0; i < Vertices.Length; i++) {
            var vert = Vertices[i];
            float z = vert.z;
            vert.z = 1; //Set Z correctly in order to translate.
            Vertices[i] = m * vert;
            Vertices[i].z = z; //Reset Z
        }

        CalculateCenter();
    }

    public void ApplyTransformation(IGB283.Matrix4x4 m) {

        for (int i = 0; i < Vertices.Length; i++) {
            var vert = Vertices[i];
            Vertices[i] = m.MultiplyVector3(vert);
        }

        CalculateCenter();
    }
}
