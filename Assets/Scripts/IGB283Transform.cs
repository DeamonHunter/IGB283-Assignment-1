using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IGB283 {

    public static class IGB283Transform {

        /// <summary>
        /// Two dimensional Transform.
        /// </summary>
        /// <param name="vec">The vector to move two dimensions.</param>
        /// <returns>A 3x3 Matrix that multiplies with a vector to give the appropriate translation.</returns>
        public static Matrix3x3 Translate(Vector2 vec) {
            return new Matrix3x3(new Vector3(1, 0, vec.x), new Vector3(0, 1, vec.y), new Vector3(0, 0, 1));
        }

        /// <summary>
        /// Three dimensional Transform.
        /// </summary>
        /// <param name="vec">The vector to move three dimensions.</param>
        /// <returns>A 4x4 Matrix that multiplies with a vector to give the appropriate translation.</returns>
        public static Matrix4x4 Translate(Vector3 vec) {
            return new Matrix4x4(new Vector4(1, 0, 0, vec.x), new Vector4(0, 1, 0, vec.y), new Vector4(0, 0, 1, vec.z), new Vector4(0, 0, 0, 1));
        }

        /// <summary>
        /// Two Dimensional Rotation.
        /// </summary>
        /// <param name="angle">The amount, in degrees, to rotate by.</param>
        /// <returns>A 3x3 Matrix that multiplies with a vector to give the appropriate rotation.</returns>
        public static Matrix3x3 Rotate(float angle) {
            return new Matrix3x3(new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), -Mathf.Sin(Mathf.Deg2Rad * angle), 0),
                new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0),
                new Vector3(0, 0, 1)
            );
        }

        /// <summary>
        /// Three Dimensional rotation around a singular axis.
        /// </summary>
        /// <param name="angle">The amount, in degrees, to rotate by.</param>
        /// <param name="axis">Which <see cref="Axis"/> to rotate around.</param>
        /// <returns>A 4x4 Matrix that multiplies with a vector to give the appropriate rotation.</returns>
        public static Matrix4x4 Rotate(float angle, Axis axis) {
            switch (axis) {
                case Axis.Z:
                    return new Matrix4x4(new Vector4(Mathf.Cos(Mathf.Deg2Rad * angle), -Mathf.Sin(Mathf.Deg2Rad * angle), 0, 0),
                        new Vector4(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0, 0),
                        new Vector4(0, 0, 1, 0),
                        new Vector4(0, 0, 0, 1)
                    );
                case Axis.Y:
                    return new Matrix4x4(new Vector4(Mathf.Cos(Mathf.Deg2Rad * angle), 0, Mathf.Sin(Mathf.Deg2Rad * angle), 0),
                        new Vector4(0, 1, 0, 0),
                        new Vector4(-Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle), 0),
                        new Vector4(0, 0, 0, 1)
                    );
                case Axis.X:
                    return new Matrix4x4(new Vector4(1, 0, 0, 0),
                        new Vector4(0, Mathf.Cos(Mathf.Deg2Rad * angle), -Mathf.Sin(Mathf.Deg2Rad * angle), 0),
                        new Vector4(0, Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0),
                        new Vector4(0, 0, 0, 1)
                    );
                default:
                    return null;
            }
        }


        /// <summary>
        /// Three Dimensional Rotation around all 3 axises.
        /// </summary>
        /// <param name="angle">The amount, in degrees, to rotate by contained in a vector.</param>
        /// <returns>A 4x4 Matrix that multiplies with a vector to give the appropriate rotation.</returns>
        public static Matrix4x4 Rotate(Vector3 angles) {
            Matrix4x4 rx = Rotate(angles.x, Axis.X);
            Matrix4x4 ry = Rotate(angles.y, Axis.Y);
            Matrix4x4 rz = Rotate(angles.z, Axis.Z);
            return rz * ry * rx;
        }

        /// <summary>
        /// Two Dimensional Scale matrix.
        /// </summary>
        /// <param name="mult">The multiplier for each direction.</param>
        /// <returns>A 3x3 Matrix that multiplies with a vector to give the appropriate scale.</returns>
        public static Matrix3x3 Scale(Vector2 mult) {
            return new Matrix3x3(new Vector3(mult.x, 0, 0), new Vector3(0, mult.y, 0), new Vector3(0, 0, 1));
        }

        /// <summary>
        /// Three Dimensional Scale matrix.
        /// </summary>
        /// <param name="mult">The multiplier for each direction.</param>
        /// <returns>A 4x4 Matrix that multiplies with a vector to give the appropriate scale.</returns>
        public static Matrix4x4 Scale(Vector3 mult) {
            return new Matrix4x4(new Vector4(mult.x, 0, 0, 0), new Vector4(0, mult.y, 0, 0), new Vector4(0, 0, mult.z, 0), new Vector4(0, 0, 0, 1));
        }

        /// <summary>
        /// The axis to rotate around.
        /// </summary>
        public enum Axis { X, Y, Z }
    }
}
