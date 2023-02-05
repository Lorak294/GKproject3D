using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GKproject3D
{
    public class Scene
    {
        public Camera Camera { get; set; }

        // drawing 
        public float[,] Zbuffer { get; set; }
        public LockBitmap LockBitmap { get; set; }

        // objects
        public List<Object3D> Objects { get; set; }
        public Object3D Car { get; set; }

        // animation
        public bool AnimationActive { get; set; }
        public float CarAnimationAngle { get; set; }

        // lightsources
        public LightSource CandleLight { get; set; }


        public Scene(Camera camera, Bitmap canvas, List<Object3D> objects, Object3D car, LightSource candleLight, bool animationActive = false)
        {
            Zbuffer = new float[canvas.Width, canvas.Height];        
            LockBitmap = new LockBitmap(canvas);
            CarAnimationAngle = 0;
            
            Objects = objects;
            Car = car;
            AnimationActive = animationActive;
            Camera = camera; ;
            CandleLight = candleLight;
        }

        public void ResetZBuffer()
        {
            for (int i = 0; i < Zbuffer.GetLength(0); i++)
            {
                for (int j = 0; j < Zbuffer.GetLength(1); j++)
                {
                    Zbuffer[i, j] = 1000.0f;
                }
            }
        }

    }
}
