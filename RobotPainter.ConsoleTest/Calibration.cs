using RobotPainter.Application;
using RobotPainter.Application.PhotoTransforming;
using RobotPainter.Calculations.Brushes;
using RobotPainter.Calculations.StrokeGeneration;
using RobotPainter.Communications;
using RobotPainter.Communications.Converting;
using RobotPainter.Core;
using System.Drawing;

namespace RobotPainter.ConsoleTest
{
    public class Calibration
    {
        private static readonly string result_path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\calibration\";
        private static readonly string temp_path = @"C:\Users\User\source\repos\RobotPainter\Temp";
        private static readonly string photo_path = @"C:\Users\User\source\repos\RobotPainter\Photos";

        private readonly static IBrushModel _brush = new BasicBrushModel();
        private readonly IPhotoTransformer _transformer = new PhotoTransformer();
        //private readonly IPhotoTransformer _transformer = new DummyTransformer();
        private BrushPainter modelPainter = new BrushPainter();
        private IPainter _painter;

        private readonly static int width = 1200;
        private readonly static int height = 900;

        private readonly static double canvas_width = 400.0;
        private readonly static double canvas_height = 300.0;

        private Calibration()
        {
            var canvas = new Bitmap(width, height);
            using(var g =  Graphics.FromImage(canvas))
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, width, height));
            modelPainter.InitializePainter(canvas, width / canvas_width, height / canvas_height, _brush);
        }

        public static async Task<Calibration> Create()
        {
            Calibration res = new Calibration();

            //real
            res._painter = await RobotController.Create(new StrokeOnlyPlt(), temp_path, photo_path, canvas_width, canvas_height);

            //test
            /*var test_painter = new BrushPainter();
            var canvas = new Bitmap(width, height);
            using (var g = Graphics.FromImage(canvas))
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, width, height));
            test_painter.InitializePainter(canvas, width / canvas_width, height / canvas_height, _brush);
            res._painter = test_painter;*/

            return res;
        }

        private struct StrokePoint
        {
            public double x; 
            public double y;
            public double w;

            public StrokePoint(double x, double y, double w)
            {
                this.x = x;
                this.y = y;
                this.w = w;
            }
        }

        public async Task StraightLinesAsync()
        {
            PointD p0 = new PointD(220.0, 20.0);
            PointD v = new PointD(20.0, 0.0);
            int n_strokes = 1;

            string image_path = result_path + @"straight_lines\";

            List<StrokePoint> stroke_point_path = new()
            {
                //assume start in (0,0,0)
                new StrokePoint(0, 6, 5),
                new StrokePoint(0, 12, 5),
            };
            var stroke_path = stroke_point_path.Select(p => new Point3D(p.x, p.y, _brush.CalculateZCoordinate(p.w))).ToList();
            stroke_path.Insert(0, new Point3D(0, 0, 0));
            stroke_path.Add(new Point3D(0, 18, 0));
            BrushstrokeBuilder.AddRunaways(stroke_path, new BrushstrokeBuilder.Options());
            var root_path = _brush.CalculateBrushRootPath(stroke_path);
            List<Point3D> measuring_point = new()
            {
                new Point3D(8, 0, 10),
                new Point3D(8, 0, -1),
                new Point3D(8, 0, 10),
            };
            var path = measuring_point.Concat(root_path).ToList();

            List<BrushstrokeInfo> res = new();

            for(int i = 0; i < n_strokes; i++)
            {
                res.Add(new BrushstrokeInfo()
                {
                    Color = new ColorLab(0, 0, 0),
                    RootPath = path.Select(p => new Point3D(p0.x + p.x + i * v.x, p0.y + p.y + i * v.y, p.z)).ToList()
                });
            }

            await modelPainter.ApplyStrokes(res);
            Bitmap model_image = await modelPainter.GetFeedback();
            model_image.Save(image_path + "model.png");

            await _painter.ApplyStrokes(res);
            Bitmap real_image = _transformer.Transform(await _painter.GetFeedback(), width, height);
            real_image.Save(image_path + "real.png");
        }

        public async Task CurvedLinesAsync()
        {
            PointD p0 = new PointD(220.0, 50.0);
            PointD v = new PointD(20.0, 0.0);
            int n_strokes = 1;

            string image_path = result_path + @"curved_lines\";

            List<StrokePoint> stroke_point_path = new()
            {
                //assume start in (0,0,0)
                new StrokePoint(0, 4, 5),
                new StrokePoint(3, 9, 5),
                new StrokePoint(7, 9, 5),
                //new StrokePoint(10, 9, 5),
            };
            var stroke_path = stroke_point_path.Select(p => new Point3D(p.x, p.y, _brush.CalculateZCoordinate(p.w))).ToList();
            stroke_path.Insert(0, new Point3D(0, 0, 0));
            stroke_path.Add(new Point3D(9, 9, 0));
            BrushstrokeBuilder.AddRunaways(stroke_path, new BrushstrokeBuilder.Options());
            var root_path = _brush.CalculateBrushRootPath(stroke_path);
            List<Point3D> measuring_point = new()
            {
                new Point3D(8, 0, 10),
                new Point3D(8, 0, -1),
                new Point3D(8, 0, 10),
            };
            var path = measuring_point.Concat(root_path).ToList();

            List<BrushstrokeInfo> res = new();

            for (int i = 0; i < n_strokes; i++)
            {
                res.Add(new BrushstrokeInfo()
                {
                    Color = new ColorLab(0, 0, 0),
                    RootPath = path.Select(p => new Point3D(p0.x + p.x + i * v.x, p0.y + p.y + i * v.y, p.z)).ToList()
                });
            }

            await modelPainter.ApplyStrokes(res);
            Bitmap model_image = await modelPainter.GetFeedback();
            model_image.Save(image_path + "model.png");

            await _painter.ApplyStrokes(res);
            Bitmap real_image = _transformer.Transform(await _painter.GetFeedback(), width, height);
            real_image.Save(image_path + "real.png");
        }
    }
}
