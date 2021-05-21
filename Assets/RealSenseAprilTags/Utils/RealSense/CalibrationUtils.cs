#if REALSENSE
using Intel.RealSense;
using UnityEngine;

public static class CalibrationUtils
{

    /// <summary>
    /// Calculate projection matrix from camera matrix values.
    /// </summary>
    /// <returns>
    /// Projection matrix.
    /// </returns>
    public static Matrix4x4 CalculateProjectionMatrixFromCameraMatrixValues(Intrinsics intrinsics, Camera currentCamera)
    {
        var projectionMatrix = new Matrix4x4
        {
            m00 = intrinsics.fx,
            m11 = -intrinsics.fy,
            m03 = intrinsics.ppx / intrinsics.width,
            m13 = intrinsics.ppy / intrinsics.height,
            m22 = (currentCamera.nearClipPlane + currentCamera.farClipPlane) * 0.5f,
            m23 = currentCamera.nearClipPlane * currentCamera.farClipPlane,
        };
        float r = ((float) intrinsics.width / Screen.width);
        projectionMatrix = Matrix4x4.Ortho(0, Screen.width * r, Screen.height * r, 0, currentCamera.nearClipPlane, currentCamera.farClipPlane) * projectionMatrix;
        projectionMatrix.m32 = -1;


        return projectionMatrix;

    }

  
    public static Matrix4x4 ConvertToMatrix(Vector3 position, Quaternion rotation)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, Vector3.one);


        return matrix;
    }

    
}
#endif
