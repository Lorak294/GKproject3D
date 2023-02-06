using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GKproject3D
{
    public class LightSource
    {
        public Vector3 Position { get; set; }
        public Vector3 Id { get; set; }
        public Vector3 Is { get; set; }
        
        public LightSource(Vector3 position, Vector3 iS, Vector3 iD)
        {
            Position = position;
            Is = iS;
            Id = iD;
        }

        public virtual bool CheckIfPointIsLit(Vector3 point)
        {
            return true;
        }

    }

    public class SpotLight : LightSource
    {
        public Vector3 LightDirection { get; set; }
        public float CutOffAngle { get; set; }

        public SpotLight(Vector3 position, Vector3 iS, Vector3 iD, Vector3 lightDirection, float cutoffAngle) : base(position,iS,iD)
        {
            LightDirection = lightDirection;
            CutOffAngle = cutoffAngle;
        }


        public override bool CheckIfPointIsLit(Vector3 point)
        {
            Vector3 pointVersor = Vector3.Normalize(point - Position);

            float alpha = Vector3.Dot(pointVersor, LightDirection);

            return alpha >= CutOffAngle;
        }

        public void Move(Vector3 newPosition, Vector3 newLightDirection)
        {
            Position = newPosition;
            LightDirection = newLightDirection;
        }

    }
}
