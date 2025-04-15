using System.Drawing;

namespace RobotPainter.Calculations
{
    public static class ImageProcessor
    {
        /// <summary>
        /// Calculates norms to gradient of L, smoothed out by rolling averages.
        /// </summary>
        /// <param name="bmp">Image to process.</param>
        /// <param name="n_avg">Rolling average will be calculated from (1+2*n)^2 cells around every cell.</param>
        /// <returns>(double[,], double[,]) - x and y components of the norm.</returns>
        public static (double[,], double[,]) LNormWithRollAvg(LabBitmap bmp, int n_avg)
        {
            double[,] u, v;
            (u, v) = LGrad(bmp);
            u = RollingAvg(u, n_avg);
            v = RollingAvg(v, n_avg);
            return Norm(u, v);
        }

        /// <summary>
        /// Calculates x and y components of the gradient of L component for target image in L*a*b* color-space.
        /// </summary>
        /// <param name="bmp">Image to process.</param>
        /// <returns>(double[,], double[,]) - x and y components of the gradient of L.</returns>
        public static (double[,], double[,]) LGrad(LabBitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            double[,] u = new double[w, h];
            double[,] v = new double[w, h];

            //calculate gradient
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    double curr_u, curr_v;

                    //calculating y component of the gradient
                    if (i == 0)
                    {
                        curr_u = bmp.GetPixel(i + 1, j).L - bmp.GetPixel(i, j).L;
                    }
                    else if (i == w - 1)
                    {
                        curr_u = bmp.GetPixel(i, j).L - bmp.GetPixel(i - 1, j).L;
                    }
                    else
                    {
                        curr_u = (bmp.GetPixel(i + 1, j).L - bmp.GetPixel(i - 1, j).L) / 2.0;
                    }

                    //calculating y component of the gradient
                    if (j == 0)
                    {
                        curr_v = bmp.GetPixel(i, j + 1).L - bmp.GetPixel(i, j).L;
                    }
                    else if (j == h - 1)
                    {
                        curr_v = bmp.GetPixel(i, j).L - bmp.GetPixel(i, j - 1).L;
                    }
                    else
                    {
                        curr_v = (bmp.GetPixel(i, j + 1).L - bmp.GetPixel(i, j - 1).L) / 2.0;
                    }

                    //result
                    u[i, j] = curr_u;
                    v[i, j] = curr_v;
                }
            }
            return (u, v);
        }

        /// <summary>
        /// Calculates x and y components of the norm to gradient of L component of the image in L*a*b* color-space.
        /// </summary>
        /// <param name="u">x-component of the gradient.</param>
        /// <param name="v">y-component of the gradient.</param>
        /// <returns>(double[,], double[,]) - x and y components of the norm to gradient.</returns>
        public static (double[,], double[,]) Norm(double[,] u, double[,] v)
        {
            if (u.GetLength(0) !=  v.GetLength(0) || u.GetLength(1) != v.GetLength(1))
            {
                throw new InvalidOperationException();
            } 
            int w = u.GetLength(0);
            int h = u.GetLength(1);

            double[,] u_norm = new double[w, h];
            double[,] v_norm = new double[w, h];

            //calculate gradient
            for (int i = 0; i <  w; i++)
            {
                for(int j = 0; j < h; j++)
                {
                    double curr_u = u[i, j];
                    double curr_v = v[i, j];

                    //rotating 90 degrees
                    double r00 = 0.0, r01 = -1.0;
                    double r10 = 1.0, r11 = 0.0; //rotating matrix
                    double new_u = curr_u * r00 + curr_v * r01;
                    double new_v = curr_u * r10 + curr_v * r11;
                    curr_u = new_u;
                    curr_v = new_v;

                    //norm
                    double l = Math.Sqrt(curr_u*curr_u + curr_v*curr_v);
                    if(l > 0)
                    {
                        curr_u /= l;
                        curr_v /= l;
                    }

                    //result
                    u_norm[i, j] = curr_u;
                    v_norm[i, j] = curr_v;
                }
            }
            return (u_norm, v_norm);
        }

        /// <summary>
        /// Calculates rolling average.
        /// </summary>
        /// <param name="x">Matrix to process.</param>
        /// <param name="n">Rolling average will be calculated from (1+2*n)^2 cells around every cell.</param>
        /// <returns>Matrix of rolling averages.</returns>
        public static double[,] RollingAvg(double[,] x, int n)
        {
            int w = x.GetLength(0);
            int h = x.GetLength(1);

            double[,] avg = new double[w, h];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int x_start = Math.Max(0, i - n);
                    int x_end = Math.Min(w - 1, i + n);

                    int y_start = Math.Max(0, j - n);
                    int y_end = Math.Min(h - 1, j + n);

                    double sum = 0.0;
                    int count = 0;
                    for (int u = x_start; u <= x_end; u++)
                    {
                        for (int v = y_start; v <= y_end; v++)
                        {
                            sum += x[u, v];
                            count++;
                        }
                    }
                    avg[i, j] = sum / count;
                }
            }
            return avg;
        }
    }
}
