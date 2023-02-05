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

namespace GKproject3D
{

    public class ScanlinePoint3D
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }

        public ScanlinePoint3D(Vector3 pos, Vector3 norm)
        {
            Position = pos;
            Normal = norm;
        }

        public ScanlinePoint3D Copy()
        {
            return new ScanlinePoint3D(Position, Normal);
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
            for(int i=0; i< 3;i++)
            {
                screenPoints[i] = new ScanlinePoint3D(Points[i].ScreenPosition, Points[i].WorldNormal);
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

            Vector3 positionStep0 = (screenPoints[2].Position - screenPoints[0].Position) / dy;
            Vector3 positionStep1 = (screenPoints[2].Position - screenPoints[1].Position) / dy;
            Vector3 normalStep0 = (screenPoints[2].Normal - screenPoints[0].Normal) / dy;
            Vector3 normalStep1 = (screenPoints[2].Normal - screenPoints[1].Normal) / dy;

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

                // update values forn next iteration
                edge0point.Position += positionStep0;
                edge1point.Position += positionStep1;
                edge0point.Normal += normalStep0;
                edge1point.Normal += normalStep1;

            }
        }
        private void BottomSharpCase(ScanlinePoint3D[] screenPoints, Scene scene)
        {
            // p0.Y < p1.Y <= p2.Y

            float dy1 = screenPoints[1].Position.Y - screenPoints[0].Position.Y;
            float dy2 = screenPoints[2].Position.Y - screenPoints[0].Position.Y;

            ScanlinePoint3D edgePoint1 = screenPoints[0].Copy();
            ScanlinePoint3D edgePoint2 = screenPoints[0].Copy();

            Vector3 positionStep1 = (screenPoints[1].Position - screenPoints[0].Position) / dy1;
            Vector3 positionStep2 = (screenPoints[2].Position - screenPoints[0].Position) / dy2;
            Vector3 normalStep1 = (screenPoints[1].Normal - screenPoints[0].Normal) / dy1;
            Vector3 normalStep2 = (screenPoints[2].Normal - screenPoints[0].Normal) / dy2;

            bool leftSideP1 = positionStep1.X < positionStep2.X;

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
                edgePoint1.Position += positionStep1;
                edgePoint1.Normal += normalStep1;
                edgePoint2.Position += positionStep2;
                edgePoint2.Normal += normalStep2;
            }

            if (screenPoints[1].Position.Y == screenPoints[2].Position.Y)
                return;


            // top half
            dy1 = screenPoints[2].Position.Y - screenPoints[1].Position.Y;
            positionStep1 = (screenPoints[2].Position - screenPoints[1].Position) / dy1;
            normalStep1 = (screenPoints[2].Normal - screenPoints[1].Normal) / dy1;

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
                edgePoint1.Position += positionStep1;
                edgePoint1.Normal += normalStep1;
                edgePoint2.Position += positionStep2;
                edgePoint2.Normal += normalStep2;
            }
        }
        public void FillScanline(ScanlinePoint3D leftPoint, ScanlinePoint3D rightPoint, int y, Scene scene)
        {
            // ADD INTERPOLATION 
            Color c = Color.Red;

            float dx = rightPoint.Position.X - leftPoint.Position.X;
            float zStep = (rightPoint.Position.Z - leftPoint.Position.Z) / dx;
            Vector3 normStep = (rightPoint.Normal - leftPoint.Normal) / dx;
            
            float zBuff = leftPoint.Position.Z;
            Vector3 normVec = leftPoint.Normal;
            for (int x = (int)Math.Max(0, leftPoint.Position.X); x < Math.Min(rightPoint.Position.X, scene.LockBitmap.Width); x++)
            {
                if (scene.Zbuffer[x, y] > zBuff)
                {
                    //lockBitmap.SetPixel(x, y, NormalVectorToColor(Vector3.Normalize(normVec)));
                    scene.LockBitmap.SetPixel(x, y, VectorToColor(this.Material.Kd));
                    scene.Zbuffer[x, y] = zBuff;
                }
                zBuff += zStep;
                normVec += normStep;
            }
        }

        //// DELETE THIS LATER
        public static Color VectorToColor(Vector3 vec)
        {
            int r = Math.Max(127 + (int)(vec.X * 128), 0);
            int g = Math.Max(127 + (int)(vec.Y * 128), 0);
            int b = Math.Max(127 + (int)(vec.Z * 128), 0);

            return Color.FromArgb(r, g, b);
        }

        public Color CalcPhongLight(Scene scene, Vector3 pixelPosition, Vector3 N)
        {
            Vector3 finalColor = new Vector3(0, 0, 0);

            Vector3 RGB = Material.Ka; // IA = 1


            Vector3 V = Vector3.Normalize(scene.Camera.Position - pixelPosition);


            // po wszystkich żródłach światłach

            // candleLight
            Vector3 L = Vector3.Normalize(scene.CandleLight.Position - pixelPosition);
            float LN = Vector3.Dot(L, N);
            Vector3 R = 2 * LN * N - L;

            Vector3 candleFactor = Material.Kd * Vector3.Dot(L, N) * scene.CandleLight.Id + Material.Ks * (float)Math.Pow(Vector3.Dot(R, V), Material.Alpha) * scene.CandleLight.Is;


            float Ia = 1;
            finalColor = Material.Ka * Ia + candleFactor;

            return VectorToColor(finalColor);
        }


    }
    public class Object3D
    {
        private List<Triangle> triangles;
        private Color color;
        private Vector3 position;
        private Vector3 frontVec;
        public Vector3 WorldPosition { get; private set; }
        public Vector3 WorldFrontVec { get; private set; }
        public Matrix4x4 modelMatrix { get; set; }

        // constructor
        public Object3D(List<Triangle> triangles, Color c, Matrix4x4 modelMatrix)
        {
            this.triangles = triangles;
            this.color = c;
            this.modelMatrix = modelMatrix;
            position = triangles[0].Points[0].Position;
            frontVec = new Vector3(0,0,-1);
        }

        // DRAW
        public void Calculate(Scene scene, Matrix4x4 viewM, Matrix4x4 projectionM)
        {
            Vector4 v4Pos = new Vector4(position, 1);
            v4Pos = Vector4.Transform(v4Pos, modelMatrix);
            WorldPosition = new Vector3(v4Pos.X, v4Pos.Y, v4Pos.Z);
            WorldFrontVec = Vector3.TransformNormal(frontVec, modelMatrix);

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
