using MathNet.Numerics.Optimization;
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

        public VoronoiSite startingSite;
        public VoronoiPoint startingCentroid;

        public ColorLab MainColor { get { return _strokeGenerator.image.GetPixel(Convert.ToInt32(startingCentroid.X), Convert.ToInt32(startingCentroid.Y)); } }

        public PointD StartingNorm { 
            get { 
                return new PointD(
                    _strokeGenerator.u[Convert.ToInt32(startingCentroid.X), Convert.ToInt32(startingCentroid.Y)], 
                    _strokeGenerator.v[Convert.ToInt32(startingCentroid.X), Convert.ToInt32(startingCentroid.Y)]); 
            } 
        }

        public BrushstrokeRegions(StrokeGenerator parent, VoronoiSite starting_point)
        {
            _strokeGenerator = parent;
            startingSite = starting_point;
            startingCentroid = startingSite.Centroid;
        }

        public void GenerateStroke(double max_length, double max_width, double L_tol)
        {
            involvedSites = new List<VoronoiSite>();
            involvedSites.Add(startingSite);

            double curr_length = 0.0;

            //forward expansion
            VoronoiSite last_site = startingSite;
            var last_centroid = last_site.Centroid;

            PointD prev_v = new PointD(0.0, 0.0);

            while (curr_length < max_length)
            {
                int lc_ix = Convert.ToInt32(last_centroid.X);
                int lc_iy = Convert.ToInt32(last_centroid.Y);

                double vx = _strokeGenerator.u[lc_ix, lc_iy];
                double vy = _strokeGenerator.v[lc_ix, lc_iy];

                PointD norm_v = new PointD(vx, vy);

                VoronoiSite next_site = GetAdjasentSite(last_site, norm_v, prev_v);

                if (!IsValidSite(next_site)) 
                    break;
                var next_centroid = next_site.Centroid;
                if (!IsValidExpand(last_centroid, next_centroid, curr_length, max_length, L_tol))
                    break;

                curr_length += Math.Sqrt(Math.Pow(next_centroid.X - last_centroid.X, 2) + Math.Pow(next_centroid.Y - last_centroid.Y, 2));
                involvedSites.Add(next_site);

                prev_v.x = next_centroid.X - last_centroid.X;
                prev_v.y = next_centroid.Y - last_centroid.Y;

                last_site = next_site;
                last_centroid = next_centroid;
            }

            //backwards expansion
            last_site = startingSite;
            last_centroid = last_site.Centroid;
            if(involvedSites.Count > 1)
            {
                prev_v.x = involvedSites[0].X - involvedSites[1].X;
                prev_v.y = involvedSites[0].Y - involvedSites[1].Y;

            } else
            {
                prev_v.x = 0;
                prev_v.y = 0;
            }
                while (curr_length < max_length)
                {
                    int lc_ix = Convert.ToInt32(last_centroid.X);
                    int lc_iy = Convert.ToInt32(last_centroid.Y);

                    double vx = -_strokeGenerator.u[lc_ix, lc_iy];
                    double vy = -_strokeGenerator.v[lc_ix, lc_iy];

                    PointD norm_v = new PointD(vx, vy);

                    VoronoiSite next_site = GetAdjasentSite(last_site, norm_v, prev_v);

                    if (!IsValidSite(next_site))
                        break;
                    var next_centroid = next_site.Centroid;
                    if (!IsValidExpand(last_centroid, next_centroid, curr_length, max_length, L_tol))
                        break;

                    curr_length += Math.Sqrt(Math.Pow(next_centroid.X - last_centroid.X, 2) + Math.Pow(next_centroid.Y - last_centroid.Y, 2));
                    involvedSites.Insert(0, next_site);

                    prev_v.x = next_centroid.X - last_centroid.X;
                    prev_v.y = next_centroid.Y - last_centroid.Y;

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

        private VoronoiSite GetAdjasentSite(VoronoiSite site, PointD norm, PointD prev_v, double max_norm_angle = 30.0, double max_brush_angle = 30.0)
        {
            /*var centroid = site.Centroid;
            var edges = site.Cell;
            foreach (var edge in edges)
            {
                if (Geometry.CheckRaySegmentIntersection(
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
            return null;*/

            var candidates = new Dictionary<VoronoiSite, double>();
            var neighbors = site.Neighbours;
            var centroid = site.Centroid;
            foreach (var neighbor in neighbors)
            {
                var neighbor_c = neighbor.Centroid;
                var v_n = new PointD(neighbor_c.X - centroid.X, neighbor_c.Y - centroid.Y);

                double brush_angle = Geometry.Norm(prev_v) != 0 ? Geometry.CalculateAngleDeg(v_n, prev_v) : 0.0;
                double norm_angle = Geometry.CalculateAngleDeg(v_n, norm);
                if(brush_angle <= max_brush_angle && norm_angle <= max_norm_angle)
                {
                    candidates.Add(neighbor, brush_angle);
                }
            }
            if(candidates.Count == 0)
            {
                return null;
            }
            var result = candidates.OrderBy(c => c.Value).First();
            return result.Key;
        }
    }
}