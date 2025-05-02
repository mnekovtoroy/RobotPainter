namespace RobotPainter.Core
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
                    bmp.SetPixel(i, j, Apply(bmp.GetPixel(i,j)));
                }
            }
            return bmp;
        }

        public ColorLab Apply(ColorLab color)
        {
            double min_dist = double.MaxValue;
            int min_dist_i = -1;
            for (int k = 0; k < Colors.Count; k++)
            {
                if (color.DeltaE76(Colors[k]) < min_dist)
                {
                    min_dist = color.DeltaE76(Colors[k]);
                    min_dist_i = k;
                }
            }
            return Colors[min_dist_i];
        }
    }
}
