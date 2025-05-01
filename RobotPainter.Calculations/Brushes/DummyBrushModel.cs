using RobotPainter.Core;
using System.Drawing;

namespace RobotPainter.Calculations.Brushes
{
    public class DummyBrushModel : IBrushModel
    {
        public List<Point3D> CalculateBrushRootPath(List<Point3D> desired_path)
        {
            throw new NotImplementedException();
        }

        public double CalculateZCoordinate(double w)
        {
            throw new NotImplementedException();
        }

        public void DrawStroke(Graphics g, Brush br, List<Point3D> root_path, double x_scale_coeff, double y_scale_coeff, double mult_coeff = 100)
        {
            throw new NotImplementedException();
        }
    }
}
