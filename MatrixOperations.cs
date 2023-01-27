using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GKproject3D
{
    public static class MatrixOperations
    {
        public static Matrix4x4 TranslationMatrix(float x, float y, float z)
        {
            return new Matrix4x4(
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1
                );
        }

        public static Matrix4x4 ScaleMatrix(float x, float y, float z)
        {
            return new Matrix4x4(
                x, 0, 0, 0,
                0, y, 0, 0,
                0, 0, z, 0,
                0, 0, 0, 1
                );
        }

        public static Matrix4x4 RotationOXMatrix(float a)
        {
            float cosA = (float)Math.Cos(a);
            float sinA = (float)Math.Sin(a);
            return new Matrix4x4(
                1,  0,   0,     0,
                0,  cosA, -sinA,0,
                0,  sinA, cosA, 0,
                0,  0,   0,     1
                );
        }

        public static Matrix4x4 RotationOYMatrix(float a)
        {
            float cosA = (float)Math.Cos(a);
            float sinA = (float)Math.Sin(a);
            return new Matrix4x4(
                cosA, 0, -sinA, 0,
                0, 1, 0, 0,
                sinA, 0, cosA, 0,
                0, 0, 0, 1
                );
        }

        public static Matrix4x4 RotationOZMatrix(float a)
        {
            float cosA = (float)Math.Cos(a);
            float sinA = (float)Math.Sin(a);
            return new Matrix4x4(
                cosA, -sinA,0, 0,
                sinA, cosA, 0, 0,
                0,  0,      1, 0,
                0,  0,      0, 1
                );
        }

        public static Matrix4x4 ProjectionMatrix(float fov, float screenWidth, float screenHeight)
        {
            float cX = (float)screenWidth / 2.0f;
            float cY = (float)screenHeight / 2.0f;
            float s = ((float)screenHeight / 2.0f) / (float)Math.Tan(fov / 2);

            return new Matrix4x4(
                s, 0, cX, 0,
                0, s, cY, 0,
                0, 0, 0, 1,
                0, 0, 1, 0
                );
        }

        public static Vector4 MultiplyMatrixVector(Matrix4x4 m, Vector4 v)
        {
            //return new Vector4(
            //    m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z + m.M14 * v.W,
            //    m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z + m.M24 * v.W,
            //    m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z + m.M34 * v.W,
            //    m.M41 * v.X + m.M42 * v.Y + m.M43 * v.Z + m.M44 * v.W
            //    );

            return new Vector4(
                m.M11 * v.X + m.M21 * v.Y + m.M31 * v.Z + m.M41 * v.W,
                m.M12 * v.X + m.M22 * v.Y + m.M32 * v.Z + m.M42 * v.W,
                m.M13 * v.X + m.M23 * v.Y + m.M33 * v.Z + m.M43 * v.W,
                m.M14 * v.X + m.M24 * v.Y + m.M34 * v.Z + m.M44 * v.W
    );
        }
    }
}
