﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GKproject3D
{
    public enum CameraMode
    {
        Static,
        Follow,
        Behind
    }
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 UpVector { get; private set; }
        public CameraMode Mode { get; set; }

        public float FOV { get; set; }
        public float N { get; set; }
        public float F { get; set; }
        public float AspectRatio { get; set; }

        public Vector3 ZAxis
        {
            get
            {
                return Vector3.Normalize(Position - Target);
            }
        }
        public Vector3 XAxis
        {
            get
            {
                return Vector3.Normalize(Vector3.Multiply(UpVector, ZAxis));
            }
        }
        public Vector3 YAxis
        {
            get
            {
                return Vector3.Normalize(Vector3.Multiply(ZAxis, XAxis));
            }
        }

        public Camera(Vector3 position, Vector3 target, Vector3 upVector, float fov, float n, float f, float aspectRatio)
        {
            this.Position = position;
            this.Target = target;
            this.UpVector = upVector;
            FOV = fov;
            N = n;
            F = f;
            AspectRatio = aspectRatio;
            Mode = CameraMode.Static;
        }

    }
}
