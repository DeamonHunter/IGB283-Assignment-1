using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IGB283 {
    /// <summary>
    /// A watered down matrix 4x4 class with only essential elements.
    /// </summary>
    public class Matrix4x4 {
        private const int matrixOrder = 4;

        // The transpose of the matrix
        public static Matrix4x4 identity {
            get {
                return new Matrix4x4(
                    new Vector4(1, 0, 0, 0),
                    new Vector4(0, 1, 0, 0),
                    new Vector4(0, 0, 1, 0),
                    new Vector4(0, 0, 0, 1));
            }
        }


        // Constructor
        // Arraylist to contain the vector data
        private List<Vector4> m = new List<Vector4>();

        // Create a matrix 3x3 with specified values
        public Matrix4x4(Vector4 r1, Vector4 r2, Vector4 r3, Vector4 r4) {
            m.Add(r1);
            m.Add(r2);
            m.Add(r3);
            m.Add(r4);
        }

        // Create a matrix 3x3 initialised with zeros
        public Matrix4x4() {
            m.Add(Vector4.zero);
            m.Add(Vector4.zero);
            m.Add(Vector4.zero);
            m.Add(Vector4.zero);
        }


        // Transform a point by this matrix
        public Vector3 MultiplyVector3(Vector3 p) {
            Vector4 point = MultiplyVector4(new Vector4(p.x, p.y, p.z, 1));
            return new Vector3(point.x, point.y, point.z);
        }

        public Vector4 MultiplyVector4(Vector4 p) {
            Vector4 v = new Vector4(
                m[0][0] * p[0] + m[0][1] * p[1] + m[0][2] * p[2] + m[0][3] * p[3],
                m[1][0] * p[0] + m[1][1] * p[1] + m[1][2] * p[2] + m[1][3] * p[3],
                m[2][0] * p[0] + m[2][1] * p[1] + m[2][2] * p[2] + m[2][3] * p[3],
                m[2][0] * p[0] + m[2][1] * p[1] + m[2][2] * p[2] + m[2][3] * p[3]
            );
            return v;
        }

        // Set a column of the matrix
        public void SetColumn(int index, Vector3 column) {
            // Get the three row vectors of the matrix
            Vector3 r1 = m[0];
            Vector3 r2 = m[1];
            Vector3 r3 = m[2];

            // Set the index'th value to be the value from the column vector
            r1[index] = column[0];
            r2[index] = column[1];
            r3[index] = column[2];

            // Reassign the rows to he matrix
            m[0] = r1;
            m[1] = r2;
            m[2] = r3;
        }

        // Set a row of the matrix
        public void SetRow(int index, Vector3 row) {
            m[index] = row;
        }


        // Return a string of the matrix
        public override string ToString() {
            string s = "";
            s = string.Format(
                "{0,-12:0.00000}{1,-12:0.00000}{2,-12:0.00000}{3,-12:0.00000}\r\n" +
                "{4,-12:0.00000}{5,-12:0.00000}{6,-12:0.00000}{7,-12:0.00000}\r\n" +
                "{8,-12:0.00000}{9,-12:0.00000}{10,-12:0.00000}{11,-12:0.00000}\r\n" +
                "{12,-12:0.00000}{13,-12:0.00000}{14,-12:0.00000}{15,-12:0.00000}\r\n",
                m[0].x, m[0].y, m[0].z, m[0].w,
                m[1].x, m[1].y, m[1].z, m[1].w,
                m[2].x, m[2].y, m[2].z, m[2].w,
                m[3].x, m[3].y, m[3].z, m[3].w);

            return s;
        }

        // Operators
        // Multiply two matrices together
        public static Matrix4x4 operator *(Matrix4x4 b, Matrix4x4 c) {
            return new Matrix4x4(
                new Vector4(b.m[0].x * c.m[0].x + b.m[0].y * c.m[1].x + b.m[0].z * c.m[2].x + b.m[0].w * c.m[3].x,
                    b.m[0].x * c.m[0].y + b.m[0].y * c.m[1].y + b.m[0].z * c.m[2].y + b.m[0].w * c.m[3].y,
                    b.m[0].x * c.m[0].z + b.m[0].y * c.m[1].z + b.m[0].z * c.m[2].z + b.m[0].w * c.m[3].z,
                    b.m[0].x * c.m[0].w + b.m[0].y * c.m[1].w + b.m[0].z * c.m[2].w + b.m[0].w * c.m[3].w),
                new Vector4(b.m[1].x * c.m[0].x + b.m[1].y * c.m[1].x + b.m[1].z * c.m[2].x + b.m[1].w * c.m[3].x,
                    b.m[1].x * c.m[0].y + b.m[1].y * c.m[1].y + b.m[1].z * c.m[2].y + b.m[1].w * c.m[3].y,
                    b.m[1].x * c.m[0].z + b.m[1].y * c.m[1].z + b.m[1].z * c.m[2].z + b.m[1].w * c.m[3].z,
                    b.m[1].x * c.m[0].w + b.m[1].y * c.m[1].w + b.m[1].z * c.m[2].w + b.m[1].w * c.m[3].w),
                new Vector4(b.m[2].x * c.m[0].x + b.m[2].y * c.m[1].x + b.m[2].z * c.m[2].x + b.m[2].w * c.m[3].x,
                    b.m[2].x * c.m[0].y + b.m[2].y * c.m[1].y + b.m[2].z * c.m[2].y + b.m[2].w * c.m[3].y,
                    b.m[2].x * c.m[0].z + b.m[2].y * c.m[1].z + b.m[2].z * c.m[2].z + b.m[2].w * c.m[3].z,
                    b.m[2].x * c.m[0].w + b.m[2].y * c.m[1].w + b.m[2].z * c.m[2].w + b.m[2].w * c.m[3].w),
                new Vector4(b.m[3].x * c.m[0].x + b.m[3].y * c.m[1].x + b.m[1].z * c.m[2].x + b.m[1].w * c.m[3].x,
                    b.m[3].x * c.m[0].y + b.m[3].y * c.m[1].y + b.m[3].z * c.m[2].y + b.m[3].w * c.m[3].y,
                    b.m[3].x * c.m[0].z + b.m[3].y * c.m[1].z + b.m[3].z * c.m[2].z + b.m[3].w * c.m[3].z,
                    b.m[3].x * c.m[0].w + b.m[3].y * c.m[1].w + b.m[3].z * c.m[2].w + b.m[3].w * c.m[3].w));
        }

        // Override multiplication variables to allow vector mult.
        public static Vector3 operator *(Matrix4x4 left, Vector3 right) {
            return left.MultiplyVector3(right);
        }
        public static Vector4 operator *(Matrix4x4 left, Vector4 right) {
            return left.MultiplyVector4(right);
        }

        // Multiply a matrix by a scalar 
        public static Matrix4x4 operator *(float b, Matrix4x4 c) {
            c.m[0] *= b;
            c.m[1] *= b;
            c.m[2] *= b;
            c.m[3] *= b;
            return c;
        }

        public static Matrix4x4 operator *(Matrix4x4 c, float b) {
            return b * c;
        }

        // Test the equality of this matrix and another
        public bool Equals(Matrix4x4 m2) {
            return m[0] == m2.m[0] | m[1] == m2.m[1] | m[2] == m2.m[2] | m[3] == m2.m[3];
        }

    }
}
