using System;
using System.ComponentModel;
using System.Drawing;
using RobotPainter.Calculations.Brushes;
using RobotPainter.Calculations.Clustering;
using RobotPainter.Calculations.Core;
using RobotPainter.Calculations.ImageProcessing;
using RobotPainter.Calculations.Optimization;
using RobotPainter.Calculations.StrokeGeneration;
using RobotPainter.Visualization;
using SharpVoronoiLib;

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
            Bitmap image = new Bitmap(path + "test_ball2.jpg");

            int n_avg = 7;
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

            var sites = LegacyVoronoiStrokeGenerator.GenerateRandomMesh(200, image.Width, image.Height);

            Bitmap voronoi = new Bitmap(image.Width, image.Height);            
            voronoi.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            int centroid_r = 2;
            Color edge_c = Color.Blue;
            Color centroid_c = Color.Blue;
            int point_r = 2;
            Color point_c = Color.Red;
            VoronoiVisualizer.VisualizeVoronoiInline(image, sites, edge_c, centroid_c, centroid_r);
            VoronoiVisualizer.VisualisePointsInline(image, sites.Select(s => (s.X, s.Y)).ToList(), point_c, point_r);

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
        
        public static void VoronoiFillTest()
        {
            List<Color> colors = new List<Color>() {
                Color.Gray,
                Color.LightGray,
                Color.DarkGray
            };

            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball.jpg");

            LabBitmap lbmp = new LabBitmap(image);

            double[,] u, v;
            (u, v) = ImageProcessor.LNormWithRollAvg(lbmp, 3);

            var voronoi = new LegacyVoronoiStrokeGenerator(200, lbmp, u, v, new GradDescent());
            DateTime start = DateTime.Now;
            int[,] mask = voronoi.MaskPointsToNearestSites();
            DateTime end = DateTime.Now;
            Console.WriteLine($"Total time to mask {Math.Round((end-start).TotalMilliseconds, 3)} ms.");

            Bitmap fill = new Bitmap(image.Width, image.Height);
            fill.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            VoronoiVisualizer.VoronoiFillInline(fill, mask, colors);
            VoronoiVisualizer.VisualizeVoronoiInline(fill, voronoi.sites, Color.Black, Color.Black, 0);
            //VoronoiVisualizer.VisualisePointsInline(fill, voronoi.sites.Select(x => (x.X, x.Y)).ToList(), Color.Black, 2);
            fill.Save(path + "test_fill.png");

            bool flag = false;
            foreach(var i in mask)
            {
                if(i == -1)
                {
                    flag = true;
                    break;
                }
            }
            if(flag)
            {
                Console.WriteLine("Cringe");
            }
            else
            {
                Console.WriteLine("All good");
            }
        }

        public static void LdiffPullTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball.jpg");
            int sites_n = 2000;

            LabBitmap lbmp = new LabBitmap(image);

            double[,] u, v;
            (u, v) = ImageProcessor.LNormWithRollAvg(lbmp, 3);

            var voronoi = new LegacyVoronoiStrokeGenerator(sites_n, lbmp, u, v, new GradDescent());

            int[] check_intervals = new int[] { 1, 2, 5};

            var pre = lbmp.ToBitmap();
            VoronoiVisualizer.VisualizeVoronoiInline(pre, voronoi.sites, Color.Blue, Color.Blue, 0);
            VoronoiVisualizer.VisualisePointsInline(pre, voronoi.sites.Select(s => (s.X, s.Y)).ToList(), Color.Red, 1);
            pre.Save(path + $"test_Lpull_{sites_n}_0.png");
            pre.Dispose();
            for (int i = 0; i < check_intervals.Max(); i++)
            {
                voronoi.PerformLPull();
                if(check_intervals.Contains(i+1))
                {
                    Console.WriteLine($"{i+1} done.");
                    var res = lbmp.ToBitmap();
                    VoronoiVisualizer.VisualizeVoronoiInline(res, voronoi.sites, Color.Blue, Color.Blue, 0);
                    VoronoiVisualizer.VisualisePointsInline(res, voronoi.sites.Select(s => (s.X, s.Y)).ToList(), Color.Red, 1);
                    res.Save(path + $"test_Lpull_{sites_n}_{i+1}.png");
                    res.Dispose();
                }
            }
        }

        public static void CostFuncTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball.jpg");
            int sites_n = 200;

            LabBitmap lbmp = new LabBitmap(image);

            double[,] u, v;
            (u, v) = ImageProcessor.LNormWithRollAvg(lbmp, 3);

            var voronoi = new LegacyVoronoiStrokeGenerator(sites_n, lbmp, u, v, new GradDescent());

            int[] check_intervals = new int[] { 1, 2, 5 };

            var pre = lbmp.ToBitmap();
            VoronoiVisualizer.VisualizeVoronoiInline(pre, voronoi.sites, Color.Blue, Color.Blue, 0);
            VoronoiVisualizer.VisualisePointsInline(pre, voronoi.sites.Select(s => (s.X, s.Y)).ToList(), Color.Red, 1);
            pre.Save(path + $"test_Lpull_{sites_n}_0.png");
            pre.Dispose();
            for (int i = 0; i < check_intervals.Max(); i++)
            {
                voronoi.PerformLPull();
                if (check_intervals.Contains(i + 1))
                {
                    var tracker = new List<VoronoiSite>() { voronoi.sites[0] };
                    Console.WriteLine($"{i + 1} done.");


                    double cc = voronoi.ColorCost(voronoi.sites[0]);
                    double dc = voronoi.DirectionalCost(voronoi.sites[0]);
                    double sc = voronoi.ShapeCost(voronoi.sites[0]);
                    double tc = (1.0 + dc + sc) * cc;
                    Console.WriteLine($"\tColor cost:\t{cc}");
                    Console.WriteLine($"\tDirectioanl cost:\t{dc}");
                    Console.WriteLine($"\tShape cost:\t{sc}");
                    Console.WriteLine($"\t\tTotal cost: {tc}");

                    var res = lbmp.ToBitmap();
                    VoronoiVisualizer.VisualizeVoronoiInline(res, tracker, Color.Blue, Color.Blue, 0);
                    VoronoiVisualizer.VisualisePointsInline(res, tracker.Select(s => (s.X, s.Y)).ToList(), Color.Red, 1);

                    int c_x = Convert.ToInt32(tracker[0].Centroid.X);
                    int c_y = Convert.ToInt32(tracker[0].Centroid.Y);
                    VectorVisualizer.VisualiseSingleVectorInline(
                        res,
                        c_x,
                        c_y,
                        u[c_x, c_y],
                        v[c_x, c_y],
                        Color.Blue,
                        Color.Blue,
                        point_radius: 1);
                    res.Save(path + $"test_Lpull_{sites_n}_{i + 1}.png");
                    res.Dispose();
                }
            }
        }

        public static void OptimizationTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test3.jpg");
            int sites_n = 1000;

            LabBitmap lbmp = new LabBitmap(image);

            double[,] u, v;
            (u, v) = ImageProcessor.LNormWithRollAvg(lbmp, 3);

            var optimizer = new GradDescent()
            {
                a = 0.4,
                l = 1.1,
                h = 0.1,
                max_step = 3.0,
                tol = 0.3,
                kmax = 3
            };
            var voronoi = new LegacyVoronoiStrokeGenerator(sites_n, lbmp, u, v, optimizer);

            int[] check_intervals = new int[] { 1, 2, 3, 4, 5 };

            var pre = lbmp.ToBitmap();
            VoronoiVisualizer.VisualizeVoronoiInline(pre, voronoi.sites, Color.Blue, Color.Blue, 0);
            VoronoiVisualizer.VisualisePointsInline(pre, voronoi.sites.Select(s => (s.X, s.Y)).ToList(), Color.Red, 1);
            pre.Save(path + @$"test_test_optimization_{sites_n}_0.png");
            pre.Dispose();
            for (int i = 0; i < check_intervals.Max(); i++)
            {
                voronoi.Optimize5(1);
                if (check_intervals.Contains(i + 1))
                {
                    Console.WriteLine($"{i + 1} done.");
                    var res = lbmp.ToBitmap();
                    VoronoiVisualizer.VisualizeVoronoiInline(res, voronoi.sites, Color.Blue, Color.Blue, 0);
                    VoronoiVisualizer.VisualisePointsInline(res, voronoi.sites.Select(s => (s.X, s.Y)).ToList(), Color.Red, 1);
                    res.Save(path + @$"test_test_optimization_{sites_n}_{i + 1}.png");
                    res.Dispose();
                }
            }
        }

        public static void BrushstrokeGenerationTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball2.jpg");
            int sites_n = 10000;

            LabBitmap lbmp = new LabBitmap(image);

            var optimizer = new GradDescent()
            {
                a = 0.4,
                l = 1.1,
                h = 0.1,
                max_step = 3.0,
                tol = 0.3,
                kmax = 1
            };
            var generator = new StrokeGenerator(lbmp, sites_n, optimizer, 11);

            generator.Lfit(20, 2.0);

            generator.CalculateStorkes();
            var stroke_visualised = generator.GetColoredStrokeMap();

            VoronoiVisualizer.VisualizeVoronoiInline(image, generator.sites, Color.Blue, Color.Blue, 0);
            image.Save(path + @"test_strokes\voronoi.png");
            
            var res_bitmap = stroke_visualised.ToBitmap();
            res_bitmap.Save(path + @"test_strokes\strokes.png");

            foreach(var stroke in generator.strokes)
            {
                VoronoiVisualizer.VisualizeStrokeInline(res_bitmap, stroke.involvedSites.Select(s => (Convert.ToInt32(s.Centroid.X), Convert.ToInt32(s.Centroid.Y))).ToList(), Color.Blue, Color.Red, 1);
            }
            res_bitmap.Save(path + @"test_strokes\stroke_lines.png");

            var res_bitmap2 = stroke_visualised.ToBitmap();
            VoronoiVisualizer.VisualizeVoronoiInline(res_bitmap2, generator.sites, Color.Blue, Color.Red, 1);
            res_bitmap2.Save(path + @"test_strokes\voronoi_strokes.png");
        }

        public static void SingleBrushModelTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball2.jpg");
            int sites_n = 1000;
            double real_width = 300;
            double real_height = 300;

            LabBitmap lbmp = new LabBitmap(image);

            var optimizer = new GradDescent()
            {
                a = 0.4,
                l = 1.1,
                h = 0.1,
                max_step = 3.0,
                tol = 0.3,
                kmax = 1
            };
            var brushModel = new BasicBrushModel();
            var generator = new StrokeGenerator(lbmp, sites_n, optimizer, 11);

            generator.Lfit(20, 2.0);

            generator.CalculateStorkes();
            var stroke_visualised = generator.GetColoredStrokeMap();

            VoronoiVisualizer.VisualizeVoronoiInline(image, generator.sites, Color.Blue, Color.Blue, 0);
            image.Save(path + @"brushmodel_test\voronoi.png");

            var res_bitmap = stroke_visualised.ToBitmap();
            res_bitmap.Save(path + @"brushmodel_test\strokes.png");

            Bitmap result_image = new Bitmap(image.Width, image.Height);
            result_image.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            var b_calc = new BrushstrokeBuilder(real_width / image.Width, real_height / image.Height, brushModel);
            int n_brushstrokes_to_draw = 1;

            for(int i = 0; i < n_brushstrokes_to_draw; i++)
            {
                var stroke = generator.strokes[i];
                var brush_desired_path = b_calc.GetDesiredPath(stroke);
                var brush_root_path = b_calc.GetBrushPath(stroke);

                using(var g = Graphics.FromImage(result_image))
                {
                    var br = new SolidBrush(stroke.MainColor.ToRgb());
                    brushModel.DrawStroke(g, br, brush_root_path, image.Width / real_width, image.Height / real_height, mult_coeff: 100);
                }

                var stroke_skeleton = brushModel.CalculateStrokeSkeleton(brush_root_path);

                double x_scale = image.Width / real_width;
                double y_scale = image.Height / real_height;

                var test = stroke_skeleton.points.Select(p => p.z).ToList();
                VoronoiVisualizer.VisualizeVoronoiInline(result_image, stroke.involvedSites, Color.Blue, Color.Red, 1);
                VoronoiVisualizer.VisualizeStrokeInline(result_image, brush_desired_path.Select(p => (Convert.ToInt32(p.x * x_scale), Convert.ToInt32(p.y * y_scale))).ToList(), Color.Green, Color.Orange, 1);
                VoronoiVisualizer.VisualizeStrokeInline(result_image, stroke_skeleton.points.Select(p => (Convert.ToInt32(p.x * x_scale), Convert.ToInt32(p.y * y_scale))).ToList(), Color.Red, Color.Blue, 0);
                VoronoiVisualizer.VisualisePointsInline(result_image, [(brush_desired_path[0].x * x_scale, brush_desired_path[0].y * y_scale)], Color.Pink, 1);
            }
            result_image.Save(path + @"brushmodel_test\actual_strokes.png");
        }

        public static void FullBrushModelTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball2.jpg");
            int sites_n = 50000;
            double real_width = 300;
            double real_height = 300;

            LabBitmap lbmp = new LabBitmap(image);

            var optimizer = new GradDescent()
            {
                a = 0.4,
                l = 1.1,
                h = 0.1,
                max_step = 3.0,
                tol = 0.3,
                kmax = 1
            };
            var brushModel = new BasicBrushModel();
            var generator = new StrokeGenerator(lbmp, sites_n, optimizer, 11);

            generator.Lfit(2, 2.0);

            generator.CalculateStorkes();
            var stroke_visualised = generator.GetColoredStrokeMap();

            VoronoiVisualizer.VisualizeVoronoiInline(image, generator.sites, Color.Blue, Color.Blue, 0);
            image.Save(path + @"fullbrushmodel_test\voronoi.png");

            var res_bitmap = stroke_visualised.ToBitmap();
            res_bitmap.Save(path + @"fullbrushmodel_test\strokes.png");

            Bitmap result_image = new Bitmap(image.Width, image.Height);
            result_image.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            var b_calc = new BrushstrokeBuilder(real_width / image.Width, real_height / image.Height, brushModel);

            Console.WriteLine($"Total strokes: {generator.strokes.Count}");

            using (var g = Graphics.FromImage(result_image))
            {
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0,0,image.Width, image.Height));
                for(int i =  0; i < generator.strokes.Count; i++)
                {
                    var stroke = generator.strokes[i];
                    var brush_root_path = b_calc.GetBrushPath(stroke);

                    var br = new SolidBrush(stroke.MainColor.ToRgb());
                    brushModel.DrawStroke(g, br, brush_root_path, image.Width / real_width, image.Height / real_height, mult_coeff: 100);

                    /*var stroke_skeleton = brushModel.CalculateStrokeSkeleton(brush_root_path);

                    double x_scale = image.Width / real_width;
                    double y_scale = image.Height / real_height;

                    VoronoiVisualizer.VisualizeStrokeInline(result_image, stroke_skeleton.points.Where(p => p.z < 0).Select(p => (Convert.ToInt32(p.x * x_scale), Convert.ToInt32(p.y * y_scale))).ToList(), Color.Red, Color.Blue, 0);*/
                }
            }            
            result_image.Save(path + @"fullbrushmodel_test\actual_strokes.png");

            foreach (var stroke in generator.strokes)
            {
                VoronoiVisualizer.VisualizeStrokeInline(res_bitmap, stroke.involvedSites.Select(s => (Convert.ToInt32(s.Centroid.X), Convert.ToInt32(s.Centroid.Y))).ToList(), Color.Blue, Color.Red, 1);
                VoronoiVisualizer.VisualizeStrokeInline(result_image, stroke.involvedSites.Select(s => (Convert.ToInt32(s.Centroid.X), Convert.ToInt32(s.Centroid.Y))).ToList(), Color.Blue, Color.Red, 1);
            }
            res_bitmap.Save(path + @"fullbrushmodel_test\strokes_lines.png");
            result_image.Save(path + @"fullbrushmodel_test\actual_strokes_lines.png");
        }

        public static void ClusteringTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball2.jpg");
            int sites_n = 5000;
            int n_clusters = 8;
            var optimizer = new GradDescent()
            {
                a = 0.4,
                l = 1.1,
                h = 0.1,
                max_step = 3.0,
                tol = 0.3,
                kmax = 1
            };


            var lbmp = new LabBitmap(image);
            var generator = new StrokeGenerator(lbmp, sites_n, optimizer, n_rolling_avg: 11);

            List<ColorLab> all_colors = generator.sites.Select(s => s.Centroid).Select(c => new ColorLab(generator.image.GetPixel(Convert.ToInt32(c.X), Convert.ToInt32(c.Y)).L, 0, 0)).ToList();
;
            List<ColorLab> clusters = KMeansClustering.FindClusters(all_colors, n_clusters);

            var palette = new Palette();
            palette.Colors = clusters;
            var result = palette.Apply(new LabBitmap(image)).ToBitmap();
            result.Save(path + "paletted.png");
        }
    }
}
