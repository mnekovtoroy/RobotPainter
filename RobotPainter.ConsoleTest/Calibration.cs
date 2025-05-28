using RobotPainter.Application;
using RobotPainter.Application.PhotoTransforming;
using RobotPainter.Calculations.Brushes;
using RobotPainter.Communications;
using RobotPainter.Communications.Converting;
using RobotPainter.Core;
using System.CodeDom;
using System.Drawing;

namespace RobotPainter.ConsoleTest
{
    public class Calibration
    {
        private static readonly string result_path = @"C:\Users\User\source\repos\RobotPainter\RobotPainter.ConsoleTest\test_images\calibration\";
        private static readonly string temp_path = @"C:\Users\User\source\repos\RobotPainter\Temp";
        private static readonly string photo_path = @"C:\Users\User\source\repos\RobotPainter\Photos";

        private IBrushModel _brush = new BasicBrushModel();
        private readonly IPhotoTransformer _transformer = new PhotoTransformer();
        private BrushPainter modelPainter = new BrushPainter();
        private IPainter _painter;

        private readonly static int width = 400;
        private readonly static int height = 300;

        private Calibration()
        {
            var blank_canvas = new Bitmap(1, 1);
            blank_canvas.SetPixel(0, 0, Color.White);
            var canvas = new Bitmap(blank_canvas, new Size(Convert.ToInt32(width), Convert.ToInt32(height)));
            modelPainter.InitializePainter(canvas, 1.0, 1.0, _brush);
            blank_canvas.Dispose();
        }

        public static async Task<Calibration> Create()
        {
            Calibration res = new Calibration();

            res._painter = await RobotController.Create(new StrokeOnlyPlt(), temp_path, photo_path, width, height);

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
            PointD p0 = new PointD(0, 0);
            PointD v = new PointD(0, 0);

            string image_path = result_path + @"straight_lines\";

            List<StrokePoint> stroke_point_path = new()
            {
                new StrokePoint(0, 0, 0),
                new StrokePoint(0, 12, 3),
                new StrokePoint(0, 20, 7),
                new StrokePoint(0, 32, 3),
                new StrokePoint(0, 44, 0),
            };
            var stroke_path = stroke_point_path.Select(p => new Point3D(p.x, p.y, _brush.CalculateZCoordinate(p.w))).ToList();
            List<Point3D> measuring_point = new()
            {
                new Point3D(5, 0, 10),
                new Point3D(5, 0, -3),
                new Point3D(5, 0, 10),
            };
            var path = measuring_point.Concat(stroke_path).ToList();

            List<BrushstrokeInfo> res = [new BrushstrokeInfo() { RootPath = path, Color = new ColorLab(0, 0, 0) }];




            await modelPainter.ApplyStrokes(res);
            Bitmap model_image = await modelPainter.GetFeedback();
            model_image.Save(result_path + "model.png");

            await _painter.ApplyStrokes(res);
            Bitmap real_image = _transformer.Transform(await _painter.GetFeedback(), width, height);
            model_image.Save(result_path + "real.png");
        }

        public static void CurvedLines()
        {

        }
    }
}
