using System.Diagnostics.Tracing;
using System.Globalization;
using System.Numerics;

namespace GKproject3D
{
    public partial class Form1 : Form
    {
        private List<Object3D> objects;


        private Camera camera;
        private const int FOV_DEGREES = 90;
        const float A = 1.0f;
        public Form1()
        {
            InitializeComponent();
            imageBox.Image = new Bitmap(imageBox.Width, imageBox.Height);
            objects = new List<Object3D>();
            camera = new Camera(new Vector3(0.9f, 1.0f, 0.7f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0, 1, 0));

            objects.Add(new Object3D("../../../objects/treeScaled.obj", Color.Olive));





            //List<Triangle> triList = new List<Triangle>();
            //Point3D p1 = new Point3D(new Vector4(-A, -A, 0, 1), new Vector3(1, 1, 1));
            //Point3D p2 = new Point3D(new Vector4(A, -A, 0, 1), new Vector3(1, 1, 1));
            //Point3D p3 = new Point3D(new Vector4(0, A, 0, 1), new Vector3(1, 1, 1));
            //Point3D p4 = new Point3D(new Vector4(0, 0, A, 1), new Vector3(1, 1, 1));



            //triList.Add(new Triangle(p1, p2, p3));
            //triList.Add(new Triangle(p1, p2, p4));
            //triList.Add(new Triangle(p2, p3, p4));
            //triList.Add(new Triangle(p3, p1, p4));
            //objects.Add(new Object3D(triList, Color.Black));




            //triList[0].FillOutTest((Bitmap)imageBox.Image);
        }

        private void drawObject(Object3D obj)
        {
            // MODEL
            //Matrix4x4 modelM = Matrix4x4.CreateTranslation(0, 0, -0.5f);
            Matrix4x4 modelM = Matrix4x4.Identity;
            //Matrix4x4 modelM = Matrix4x4.CreateScale(2);

            // VIEW
            Matrix4x4 viewM = Matrix4x4.CreateLookAt(camera.Position, camera.Target, camera.UpVector);

            // PROJECTION
            float fov = (float)(Math.PI / 180) * FOV_DEGREES;
            float aspectRatio = imageBox.Width / imageBox.Height;
            float n = 1;
            float f = 100;
            Matrix4x4 projectionM = Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, n, f);


            Matrix4x4 M = modelM * viewM * projectionM;



            obj.Draw(imageBox.Image, M,imageBox.Width,imageBox.Height);



            Point3D middlePoint = new Point3D(new Vector4(0, 0, 0, 1), new Vector3(1, 0, 0));

            var midPos = middlePoint.getScreenPosition(M, imageBox.Width, imageBox.Height);
            using (Graphics g = Graphics.FromImage(imageBox.Image))
            {
                g.DrawEllipse(Pens.Red, midPos.X - 1.0f, midPos.Y - 1.0f, 2, 2);
            }


            imageBox.Refresh();

        }

        private void button1_Click(object sender, EventArgs e)
        {
           drawObject(objects[0]);

        }
    }
}