using RobotPainter.Core;

namespace RobotPainter.Calculations
{
    public static class Geometry
    {
        public static bool CheckRaySegmentIntersection(
            double v_x0, double v_y0, double v_dx, double v_dy,
            double s_x0, double s_y0, double s_x1, double s_y1)
        {
            return GetRaySegmentIntersectionPoint(v_x0, v_y0, v_dx, v_dy, s_x0, s_y0, s_x1, s_y1).HasValue;
        }

        public static PointD? GetRaySegmentIntersectionPoint(
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
                return null;
            }
            //fidning intersection point
            double x_i = -(c_v * b_s - c_s * b_v) / den;
            double y_i = -(a_v * c_s - a_s * c_v) / den;

            //checking if intersection point is on the segment & the ray
            bool isBetweenXS = x_i >= Math.Min(s_x0, s_x1) && x_i <= Math.Max(s_x0, s_x1);
            bool isBetweenYS = y_i >= Math.Min(s_y0, s_y1) && y_i <= Math.Max(s_y0, s_y1);
            bool isOnS = (s_dx <= 1e-5 || isBetweenXS) && (s_dy <= 1e-5 || isBetweenYS); //in case segment line is parallel to either of the axes, to exclude numerical error

            bool isOnV = (v_dx * (x_i - v_x0) + v_dy * (y_i - v_y0)) > 0;

            if (isOnS && isOnV)
            {
                return new PointD(x_i, y_i);
            } else
            {
                return null;
            }
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
            if (l1 == 0 || l2 == 0) return 0;
            double cos = (v1.x * v2.x + v1.y * v2.y) / (l1 * l2);
            if (cos > 1.0) cos = 1.0; //in case of a numerical error
            if (cos < -1.0) cos = -1.0;
            return Math.Acos(cos) * 180.0 / Math.PI;
        }

        public static PointD GetBisectorVector(PointD p0, PointD p1, PointD p2)
        {
            PointD v1 = (p0 - p1) / Norm(p0 - p1);
            PointD v2 = (p2 - p1) / Norm(p2 - p1);
            if(Norm(v1 + v2) < 1e-3)
            {
                //if vectors are at 180 degree angle, return a pe to one of them
                double[,] M = new double[2, 2];
                M[0, 0] = 0; M[0, 1] = -1;
                M[1, 0] = 1; M[1, 1] = 0;
                return Rotate(v2, M);
            }

            return (v1 + v2) / Norm(v1 + v2);
        }

        public static double Norm(PointD p)
        {
            return Math.Sqrt(p.x * p.x + p.y * p.y);
        }

        public static double Norm(Point3D p)
        {
            return Math.Sqrt(p.x * p.x + p.y * p.y + p.z * p.z);
        }

        public static PointD Rotate(PointD p, double[,] M)
        {
            return new PointD(p.x * M[0, 0] + p.y * M[0, 1], p.x * M[1, 0] + p.y * M[1, 1]);
        }

        public static PointD Scale(PointD p, double x_scale_coeff, double y_scale_coeff)
        {
            return new PointD(p.x * x_scale_coeff, p.y * y_scale_coeff);
        }
    }
}
