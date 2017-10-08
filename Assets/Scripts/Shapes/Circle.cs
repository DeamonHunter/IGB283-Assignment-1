using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class that defines a circle based on a center point
/// </summary>
public class Circle : Shape {
    private int[] triangles;

    /// <summary>
    /// Constructer for Circle.
    /// </summary>
    /// <param name="center">The center point of the circle.</param>
    /// <param name="radius">The distance the edge of the circle is from the center.</param>
    /// <param name="numOfTriangles">Number of triangles to use as part of this circle</param>
    public Circle(Vector3 center, float radius, int numOfTriangles) {
        Debug.Assert(numOfTriangles > 3);

        Vertices = new Vector3[numOfTriangles + 1];
        triangles = new int[numOfTriangles * 3];
        float theta = 360f / (numOfTriangles);

        Vertices[0] = center;
        for (int i = 0; i < numOfTriangles; i++) {
            Vertices[i + 1] = new Vector3(center.x + Mathf.Cos(i * theta * Mathf.Deg2Rad) * radius, center.y + Mathf.Sin(i * theta * Mathf.Deg2Rad) * radius);
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            if (i == numOfTriangles - 1)
                triangles[i * 3 + 2] = 1;
            else
                triangles[i * 3 + 2] = i + 2;
        }
    }

    public override int[] GetTriangles(int offset) {
        return triangles;
    }
}
