using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPainter.Calculations
{
    public static class Geometry
    {
        public static bool CheckIntersectionRaySegment(
            double v_x0, double v_y0, double v_dx, double v_dy,
            double s_x0, double s_y0, double s_x1, double s_y1)
        {
            //formulas: http://e-maxx.ru/algo/lines_intersection
            double s_dx = s_x1 - s_x0;
            double s_dy = s_y1 - s_y0;

            double a_v, b_v, c_v;
            double a_s, b_s, c_s;

            (a_v, b_v, c_v) = GetLine(new PointD(v_x0, v_y0), new PointD(v_x0 + v_dx, v_y0 + v_dy));
            (a_s, b_s, c_s) = GetLine(new PointD(s_x0, s_y0), new PointD(s_x1, s_y1));

            double den = a_v * b_s - a_s * b_v;
            if (den == 0)
            {
                //can check if they're on the same line, but i dont think its important
                return false;
            }
            //fidning intersection point
            double x_i = -(c_v * b_s - c_s * b_v) / den;
            double y_i = -(a_v * c_s - a_s * c_v) / den;

            //checking if intersection point is on the segment & the ray
            bool isOnS =
                x_i >= Math.Min(s_x0, s_x1) &&
                x_i <= Math.Max(s_x0, s_x1) &&
                y_i >= Math.Min(s_y0, s_y1) &&
                y_i <= Math.Max(s_y0, s_y1);
            bool isOnV = (v_dx * (x_i - v_x0) + v_dy * (y_i - v_y0)) > 0;
            return isOnS && isOnV;
        }

        public static PointD GetIntersectionPoint(PointD lx0, PointD lx2, PointD mx1, PointD mx2)
        {
            double a_v, b_v, c_v;
            double a_s, b_s, c_s;

            (a_v, b_v, c_v) = GetLine(lx0, lx2);
            (a_s, b_s, c_s) = GetLine(mx1, mx2);

            double den = a_v * b_s - a_s * b_v;
            if (den == 0)
            {
                //can check if they're on the same line, but i dont think its important
                throw new Exception("Lines are parallel");
            }
            //fidning intersection point
            double x_i = -(c_v * b_s - c_s * b_v) / den;
            double y_i = -(a_v * c_s - a_s * c_v) / den;

            return new PointD(x_i, y_i);
        }

        public static (double, double, double) GetLine(PointD p0, PointD p1)
        {
            double A = p1.y - p0.y;
            double B = p0.x - p1.x;
            double C = p1.x * p0.y - p0.x * p1.y;
            return (A, B, C);
        }

        public static double CalculateAngleDeg(PointD v1, PointD v2)
        {
            double l1 = Math.Sqrt(v1.x * v1.x + v1.y * v1.y);
            double l2 = Math.Sqrt(v2.x * v2.x + v2.y * v2.y);
            return Math.Acos((v1.x * v2.x + v1.y * v2.y) / (l1 * l2)) * 180.0 / Math.PI;
        }
    }
}
