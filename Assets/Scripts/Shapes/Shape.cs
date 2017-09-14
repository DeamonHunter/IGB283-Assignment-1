using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IGB283;
using UnityEngine;

/// <summary>
/// The base class of all shapes. Defines each singular vertex as well as other needed variables.
/// </summary>
public abstract class Shape {

    #region Shape Properties
    /// <summary>
    /// The list of all vertices that make up this shape
    /// </summary>
    public Vector3[] Vertices;

    /// <summary>
    /// The center point of the shape. For rotation reasons.
    /// </summary>
    public Vector3 Center;

    /// <summary>
    /// The maximum distance this shape can be interacted with.
    /// </summary>
    public float InteractionRadius;
    #endregion

    #region Movement Properties

    /// <summary>
    /// The speed the shape moves. Used for translating between two points.
    /// </summary>
    public float Speed;

    /// <summary>
    /// The speed the shape rotates in each axis. Only Z is used when 2D.
    /// </summary>
    public Vector3 RotationSpeed;

    /// <summary>
    /// Determines which direction the shape will move.
    /// </summary>
    public bool MoveLeft;
    #endregion

    /// <summary>
    /// Setup the shape for first use.
    /// </summary>
    public void Setup() {
        CalculateCenter();
        InteractionRadius = Vertices.Max(vert => (vert - Center).magnitude);
    }

    /// <summary>
    /// Get all triangles in order to render shape correctly.
    /// </summary>
    /// <param name="offset">The current rendering offset. As triangles are 3 numbers in a row.</param>
    /// <returns>An array of points that will render the shape correctly.</returns>
    public abstract int[] GetTriangles(int offset);

    /// <summary>
    /// Calculate the center and store it to <see cref="Center"/>.
    /// </summary>
    public void CalculateCenter() {
        Center = Vector3.zero;
        foreach (Vector3 point in Vertices) {
            Center += point;
        }
        Center /= Vertices.Length;
    }

    /// <summary>
    /// Apply the matrix transformation to all points in this shape. Ignores 3rd dimension.
    /// </summary>
    /// <param name="m">A 3x3 transformation matrix.</param>
    public void ApplyTransformation(Matrix3x3 m) {
        for (int i = 0; i < Vertices.Length; i++) {
            Vector3 vert = Vertices[i];
            float z = vert.z;
            vert.z = 1; //Set Z correctly in order to translate.
            Vertices[i] = m * vert;
            Vertices[i].z = z; //Reset Z
        }
        CalculateCenter();
    }

    /// <summary>
    /// Apply the matrix transformation to all points in this shape.
    /// </summary>
    /// <param name="m">A 4x4 transformation matrix.</param>
    public void ApplyTransformation(IGB283.Matrix4x4 m) {
        for (int i = 0; i < Vertices.Length; i++) {
            Vector3 vert = Vertices[i];
            Vertices[i] = m.MultiplyVector3(vert);
        }
        CalculateCenter();
    }
}
