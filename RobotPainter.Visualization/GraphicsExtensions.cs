using RobotPainter.Core;
using SharpVoronoiLib;
using System;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace RobotPainter.Visualization
{
    public static class GraphicsExtensions
    {
        public static void DrawVectorField(this Graphics g, Pen p, int w, int h, double[,] u, double[,] v, int n_arrows, double length_coeff, double tip_length_coeff = 0.4, int tip_angle = 30)
        {
            int s_dim = w < h ? w : h;

            int step = s_dim / n_arrows;

            int arrow_length = Convert.ToInt32((s_dim / n_arrows) * length_coeff);
            int tip_length = Convert.ToInt32(arrow_length * tip_length_coeff);

            for (int i = step / 2; i < w; i += step)
            {
                for (int j = step / 2; j < h; j += step)
                {
                    int end_x = Convert.ToInt32(i + u[i, j] * arrow_length);
                    int end_y = Convert.ToInt32(j + v[i, j] * arrow_length);
                    DrawVector(g, p, new Point(i, j), new Point(end_x, end_y), tip_length, tip_angle);
                }
            }
        }

        public static void DrawVector(this Graphics g, Pen p, Point p0, Point p1, int tip_length, int tip_angle)
        {
            g.DrawLine(p, p0, p1);

            //getting vector scaled vector to draw the tip
            double l = Math.Sqrt(Math.Pow(p1.X - p0.X, 2) + Math.Pow(p1.Y - p0.Y, 2));
            PointD v = new PointD();
            if (l > 0)
            {
                v.x = (p0.X - p1.X) * tip_length / l;
                v.y = (p0.Y - p1.Y) * tip_length / l;
            }
            else
            {
                return;
            }

            //rotation matrix 1
            double angle_l = tip_angle * Math.PI / 180.0;
            double l00 = Math.Cos(angle_l), l01 = Math.Sin(-angle_l);
            double l10 = Math.Sin(angle_l), l11 = Math.Cos(angle_l);
            //rotation matrix 2
            double angle_r = -tip_angle * Math.PI / 180.0;
            double r00 = Math.Cos(angle_r), r01 = Math.Sin(-angle_r);
            double r10 = Math.Sin(angle_r), r11 = Math.Cos(angle_r);

            int v_Xl = Convert.ToInt32(v.x * l00 + v.y * l01);
            int v_Yl = Convert.ToInt32(v.x * l10 + v.y * l11);
            int v_Xr = Convert.ToInt32(v.x * r00 + v.y * r01);
            int v_Yr = Convert.ToInt32(v.x * r10 + v.y * r11);
            Size v_l = new Size(v_Xl, v_Yl);
            Size v_r = new Size(v_Xr, v_Yr);

            g.DrawLine(p, p1, p1 + v_l);
            g.DrawLine(p, p1, p1 + v_r);
        }

        public static void DrawDot(this Graphics g, Brush b, PointD p, int r)
        {
            g.DrawDot(b, new Point(Convert.ToInt32(p.x), Convert.ToInt32(p.y)), r);
        }

        public static void DrawDot(this Graphics g, Brush b, Point p, int r)
        {
            g.FillEllipse(b, p.X - r, p.Y - r, r * 2, r * 2);
        }

        public static void DrawVoronoi(this Graphics g, Pen p, List<VoronoiSite> sites)
        {
            foreach (var site in sites)
            {
                var edges = site.Cell;
                foreach (var edge in edges)
                {
                    int x0, y0, x1, y1;
                    x0 = Convert.ToInt32(edge.Start.X);
                    x1 = Convert.ToInt32(edge.Start.Y);
                    y0 = Convert.ToInt32(edge.End.X);
                    y1 = Convert.ToInt32(edge.End.Y);
                    g.DrawLine(p, x0, x1, y0, y1);
                }
            }
        }

        public static void FillVoronoi(this Graphics g, List<Brush> brushes, List<VoronoiSite> sites)
        {
            for(int i = 0; i < sites.Count; i++)
            {
                PointF[] polygon = sites[i].ClockwisePoints.Select(p => new PointF(Convert.ToSingle(p.X), Convert.ToSingle(p.Y))).ToArray();

                g.FillPolygon(brushes[i % brushes.Count], polygon);
            }
        }

        public static void FillVoronoi(this Graphics g, List<SolidBrush> brushes, int[,] mask)
        {
            for (int i = 0; i < mask.GetLength(0); i++)
            {
                for (int j = 0; j < mask.GetLength(1); j++)
                {
                    if (mask[i,j] >= 0)
                    {
                        g.FillRectangle(brushes[mask[i, j] % brushes.Count], i, j, 1, 1);
                    }
                }
            }
        }

        public static void DrawPoints(this Graphics g, Brush b, List<PointD> points, int r)
        {
            for(int i = 0; i < points.Count; i++)
            {
                g.DrawDot(b, points[i], r);
            }
        }

        public static void DrawPoints(this Graphics g, Brush b, List<Point> points, int r)
        {
            for (int i = 0; i < points.Count; i++)
            {
                g.DrawDot(b, points[i], r);
            }
        }

        public static void DrawLines(this Graphics g, Pen p, List<PointD> points)
        {
            if (points == null || points.Count <= 1)
                return;
            g.DrawLines(p, points.Select(p => new PointF(Convert.ToSingle(p.x), Convert.ToSingle(p.y))).ToArray());
        }
    }
}
