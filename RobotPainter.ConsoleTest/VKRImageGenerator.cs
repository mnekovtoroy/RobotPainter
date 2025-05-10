using RobotPainter.Calculations;
using RobotPainter.Calculations.Brushes;
using RobotPainter.Calculations.Clustering;
using RobotPainter.Calculations.ImageProcessing;
using RobotPainter.Calculations.StrokeGeneration;
using RobotPainter.Core;
using RobotPainter.Visualization;
using SharpVoronoiLib;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Drawing;
using System.Numerics;

namespace RobotPainter.ConsoleTest
{
    public class VKRImageGenerator
    {
        private string orig_path_ball = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\";
        private string dest_path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\vkr_images\";

        private Bitmap orig_ball;
        private LabBitmap lab_ball;

        private readonly int width;
        private readonly int height;

        private readonly double single_canvas_width = 100.0;
        private readonly double single_canvas_height = 100.0;

        private readonly double all_canvas_width = 200.0;
        private readonly double all_canvas_height = 200.0;

        public VKRImageGenerator()
        {
            orig_ball = new Bitmap(orig_path_ball + "test_ball2.jpg");

            lab_ball = new LabBitmap(orig_ball);

            width = orig_ball.Width;
            height = orig_ball.Height;

        }

        public void GenerateImages()
        {
            //Image_1_1();
            //Image_1_2();
            Image_1_3();
            //Image_1_4();
            Image_1_5();
            //Image_1_6();
            //Image_1_7();
            //Image_1_8();
        }

        //for 1.3 and 1.5
        double margin = 5;
        private StrokeSites strokeSites;
        private Brushstroke stroke;
        public void InitSingleStroke()
        {
            var bsb_opt = new BrushstrokeBuilder.Options()
            {
                xResizeCoeff = single_canvas_width / width,
                yResizeCoeff = single_canvas_height / height,
                //Overlap = 0.2
            };
            int voronoi_n = StrokeGenerator.CalculateDesiredVoronoiN(single_canvas_width, single_canvas_width, bsb_opt.MaxWidth, bsb_opt.Overlap);
            var generator = new StrokeGenerator(lab_ball, voronoi_n, new bool[width, height], new double[width, height], new StrokeGenerator.Options());
            strokeSites = generator.GetNextStrokeSites(new StrokeSitesBuilder.Options() { L_tol = 1000.0, CanvasMaxStrokeLength = 1000.0 });
            stroke = BrushstrokeBuilder.GenerateBrushstroke(strokeSites, bsb_opt);
        }

        //for 1.4 and 1.6 and 1.7
        private StrokeGenerator allGenerator;
        private List<StrokeSites> allStrokeSites;
        private List<Brushstroke> allStrokes;
        public void InitAllStrokes()
        {
            var bsb_opt = new BrushstrokeBuilder.Options()
            {
                xResizeCoeff = all_canvas_width / width,
                yResizeCoeff = all_canvas_height / height
            };

            int voronoi_n = StrokeGenerator.CalculateDesiredVoronoiN(all_canvas_width, all_canvas_height, bsb_opt.MaxWidth, bsb_opt.Overlap);
            allGenerator = new StrokeGenerator(lab_ball, voronoi_n, new bool[width, height], new double[width, height], new StrokeGenerator.Options()
            {
                RelaxationIterations = 5,
                LpullIterations = 5
            });

            allStrokeSites = allGenerator.CalculateAllStorkes(new StrokeSitesBuilder.Options());
            allStrokes = allStrokeSites.Select(ss => BrushstrokeBuilder.GenerateBrushstroke(ss, bsb_opt)).ToList();
        }

        public void Image_1_1()
        {
            //vector field visualization
            int n_avg = 12;
            int n_arrows = 40;
            double arrow_length_coeff = 0.5;
            Color c = Color.Blue;

            double[,] u, v;
            (u, v) = ImageProcessor.LNormWithRollAvg(lab_ball, n_avg);

            //visualise
            var vector_field = new Bitmap(orig_ball);
            using (var g = Graphics.FromImage(vector_field))
            {
                Pen p = new Pen(c);
                g.DrawVectorField(p, width, height, u, v, n_arrows, arrow_length_coeff);
            }

            orig_ball.Save(dest_path + "image_1_1_a.png");
            vector_field.Save(dest_path + "image_1_1_b.png");
            vector_field.Dispose();
        }

        public void Image_1_2()
        {
            int n_voronoi = 3000;
            int relax_iter = 5;
            int lfit_iter = 5;
            double lfit_step = 2.0;
            Bitmap random_sites = new Bitmap(orig_ball);
            Bitmap relaxed_sites = new Bitmap(orig_ball);
            Bitmap lfit_sites = new Bitmap(orig_ball);
            Pen p = new Pen(Color.Blue);

            var generator = new StrokeGenerator(lab_ball, n_voronoi, new bool[lab_ball.Width, lab_ball.Height], new double[lab_ball.Width, lab_ball.Height], new StrokeGenerator.Options() { LpullIterations = 0, RelaxationIterations = 0 });
            using (var g = Graphics.FromImage(random_sites))
            {
                g.DrawVoronoi(p, generator.sites);
            }

            var plane = new VoronoiPlane(0, 0, lab_ball.Width - 1, lab_ball.Height - 1);
            plane.SetSites(generator.sites.Select(s => new VoronoiSite(s.X, s.Y)).ToList());
            plane.Tessellate();
            plane.Relax(relax_iter);
            generator.sites = plane.Sites;
            using (var g = Graphics.FromImage(relaxed_sites))
            {
                g.DrawVoronoi(p, generator.sites);
            }

            generator.Lfit(lfit_iter, lfit_step);
            using (var g = Graphics.FromImage(lfit_sites))
            {
                g.DrawVoronoi(p, generator.sites);
            }

            random_sites.Save(dest_path + "image_1_2_a.png");
            relaxed_sites.Save(dest_path + "image_1_2_b.png");
            lfit_sites.Save(dest_path + "image_1_2_c.png");

            random_sites.Dispose();
            relaxed_sites.Dispose();
            lfit_sites.Dispose();
        }

        public void Image_1_3()
        {
            if (strokeSites == null || stroke == null)
                InitSingleStroke();

            Bitmap strokesites_image = new Bitmap(width, height);
            Pen p = new Pen(Color.Blue);
            Brush b = new SolidBrush(Color.LightGray);
            Brush b_p = new SolidBrush(Color.Blue);
            using (var g = Graphics.FromImage(strokesites_image))
            {
                g.FillVoronoi([b], strokeSites.involvedSites);
                g.DrawVoronoi(p, strokeSites.involvedSites);
                //g.DrawPoints(b_p, strokeSites.involvedSites.Select(s => new PointD(s.Centroid.X, s.Centroid.Y)).ToList(), 1);
            }

            double min_x = strokeSites.involvedSites.Select(s => s.Points.Min(p => p.X)).Min() - margin;
            double min_y = strokeSites.involvedSites.Select(s => s.Points.Min(p => p.Y)).Min() - margin;
            double max_x = strokeSites.involvedSites.Select(s => s.Points.Max(p => p.X)).Max() + margin;
            double max_y = strokeSites.involvedSites.Select(s => s.Points.Max(p => p.Y)).Max() + margin;
            Point upper_left = new Point(Convert.ToInt32(min_x), Convert.ToInt32(min_y));
            Size size = new Size(Convert.ToInt32(max_x - min_x), Convert.ToInt32(max_y - min_y));

            var cropped = strokesites_image.Clone(new Rectangle(upper_left, size), strokesites_image.PixelFormat);
            cropped.Save(dest_path + "image_1_3.png");

            strokesites_image.Dispose();
            cropped.Dispose();
        }

        public void Image_1_4()
        {
            if (allStrokeSites == null || allStrokes == null || allGenerator == null)
                InitAllStrokes();

            Bitmap stroke_sites_fill = new Bitmap(width, height);

            using (var g = Graphics.FromImage(stroke_sites_fill))
            {
                foreach (var ss in allStrokeSites)
                {
                    g.FillVoronoi([new SolidBrush(ss.MainColor.ToRgb())], ss.involvedSites);
                }
            }

            Bitmap stroke_sites_lines = new Bitmap(stroke_sites_fill);
            Pen p = new Pen(Color.Blue);
            Brush b = new SolidBrush(Color.Red);
            using (var g = Graphics.FromImage(stroke_sites_lines))
            {
                foreach (var ss in allStrokeSites)
                {
                    var points = ss.involvedSites.Select(s => new PointD(s.Centroid.X, s.Centroid.Y)).ToList();
                    g.DrawLines(p, points);
                    g.DrawPoints(b, points, 1);
                }
            }

            stroke_sites_fill.Save(dest_path + "image_1_4_a.png");
            stroke_sites_lines.Save(dest_path + "image_1_4_b.png");

            stroke_sites_fill.Dispose();
            stroke_sites_lines.Dispose();
        }

        public void Image_1_5()
        {
            if (strokeSites == null || stroke == null)
                InitSingleStroke();

            Bitmap strokesites_image = new Bitmap(width, height);
            Pen p_voronoi = new Pen(Color.Blue);
            Pen p_desired = new Pen(Color.LightBlue);
            Pen p_root = new Pen(Color.Green);
            Pen p_skeleton = new Pen(Color.Red);
            Brush b = new SolidBrush(Color.Black);
            double x_scale = width / single_canvas_width;
            double y_scale = height / single_canvas_height;
            //Brush b_p = new SolidBrush(Color.Blue);
            using (var g = Graphics.FromImage(strokesites_image))
            {
                stroke.brushModel.DrawStroke(g, b, stroke.RootPath, x_scale, y_scale);
                g.DrawVoronoi(p_voronoi, strokeSites.involvedSites);
                //g.DrawLines(p_desired, stroke.DesiredPath.Select(p => new PointD(p.x * x_scale, p.y * y_scale)).ToList());
                g.DrawLines(p_root, stroke.RootPath.Select(p => new PointD(p.x * x_scale, p.y * y_scale)).ToList());
                var skeleton = ((BasicBrushModel)stroke.brushModel).CalculateStrokeSkeleton(stroke.RootPath);
                g.DrawLines(p_skeleton, skeleton.points.Select(p => new PointD(p.x * x_scale, p.y * y_scale)).ToList());
            }

            double min_x = stroke.RootPath.Min(p => p.x) * x_scale - margin;
            double min_y = stroke.RootPath.Min(p => p.y) * y_scale - margin;
            double max_x = stroke.RootPath.Max(p => p.x) * x_scale + margin;
            double max_y = stroke.RootPath.Max(p => p.y) * y_scale + margin;
            Point upper_left = new Point(Convert.ToInt32(min_x), Convert.ToInt32(min_y));
            Size size = new Size(Convert.ToInt32(max_x - min_x), Convert.ToInt32(max_y - min_y));

            var cropped = strokesites_image.Clone(new Rectangle(upper_left, size), strokesites_image.PixelFormat);
            cropped.Save(dest_path + "image_1_5.png");

            strokesites_image.Dispose();
            cropped.Dispose();
        }

        public void Image_1_6()
        {
            if (allStrokeSites == null || allStrokes == null || allGenerator == null)
                InitAllStrokes();

            var result = new Bitmap(width, height);
            double x_scale = width / all_canvas_width;
            double y_scale = height / all_canvas_height;
            using (var g = Graphics.FromImage(result))
            {
                foreach (var stroke in allStrokes)
                {
                    stroke.brushModel.DrawStroke(g, new SolidBrush(stroke.Color.ToRgb()), stroke.RootPath, x_scale, y_scale);
                }
            }
            result.Save(dest_path + "image_1_6.png");
            result.Dispose();
        }

        public void Image_1_7()
        {
            if (allStrokeSites == null || allStrokes == null || allGenerator == null)
                InitAllStrokes();

            var clusterer = new KMeansClustering();
            var colors = clusterer.FindClusters(allGenerator.sites.Select(s =>
            {
                var centroid = s.Centroid;
                var color = lab_ball.GetPixel(Convert.ToInt32(centroid.X), Convert.ToInt32(centroid.Y));
                color.a = 0;
                color.b = 0;
                return color;
            }).ToList(), 20);
            var palette = new Palette() { Colors = colors };

            var result = new Bitmap(width, height);
            double x_scale = width / all_canvas_width;
            double y_scale = height / all_canvas_height;
            using (var g = Graphics.FromImage(result))
            {
                foreach (var stroke in allStrokes)
                {
                    stroke.brushModel.DrawStroke(g, new SolidBrush(palette.Apply(stroke.Color).ToRgb()), stroke.RootPath, x_scale, y_scale);
                }
            }
            result.Save(dest_path + "image_1_7.png");
            result.Dispose();
        }

        public void Image_1_8()
        {
            string[] layer_names = ["a", "b", "c"];
            double[] max_width = [7.0, 5.0, 3.0];
            double[] overlap = [1.5, 1.0, 0.5];
            double[] error_tol = [2.0, 5.0, 10.0];
            double canvas_width = 300.0;
            double canvas_height = 300.0;
            var brush = new BasicBrushModel();

            var robot_painter = new RobotPainterCalculator(orig_ball, canvas_width, canvas_height);
            robot_painter.AllLayersOptions = new List<RobotPainterCalculator.LayerOptions>();

            for (int i = 0; i < max_width.Length; i++)
            {
                var layer_options = RobotPainterCalculator.CreateLayerOptions();

                layer_options.MaxWidth = max_width[i];
                layer_options.Overlap = overlap[i];
                layer_options.ErrorTolerance = error_tol[i];
                layer_options.RollingAverageN = 13;

                robot_painter.AllLayersOptions.Add(layer_options);
            }

            Bitmap result = new Bitmap(width, height);

            robot_painter.SetInitialCanvas(result);

            for (int i = 0; i < robot_painter.AllLayersOptions.Count; i++)
            {
                robot_painter.InitializeStrokeGenerator();

                var strokes = robot_painter.GetAllBrushstrokes();
                Console.WriteLine($"Number of strokes: {strokes.Count}");


                using (var g = Graphics.FromImage(result))
                {
                    for (int j = 0; j < strokes.Count; j++)
                    {
                        brush.DrawStroke(g, new SolidBrush(strokes[j].Color.ToRgb()), strokes[j].RootPath, width / canvas_width, height / canvas_height);
                    }
                }

                var strokes_visuals = new Bitmap(result);
                for (int j = 0; j < strokes.Count; j++)
                {
                    var stroke_path = strokes[j].DesiredPath.Select(p => (Convert.ToInt32(p.x * width / canvas_width), Convert.ToInt32(p.y * height / canvas_height))).ToList();
                    VoronoiVisualizer.VisualizeStrokeInline(strokes_visuals, stroke_path, Color.Blue, Color.Red, 1);
                }
                strokes_visuals.Save(dest_path + @$"image_1_8_{layer_names[i]}_1.png");

                robot_painter.ApplyFeedback(result);
                robot_painter.AdvanceLayer();
                result.Save(dest_path + @$"image_1_8_{layer_names[i]}_2.png");
            }
        }
    }
}
