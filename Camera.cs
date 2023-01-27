using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GKproject3D
{
    public class Camera
    {
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; private set; }
        public Vector3 UpVector { get; private set; }

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

        public Camera(Vector3 position, Vector3 target, Vector3 upVector)
        {
            this.Position = position;
            this.Target = target;
            this.UpVector = upVector;
        }

        public void LookAt(Vector3 target)
        {
            Target = target;
        }

        public void MoveTo(Vector3 position)
        {
            Position = position;
        }

    }
}
