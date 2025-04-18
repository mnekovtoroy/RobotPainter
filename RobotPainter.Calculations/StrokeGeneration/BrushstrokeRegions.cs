using SharpVoronoiLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPainter.Calculations.StrokeGeneration
{
    public class BrushstrokeRegions
    {
        public readonly StrokeGenerator _strokeGenerator;

        public List<VoronoiSite> involvedSites;

        public VoronoiSite startingPoint;
        public VoronoiPoint startingCentroid;

        public ColorLab MainColor { get { return _strokeGenerator.image.GetPixel(Convert.ToInt32(startingCentroid.X), Convert.ToInt32(startingCentroid.Y)); } }


        public BrushstrokeRegions(StrokeGenerator parent, VoronoiSite starting_point)
        {
            _strokeGenerator = parent;
            startingPoint = starting_point;
            startingCentroid = startingPoint.Centroid;
        }

        public void GenerateStroke(double max_length, double max_width, double L_tol)
        {
            involvedSites = new List<VoronoiSite>();
            involvedSites.Add(startingPoint);

            double curr_length = 0.0;

            //forward expansion
            VoronoiSite last_site = startingPoint;
            var last_centroid = last_site.Centroid;
            while (curr_length < max_length)
            {
                int lc_ix = Convert.ToInt32(last_centroid.X);
                int lc_iy = Convert.ToInt32(last_centroid.Y);

                double vx = _strokeGenerator.u[lc_ix, lc_iy];
                double vy = _strokeGenerator.v[lc_ix, lc_iy];

                VoronoiSite next_site = GetAdjasentSite(last_site, vx, vy);

                if (!IsValidSite(next_site)) 
                    break;
                var next_centroid = next_site.Centroid;
                if (!IsValidExpand(last_centroid, next_centroid, curr_length, max_length, L_tol))
                    break;

                curr_length += Math.Sqrt(Math.Pow(next_centroid.X - last_centroid.X, 2) + Math.Pow(next_centroid.Y - last_centroid.Y, 2));
                involvedSites.Add(next_site);
                last_site = next_site;
                last_centroid = next_centroid;
            }

            //backwards expansion
            last_site = startingPoint;
            last_centroid = last_site.Centroid;
            while (curr_length < max_length)
            {
                int lc_ix = Convert.ToInt32(last_centroid.X);
                int lc_iy = Convert.ToInt32(last_centroid.Y);

                double vx = -_strokeGenerator.u[lc_ix, lc_iy];
                double vy = -_strokeGenerator.v[lc_ix, lc_iy];

                VoronoiSite next_site = GetAdjasentSite(last_site, vx, vy);

                if (!IsValidSite(next_site))
                    break;
                var next_centroid = next_site.Centroid;
                if (!IsValidExpand(last_centroid, next_centroid, curr_length, max_length, L_tol))
                    break;

                curr_length += Math.Sqrt(Math.Pow(next_centroid.X - last_centroid.X, 2) + Math.Pow(next_centroid.Y - last_centroid.Y, 2));
                involvedSites.Insert(0, next_site);
                last_site = next_site;
                last_centroid = next_centroid;
            }
        }

        private bool IsValidExpand(VoronoiPoint last_centroid, VoronoiPoint next_centroid, double curr_length, double max_length, double L_tol)
        {
            int nc_ix = Convert.ToInt32(next_centroid.X);
            int nc_iy = Convert.ToInt32(next_centroid.Y);

            double main_L = MainColor.L;
            double next_c_L = _strokeGenerator.image.GetPixel(nc_ix, nc_iy).L;

            if (Math.Abs(next_c_L - main_L) > L_tol)
            {
                return false;
            }

            double add_length = Math.Sqrt(Math.Pow(next_centroid.X - last_centroid.X, 2) + Math.Pow(next_centroid.Y - last_centroid.Y, 2));
            if (curr_length + add_length > max_length)
            {
                return false;
            }

            return true;
        }

        private bool IsValidSite(VoronoiSite site)
        {
            return site != null && !involvedSites.Contains(site) && !_strokeGenerator.IsSiteReserved(site);
        }

        private VoronoiSite GetAdjasentSite(VoronoiSite site, double dx, double dy)
        {
            var centroid = site.Centroid;
            var edges = site.Cell;
            foreach (var edge in edges)
            {
                if (CheckIntersection(
                    centroid.X, centroid.Y, dx, dy,
                    edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y))
                {
                    //check first to the left of the edge, then to the right of the edge
                    if (edge.Left != null && edge.Left != site)
                    {
                        return edge.Left;
                    }
                    else if (edge.Right != null && edge.Right != site)
                    {
                        return edge.Right;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        private static bool CheckIntersection(
            double v_x0, double v_y0, double v_dx, double v_dy,
            double s_x0, double s_y0, double s_x1, double s_y1)
        {
            //formulas: http://e-maxx.ru/algo/lines_intersection
            double s_dx = s_x1 - s_x0;
            double s_dy = s_y1 - s_y0;

            double a_v, b_v, c_v;
            double a_s, b_s, c_s;

            (a_v, b_v, c_v) = GetLine(v_x0, v_y0, v_x0 + v_dx, v_y0 + v_dy);
            (a_s, b_s, c_s) = GetLine(s_x0, s_y0, s_x1, s_y1);

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

        private static (double, double, double) GetLine(double x0, double y0, double x1, double y1)
        {
            double A = y1 - y0;
            double B = x0 - x1;
            double C = x1 * y0 - x0 * y1;
            return (A, B, C);
        }
    }
}