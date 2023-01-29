using System.Diagnostics.Tracing;
using System.Globalization;
using System.Numerics;

namespace GKproject3D
{
    public partial class Form1 : Form
    {
        private List<Object3D> objects;

        private LockBitmap lockBitmap;
        private float[,] zBufferMark;


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
            

            objects = new List<Object3D>();
            camera = new Camera(new Vector3(-1.7f, 0.5f, -2.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0, 1, 0));

            objects.Add(new Object3D("../../../objects/treeScaled.obj", Color.Olive));
            objects.Add(new Object3D("../../../objects/carScaled.obj", Color.Red));
        }


        private void renderScene()
        {
            ResetZBuffer();
            // MODEL
            Matrix4x4 treeModelM = Matrix4x4.Identity;
            Matrix4x4 carModelM = Matrix4x4.CreateScale(0.4f) * Matrix4x4.CreateTranslation(1.0f, 0, 0);

            // VIEW
            Matrix4x4 viewM = Matrix4x4.CreateLookAt(camera.Position, camera.Target, camera.UpVector);

            // PROJECTION
            float fov = (float)(Math.PI / 180) * FOV_DEGREES;
            float aspectRatio = imageBox.Width / imageBox.Height;
            float n = 1;
            float f = 100;
            Matrix4x4 projectionM = Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, n, f);

            lockBitmap.LockBits();
            objects[0].Draw(lockBitmap, treeModelM, viewM, projectionM, zBufferMark, camera);
            objects[1].Draw(lockBitmap, carModelM, viewM, projectionM, zBufferMark, camera);
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


        private void button1_Click(object sender, EventArgs e)
        {
            renderScene();
        }
    }
}