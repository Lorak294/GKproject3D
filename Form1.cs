using System.Diagnostics.Tracing;
using System.Drawing;
using System.Globalization;
using System.Numerics;

namespace GKproject3D
{
    public partial class Form1 : Form
    {
        private const int FOV_DEGREES = 90;
        private const int SPOTLIGHT_ANGLE = 45;

        private List<Object3D> objects;
        Object3D car = null!;
        private Camera camera;

        private Scene scene;
        public Form1()
        {
            InitializeComponent();
            imageBox.Image = new Bitmap(imageBox.Width, imageBox.Height);

            animationStartBtn.Enabled = true;
            animationStopBtn.Enabled = false;

            // camera
            camera = new Camera(
                new Vector3(5, 5, 5), 
                new Vector3(0.0f, 0.0f, 0.0f), 
                new Vector3(0, 1, 0), 
                (float)(Math.PI / 180) * FOV_DEGREES, 
                0.1f,
                100,
                (float)imageBox.Width / imageBox.Height);

            xPosBox.Text = camera.Position.X.ToString();
            yPosBox.Text = camera.Position.Y.ToString();
            zPosBox.Text = camera.Position.Z.ToString();
            xTargetBox.Text = camera.Target.X.ToString();
            yTargetBox.Text = camera.Target.Y.ToString();
            zTargetBox.Text = camera.Target.Z.ToString();

            // objects
            objects = new List<Object3D>();
            ImportScene("../../../objects/objectsWithMaterials/scene.obj");

            // lightSources
            List<LightSource> lightsSources = new List<LightSource>();
            //  candle light
            //candleLight = new LightSource(new Vector3(0.012919f, 1.9f, -6.139567f), 0.5f, 0.5f);



            lightsSources.Add(new LightSource(new Vector3(0.012919f, 1.9f, -6.139567f),Vector3.One*0.5f, Vector3.One * 0.5f));
            // car spotlight
            SpotLight carSpotlight = new SpotLight(car.WorldPosition, Vector3.One, Vector3.One, car.WorldFrontVec, (float)Math.Cos((Math.PI / 180) * SPOTLIGHT_ANGLE));
            lightsSources.Add(carSpotlight);
            // police light spotlight
            SpotLight policeLight = new SpotLight(car.WorldPosition, Vector3.UnitZ, Vector3.UnitZ, car.WorldFrontVec, (float)Math.Cos((Math.PI / 180) * SPOTLIGHT_ANGLE));
            lightsSources.Add(policeLight);



            scene = new Scene(camera, (Bitmap)imageBox.Image, objects, car,lightsSources,carSpotlight, policeLight);

            renderScene();
        }


        private void renderScene()
        {
            scene.ResetZBuffer();
            // VIEW
            Matrix4x4 viewM = Matrix4x4.CreateLookAt(scene.Camera.Position, scene.Camera.Target, scene.Camera.UpVector);
            // PROJECTION
            Matrix4x4 projectionM = Matrix4x4.CreatePerspectiveFieldOfView(scene.Camera.FOV, scene.Camera.AspectRatio, scene.Camera.N, scene.Camera.F);

            scene.LockBitmap.Clear(Color.White);
            scene.LockBitmap.LockBits();

            foreach(Object3D obj in scene.Objects)
            {
                obj.Draw(scene, viewM, projectionM);
            }
            scene.LockBitmap.UnlockBits();




            // lightSource
            using(Graphics g = Graphics.FromImage(imageBox.Image))
            {

                        Vector4 v4 = new Vector4(scene.CarSpotlight.Position, 1);
                        v4 = Vector4.Transform(v4, viewM);
                        v4 = Vector4.Transform(v4, projectionM);

                        Vector3 NDCPosition = new Vector3(v4.X / v4.W, v4.Y / v4.W, v4.Z / v4.W);


                       Vector3 ScreenPosition = new Vector3(
                            (float)Math.Round((NDCPosition.X + 1) * scene.LockBitmap.Width / 2),
                            (float)Math.Round((1 - NDCPosition.Y) * scene.LockBitmap.Height / 2),
                            (NDCPosition.Z + 1) / 2);

                g.DrawEllipse(Pens.Red, ScreenPosition.X, ScreenPosition.Y, 10, 10);


                v4 = new Vector4(scene.PoliceLight.Position, 1);
                v4 = Vector4.Transform(v4, viewM);
                v4 = Vector4.Transform(v4, projectionM);

                NDCPosition = new Vector3(v4.X / v4.W, v4.Y / v4.W, v4.Z / v4.W);


                ScreenPosition = new Vector3(
                     (float)Math.Round((NDCPosition.X + 1) * scene.LockBitmap.Width / 2),
                     (float)Math.Round((1 - NDCPosition.Y) * scene.LockBitmap.Height / 2),
                     (NDCPosition.Z + 1) / 2);

                g.DrawEllipse(Pens.Green, ScreenPosition.X, ScreenPosition.Y, 10, 10);
            }


            imageBox.Refresh();
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
            scene.AnimationActive = active;
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
            scene.CarAnimationAngle += 0.2f;
            scene.PoliceLightAngle += 0.2f;
            Matrix4x4 carModelM =  Matrix4x4.CreateRotationY(scene.CarAnimationAngle);
            scene.Car.modelMatrix = carModelM;
        }

        private void UpdateCameraPos()
        {
            switch(scene.Camera.Mode)
            {
                case CameraMode.Follow:
                    scene.Camera.Target = scene.Car.WorldPosition;
                    break;
                case CameraMode.Behind:
                    scene.Camera.Position = (scene.Car.WorldPosition + new Vector3(0,2.5f,0) - scene.Car.WorldFrontVec*2);
                    scene.Camera.Target = scene.Car.WorldPosition;
                    break;
                default:
                    break;
            }

            xPosBox.Text = scene.Camera.Position.X.ToString();
            yPosBox.Text = scene.Camera.Position.Y.ToString();
            zPosBox.Text = scene.Camera.Position.Z.ToString();
            xTargetBox.Text = scene.Camera.Target.X.ToString();
            yTargetBox.Text = scene.Camera.Target.Y.ToString();
            zTargetBox.Text = scene.Camera.Target.Z.ToString();

        }

        private void CameraRBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (staticCameraRBtn.Checked)
                scene.Camera.Mode = CameraMode.Static;
            else if (followingCameraRBtn.Checked)
                scene.Camera.Mode = CameraMode.Follow;
            else
                scene.Camera.Mode = CameraMode.Behind;

            UpdateCameraPos();
            renderScene();
        }

        private void ShadingRBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (staticShadingRB.Checked)
                scene.ShadingMode = ShadingMode.Static;
            else if (gouraudShadingRB.Checked)
                scene.ShadingMode = ShadingMode.Gouraud;
            else
                scene.ShadingMode = ShadingMode.Phong;
            renderScene();
        }



        // SCENE IMPORTING
        private void ImportScene(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);


            string lineStr = sr.ReadLine()!;



            // MATERIALS IMPORT
            Dictionary<string, Material> materials = new Dictionary<string, Material>();
            string currentMaterialName = "";
            string[] firstLine = lineStr.Split(' ');
            if(firstLine.Length == 2 && firstLine[0] == "mtllib")
            {
                materials = ImportMaterials("../../../objects/objectsWithMaterials/" + firstLine[1]);
            }


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
                        if(lineStr.StartsWith("usemtl"))
                        {
                            currentMaterialName = lineStr.Split(' ')[1];
                        }
                        if (lineStr.StartsWith('f'))
                        {
                            trigList.Add(ParseTriangle(lineStr, vertList, normVectorList, materials[currentMaterialName]));
                        }
                        lineStr = sr.ReadLine()!;
                    }

                    // marking CAR as special object
                    objects.Add(new Object3D(trigList, Matrix4x4.Identity));
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
        private Triangle ParseTriangle(string figureStr, List<Vector3> vertList, List<Vector3> normVectorList, Material material)
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

            return new Triangle(selectedVerts,material);
        }
        private Dictionary<string,Material> ImportMaterials(string filepath)
        {
            // TO FINISH
            Dictionary<string,Material> materials = new Dictionary<string,Material>();

            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);


            string lineStr = sr.ReadLine()!;
            string materialName = "error";
            Vector3? kaVec = null, kdVec = null, ksVec = null;
            float? alpha = null;

            while (lineStr != null)
            {
                while(lineStr != null && !lineStr.StartsWith("newmtl"))
                {
                    string[] parameters = lineStr.Split(' ');

                    

                    switch(parameters[0])
                    {
                        case "Ka":
                            kaVec = new Vector3(float.Parse(parameters[1], NumberStyles.Any, CultureInfo.InvariantCulture),
                                float.Parse(parameters[2], NumberStyles.Any, CultureInfo.InvariantCulture),
                                float.Parse(parameters[3], NumberStyles.Any, CultureInfo.InvariantCulture));
                            break;
                        case "Kd":
                            kdVec = new Vector3(float.Parse(parameters[1], NumberStyles.Any, CultureInfo.InvariantCulture),
                                float.Parse(parameters[2], NumberStyles.Any, CultureInfo.InvariantCulture),
                                float.Parse(parameters[3], NumberStyles.Any, CultureInfo.InvariantCulture));
                            break;
                        case "Ks":
                            ksVec = new Vector3(float.Parse(parameters[1], NumberStyles.Any, CultureInfo.InvariantCulture),
                                float.Parse(parameters[2], NumberStyles.Any, CultureInfo.InvariantCulture),
                                float.Parse(parameters[3], NumberStyles.Any, CultureInfo.InvariantCulture));
                            break;
                        case "alpha":
                            alpha = float.Parse(parameters[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                            break;
                    }
                    lineStr = sr.ReadLine()!;
                }
                if(kaVec.HasValue && kdVec.HasValue && ksVec.HasValue && alpha.HasValue)
                    materials.Add(materialName,new Material(ksVec.Value, kdVec.Value, kaVec.Value, alpha.Value));

                if(lineStr != null)
                {
                    string[] lineArguments = lineStr.Split(' ');
                    materialName = lineArguments[1];

                    lineStr = sr.ReadLine()!;
                }
            }
               





            sr.Close();
            fs.Close();
            return materials;
        }

    }
}