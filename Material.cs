using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GKproject3D
{
    public class Material
    {
        public Vector3 Ks { get; set; }
        public Vector3 Kd { get; set; }
        public Vector3 Ka { get; set; }
        public float Alpha { get; set; }

        public Material (Vector3 ks, Vector3 kd, Vector3 ka, float alpha)
        {
            Ks = ks;
            Kd = kd;
            Ka = ka;
            Alpha = alpha;
        }
    }
}
