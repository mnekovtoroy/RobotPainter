using RobotPainter.Calculations.Brushes;

namespace RobotPainter.Calculations.Core
{
    public class Brushstroke
    {
        public IBrushModel brushModel { get; set; }

        public List<Point3D> RootPath { get; set; }

        public List<Point3D> DesiredPath { get; set; }

        public ColorLab Color { get; set; }
    }
}
