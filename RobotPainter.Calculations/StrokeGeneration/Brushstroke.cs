using SharpVoronoiLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPainter.Calculations.StrokeGeneration
{
    public class Brushstroke
    {
        public readonly StrokeGenerator _strokeGenerator;

        public List<VoronoiSite> involvedSites;

        public VoronoiSite startingPoint;


        public Brushstroke(StrokeGenerator parent, VoronoiSite starting_point)
        {
            _strokeGenerator = parent;
            startingPoint = starting_point;
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
            int lc_ix = Convert.ToInt32(last_centroid.X);
            int lc_iy = Convert.ToInt32(last_centroid.Y);

            int nc_ix = Convert.ToInt32(next_centroid.X);
            int nc_iy = Convert.ToInt32(next_centroid.Y);

            double last_c_L = _strokeGenerator.image.GetPixel(lc_ix, lc_iy).L;
            double next_c_L = _strokeGenerator.image.GetPixel(nc_ix, nc_iy).L;

            if (Math.Abs(next_c_L - last_c_L) > L_tol)
            {
                return false;
            }

            double add_length = Math.Sqrt(Math.Pow(next_centroid.X - last_centroid.X, 2) + Math.Pow(next_centroid.Y - last_centroid.Y, 2));
            if (curr_length + add_length < max_length)
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
            double s_dx = s_x1 - s_x0;
            double s_dy = s_y1 - s_y0;

            // check if vector and segment are parallel
            double cross = v_dx * s_dy - v_dy * s_dx;
            if (Math.Abs(cross) < 1e-10)
            {
                // check if they are on the same line
                double dx = s_x0 - v_x0;
                double dy = s_y0 - v_y0;
                double cross2 = dx * v_dy - dy * v_dx;
                if (Math.Abs(cross2) < 1e-10)
                {
                    // check if there is overlap between them
                    double t0 = (s_x0 - v_x0) * v_dx + (s_y0 - v_y0) * v_dy;
                    double t1 = (s_x1 - v_x0) * v_dx + (s_y1 - v_y0) * v_dy;
                    return t0 >= 0 || t1 >= 0;
                }
                return false;
            }

            
            double s_T = ((v_x0 - s_x0) * v_dy - (v_y0 - s_y0) * v_dx) / cross;
            double v_T = ((v_x0 - s_x0) * s_dy - (v_y0 - s_y0) * s_dx) / cross;

            //check that intersection is on the ray (from the vector) and on the 
            return s_T >= 0 && s_T <= 1 && v_T >= 0;
        }
    }
}