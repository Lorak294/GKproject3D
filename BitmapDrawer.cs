using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKproject3D
{
    public class BitmapDrawer
    {
        private Bitmap orgBitmap;
        private byte[] rgbValues;
        



        public BitmapDrawer(Bitmap bmp)
        {
            this.orgBitmap = bmp;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            
            rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
        }







    }
}
