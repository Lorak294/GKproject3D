using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Globalization;
using System.Security.Cryptography.Pkcs;

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

        public void Draw(Image image, Matrix4x4 matrix, float screenWidth, float screenHeight, bool outline = false)
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

            if(!outline)
            
                FillOut(screenPoints, (Bitmap)image);
            else
            using(Graphics g = Graphics.FromImage(image))
            {
                g.DrawLine(Pens.Black, screenPoints[0].position.X, screenPoints[0].position.Y, screenPoints[1].position.X, screenPoints[1].position.Y);
                g.DrawLine(Pens.Black, screenPoints[1].position.X, screenPoints[1].position.Y, screenPoints[2].position.X, screenPoints[2].position.Y);
                g.DrawLine(Pens.Black, screenPoints[2].position.X, screenPoints[2].position.Y, screenPoints[0].position.X, screenPoints[0].position.Y);
            }
        }



        // SCAN LINE FILLING ALGHORITM
        public void FillOut(Point3D[] screenPoints, Bitmap canvas )
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
                BottomFlatCase(screenPoints, canvas);
            }
            else
            {
                BottomSharpCase(screenPoints, canvas);
            }
        }
        private void BottomFlatCase(Point3D[] screenPoints, Bitmap canvas)
        {
            // p0.Y == p1.Y and p0.X <= p1.X

            float invSlope0 = (screenPoints[2].position.X - screenPoints[0].position.X) / (screenPoints[2].position.Y - screenPoints[0].position.Y);
            float invSlope1 = (screenPoints[2].position.X - screenPoints[1].position.X) / (screenPoints[2].position.Y - screenPoints[1].position.Y);

            float curX0 = screenPoints[0].position.X;
            float curX1 = curX0;

            int lowY = (int)screenPoints[0].position.Y;
            int topY = (int)Math.Round(screenPoints[2].position.Y);

            if (topY > canvas.Height) topY = canvas.Height;
            for (int scanlineY = lowY; scanlineY <= topY; scanlineY++)
            {

                // fill scanline
                if(scanlineY >= 0)
                    FillScanline((int)curX0, (int)curX1, scanlineY, canvas);
                curX0 += invSlope0;
                curX1 += invSlope1;
            }
        }
        private void BottomSharpCase(Point3D[] screenPoints, Bitmap canvas)
        {
            // p0.Y < p1.Y <= p2.Y

            float invSlope1 = (screenPoints[1].position.X - screenPoints[0].position.X) / (screenPoints[1].position.Y - screenPoints[0].position.Y);
            float invSlope2 = (screenPoints[2].position.X - screenPoints[0].position.X) / (screenPoints[2].position.Y - screenPoints[0].position.Y);

            bool leftSideP1 = invSlope1 < invSlope2;

            float curX1 = screenPoints[0].position.X;
            float curX2 = curX1;

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
                        FillScanline((int)curX1, (int)curX2, scanlineY, canvas);
                    else
                        FillScanline((int)curX2, (int)curX1, scanlineY, canvas);
                }

                curX1 += invSlope1;
                curX2 += invSlope2;
            }

            if (screenPoints[1].position.Y == screenPoints[2].position.Y)
                return;


            // top half
            invSlope1 = (screenPoints[2].position.X - screenPoints[1].position.X) / (screenPoints[2].position.Y - screenPoints[1].position.Y);
            topY = (int)Math.Round(screenPoints[2].position.Y);

            curX1 = screenPoints[1].position.X;

            if (topY > canvas.Height) topY = canvas.Height;
            for (; scanlineY <= topY; scanlineY++)
            {
                // fill scanline
                if (scanlineY > 0)
                {
                    if (leftSideP1)
                        FillScanline((int)curX1, (int)curX2, scanlineY, canvas);
                    else
                        FillScanline((int)curX2, (int)curX1, scanlineY, canvas);
                }
                curX1 += invSlope1;
                curX2 += invSlope2;
            }
        }
        public void FillScanline( int startX, int endX, int y, Bitmap canvas)
        {
            // ADD INTERPOLATION 
            Color c = Color.Red;
            for (int x = Math.Max(0,startX); x < Math.Min(endX,canvas.Width); x++)
            {
                canvas.SetPixel(x, y, c);
            }
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
        public void Draw(Image image, Matrix4x4 matrix, float screenWidth, float screenHeight)
        {
            foreach(Triangle t in triangles)
            {
                t.Draw(image, matrix, screenWidth, screenHeight);
            }

        }
    }
}
