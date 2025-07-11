﻿using RobotPainter.Core;
using System.Drawing;

namespace RobotPainter.Calculations.Brushes
{
    public interface IBrushModel
    {
        public double CalculateZCoordinate(double w);

        public List<Point3D> CalculateBrushRootPath(List<Point3D> desired_path);

        public void DrawStroke(Graphics g, Brush br, List<Point3D> root_path, double x_scale_coeff, double y_scale_coeff, double mult_coeff = 100);
    }
}
