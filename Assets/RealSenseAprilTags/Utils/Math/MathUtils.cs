using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Babilinapps.RealSenseAprilTags.Utils.Math
{
    public static class MathUtils
    {
#region Matrices Calculation Info

        //Learn about Matrices here: https://github.com/Bunny83/Unity-Articles/blob/master/Matrix%20crash%20course.md

        //  member variables |      indices
        // ------------------|-----------------
        // m00 m01 m02 m03   |   00  04  08  12
        // m10 m11 m12 m13   |   01  05  09  13
        // m20 m21 m22 m23   |   02  06  10  14
        // m30 m31 m32 m33   |   03  07  11  15

#endregion

#region Quaternion Average Calculation Info

        //Based off - except with built-in Unity Methods
        //https://forum.unity.com/threads/average-quaternions.86898/

#endregion

        public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;

            if (Vector3.Cross(matrix.GetColumn(0), matrix.GetColumn(1)).normalized != (Vector3) matrix.GetColumn(2).normalized)
            {
                scale.x *= -1;
            }

            return scale;
        }





        public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
        {
           //https://docs.unity3d.com/ScriptReference/Matrix4x4.html

            Vector3 translate;
            translate.x = matrix.m03;
            translate.y = matrix.m13;
            translate.z = matrix.m23;
            return translate;
        }

        [Obsolete("Method is obsolete. use  matrix.rotation instead", false)]
        public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
        {
            //https://answers.unity.com/questions/11363/converting-matrix4x4-to-quaternion-vector3.html
            //Column index 2 
            Vector3 forward = new Vector3(matrix.m02, matrix.m12, matrix.m22);
            //Column index 1 
            Vector3 upwards = new Vector3(matrix.m01, matrix.m11, matrix.m21);


            return Quaternion.LookRotation(forward, upwards);
        }

#region Quaternion Averages

        public static Quaternion Average(Quaternion reference, Quaternion[] source)
        {
            var referenceInverse = Quaternion.Inverse(reference);

            Vector3 result = new Vector3();
            float angle = 0;
            foreach (var q in source)
            {
                (referenceInverse * q).ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
                result += rotationAxis;
                angle += angleInDegrees;

            }

            result /= source.Length;
            angle /= source.Length;
            return reference * Quaternion.AngleAxis(angle, result);
        }
        public static Quaternion Average(Quaternion[] source)
        {
            Vector3 result = new Vector3();
            float angle = 0;
            foreach (var q in source)
            {
                q.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
                result += rotationAxis;
                angle += angleInDegrees;
            }

            result /= source.Length;
            angle /= source.Length;
            return Quaternion.AngleAxis(angle, result);
        }
        public static Quaternion Average(Quaternion[] source, int iterations)
        {
            Assert.IsFalse(source.Length > 0);
            var reference = Quaternion.identity;
            for (int i = 0; i < iterations; i++)
            {
                reference = Average(reference, source);
            }

            return reference;
        }
#endregion


#region Vector Averages
        public static Vector3 Average(Vector3[] source)
        {
           return source.Aggregate(Vector3.zero, (acc, v) => acc + v) / source.Length;
        }

        public static Vector3 Average(Vector3 reference, Vector3[] source)
        {
            return source.Aggregate(reference, (acc, v) => acc + v) / source.Length;
        }

        public static Vector3 Average(Vector3 reference, Vector3[] source, int iterations)
        {
            Assert.IsFalse(source.Length > 0);
           
            for (int i = 0; i < iterations; i++)
            {
                reference = Average(reference, source);
            }

            return reference;
        }
        public static Vector3 Average(Vector3[] source, int iterations)
        {
            Assert.IsFalse(source.Length > 0);
            var reference = Vector3.zero;
            for (int i = 0; i < iterations; i++)
            {
                reference = Average(reference, source);
            }

            return reference;
        }

#endregion
    }
}
