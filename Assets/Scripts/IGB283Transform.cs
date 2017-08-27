using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IGB283 {

    public static class IGB283Transform {

        public static void Translate(Shape shape, Vector3 vec) {
            for (int i = 0; i < shape.Vertices.Length; i++) {
                shape.Vertices[i] += vec;
            }
        }

        /// <summary>
        /// Rotate the shape by angle around the shape's center
        /// </summary>
        /// <param name="shape">The shape to rotate.</param>
        /// <param name="angle">The amount, in degrees, to rotate by.</param>
        public static void Rotate(Shape shape, float angle) {
            Rotate(shape, shape.Center, angle);
        }

        /// <summary>
        /// Rotate the shape by angle around the point origin.
        /// </summary>
        /// <param name="shape">The shape to rotate.</param>
        /// <param name="origin">The point to rotate around.</param>
        /// <param name="angle">The amount, in degrees, to rotate by.</param>
        public static void Rotate(Shape shape, Vector3 origin, float angle) {
            var m = new Matrix3x3(new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), -Mathf.Sin(Mathf.Deg2Rad * angle), 0),
                new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0),
                new Vector3(0, 0, 1)
                );

            for (int i = 0; i < shape.Vertices.Length; i++) {
                shape.Vertices[i] -= origin;
                shape.Vertices[i] *= m;
                shape.Vertices[i] += origin;
            }

            shape.CalculateCenter();
        }
        public static void Scale(Shape shape, float mult) {
            Scale(shape, Vector3.one * mult, shape.Center);
        }

        public static void Scale(Shape shape, Vector3 mult) {
            Scale(shape, mult, shape.Center);
        }

        public static void Scale(Shape shape, float mult, Vector3 origin) {
            Scale(shape, Vector3.one * mult, origin);
        }
        public static void Scale(Shape shape, Vector3 mult, Vector3 origin) {
            for (int i = 0; i < shape.Vertices.Length; i++) {
                shape.Vertices[i] -= origin;
                shape.Vertices[i] = ElementWiseVectorMult(shape.Vertices[i], mult);
                shape.Vertices[i] += origin;
            }

            shape.CalculateCenter();
        }

        public static Vector3 ElementWiseVectorMult(Vector3 left, Vector3 right) {
            return new Vector3(left.x * right.x, left.y * right.y, left.z * right.z);
        }
    }

}
