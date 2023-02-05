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

        public float Is { get; set; }
        public float Id { get; set; }
        
        public LightSource(Vector3 position)
        {
            Position = position;
        }

    }
}
