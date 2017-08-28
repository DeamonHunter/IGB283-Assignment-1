using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IGB283 {

    public static class IGB283Transform {

        public static Matrix3x3 Translate(Vector3 vec) {
            return new Matrix3x3(new Vector3(1, 0, vec.x), new Vector3(0, 1, vec.y), new Vector3(0, 0, 1));
        }

        /// <summary>
        /// Rotate the shape by angle around the point origin.
        /// </summary>
        /// <param name="shape">The shape to rotate.</param>
        /// <param name="origin">The point to rotate around.</param>
        /// <param name="angle">The amount, in degrees, to rotate by.</param>
        public static Matrix3x3 Rotate(float angle) {
            return new Matrix3x3(new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), -Mathf.Sin(Mathf.Deg2Rad * angle), 0),
                new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0),
                new Vector3(0, 0, 1)
            );
        }

        public static Matrix3x3 Scale(Vector3 mult) {
            return new Matrix3x3(new Vector3(mult.x, 0, 0), new Vector3(0, mult.y, 0), new Vector3(0, 0, mult.z));
        }
    }
}
