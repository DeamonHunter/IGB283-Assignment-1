//using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using Matrix4x4 = IGB283.Matrix4x4;

public class Matrix4x4Test {

    [Test]
    public void Matrix4x4TestSimplePasses() {
        Matrix4x4 identity = Matrix4x4.identity;
        Matrix4x4 a = new Matrix4x4(new Vector4(10, 948, 23, 5), new Vector4(1, 948, 23, 5), new Vector4(10, 948, 23, 5), new Vector4(1, 948, 23, 5));
        Matrix4x4 b = new Matrix4x4(new Vector4(10, 1, 948, 5), new Vector4(1, 948, 23, 5), new Vector4(10, 948, 23, 5), new Vector4(1, 948, 23, 5));
        Matrix4x4 r = new Matrix4x4(new Vector4(1283, 925258, 31928, 4930), new Vector4(1193, 925249, 23396, 4885), new Vector4(1283, 925258, 31928, 4930), new Vector4(1193, 925249, 23396, 4885));
        Matrix4x4 c = new Matrix4x4(new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 5), new Vector4(0, 0, 1, 5), new Vector4(0, 0, 0, 1));
        Vector3 vec = new Vector3(1, 1, 1);
        Vector3 rvec = new Vector3(2, 6, 6);
        Debug.Log(identity * identity);
        Debug.Assert((identity * identity) == identity);
        Debug.Log(a * b);
        Debug.Assert((a * b) == r);
        Debug.Log(c.MultiplyVector3(vec));
        Debug.Assert(c.MultiplyVector3(vec) == rvec);

    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator Matrix4x4TestWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
