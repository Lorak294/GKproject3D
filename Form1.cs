using System.Diagnostics.Tracing;
using System.Drawing;
using System.Globalization;
using System.Numerics;

namespace GKproject3D
{
    public partial class Form1 : Form
    {
        private List<Object3D> objects;
        Object3D car;

        private LockBitmap lockBitmap;
        private float[,] zBufferMark;

        private bool animationActive;
        private float carAnimationAngle = 0.0f;


        private Camera camera;
        private const int FOV_DEGREES = 90;
        const float A = 1.0f;
        public Form1()
        {
            InitializeComponent();
            imageBox.Image = new Bitmap(imageBox.Width, imageBox.Height);
            lockBitmap = new LockBitmap((Bitmap)imageBox.Image);

            zBufferMark = new float[imageBox.Width, imageBox.Height];
            ResetZBuffer();

            animationActive = false;
            animationStartBtn.Enabled = !animationActive;
            animationStopBtn.Enabled = animationActive;



            objects = new List<Object3D>();
            camera = new Camera(
                new Vector3(10, 10, 10), 
                new Vector3(0.0f, 0.0f, 0.0f), 
                new Vector3(0, 1, 0), 
                (float)(Math.PI / 180) * FOV_DEGREES, 
                0.1f,
                100,
                (float)imageBox.Width / imageBox.Height);

            // MODELS
            Matrix4x4 treeModelM = Matrix4x4.CreateScale(3.0f);
            Matrix4x4 rockModelM = Matrix4x4.CreateScale(2.0f) * Matrix4x4.CreateTranslation(0, 0, -5.0f);
            Matrix4x4 carModelM =  Matrix4x4.CreateTranslation(0, 0, 3);
            Matrix4x4 candleModelM = Matrix4x4.CreateScale(0.4f) * Matrix4x4.CreateTranslation(0, 1.5f, -5.0f);
            //car = new Object3D("../../../objects/carScaled.obj", Color.Red, carModelM);

            //objects.Add(car);
            //objects.Add(new Object3D("../../../objects/treeScaled.obj", Color.Olive, treeModelM));
            //objects.Add(new Object3D("../../../objects/grass.obj", Color.Green, Matrix4x4.Identity));
            //objects.Add(new Object3D("../../../objects/rockScaled.obj", Color.Gray, rockModelM));
            //objects.Add(new Object3D("../../../objects/candleScaled.obj", Color.LightGoldenrodYellow, candleModelM));

            ImportScene("../../../objects/scene.obj");

            renderScene();
        }


        private void renderScene()
        {
            ResetZBuffer();
            // VIEW
            Matrix4x4 viewM = Matrix4x4.CreateLookAt(camera.Position, camera.Target, camera.UpVector);
            // PROJECTION
            Matrix4x4 projectionM = Matrix4x4.CreatePerspectiveFieldOfView(camera.FOV, camera.AspectRatio, camera.N, camera.F);

            lockBitmap.Clear(Color.White);
            lockBitmap.LockBits();

            foreach(Object3D obj in objects)
            {
                obj.Draw(lockBitmap, viewM, projectionM, zBufferMark, camera);
            }
            lockBitmap.UnlockBits();
            imageBox.Refresh();
        }

        private void ResetZBuffer()
        {
           for(int i=0; i<zBufferMark.GetLength(0); i++)
           {
                for(int j=0; j<zBufferMark.GetLength(1); j++)
                {
                    zBufferMark[i,j] = 1000.0f;
                }
           }
        }


        private void animationStartBtn_Click(object sender, EventArgs e)
        {
            setAnimation(true);
            animationTimer.Start();
        }

        private void animationStopBtn_Click(object sender, EventArgs e)
        {
            setAnimation(false);
            animationTimer.Stop();
        }
        public void setAnimation(bool active)
        {
            animationActive = active;
            animationStartBtn.Enabled = !active;
            animationStopBtn.Enabled = active;
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            CarMovement();
            UpdateCameraPos();
            renderScene();
        }

        private void CarMovement()
        {
            // experimental
            carAnimationAngle += 0.2f;
            Matrix4x4 carModelM =  Matrix4x4.CreateRotationY(carAnimationAngle);

            car.modelMatrix = carModelM;
        }

        private void UpdateCameraPos()
        {
            switch(camera.Mode)
            {
                case CameraMode.Follow:
                    camera.Target = car.WorldPosition;
                    break;
                case CameraMode.Behind:
                    camera.Position = (car.WorldPosition + new Vector3(0,2.5f,0) - car.WorldFrontVec*2);
                    camera.Target = car.WorldPosition;
                    break;
                default:
                    break;
            }

            xPosBox.Text = camera.Position.X.ToString();
            yPosBox.Text = camera.Position.Y.ToString();
            zPosBox.Text = camera.Position.Z.ToString();
            xTargetBox.Text = camera.Target.X.ToString();
            yTargetBox.Text = camera.Target.Y.ToString();
            zTargetBox.Text = camera.Target.Z.ToString();

        }

        private void CameraRBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (staticCameraRBtn.Checked)
                camera.Mode = CameraMode.Static;
            else if (followingCameraRBtn.Checked)
                camera.Mode = CameraMode.Follow;
            else
                camera.Mode = CameraMode.Behind;

            UpdateCameraPos();
            renderScene();
        }



        // SCENE IMPORTING
        private void ImportScene(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);


            string lineStr = sr.ReadLine()!;

            objects = new List<Object3D>();

            List<Vector3> vertList = new List<Vector3>();
            List<Vector3> normVectorList = new List<Vector3>();

            while (lineStr != null)
            {
                while(!lineStr.StartsWith('o'))
                    lineStr = sr.ReadLine()!;

                if (lineStr.StartsWith('o'))
                {
                    // importing object
                    List<Triangle> trigList = new List<Triangle>();

                    string[] header = lineStr.Split(' ');
                    string name = header[1];

                    lineStr = sr.ReadLine()!;

                    // vertices
                    while (lineStr.StartsWith("v "))
                    {
                        vertList.Add(ParseVertex(lineStr));
                        lineStr = sr.ReadLine()!;
                    }

                    // normal vectors
                    while (lineStr.StartsWith("vn"))
                    {
                        normVectorList.Add(ParseNormVector(lineStr));
                        lineStr = sr.ReadLine()!;
                    }

                    // triangles
                    while (lineStr != null && !lineStr.StartsWith('o'))
                    {
                        if (lineStr.StartsWith('f'))
                        {
                            trigList.Add(ParseTriangle(lineStr, vertList, normVectorList));
                        }
                        lineStr = sr.ReadLine()!;
                    }

                    objects.Add(new Object3D(trigList, Color.AliceBlue, Matrix4x4.Identity));
                    if(name == "Car")
                    {
                        car = objects.Last();
                    }
                }
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


    }
}