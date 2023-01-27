using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Globalization;

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
                throw new Exception("points list ins not of 3 length!");

            this.points = points.ToArray();
        }

        public void Draw(Image image, Matrix4x4 matrix, float screenWidth, float screenHeight)
        {
            Vector3 p0_screen = points[0].getScreenPosition(matrix, screenWidth, screenHeight);
            Vector3 p1_screen = points[1].getScreenPosition(matrix, screenWidth, screenHeight);
            Vector3 p2_screen = points[2].getScreenPosition(matrix, screenWidth, screenHeight);

            using (Graphics g = Graphics.FromImage(image))
            {
                g.DrawLine(Pens.Black, p0_screen.X, p0_screen.Y, p1_screen.X, p1_screen.Y);
                g.DrawLine(Pens.Black, p1_screen.X, p1_screen.Y, p2_screen.X, p2_screen.Y);
                g.DrawLine(Pens.Black, p2_screen.X, p2_screen.Y, p0_screen.X, p0_screen.Y);
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
