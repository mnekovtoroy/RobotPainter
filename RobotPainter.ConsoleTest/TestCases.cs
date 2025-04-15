using System;
using System.Drawing;
using RobotPainter.Calculations;
using RobotPainter.Calculations.Optimization;
using RobotPainter.Visualization;

namespace RobotPainter.ConsoleTest
{
    public static class TestCases
    {
        public static void ColorConversionTest()
        {
            Color test = Color.FromArgb(56, 23, 117);
            ColorLab colorLab = ColorLab.FromRgb(test);
            Color reverse = colorLab.ToRgb();

            Console.WriteLine($"Original:\n\tL: {test.R}\ta: {test.G}\tb: {test.B}");
            Console.WriteLine($"L*a*b*:\n\tL: {colorLab.L}\ta: {colorLab.a}\tb: {colorLab.b}");
            Console.WriteLine($"Reversed:\n\tL: {reverse.R}\ta: {reverse.G}\tb: {reverse.B}");
        }

        public static void PaletteApplianceTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap original = new Bitmap(path + "test5.jpg");
            original.Save(path + "1_original.png");

            LabBitmap lbm = new LabBitmap(original);
            Bitmap reverse = lbm.ToBitmap();
            reverse.Save(path + "2_reverse.png");

            Palette plt = new Palette();
            plt.Colors.Add(ColorLab.FromRgb(Color.White));
            plt.Colors.Add(ColorLab.FromRgb(Color.LightGray));
            plt.Colors.Add(ColorLab.FromRgb(Color.Gray));
            plt.Colors.Add(ColorLab.FromRgb(Color.DarkGray));
            plt.Colors.Add(ColorLab.FromRgb(Color.Black));

            Bitmap plted = plt.Apply(lbm).ToBitmap();
            plted.Save(path + "3_paletted.png");
        }

        public static void VectorTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball.jpg");

            int n_avg = 3;
            int n_arrows = 20;

            int l_s = image.Width < image.Height ? image.Width : image.Height;
            int arrow_length = (l_s / 20) / 3;
            int tip_length = arrow_length / 4;

            LabBitmap lbmp = new LabBitmap(image);

            double[,] u, v;
            (u, v) = ImageProcessor.LNormWithRollAvg(lbmp, n_avg);
            //visualise
            VectorVisualizer.VisualizeVectorsInline(image, u, v, n_arrows, Color.Blue, Color.Blue, arrow_length, tip_length);

            image.Save(path + "norm_visualised.png");
        }

        public static void VoronoiVisualizerTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball.jpg");

            List<(double, double)> points;
            var sites = VoronoiStrokeGenerator.GenerateRandomMesh(200, image.Width, image.Height, out points);

            Bitmap voronoi = new Bitmap(image.Width, image.Height);            
            voronoi.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            int centroid_r = 2;
            Color edge_c = Color.Blue;
            Color centroid_c = Color.Blue;
            int point_r = 2;
            Color point_c = Color.Red;
            VoronoiVisualizer.VisualizeVoronoiInline(image, sites, edge_c, centroid_c, centroid_r);
            VoronoiVisualizer.VisualisePointsInline(image, points, point_c, point_r);

            image.Save(path + "test_voronoi.png");
        }

        public static void OptimizatoinTest()
        {
            //var func = (double x, double y) => { return Math.Sqrt(x * x + y * y + 5); };
            var func = (double x, double y) => { return Math.Pow(x * x + y - 11, 2) + Math.Pow(x + y * y - 7, 2); };

            double x0 = 2.0;
            double y0 = 3.0;

            int kmax = 1000;
            double max_step = double.MaxValue;
            double tol = 1e-10;
            double a = 3e-2;
            double l = 1.2;
            double h = 1e-4;


            double xopt, yopt;

            IOptimizer optimizer = new GradDescent()
            {
                kmax = kmax,
                max_step = max_step,
                tol = tol,
                a = a,
                l = l,
                h = h
            };

            (xopt, yopt) = optimizer.Optimize(func, x0, y0);

            Console.WriteLine($"x = {Math.Round(xopt, 3)}\ny = {Math.Round(yopt,3)}");
        }
    }
}
