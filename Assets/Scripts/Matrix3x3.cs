﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IGB283 {
    public class Matrix3x3 {
        private const int matrixOrder = 3;

        // The element at x, y
        public float this[int row, int column] {
            get { return m[row][column]; }
            set {
                Vector3 v = m[row];
                v[column] = value;
                m[row] = v;
            }
        }

        // The transpose of the matrix
        public Matrix3x3 transpose {
            get {
                return new Matrix3x3(
                    new Vector3(m[0].x, m[1].x, m[2].x),
                    new Vector3(m[0].y, m[1].y, m[2].y),
                    new Vector3(m[0].z, m[1].z, m[2].z));
            }
        }

        // Constructor
        // Arraylist to contain the vector data
        private List<Vector3> m = new List<Vector3>();

        // Create a matrix 3x3 with specified values
        public Matrix3x3(Vector3 r1, Vector3 r2, Vector3 r3) {
            m.Add(r1);
            m.Add(r2);
            m.Add(r3);
        }

        // Create a matrix 3x3 initialised with zeros
        public Matrix3x3() {
            m.Add(Vector3.zero);
            m.Add(Vector3.zero);
            m.Add(Vector3.zero);
        }

        // Public Functions
        // get a column of the matrix
        public Vector3 GetColumn(int column) {
            return new Vector3(m[0][column], m[1][column], m[2][column]);
        }

        // get a row of the matrix
        public Vector3 GetRow(int row) {
            return m[row];
        }

        // Transform a point by this matrix
        public Vector3 MultiplyPoint(Vector3 p) {
            p.z = 1.0f;
            Vector3 v = new Vector3(
                m[0][0] * p[0] + m[0][1] * p[1] + m[0][2] * p[2],
                m[1][0] * p[0] + m[1][1] * p[1] + m[1][2] * p[2],
                m[2][0] * p[0] + m[2][1] * p[1] + m[2][2] * p[2]
            );
            return v;
        }

        // Transform a direction by this matrix
        public Vector3 MultiplyPoint3x4(Vector3 p) {
            throw new System.NotImplementedException();
        }

        // Set a column of the matrix
        public Vector3 MultiplyVector(Vector3 v) {
            throw new System.NotImplementedException();
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

        // Sets this matrix to a translation, rotation and scaling matrix
        public void SetTRS(Vector3 pos, Quaternion q, Vector3 s) {
            throw new System.NotImplementedException("If you want to use this method you will need to implement it yourself");
        }

        // Return a string of the matrix
        public override string ToString() {
            string s = "";
            s = string.Format(
                "{0,-12:0.00000}{1,-12:0.00000}{2,-12:0.00000}\r\n" +
                "{3,-12:0.00000}{4,-12:0.00000}{5,-12:0.00000}\r\n" +
                "{6,-12:0.00000}{7,-12:0.00000}{8,-12:0.00000}\r\n",
                m[0].x, m[0].y, m[0].z,
                m[1].x, m[1].y, m[1].z,
                m[2].x, m[2].y, m[2].z);

            return s;
        }

        // Operators
        // Multiply two matrices together
        public static Matrix3x3 operator *(Matrix3x3 b, Matrix3x3 c) {
            return new Matrix3x3(
                new Vector3(b.m[0].x * c.m[0].x + b.m[0].y * c.m[1].x + b.m[0].z * c.m[2].x,
                    b.m[0].x * c.m[0].y + b.m[0].y * c.m[1].y + b.m[0].z * c.m[2].y,
                    b.m[0].x * c.m[0].z + b.m[0].y * c.m[1].z + b.m[0].z * c.m[2].z),
                new Vector3(b.m[1].x * c.m[0].x + b.m[1].y * c.m[1].x + b.m[1].z * c.m[2].x,
                    b.m[1].x * c.m[0].y + b.m[1].y * c.m[1].y + b.m[1].z * c.m[2].y,
                    b.m[1].x * c.m[0].z + b.m[1].y * c.m[1].z + b.m[1].z * c.m[2].z),
                new Vector3(b.m[2].x * c.m[0].x + b.m[2].y * c.m[1].x + b.m[2].z * c.m[2].x,
                    b.m[2].x * c.m[0].y + b.m[2].y * c.m[1].y + b.m[2].z * c.m[2].y,
                    b.m[2].x * c.m[0].z + b.m[2].y * c.m[1].z + b.m[2].z * c.m[2].z));
        }

        public static Vector3 operator *(Matrix3x3 left, Vector3 right) {
            return new Vector3(left.m[0].x * right.x + left.m[0].y * right.y + left.m[0].z * right.z,
                left.m[1].x * right.x + left.m[1].y * right.y + left.m[1].z * right.z,
                left.m[2].x * right.x + left.m[2].y * right.y + left.m[2].z * right.z);
        }

        // Multiply a matrix by a scalar 
        public static Matrix3x3 operator *(float b, Matrix3x3 c) {
            c.m[0] *= b;
            c.m[1] *= b;
            c.m[2] *= b;
            return c;
        }

        public static Matrix3x3 operator *(Matrix3x3 c, float b) {
            return b * c;
        }

        // Test the equality of this matrix and another
        public bool Equals(Matrix3x3 m2) {
            return m[0] == m2.m[0] | m[1] == m2.m[1] | m[2] == m2.m[2];
        }

    }
}
