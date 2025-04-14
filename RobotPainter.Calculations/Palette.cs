using System.Drawing;

namespace RobotPainter.Calculations
{
    public class Palette
    {
        public List<ColorLab> Colors { get; set; }

        public Palette()
        {
            Colors = new List<ColorLab>();
        }

        public LabBitmap Apply(LabBitmap bmp)
        {
            for(int i = 0; i < bmp.Width; i++)
            {
                for(int j = 0; j < bmp.Height; j++)
                {
                    double min_dist = double.MaxValue;
                    int min_dist_i = -1;
                    for(int k = 0; k < Colors.Count; k++)
                    {
                        if (bmp.GetPixel(i, j).DeltaE76(Colors[k]) < min_dist)
                        {
                            min_dist = bmp.GetPixel(i, j).DeltaE76(Colors[k]);
                            min_dist_i = k;
                        }
                    }
                    bmp.SetPixel(i, j, Colors[min_dist_i]);
                }
            }
            return bmp;
        }
    }
}
