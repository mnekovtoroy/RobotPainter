﻿using SharpVoronoiLib;
using System.ComponentModel;
using System.Drawing;

namespace RobotPainter.Visualization
{
    public static class VoronoiVisualizer
    {
        public static void VisualizeVoronoiInline(Bitmap image, List<VoronoiSite> sites, Color edge_c, Color centroid_c, int centroid_r = 1)
        {
            using (var g = Graphics.FromImage(image))
            {
                Pen p = new Pen(edge_c);
                Brush b = new SolidBrush(centroid_c);
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
                    int px, py;
                    px = Convert.ToInt32(site.Centroid.X);
                    py = Convert.ToInt32(site.Centroid.Y);
                    g.FillEllipse(b, px - centroid_r, py - centroid_r, centroid_r * 2, centroid_r * 2);
                }
            }
        }

        public static void VisualisePointsInline(Bitmap image, List<(double, double)> points, Color point_c, int point_r = 1)
        {
            using (var g = Graphics.FromImage(image))
            {
                Brush b = new SolidBrush(point_c);
                foreach(var p in points)
                {
                    int px, py;
                    px = Convert.ToInt32(p.Item1);
                    py = Convert.ToInt32(p.Item2);
                    g.FillEllipse(b, px - point_r, py - point_r, point_r * 2, point_r * 2);
                }
            }
        }

        public static void VoronoiFillInline(Bitmap bmp, int[,] mask, List<Color> colors)
        {
            for(int i = 0; i < mask.GetLength(0); i++)
            {
                for(int j = 0; j < mask.GetLength(1); j++)
                {
                    bmp.SetPixel(i, j, mask[i, j] == -1 ? Color.Red : colors[mask[i, j] % colors.Count]);
                }
            }
        }

        public static void VisualizeStrokeInline(Bitmap bmp, List<(int, int)> stroke_line, Color lines_c, Color point_c, int point_r)
        {
            using (var g = Graphics.FromImage(bmp))
            {
                var p = new Pen(lines_c);
                var b = new SolidBrush(point_c);
                for(int i = 0; i < stroke_line.Count - 1; i++)
                {
                    g.DrawLine(p, stroke_line[i].Item1, stroke_line[i].Item2, stroke_line[i + 1].Item1, stroke_line[i + 1].Item2);
                    g.FillEllipse(b, stroke_line[i].Item1 - point_r, stroke_line[i].Item2 - point_r, point_r * 2, point_r * 2);
                }
                g.FillEllipse(b, stroke_line[stroke_line.Count - 1].Item1 - point_r, stroke_line[stroke_line.Count - 1].Item2 - point_r, point_r * 2, point_r * 2);
            }
        }
    }
}
