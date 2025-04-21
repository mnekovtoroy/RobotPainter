using RobotPainter.Calculations.Core;
using System.Drawing;

namespace RobotPainter.Calculations
{
    public class LabBitmap
    {
        private ColorLab[,] _image;
        private int w;
        private int h;

        public int Width {  get { return w; } }
        public int Height {  get { return h; } }

        public LabBitmap(Bitmap image)
        {
            w = image.Width;
            h = image.Height;

            _image = new ColorLab[w,h];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    _image[i, j] = ColorLab.FromRgb(image.GetPixel(i, j));
                }
            }
        }

        public LabBitmap(int width, int height)
        {
            w = width;
            h = height;
            _image = new ColorLab[w, h];
        }

        public Bitmap ToBitmap()
        {
            var bm = new Bitmap(w, h);

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    bm.SetPixel(i, j, GetPixel(i, j).ToRgb());
                }
            }

            return bm;
        }

        public ColorLab GetPixel(int x, int y)
        {
            return _image[x, y];
        }

        public void SetPixel(int x, int y, ColorLab color)
        {
            _image[x, y] = color;
        }

        internal static ColorLab[,] CalculateDifference(LabBitmap bmp1, LabBitmap bmp2)
        {
            if(bmp1.h != bmp2.h || bmp1.w != bmp2.w)
            {
                throw new ArgumentException("bitmaps must have save dimensions");
            }
            ColorLab[,] difference = new ColorLab[bmp1.h, bmp2.h];
            for (int i = 0; i < bmp1.w; i++)
            {
                for (int j = 0; j < bmp1.h; j++)
                {
                    difference[i, j] = bmp1.GetPixel(i, j) - bmp2.GetPixel(i, j);
                }
            }
            return difference;
        }
    }
}
