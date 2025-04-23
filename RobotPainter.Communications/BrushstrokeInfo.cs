using RobotPainter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPainter.Communications
{
    public class BrushstrokeInfo
    {
        public List<Point3D> RootPath { get; set; }

        public ColorLab Color { get; set; }
    }
}
