using System.Drawing;
using RobotPainter.Calculations;
using RobotPainter.Calculations.Brushes;
using RobotPainter.Calculations.Clustering;
using RobotPainter.Core;
using RobotPainter.Calculations.ImageProcessing;
using RobotPainter.Calculations.Optimization;
using RobotPainter.Calculations.StrokeGeneration;
using RobotPainter.Visualization;
using RobotPainter.Communications;
using RobotPainter.Communications.Converting;
using RobotPainter.Application.PhotoTransforming;

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

            var sites = StrokeGenerator.GenerateRandomRelaxedMesh(200, image.Width, image.Height);

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

            IOptimizer optimizer = new GradDescent(new GradDescent.Options()
            {
                kmax = kmax,
                max_step = max_step,
                tol = tol,
                a = a,
                l = l,
                h = h
            });

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

            var voronoi = new StrokeGenerator(lbmp, 200, new bool[image.Width, image.Height], new double[image.Width, image.Height], new StrokeGenerator.Options());
            DateTime start = DateTime.Now;
            int[,] mask = voronoi.GetVoronoiMask();
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

            var voronoi = new StrokeGenerator(lbmp, sites_n, new bool[image.Width, image.Height], new double[image.Width, image.Height], new StrokeGenerator.Options());

            int[] check_intervals = new int[] { 1, 2, 5};

            var pre = lbmp.ToBitmap();
            VoronoiVisualizer.VisualizeVoronoiInline(pre, voronoi.sites, Color.Blue, Color.Blue, 0);
            VoronoiVisualizer.VisualisePointsInline(pre, voronoi.sites.Select(s => (s.X, s.Y)).ToList(), Color.Red, 1);
            pre.Save(path + $"test_Lpull_{sites_n}_0.png");
            pre.Dispose();
            for (int i = 0; i < check_intervals.Max(); i++)
            {
                voronoi.Lfit(1, 2.0);
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

        public static void BrushstrokeGenerationTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball2.jpg");
            double canvas_width = 30.0;
            double canvas_height = 30.0;
            int sites_n = 10000;

            LabBitmap lbmp = new LabBitmap(image);

            var generator = new StrokeGenerator(lbmp, sites_n, new bool[image.Width, image.Height], new double[image.Width, image.Height], new StrokeGenerator.Options());

            generator.Lfit(20, 2.0);

            generator.CalculateAllStorkes(new StrokeSitesBuilder.Options()
            {
                xResizeCoeff = canvas_width / image.Width,
                yResizeCoeff = canvas_height / image.Height
            });
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
            double canvas_width = 150;
            double canvas_height = 150;

            var ssb_opt = new StrokeSitesBuilder.Options()
            {
                xResizeCoeff = canvas_width / image.Width,
                yResizeCoeff = canvas_height / image.Height
            };
            var bbs_opt = new BrushstrokeBuilder.Options()
            {
                xResizeCoeff = canvas_width / image.Width,
                yResizeCoeff = canvas_height / image.Height
            };
            var sg_opt = new StrokeGenerator.Options();

            int sites_n = StrokeGenerator.CalculateDesiredVoronoiN(canvas_width, canvas_height, bbs_opt.MaxWidth, bbs_opt.Overlap);
            LabBitmap lbmp = new LabBitmap(image);

            var brushModel = new BasicBrushModel();
            var generator = new StrokeGenerator(lbmp, sites_n, new bool[image.Width, image.Height], new double[image.Width, image.Height], sg_opt);

            generator.CalculateAllStorkes(ssb_opt);
            var stroke_visualised = generator.GetColoredStrokeMap();

            VoronoiVisualizer.VisualizeVoronoiInline(image, generator.sites, Color.Blue, Color.Blue, 0);
            image.Save(path + @"brushmodel_test\voronoi.png");

            var res_bitmap = stroke_visualised.ToBitmap();
            res_bitmap.Save(path + @"brushmodel_test\strokes.png");

            Bitmap result_image = new Bitmap(image.Width, image.Height);
            result_image.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            int n_brushstrokes_to_draw = 1;

            for(int i = 0; i < n_brushstrokes_to_draw; i++)
            {
                var stroke = generator.strokes[i];
                var brushstroke = BrushstrokeBuilder.GenerateBrushstroke(stroke, bbs_opt);

                using(var g = Graphics.FromImage(result_image))
                {
                    var br = new SolidBrush(stroke.MainColor.ToRgb());
                    brushModel.DrawStroke(g, br, brushstroke.RootPath, image.Width / canvas_width, image.Height / canvas_height, mult_coeff: 100);
                }

                var stroke_skeleton = brushModel.CalculateStrokeSkeleton(brushstroke.RootPath);

                double x_scale = image.Width / canvas_width;
                double y_scale = image.Height / canvas_height;

                var test = stroke_skeleton.points.Select(p => p.z).ToList();
                VoronoiVisualizer.VisualizeVoronoiInline(result_image, stroke.involvedSites, Color.Blue, Color.Red, 1);
                VoronoiVisualizer.VisualizeStrokeInline(result_image, brushstroke.DesiredPath.Select(p => (Convert.ToInt32(p.x * x_scale), Convert.ToInt32(p.y * y_scale))).ToList(), Color.Green, Color.Orange, 1);
                //VoronoiVisualizer.VisualizeStrokeInline(result_image, stroke_skeleton.points.Select(p => (Convert.ToInt32(p.x * x_scale), Convert.ToInt32(p.y * y_scale))).ToList(), Color.Red, Color.Blue, 0);
                VoronoiVisualizer.VisualisePointsInline(result_image, [(brushstroke.DesiredPath[0].x * x_scale, brushstroke.DesiredPath[0].y * y_scale)], Color.Pink, 1);
            }
            result_image.Save(path + @"brushmodel_test\actual_strokes.png");
        }

        public static void FullBrushModelTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball2.jpg");
            //int sites_n = 15000;
            double canvas_width = 300;
            double canvas_height = 300;
            var brush = new BasicBrushModel();

            var robot_painter = new RobotPainterCalculator(image, canvas_width, canvas_height);
            robot_painter.AllLayersOptions = [RobotPainterCalculator.CreateLayerOptions()];
            //robot_painter.AllLayersOptions[0].NVoronoi = sites_n;
            robot_painter.InitializeStrokeGenerator();

            var strokes = robot_painter.GetAllBrushstrokes();
            Console.WriteLine($"Number of strokes: {strokes.Count}");
            Bitmap result = new Bitmap(image.Width, image.Height);
            using(var g =  Graphics.FromImage(result))
            {
                for (int i = 0; i < strokes.Count; i++)
                {
                    brush.DrawStroke(g, new SolidBrush(strokes[i].Color.ToRgb()), strokes[i].RootPath, image.Width / canvas_width, image.Height / canvas_height);
                }
            }
            result.Save(path + @"fullbrushmodel_test\actual_strokes.png");
        }

        public static void ClusteringTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball2.jpg");
            int sites_n = 5000;
            int n_clusters = 8;

            var lbmp = new LabBitmap(image);
            var generator = new StrokeGenerator(lbmp, sites_n, new bool[image.Width, image.Height], new double[image.Width, image.Height], new StrokeGenerator.Options());

            List<ColorLab> all_colors = generator.sites.Select(s => s.Centroid).Select(c => new ColorLab(generator.image.GetPixel(Convert.ToInt32(c.X), Convert.ToInt32(c.Y)).L, 0, 0)).ToList();
;
            var clusterer = new KMeansClustering();
            List<ColorLab> clusters = clusterer.FindClusters(all_colors, n_clusters);

            var palette = new Palette();
            palette.Colors = clusters;
            var result = palette.Apply(new LabBitmap(image)).ToBitmap();
            result.Save(path + "paletted.png");
        }

        public static void ToPltTest()
        {
            var path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_plt\";

            //test_calligraphy_sedov.m --"t"
            var root_path = new List<Point3D> {
                new Point3D(55.5483330293793, 32.8239968217536, 2.25000000000000),
                new Point3D(51.9312381882220, 28.4611092298424, -0.155204257767549),
                new Point3D(44.4715413462125, 19.4633305668608, -0.0264430111751386),
                new Point3D(39.0385903206145, 14.6406734909229, -0.0162715460735159),
                new Point3D(32.2177259241430, 10.2712769560302, -0.208608272197352),
                new Point3D(27.7591012124281, 10.5706525800299, -0.549093190163909),
                new Point3D(21.6248995867494, 18.7038029106224, -3.66149473090046),
                new Point3D(31.6302034596677, 26.7376541346915, -4.07761195122949),
                new Point3D(37.4663948369331, 32.8054355420039, -4.26132235156708),
                new Point3D(39.8531305999195, 40.5486397480948, -4.31891438445548),
                new Point3D(44.7707287653652, 44.2888645387676, -4.36614738202227),
                new Point3D(43.7607136814139, 45.4291249989077, 2.25000000000000)
            };

            IColorToCoordConverter color2coord= new ManualColorToCoord(new List<ColorLab> { new ColorLab() }, new PointD(0,0), 3, 3, 10, 2);
            IPltConverter pltConverter = new PltConverter(color2coord);

            var brushstrokeInfo = new BrushstrokeInfo()
            {
                Color = new ColorLab(),
                RootPath = root_path
            };

            var commands = pltConverter.ToPlt(new List<BrushstrokeInfo> { brushstrokeInfo });

            pltConverter.SavePlt(commands, path + "test.plt");
        }

        public static async Task CommunicationsTest()
        {
            var path_plt = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_plt\";
            var path_img = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_photo\";
            var path_bmp = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_bmp\";

            //test_calligraphy_sedov.m --"t"
            /*var root_path = new List<Point3D> {
                new Point3D(55.5483330293793, 32.8239968217536, 2.25000000000000),
                new Point3D(51.9312381882220, 28.4611092298424, -0.155204257767549),
                new Point3D(44.4715413462125, 19.4633305668608, -0.0264430111751386),
                new Point3D(39.0385903206145, 14.6406734909229, -0.0162715460735159),
                new Point3D(32.2177259241430, 10.2712769560302, -0.208608272197352),
                new Point3D(27.7591012124281, 10.5706525800299, -0.549093190163909),
                new Point3D(21.6248995867494, 18.7038029106224, -3.66149473090046),
                new Point3D(31.6302034596677, 26.7376541346915, -4.07761195122949),
                new Point3D(37.4663948369331, 32.8054355420039, -4.26132235156708),
                new Point3D(39.8531305999195, 40.5486397480948, -4.31891438445548),
                new Point3D(44.7707287653652, 44.2888645387676, -4.36614738202227),
                new Point3D(43.7607136814139, 45.4291249989077, 2.25000000000000)
            };*/
            var root_path = new List<Point3D>
            {
                new Point3D(79.1348643424727, 69.7979796195486, 2.25000000000000),
                new Point3D(69.2162539687767, 54.7869283492907, -5.27656858094736),
                new Point3D(46.8331458347884, 20.9118217438251, -4.19301301594283),
                new Point3D(45.0087648943768, 12.4938053593536, -3.92231554424775),
                new Point3D(47.8931853077501, 7.88004688058137, -3.28521233126072),
                new Point3D(52.2728778644053, 8.92587981698648, -0.885440783855692),
                new Point3D(55.3188216484262, 9.96329089237358, -0.112914417531491),
                new Point3D(58.5221790381474, 10.9406597235817, -0.0162715460735159),
                new Point3D(64.8522550402678, 15.1513537503450, -1.21971654439470),
                new Point3D(75.9234748628144, 28.3506089620225, -4.01365872093339),
                new Point3D(75.4154467839344, 43.3083051158532, -5.15824682963130),
                new Point3D(69.0541045064654, 44.6894621276125, -3.53969184353383),
                new Point3D(71.9730808719019, 46.3033833910149, 2.25000000000000)
            };

            IColorToCoordConverter color2coord = new ManualColorToCoord(new List<ColorLab> { new ColorLab() }, new PointD(0, 0), 3, 3, 10, 2);
            IPltConverter pltConverter = new PltConverter(color2coord);

            var robot = await RobotController.Create(pltConverter, path_plt, path_img);

            var brushstrokeInfo = new BrushstrokeInfo()
            {
                Color = new ColorLab(),
                RootPath = root_path
            };

            await robot.ApplyStrokes(new List<BrushstrokeInfo>() { brushstrokeInfo });

            /*for(int i = 0; i < 3; i++)
            {
                Console.WriteLine($"taking photo {i + 1}");
                var bmp = await robot.GetFeedback();
                bmp.Save(path_bmp + $"image_{i + 1}.png");
                Thread.Sleep(1000 * 10);
            }*/
        }

        public static void VoronoiNCalculationsTest()
        {
            double canvas_width = 300;
            double canvas_height = 300;
            double max_width = 5;
            double overlap = 1;


            Console.WriteLine($"Canvas width: {canvas_width}; Canvas heiht: {canvas_height};");
            Console.WriteLine($"Stroke width: {max_width}; Overlap: {overlap};");
            int voronoiN = StrokeGenerator.CalculateDesiredVoronoiN(canvas_width, canvas_height, max_width, overlap);
            Console.WriteLine($"Calculated voronoi N: {voronoiN}");
        }

        public static void FeedbackTest()
        {
            string path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
            Bitmap image = new Bitmap(path + "test_ball2.jpg");
            double[] max_width = [7];
            double[] overlap = [1.5];
            double canvas_width = 300;
            double canvas_height = 300;
            var brush = new BasicBrushModel();

            var robot_painter = new RobotPainterCalculator(image, canvas_width, canvas_height);
            robot_painter.AllLayersOptions = new List<RobotPainterCalculator.LayerOptions>();

            for(int i = 0; i < max_width.Length; i++)
            {
                var layer_options = RobotPainterCalculator.CreateLayerOptions();

                layer_options.MaxWidth = max_width[i];
                layer_options.Overlap = overlap[i];
                //layer_options.NVoronoi = StrokeGenerator.CalculateDesiredVoronoiN(canvas_width, canvas_height, max_width[i], overlap[i]);
                layer_options.ErrorTolerance = 5.0;
                layer_options.RollingAverageN = 15;

                robot_painter.AllLayersOptions.Add(layer_options);
            }

            Bitmap result = new Bitmap(image.Width, image.Height);

            robot_painter.SetInitialCanvas(result);

            for(int i = 0; i < robot_painter.AllLayersOptions.Count; i++)
            {
                robot_painter.InitializeStrokeGenerator();

                var strokes = robot_painter.GetAllBrushstrokes();
                Console.WriteLine($"Number of strokes: {strokes.Count}");


                using (var g = Graphics.FromImage(result))
                {
                    for (int j = 0; j < strokes.Count; j++)
                    {
                        brush.DrawStroke(g, new SolidBrush(strokes[j].Color.ToRgb()), strokes[j].RootPath, image.Width / canvas_width, image.Height / canvas_height);
                    }
                }

                var strokes_visuals = new Bitmap(result);
                for(int j = 0; j < strokes.Count; j++)
                {
                    var stroke_path = strokes[j].DesiredPath.Select(p => (Convert.ToInt32(p.x * image.Width / canvas_width), Convert.ToInt32(p.y * image.Height / canvas_height))).ToList();
                    VoronoiVisualizer.VisualizeStrokeInline(strokes_visuals, stroke_path, Color.Blue, Color.Red, 1);
                }
                strokes_visuals.Save(path + @$"layer_picture_test\strokes_layer_{i}.png");

                robot_painter.ApplyFeedback(result);
                robot_painter.AdvanceLayer();
                result.Save(path + @$"layer_picture_test\layer_{i}.png");
            }
        }

        public static void WhiteBalanceTest()
        {
            var path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_bmp\";

            var img = new Bitmap(path + "image_1.png");

            Point white_point = new Point(1803, 763);
            Point black_point = new Point(3015, 15);

            Color perfect_white = img.GetPixel(black_point.X, black_point.Y);
            Color perfect_black = img.GetPixel(white_point.X, white_point.Y);

            ImageProcessor.WhiteBalance(img, perfect_white, perfect_black);

            img.Save(path + "balanced2.png");
        }

        public static void PhotoTransformingTest()
        {
            PhotoTransformer transformer = new PhotoTransformer();

            var path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_bmp\";

            var img = new Bitmap(path + "image_1.png");

            var transformed = transformer.Transform(img, 800, 600);

            transformed.Save(path + "transformed.png");
        }
    }
}
