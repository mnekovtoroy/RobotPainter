using RobotPainter.Calculations.Brushes;
using RobotPainter.Communications;

namespace RobotPainter.Application
{
    public class BrushPainter : IPainter
    {
        private IBrushModel _brushModel;

        public Bitmap Canvas { get; set; }

        public int XScale { get; set; }
        public int YScale { get; set; }

        public BrushPainter(Bitmap canvas, int x_scale, int y_scale, IBrushModel brushModel)
        {
            _brushModel = brushModel;
            Canvas = canvas;
            XScale = x_scale;
            YScale = y_scale;
        }


        public async Task ApplyStrokes(List<BrushstrokeInfo> strokes)
        {
            await Task.Run(() =>
            {
                using (var g = Graphics.FromImage(Canvas))
                {
                    for (int i = 0; i < strokes.Count; i++)
                    {
                        var br = new SolidBrush(strokes[i].Color.ToRgb());
                        _brushModel.DrawStroke(g, br, strokes[i].RootPath, XScale, YScale);
                    }
                }
            });
        }

        public async Task<Bitmap> GetFeedback()
        {
            return Canvas;
        }
    }
}
