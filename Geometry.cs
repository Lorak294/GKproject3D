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

namespace GKproject3D
{
    public class Point3D
    {
        public Vector4 position;
        public Vector3 normal;

        // constructor
        public Point3D(Vector4 positon, Vector3 normal)
        {
            this.position = positon;
            this.normal = normal;
        }

        public Point3D Copy()
        {
            return new Point3D(position, normal);
        }

        public Vector3 getPositionV3()
        {
            return new Vector3(position.X, position.Y, position.Z);
        }

        public Vector4 getNDCPosition(Matrix4x4 matrix)
        {
            Vector4 dPos = MatrixOperations.MultiplyMatrixVector(matrix, position);
            return new Vector4( dPos.X/dPos.W, dPos.Y/dPos.W, dPos.Z/ dPos.W, 1);
        }


        public Vector3 getScreenPosition(Matrix4x4 matrix, float screenWidth, float screenHeight)
        {
            Vector4 ndcPos = getNDCPosition(matrix);

            return new Vector3(
                (ndcPos.X + 1) * screenWidth / 2,
                (1 - ndcPos.Y) * screenHeight / 2,
                (ndcPos.Z + 1) / 2
                );
        }

        public Point3D getScreenPosition(Vector4 ndcPos, float screenWidth, float screenHeight)
        {
            return new Point3D( new Vector4(
              (float)Math.Round((ndcPos.X + 1) * screenWidth / 2),
              (float)Math.Round((1 - ndcPos.Y) * screenHeight / 2),
              (float)Math.Round((ndcPos.Z + 1) / 2),
              ndcPos.W),
              this.normal
              );
        }



    }
    public class Triangle
    {
        private Point3D[] points;

        // constructor
        public Triangle(Point3D p0, Point3D p1, Point3D p2)
        {
            points = new Point3D[3];
            points[0] = p0;
            points[1] = p1;
            points[2] = p2;
        }

        public Triangle(List<Point3D> points)
        {
            if (points.Count != 3)
                throw new Exception("points list is not of 3 length!");

            this.points = points.ToArray();
        }

        public bool CheckBackFaceCulling(Camera camera)
        {
            return Vector3.Dot(points[0].getPositionV3() - camera.Position, points[0].normal) >= 0;
        }

        public void Draw(Image image, Matrix4x4 matrix, float screenWidth, float screenHeight, float[,] zBufferMark)
        {
            Vector4 p0_ndc = points[0].getNDCPosition(matrix);
            Vector4 p1_ndc = points[1].getNDCPosition(matrix);
            Vector4 p2_ndc = points[2].getNDCPosition(matrix);

            // if all of triangle is outside the screen than do not draw it
            if ((p0_ndc.X > 1 || p0_ndc.X < -1 || p0_ndc.Y > 1 || p0_ndc.Y < -1 || p0_ndc.Z > 1 || p0_ndc.Z < -1) &&
                (p1_ndc.X > 1 || p1_ndc.X < -1 || p1_ndc.Y > 1 || p1_ndc.Y < -1 || p1_ndc.Z > 1 || p1_ndc.Z < -1) &&
                (p2_ndc.X > 1 || p2_ndc.X < -1 || p2_ndc.Y > 1 || p2_ndc.Y < -1 || p2_ndc.Z > 1 || p2_ndc.Z < -1))
                return;

            Point3D[] screenPoints = new Point3D[3];

            screenPoints[0] = points[0].getScreenPosition(p0_ndc, screenWidth, screenHeight);
            screenPoints[1] = points[1].getScreenPosition(p1_ndc, screenWidth, screenHeight);
            screenPoints[2] = points[2].getScreenPosition(p2_ndc, screenWidth, screenHeight);

            FillOut(screenPoints, (Bitmap)image, zBufferMark);
        }



        // SCAN LINE FILLING ALGHORITM
        public void FillOut(Point3D[] screenPoints, Bitmap canvas, float[,] zBufferMark)
        {
            // order verts ascending by Y than ascending by X
            Array.Sort(screenPoints, 
                (p1, p2) =>
                {
                    if (p1.position.Y > p2.position.Y)
                        return 1;
                    else if (p1.position.Y < p2.position.Y)
                        return -1;
                    else
                       return  p1.position.X > p2.position.X ? 1 : -1;
                }
                );

            if (screenPoints[0].position.Y == screenPoints[1].position.Y)
            {
                BottomFlatCase(screenPoints, canvas, zBufferMark);
            }
            else
            {
                BottomSharpCase(screenPoints, canvas, zBufferMark);
            }
        }
        private void BottomFlatCase(Point3D[] screenPoints, Bitmap canvas, float[,] zBufferMark)
        {
            // p0.Y == p1.Y and p0.X <= p1.X

            float dy = screenPoints[2].position.Y - screenPoints[0].position.Y;

            Vector4 positionStep0 = (screenPoints[2].position - screenPoints[0].position) / dy;
            Vector4 positionStep1 = (screenPoints[2].position - screenPoints[1].position) / dy;
            Vector3 normalStep0 = (screenPoints[2].normal - screenPoints[0].normal) / dy;
            Vector3 normalStep1 = (screenPoints[2].normal - screenPoints[1].normal) / dy;

            Point3D edge0point = screenPoints[0].Copy();
            Point3D edge1point = screenPoints[1].Copy();

            int lowY = (int)screenPoints[0].position.Y;
            int topY = (int)Math.Round(screenPoints[2].position.Y);

            if (topY > canvas.Height) topY = canvas.Height;
            
            
            for (int scanlineY = lowY; scanlineY <= topY; scanlineY++)
            {
                // fill scanline
                if(scanlineY >= 0)
                    FillScanline(edge0point, edge1point, scanlineY, canvas, zBufferMark);

                // update values forn next iteration
                edge0point.position += positionStep0;
                edge1point.position += positionStep1;
                edge0point.normal += normalStep0;
                edge1point.normal += normalStep1;

            }
        }
        private void BottomSharpCase(Point3D[] screenPoints, Bitmap canvas, float[,] zBufferMark)
        {
            // p0.Y < p1.Y <= p2.Y

            float dy1 = screenPoints[1].position.Y - screenPoints[0].position.Y;
            float dy2 = screenPoints[2].position.Y - screenPoints[0].position.Y;

            Point3D edgePoint1 = screenPoints[0].Copy();
            Point3D edgePoint2 = screenPoints[0].Copy();

            Vector4 positionStep1 = (screenPoints[1].position - screenPoints[0].position) / dy1;
            Vector4 positionStep2 = (screenPoints[2].position - screenPoints[0].position) / dy2;
            Vector3 normalStep1 = (screenPoints[1].normal - screenPoints[0].normal) / dy1;
            Vector3 normalStep2 = (screenPoints[2].normal - screenPoints[0].normal) / dy2;

            bool leftSideP1 = positionStep1.X < positionStep2.X;

            int lowY = (int)screenPoints[0].position.Y;
            int topY = (int)Math.Round(screenPoints[1].position.Y);
            
            if(topY > canvas.Height) topY = canvas.Height;
            int scanlineY;
            for (scanlineY = lowY; scanlineY <= topY; scanlineY++)
            {
                // fill scanline
                if(scanlineY > 0)
                {
                    if(leftSideP1)
                        FillScanline(edgePoint1, edgePoint2, scanlineY, canvas, zBufferMark);
                    else
                        FillScanline(edgePoint2, edgePoint1, scanlineY, canvas, zBufferMark);
                }
                // appply steps
                edgePoint1.position += positionStep1;
                edgePoint1.normal += normalStep1;
                edgePoint2.position += positionStep2;
                edgePoint2.normal += normalStep2;
            }

            if (screenPoints[1].position.Y == screenPoints[2].position.Y)
                return;


            // top half
            dy1 = screenPoints[2].position.Y - screenPoints[1].position.Y;
            positionStep1 = (screenPoints[2].position - screenPoints[1].position) / dy1;
            normalStep1 = (screenPoints[2].normal - screenPoints[1].normal) / dy1;
            
            topY = (int)Math.Round(screenPoints[2].position.Y);

            edgePoint1 = screenPoints[1].Copy();

            if (topY > canvas.Height) topY = canvas.Height;
            
            for (; scanlineY <= topY; scanlineY++)
            {
                // fill scanline
                if (scanlineY > 0)
                {
                    if (leftSideP1)
                        FillScanline(edgePoint1, edgePoint2, scanlineY, canvas, zBufferMark);
                    else
                        FillScanline(edgePoint2, edgePoint1, scanlineY, canvas, zBufferMark);
                }
                // appply steps
                edgePoint1.position += positionStep1;
                edgePoint1.normal += normalStep1;
                edgePoint2.position += positionStep2;
                edgePoint2.normal += normalStep2;
            }
        }
        public void FillScanline( Point3D leftPoint, Point3D rightPoint, int y, Bitmap canvas, float[,] zBufferMark)
        {
            // ADD INTERPOLATION 
            Color c = Color.Red;

            float dx = rightPoint.position.X - leftPoint.position.X;
            float zStep = (rightPoint.position.Z - leftPoint.position.Z) / dx;
            Vector3 normStep = (rightPoint.normal - leftPoint.normal) / dx;

            float zBuff = leftPoint.position.Z;
            Vector3 normVec = leftPoint.normal;
            for (int x = (int)Math.Max(0, leftPoint.position.X); x < Math.Min(rightPoint.position.X,canvas.Width); x++)
            {
                if (zBufferMark[x,y] > zBuff)
                {
                    canvas.SetPixel(x, y, NormalVectorToColor(Vector3.Normalize(normVec)));
                    zBufferMark[x,y] = zBuff;
                }
                    

                zBuff += zStep;
                normVec += normStep;
            }
        }

        // DELETE THIS LATER
        public static Color NormalVectorToColor(Vector3 normV)
        {
            int r = Math.Max(127 + (int)(normV.X * 128), 0);
            int g = Math.Max(127 + (int)(normV.Y * 128), 0);
            int b = Math.Max(127 + (int)(normV.Z * 128), 0);

            return Color.FromArgb(r,g,b);
        }

        public void testFill(Bitmap canvas, float[,] zBufferMark)
        {
            Point3D[] testTrig = new Point3D[3]
            {
                new Point3D(new Vector4(10,10,1,1),new Vector3(0,1,0)),
                new Point3D(new Vector4(100,10,1,1),new Vector3(0,1,0)),
                new Point3D(new Vector4(30,100,1,1),new Vector3(0,1,0))
            };

            FillOut(testTrig, canvas, zBufferMark);
            

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

            List<Vector4> vertList = new List<Vector4>();
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
            while (lineStr != null && lineStr.StartsWith('f'))
            {
                //Debug.WriteLine(lineStr);
                try
                {
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
        private Vector4 ParseVertex(string normVectorStr)
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

            return new Vector4(x, y, z, 1);
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
        private Triangle ParseTriangle(string figureStr, List<Vector4> vertList, List<Vector3> normVectorList)
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
        public void Draw(Image image, Matrix4x4 matrix, float screenWidth, float screenHeight, float[,] zBufferMark, Camera camera)
        {
            foreach(Triangle t in triangles)
            {
                if(!t.CheckBackFaceCulling(camera))
                    t.Draw(image, matrix, screenWidth, screenHeight, zBufferMark);
            }

        }
    }

}
