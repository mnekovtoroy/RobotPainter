using RobotPainter.Calculations.StrokeGeneration;
using RobotPainter.Communications;

namespace RobotPainter.Application
{
    public static class Mapper
    {
        public static BrushstrokeInfo Map(Brushstroke brushstroke) =>
            new BrushstrokeInfo()
            {
                Color = brushstroke.Color,
                RootPath = brushstroke.RootPath
            };
    }
}
