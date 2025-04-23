using RobotPainter.Core;

namespace RobotPainter.Communications.PltCommands
{
    public class BrushstrokeCommand : IPltCommand
    {
        public List<Point3D> points;

        public string ToPlt()
        {
            throw new NotImplementedException();
        }
    }
}
