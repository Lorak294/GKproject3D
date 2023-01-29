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

        // constructor
        public Triangle(Point3D p0, Point3D p1, Point3D p2)
        {
            Points = new Point3D[3];
            Points[0] = p0;
            Points[1] = p1;
            Points[2] = p2;
        }
        public Triangle(List<Point3D> points)
        {
            if (points.Count != 3)
                throw new Exception("points list is not of 3 length!");

            Points = points.ToArray();
        }

        public bool CheckBackFaceCulling(Camera camera)
        {
            Vector3 v = Points[0].WorldPosition - camera.Position;
            return Vector3.Dot(v, Points[0].WorldNormal) >= 0;
        }

        public void Draw(LockBitmap lockBitmap, float[,] zBufferMark)
        {

            // if all of triangle is outside the screen than do not draw it
            if (!CheckVisibility())
                return;

            //lockBitmap.SetPixel((int)(Points[0].ScreenPosition.X), (int)(Points[0].ScreenPosition.Y), Color.Black);
            //lockBitmap.SetPixel((int)(Points[1].ScreenPosition.X), (int)(Points[1].ScreenPosition.Y), Color.Black);
            //lockBitmap.SetPixel((int)(Points[2].ScreenPosition.X), (int)(Points[2].ScreenPosition.Y), Color.Black);

            FillOut(lockBitmap, zBufferMark);
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
        public void FillOut(LockBitmap lockBitmap, float[,] zBufferMark)
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
                BottomFlatCase(screenPoints, lockBitmap, zBufferMark);
            }
            else
            {
                BottomSharpCase(screenPoints, lockBitmap, zBufferMark);
            }
        }

        private void BottomFlatCase(ScanlinePoint3D[] screenPoints, LockBitmap lockBitmap, float[,] zBufferMark)
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

            if (topY > lockBitmap.Height) topY = lockBitmap.Height;

            for (int scanlineY = lowY; scanlineY <= topY; scanlineY++)
            {
                // fill scanline
                if (scanlineY >= 0)
                    FillScanline(edge0point, edge1point, scanlineY, lockBitmap, zBufferMark);

                // update values forn next iteration
                edge0point.Position += positionStep0;
                edge1point.Position += positionStep1;
                edge0point.Normal += normalStep0;
                edge1point.Normal += normalStep1;

            }
        }
        private void BottomSharpCase(ScanlinePoint3D[] screenPoints, LockBitmap lockBitmap, float[,] zBufferMark)
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

            if (topY > lockBitmap.Height) topY = lockBitmap.Height;
            int scanlineY;
            for (scanlineY = lowY; scanlineY <= topY; scanlineY++)
            {
                // fill scanline
                if (scanlineY > 0)
                {
                    if (leftSideP1)
                        FillScanline(edgePoint1, edgePoint2, scanlineY, lockBitmap, zBufferMark);
                    else
                        FillScanline(edgePoint2, edgePoint1, scanlineY, lockBitmap, zBufferMark);
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

            if (topY > lockBitmap.Height) topY = lockBitmap.Height;

            for (; scanlineY <= topY; scanlineY++)
            {
                // fill scanline
                if (scanlineY > 0)
                {
                    if (leftSideP1)
                        FillScanline(edgePoint1, edgePoint2, scanlineY, lockBitmap, zBufferMark);
                    else
                        FillScanline(edgePoint2, edgePoint1, scanlineY, lockBitmap, zBufferMark);
                }
                // appply steps
                edgePoint1.Position += positionStep1;
                edgePoint1.Normal += normalStep1;
                edgePoint2.Position += positionStep2;
                edgePoint2.Normal += normalStep2;
            }
        }
        public void FillScanline(ScanlinePoint3D leftPoint, ScanlinePoint3D rightPoint, int y, LockBitmap lockBitmap, float[,] zBufferMark)
        {
            // ADD INTERPOLATION 
            Color c = Color.Red;

            float dx = rightPoint.Position.X - leftPoint.Position.X;
            float zStep = (rightPoint.Position.Z - leftPoint.Position.Z) / dx;
            Vector3 normStep = (rightPoint.Normal - leftPoint.Normal) / dx;

            float zBuff = leftPoint.Position.Z;
            Vector3 normVec = leftPoint.Normal;
            for (int x = (int)Math.Max(0, leftPoint.Position.X); x < Math.Min(rightPoint.Position.X, lockBitmap.Width); x++)
            {
                if (zBufferMark[x, y] > zBuff)
                {
                    lockBitmap.SetPixel(x, y, NormalVectorToColor(Vector3.Normalize(normVec)));
                    zBufferMark[x, y] = zBuff;
                }


                zBuff += zStep;
                normVec += normStep;
            }
        }

        //// DELETE THIS LATER
        public static Color NormalVectorToColor(Vector3 normV)
        {
            int r = Math.Max(127 + (int)(normV.X * 128), 0);
            int g = Math.Max(127 + (int)(normV.Y * 128), 0);
            int b = Math.Max(127 + (int)(normV.Z * 128), 0);

            return Color.FromArgb(r, g, b);
        }
    }
    public class Object3D
    {
        private List<Triangle> triangles;
        private Color color;
        private Vector3 position;

        // constructor
        public Object3D(List<Triangle> triangles, Color c)
        {
            this.triangles = triangles;
            this.color = c;
            position = new Vector3(0,0,0);
        }

        
        
        // OBJ FILE IMPORT CONSTRUCTOR
        public Object3D(string filepath, Color c)
        {
            position = new Vector3(0, 0, 0);
            color = c;

            List<Vector3> vertList = new List<Vector3>();
            List<Vector3> normVectorList = new List<Vector3>();
            triangles = new List<Triangle>();

            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);

            string lineStr = sr.ReadLine()!;

            // importing verts and normal vectors
            while (lineStr != null && !lineStr.StartsWith('f'))
            {
                try
                {
                    if (lineStr.StartsWith("vt"))
                    {
                        lineStr = sr.ReadLine()!;
                        continue;
                    }
                    if (lineStr.StartsWith("vn"))
                    {
                        normVectorList.Add(ParseNormVector(lineStr));
                    }
                    else if (lineStr.StartsWith("v"))
                    {
                        vertList.Add(ParseVertex(lineStr));
                    }
                    lineStr = sr.ReadLine()!;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

            }

            // importing figures
            while (lineStr != null)
            {
                //Debug.WriteLine(lineStr);
                try
                {
                    if(lineStr.StartsWith('f'))
                        triangles.Add(ParseTriangle(lineStr, vertList, normVectorList));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                lineStr = sr.ReadLine()!;
            }

            sr.Close();
            fs.Close();
        }
        private Vector3 ParseVertex(string normVectorStr)
        {
            string[] args = normVectorStr.Split(' ');

            if (args[0] != "v")
                throw new ArgumentException("Wrong string psssed to Vertex Parser.");

            float x, y, z;

            if (!float.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out x))
                throw new ArgumentException("Wrong string psssed to Vertex Parser.");
            if (!float.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out y))
                throw new ArgumentException("Wrong string psssed to Vertex Parser.");
            if (!float.TryParse(args[3], NumberStyles.Any, CultureInfo.InvariantCulture, out z))
                throw new ArgumentException("Wrong string psssed to Vertex Parser.");

            return new Vector3(x, y, z);
        }
        private Vector3 ParseNormVector(string normVectorStr)
        {
            string[] args = normVectorStr.Split(' ');

            if (args[0] != "vn")
                throw new ArgumentException("Wrong string psssed to Norm Vector Parser.");

            float x, y, z;

            if (!float.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out x))
                throw new ArgumentException("Wrong string psssed to Norm Vector Parser.");
            if (!float.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out y))
                throw new ArgumentException("Wrong string psssed to Norm Vector Parser.");
            if (!float.TryParse(args[3], NumberStyles.Any, CultureInfo.InvariantCulture, out z))
                throw new ArgumentException("Wrong string psssed to Norm Vector Parser.");

            return new Vector3(x, y, z);
        }
        private Triangle ParseTriangle(string figureStr, List<Vector3> vertList, List<Vector3> normVectorList)
        {
            List<Point3D> selectedVerts = new List<Point3D>();

            

            string[] args = figureStr.Split(' ');

            foreach (string arg in args)
            {
                if (arg == "f")
                    continue;

                string[] vertArg = arg.Split('/');

                int vertIdx, normVectorIdx;

                if (!(int.TryParse(vertArg[0], out vertIdx) && int.TryParse(vertArg[2], out normVectorIdx)))
                {
                    throw new ArgumentException("wrong string - int parsing problem");
                }

                selectedVerts.Add(new Point3D(vertList[vertIdx - 1], normVectorList[normVectorIdx - 1]));
            }

            return new Triangle(selectedVerts);
        }


        // DRAW
        public void Calculate(LockBitmap lockBitmap, Matrix4x4 modelM, Matrix4x4 viewM, Matrix4x4 projectionM, float[,] zBufferMark, Camera camera)
        {
            foreach(Triangle t in triangles)
            {
                t.BackFaced = !t.CheckBackFaceCulling(camera);
                if (t.BackFaced)
                    continue;

                for (int i=0; i< t.Points.Length; i++)
                {
                    Vector4 v4 = new Vector4(t.Points[i].Position, 1);
                    v4 = Vector4.Transform(v4, modelM);
                    t.Points[i].WorldPosition = new Vector3(v4.X, v4.Y, v4.Z);
                    v4 = Vector4.Transform(v4, viewM);
                    v4 = Vector4.Transform(v4, projectionM);

                    t.Points[i].NDCPosition = new Vector3(v4.X/v4.W, v4.Y/v4.W, v4.Z / v4.W);
                    
                    
                    t.Points[i].ScreenPosition = new Vector3(
                        (float)Math.Round((t.Points[i].NDCPosition.X + 1) * lockBitmap.Width / 2),
                        (float)Math.Round((1 - t.Points[i].NDCPosition.Y) * lockBitmap.Height / 2),
                        (t.Points[i].NDCPosition.Z + 1) / 2);

                    t.Points[i].WorldNormal = Vector3.TransformNormal(t.Points[i].Normal, modelM);
                }
            }
        }

        public void Draw(LockBitmap lockBitmap, Matrix4x4 modelM, Matrix4x4 viewM, Matrix4x4 projectionM, float[,] zBufferMark, Camera camera)
        {
            Calculate(lockBitmap, modelM, viewM, projectionM, zBufferMark, camera);

            foreach(Triangle t in triangles)
            {
                if(!t.BackFaced)
                    t.Draw(lockBitmap, zBufferMark);
            }
        }
    }

}
