using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    /// Get all triangles in order to render shape correctly.
    /// </summary>
    /// <param name="offset">The current rendering offset. As triangles are 3 numbers in a row.</param>
    /// <returns>An array of points that will render the shape correctly.</returns>
    public abstract int[] GetTriangles(int offset);
}
