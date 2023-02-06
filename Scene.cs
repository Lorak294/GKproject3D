using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GKproject3D
{
    public enum ShadingMode
    {
        Static,
        Gouraud,
        Phong
    }
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
        public float PoliceLightAngle { get; set; }

        // lightsources
        public ShadingMode ShadingMode { get; set; }
        public Vector3 AmbientLight { get; set; }
        public List<LightSource> LightSources { get; set; }
        public SpotLight CarSpotlight { get; set; }

        public SpotLight PoliceLight { get; set; }

        // fog
        public Vector3 Fog { get; set; }


        public Scene(Camera camera, Bitmap canvas, List<Object3D> objects, Object3D car,List<LightSource> lightSources, SpotLight carSpotlight,SpotLight policeLight, ShadingMode shadingMode = ShadingMode.Static, bool animationActive = false)
        {
            Zbuffer = new float[canvas.Width, canvas.Height];        
            LockBitmap = new LockBitmap(canvas);
            CarAnimationAngle = 0;
            PoliceLightAngle = 0;
            
            Objects = objects;
            Car = car;
            AnimationActive = animationActive;
            Camera = camera; ;
            CarSpotlight = carSpotlight;
            PoliceLight = policeLight;
            ShadingMode = shadingMode;
            LightSources = lightSources;

            AmbientLight = Vector3.One * 0.1f;
            Fog = Vector3.One * 0.8f;

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
