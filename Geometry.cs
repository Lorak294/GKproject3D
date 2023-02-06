using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Globalization;
using System.Security.Cryptography.Pkcs;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.CodeDom;

namespace GKproject3D
{

    public class ScanlinePoint3D
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector3 ShadingColor { get; set; }
        public Vector3 WorldPosition { get; set; }

        public ScanlinePoint3D(Vector3 pos, Vector3 norm, Vector3 shadingColor, Vector3 worldPosition)
        {
            Position = pos;
            Normal = norm;
            ShadingColor = shadingColor;
            WorldPosition = worldPosition;
        }

        public ScanlinePoint3D Copy()
        {
            return new ScanlinePoint3D(Position, Normal, ShadingColor, WorldPosition);
        }

        public static ScanlinePoint3D operator+(ScanlinePoint3D a, ScanlinePoint3D b)
        {
            return new ScanlinePoint3D(
                a.Position + b.Position, 
                a.Normal + b.Normal, 
                a.ShadingColor + b.ShadingColor, 
                a.WorldPosition + b.WorldPosition
                );
        }

    }
    public class Point3D
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        
        public Vector3 WorldPosition { get; set; }
        public Vector3 WorldNormal { get; set; }

        public Vector3 NDCPosition { get; set; }
        public Vector3 ScreenPosition { get; set; }

        // constructor
        public Point3D(Vector3 positon, Vector3 normal)
        {
            Position = positon;
            Normal = normal;
        }
    }
    public class Triangle
    {
        public Point3D[] Points { get; set; }
        public bool BackFaced { get; set; }

        public Material Material { get; set; }

        // constructor
        public Triangle(List<Point3D> points, Material material)
        {
            if (points.Count != 3)
                throw new Exception("points list is not of 3 length!");

            Points = points.ToArray();
            Material = material;
        }

        public bool CheckBackFaceCulling(Camera camera)
        {
            Vector3 v = Points[0].WorldPosition - camera.Position;
            return Vector3.Dot(v, Points[0].WorldNormal) >= 0;
        }

        public void Draw(Scene scene)
        {

            // if all of triangle is outside the screen than do not draw it
            if (!CheckVisibility())
                return;


            FillOut(scene);
        }

        public bool CheckVisibility()
        {
            if ((Points[0].NDCPosition.X > 1 || Points[0].NDCPosition.X < -1 || Points[0].NDCPosition.Y > 1 || Points[0].NDCPosition.Y < -1 || Points[0].NDCPosition.Z > 1 || Points[0].NDCPosition.Z < -1) &&
                (Points[1].NDCPosition.X > 1 || Points[1].NDCPosition.X < -1 || Points[1].NDCPosition.Y > 1 || Points[1].NDCPosition.Y < -1 || Points[1].NDCPosition.Z > 1 || Points[1].NDCPosition.Z < -1) &&
                (Points[2].NDCPosition.X > 1 || Points[2].NDCPosition.X < -1 || Points[2].NDCPosition.Y > 1 || Points[2].NDCPosition.Y < -1 || Points[2].NDCPosition.Z > 1 || Points[2].NDCPosition.Z < -1))
                return false;
            else
                return true;
        }



        // SCAN LINE FILLING ALGHORITM
        public void FillOut(Scene scene)
        {
            ScanlinePoint3D[] screenPoints = new ScanlinePoint3D[3];

            Vector3 phongColor = CalcPhongLight(scene, Points[0].WorldPosition, Points[0].WorldNormal);
            for(int i=0; i< 3;i++)
            {
                if(scene.ShadingMode == ShadingMode.Static)
                    screenPoints[i] = new ScanlinePoint3D(Points[i].ScreenPosition, Points[i].WorldNormal, phongColor, Points[i].WorldPosition);
                else
                    screenPoints[i] = new ScanlinePoint3D(
                        Points[i].ScreenPosition, 
                        Points[i].WorldNormal, 
                        CalcPhongLight(scene, Points[i].WorldPosition, Points[i].WorldNormal), 
                        Points[i].WorldPosition);
            }

            // order verts ascending by Y than ascending by X
            Array.Sort(screenPoints,
                (p1, p2) =>
                {
                    if (p1.Position.Y > p2.Position.Y)
                        return 1;
                    else if (p1.Position.Y < p2.Position.Y)
                        return -1;
                    else
                        return p1.Position.X > p2.Position.X ? 1 : -1;
                }
                );

            if (screenPoints[0].Position.Y == screenPoints[1].Position.Y)
            {
                BottomFlatCase(screenPoints, scene);
            }
            else
            {
                BottomSharpCase(screenPoints, scene);
            }
        }

        private void BottomFlatCase(ScanlinePoint3D[] screenPoints, Scene scene)
        {
            // p0.Y == p1.Y and p0.X <= p1.X

            float dy = screenPoints[2].Position.Y - screenPoints[0].Position.Y;
            ScanlinePoint3D step0 = new ScanlinePoint3D(
                (screenPoints[2].Position - screenPoints[0].Position) / dy,
                (screenPoints[2].Normal - screenPoints[0].Normal) / dy,
                (screenPoints[2].ShadingColor - screenPoints[0].ShadingColor) / dy,
                (screenPoints[2].WorldPosition - screenPoints[0].WorldPosition) / dy
                );

            ScanlinePoint3D step1 = new ScanlinePoint3D(
                (screenPoints[2].Position - screenPoints[1].Position) / dy,
                (screenPoints[2].Normal - screenPoints[1].Normal) / dy,
                (screenPoints[2].ShadingColor - screenPoints[1].ShadingColor) / dy,
                (screenPoints[2].WorldPosition - screenPoints[1].WorldPosition) / dy
            );

            ScanlinePoint3D edge0point = screenPoints[0].Copy();
            ScanlinePoint3D edge1point = screenPoints[1].Copy();

            int lowY = (int)screenPoints[0].Position.Y;
            int topY = (int)Math.Round(screenPoints[2].Position.Y);

            if (topY >= scene.LockBitmap.Height) topY = scene.LockBitmap.Height-1;

            for (int scanlineY = lowY; scanlineY <= topY; scanlineY++)
            {
                // fill scanline
                if (scanlineY >= 0)
                    FillScanline(edge0point, edge1point, scanlineY, scene);

                // apply steps
                edge0point += step0;
                edge1point += step1;

            }
        }
        private void BottomSharpCase(ScanlinePoint3D[] screenPoints, Scene scene)
        {
            // p0.Y < p1.Y <= p2.Y

            float dy1 = screenPoints[1].Position.Y - screenPoints[0].Position.Y;
            float dy2 = screenPoints[2].Position.Y - screenPoints[0].Position.Y;

            ScanlinePoint3D step1 = new ScanlinePoint3D(
                (screenPoints[1].Position - screenPoints[0].Position) / dy1,
                (screenPoints[1].Normal - screenPoints[0].Normal) / dy1,
                (screenPoints[1].ShadingColor - screenPoints[0].ShadingColor) / dy1,
                (screenPoints[1].WorldPosition - screenPoints[0].WorldPosition) / dy1
                );

            ScanlinePoint3D step2 = new ScanlinePoint3D(
                (screenPoints[2].Position - screenPoints[0].Position) / dy2,
                (screenPoints[2].Normal - screenPoints[0].Normal) / dy2,
                (screenPoints[2].ShadingColor - screenPoints[0].ShadingColor) / dy2,
                (screenPoints[2].WorldPosition - screenPoints[0].WorldPosition) / dy2
            );

            ScanlinePoint3D edgePoint1 = screenPoints[0].Copy();
            ScanlinePoint3D edgePoint2 = screenPoints[0].Copy();

            bool leftSideP1 = step1.Position.X < step2.Position.X;

            int lowY = (int)screenPoints[0].Position.Y;
            int topY = (int)Math.Round(screenPoints[1].Position.Y);

            if (topY >= scene.LockBitmap.Height) topY = scene.LockBitmap.Height-1;
            int scanlineY;
            for (scanlineY = lowY; scanlineY <= topY; scanlineY++)
            {
                // fill scanline
                if (scanlineY > 0)
                {
                    if (leftSideP1)
                        FillScanline(edgePoint1, edgePoint2, scanlineY, scene);
                    else
                        FillScanline(edgePoint2, edgePoint1, scanlineY, scene);
                }
                // appply steps
                edgePoint1 += step1;
                edgePoint2 += step2;
            }

            if (screenPoints[1].Position.Y == screenPoints[2].Position.Y)
                return;


            // top half
            dy1 = screenPoints[2].Position.Y - screenPoints[1].Position.Y;

            step1 = new ScanlinePoint3D(
                (screenPoints[2].Position - screenPoints[1].Position) / dy1,
                (screenPoints[2].Normal - screenPoints[1].Normal) / dy1,
                (screenPoints[2].ShadingColor - screenPoints[1].ShadingColor) / dy1,
                (screenPoints[2].WorldPosition - screenPoints[1].WorldPosition) / dy1
                );

            topY = (int)Math.Round(screenPoints[2].Position.Y);

            edgePoint1 = screenPoints[1].Copy();

            if (topY >= scene.LockBitmap.Height) topY = scene.LockBitmap.Height-1;

            for (; scanlineY <= topY; scanlineY++)
            {
                // fill scanline
                if (scanlineY > 0)
                {
                    if (leftSideP1)
                        FillScanline(edgePoint1, edgePoint2, scanlineY, scene);
                    else
                        FillScanline(edgePoint2, edgePoint1, scanlineY, scene);
                }
                // appply steps
                edgePoint1 += step1;
                edgePoint2 += step2;
            }
        }
        public void FillScanline(ScanlinePoint3D leftPoint, ScanlinePoint3D rightPoint, int y, Scene scene)
        {
            float dx = rightPoint.Position.X - leftPoint.Position.X;
            ScanlinePoint3D step = new ScanlinePoint3D(
                (rightPoint.Position - leftPoint.Position) / dx,
                (rightPoint.Normal - leftPoint.Normal) / dx,
                (rightPoint.ShadingColor - leftPoint.ShadingColor) / dx,
                (rightPoint.WorldPosition - leftPoint.WorldPosition) / dx
            );

            ScanlinePoint3D drawPoint = leftPoint.Copy();
            for (int x = (int)Math.Max(0, leftPoint.Position.X); x < Math.Min(rightPoint.Position.X, scene.LockBitmap.Width); x++)
            {
                if (scene.Zbuffer[x, y] > drawPoint.Position.Z)
                {
                    if (scene.ShadingMode == ShadingMode.Phong)
                        scene.LockBitmap.SetPixel(x, y, VectorToColor(CalcPhongLight(scene,drawPoint.WorldPosition,drawPoint.Normal)));
                    else
                        scene.LockBitmap.SetPixel(x, y, VectorToColor(drawPoint.ShadingColor));

                    scene.Zbuffer[x, y] = drawPoint.Position.Z;
                }
                drawPoint += step;
            }
        }
        
        public static Color VectorToColor(Vector3 vec)
        {
            int r = Math.Max(127 + (int)(vec.X * 128), 0);
            int g = Math.Max(127 + (int)(vec.Y * 128), 0);
            int b = Math.Max(127 + (int)(vec.Z * 128), 0);

            return Color.FromArgb(r, g, b);
        }

        public Vector3 CalcPhongLight(Scene scene, Vector3 pixelPosition, Vector3 N)
        {
            Vector3 finalColor = new Vector3(0, 0, 0);

            Vector3 RGB = Material.Ka; // IA = 1


            Vector3 V_real = scene.Camera.Position - pixelPosition;
            Vector3 V = Vector3.Normalize(V_real);


            // po wszystkich żródłach światłach
            Vector3 diffSpec = Vector3.Zero;

            foreach(LightSource ls in scene.LightSources)
            {
                if(ls.CheckIfPointIsLit(pixelPosition))
                {
                    Vector3 L_real =ls.Position - pixelPosition;
                    Vector3 L = Vector3.Normalize(L_real);
                    float LN = Vector3.Dot(L, N);
                    Vector3 R = Vector3.Normalize(2 * LN * N - L);

                    Vector3 lsFactor = Material.Kd * LN * ls.Id + Material.Ks * (float)Math.Pow(Vector3.Dot(R, V), Material.Alpha) * ls.Is;

                    // attenuation
                    float Ac = 1;
                    float Al = 0.04f;
                    float Aq = 0.0016f;

                    float If = 1 / (Ac + Al * L_real.Length() + Aq * L_real.LengthSquared());

                    diffSpec += lsFactor * If;
                }
            }

            float Ia = 0.0f;
            finalColor = Material.Ka * Ia + diffSpec;


            return Vector3.Clamp(finalColor, Vector3.Zero, Vector3.One);
        }


    }
    public class Object3D
    {
        private List<Triangle> triangles;
        private Vector3 position;
        private Vector3 frontVec;
        public Vector3 WorldPosition { get; private set; }
        public Vector3 WorldFrontVec { get; private set; }
        public Matrix4x4 modelMatrix { get; set; }

        // constructor
        public Object3D(List<Triangle> triangles, Matrix4x4 modelMatrix)
        {
            this.triangles = triangles;
            this.modelMatrix = modelMatrix;
            position = triangles[0].Points[0].Position;
            frontVec = new Vector3(0,0,-1);
            WorldPosition = Vector3.Transform(position, modelMatrix);
            WorldFrontVec = Vector3.TransformNormal(frontVec, modelMatrix);
        }

        // DRAW
        public void Calculate(Scene scene, Matrix4x4 viewM, Matrix4x4 projectionM)
        {
            Vector4 v4Pos = new Vector4(position, 1);
            v4Pos = Vector4.Transform(v4Pos, modelMatrix);
            WorldPosition = new Vector3(v4Pos.X, v4Pos.Y, v4Pos.Z);
            WorldFrontVec = Vector3.TransformNormal(frontVec, modelMatrix);

            if (this == scene.Car)
            {
                // Y vec = [0,1,0]
                Vector3 sideVector = Vector3.Cross(Vector3.UnitY, WorldFrontVec);


                scene.CarSpotlight.Move(WorldPosition + 0.5f* sideVector, WorldFrontVec);




                Matrix4x4 rotationMatrix = Matrix4x4.CreateRotationY(scene.PoliceLightAngle, scene.PoliceLight.Position);
                Vector3 newPoliceLightDirection = Vector3.TransformNormal(WorldFrontVec, rotationMatrix);

                scene.PoliceLight.Move(WorldPosition + 0.5f * sideVector - 0.9f*WorldFrontVec + 0.9f*Vector3.UnitY, newPoliceLightDirection);
            }

            foreach (Triangle t in triangles)
            {
                for (int i=0; i< t.Points.Length; i++)
                {
                    Vector4 v4 = new Vector4(t.Points[i].Position, 1);
                    v4 = Vector4.Transform(v4, modelMatrix);
                    t.Points[i].WorldPosition = new Vector3(v4.X, v4.Y, v4.Z);
                    v4 = Vector4.Transform(v4, viewM);
                    v4 = Vector4.Transform(v4, projectionM);

                    t.Points[i].NDCPosition = new Vector3(v4.X/v4.W, v4.Y/v4.W, v4.Z / v4.W);
                    
                    
                    t.Points[i].ScreenPosition = new Vector3(
                        (float)Math.Round((t.Points[i].NDCPosition.X + 1) * scene.LockBitmap.Width / 2),
                        (float)Math.Round((1 - t.Points[i].NDCPosition.Y) * scene.LockBitmap.Height / 2),
                        (t.Points[i].NDCPosition.Z + 1) / 2);

                    t.Points[i].WorldNormal = Vector3.TransformNormal(t.Points[i].Normal, modelMatrix);
                }
            }
        }

        public void Draw(Scene scene, Matrix4x4 viewM, Matrix4x4 projectionM)
        {
            Calculate(scene, viewM, projectionM);

            foreach(Triangle t in triangles)
            {
                if (t.CheckBackFaceCulling(scene.Camera))
                    continue;
                
                t.Draw(scene);

            }
        }
    }

}
