using RobotPainter.Calculations.Brushes;
using RobotPainter.Communications;

namespace RobotPainter.Application
{
    public class BrushPainter : IPainter
    {
        private IBrushModel? _brushModel;

        public Bitmap? Canvas { get; set; }

        public double? XScale { get; set; }
        public double? YScale { get; set; }

        public BrushPainter()
        {
        }

        public void InitializePainter(Bitmap canvas, double x_scale, double y_scale, IBrushModel brushModel)
        {
            Canvas = canvas;
            XScale = x_scale;
            YScale = y_scale;
            _brushModel = brushModel;
        }

        public async Task ApplyStrokes(List<BrushstrokeInfo> strokes)
        {
            if(Canvas == null || XScale == null || YScale == null || _brushModel == null)
            {
                throw new Exception("InitializePainter first.");
            }
            await Task.Run(() =>
            {
                using (var g = Graphics.FromImage(Canvas))
                {
                    for (int i = 0; i < strokes.Count; i++)
                    {
                        var br = new SolidBrush(strokes[i].Color.ToRgb());
                        _brushModel.DrawStroke(g, br, strokes[i].RootPath, XScale.Value, YScale.Value);
                    }
                }
            });
        }

        public async Task<Bitmap> GetFeedback()
        {
            return new Bitmap(Canvas);
        }
    }
}
