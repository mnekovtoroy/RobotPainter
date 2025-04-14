using RobotPainter.Calculations;
using System.Drawing;

namespace RobotPainter.ConsoleTest
{
    public static class ImageVisualiser
    {
        /// <summary>
        /// Visualise map of an array of vectors by overlaying the image with arrows.
        /// </summary>
        /// <param name="image">Image to overlay to</param>
        /// <param name="x">x component of the vector</param>
        /// <param name="y">y component of the vector</param>
        /// <param name="n">Number of rendered arrows in the shorter direction</param>
        /// <param name="c">Color of arrows</param>
        /// <param name="arrow_length">Length of arrows</param>
        /// <param name="tip_length">Length of the tip of the arrow</param>
        /// <param name="tip_angle">Angle of the tip of the arrow</param>
        /// <param name="point_radius">Radius of a starting point of an arrow</param>
        /// <returns>Image with arrows overlayed</returns>
        public static void VisualiseVectorsInline(Bitmap image, double[,] x, double[,] y, int n, Color arrow_c, Color dot_c, int arrow_length = 20, int tip_length = 3, int tip_angle = 30, int point_radius = 1)
        {
            int w = image.Width;
            int h = image.Height;
            int s_dim = w < h ? w : h;

            int step = s_dim / n;

            for(int i = step / 2; i < w; i+=step)
            {
                for(int j = step / 2; j < h; j+=step)
                {
                    int end_x = Convert.ToInt32(i + x[i, j] * arrow_length);
                    int end_y = Convert.ToInt32(j + y[i, j] * arrow_length);
                    DrawArrowInline(image, i, j, end_x, end_y, arrow_c, dot_c, tip_length, tip_angle, point_radius);                    
                }
            }
        }

        private static void DrawArrowInline(Bitmap image, int start_x, int start_y, int end_x, int end_y, Color arrow_c, Color dot_c, int tip_length, int tip_angle, int point_radius)
        {
            using( var g = Graphics.FromImage(image))
            {
                Pen p = new Pen(arrow_c);
                Brush b = new SolidBrush(dot_c);
                g.DrawLine(p, start_x, start_y, end_x, end_y);
                g.FillEllipse(b, start_x - point_radius, start_y - point_radius, point_radius * 2, point_radius * 2);

                //getting vector scaled vector to draw the tip
                double l = Math.Sqrt(Math.Pow(end_x - start_x, 2) + Math.Pow(end_y - start_y, 2));
                double v_x = 0, v_y = 0;
                if(l > 0)
                {
                    v_x = (start_x - end_x) * tip_length / l;
                    v_y = (start_y - end_y) * tip_length / l;
                } 
                else
                {
                    return;
                }

                //rotation matrix 1
                double angle_l = tip_angle * Math.PI / 180.0;
                double l00 = Math.Cos(angle_l), l01 = Math.Sin(-angle_l);
                double l10 = Math.Sin(angle_l), l11 = Math.Cos(angle_l);
                //rotation matrix 2
                double angle_r = -tip_angle * Math.PI / 180.0;
                double r00 = Math.Cos(angle_r), r01 = Math.Sin(-angle_r);
                double r10 = Math.Sin(angle_r), r11 = Math.Cos(angle_r);

                int v_xl = Convert.ToInt32(v_x * l00 + v_y * l01);
                int v_yl = Convert.ToInt32(v_x * l10 + v_y * l11);
                int v_xr = Convert.ToInt32(v_x * r00 + v_y * r01);
                int v_yr = Convert.ToInt32(v_x * r10 + v_y * r11);
                
                g.DrawLine(p, end_x, end_y, end_x + v_xl, end_y + v_yl);
                g.DrawLine(p, end_x, end_y, end_x + v_xr, end_y + v_yr);
            }            
        }
    }
}
