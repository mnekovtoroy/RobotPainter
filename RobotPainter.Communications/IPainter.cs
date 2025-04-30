using System.Drawing;

namespace RobotPainter.Communications
{
    public interface IPainter
    {
        public Task ApplyStrokes(List<BrushstrokeInfo> strokes);

        public Task<Bitmap> GetFeedback();
    }
}