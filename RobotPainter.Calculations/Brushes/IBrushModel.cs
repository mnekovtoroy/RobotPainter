using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPainter.Calculations.Brushes
{
    public interface IBrushModel
    {
        public readonly static string model_name;

        public double CalculateZCoordinate(double w);

        public List<Point3D> CalculateBrushRootPath(List<Point3D> desired_path);
    }
}
