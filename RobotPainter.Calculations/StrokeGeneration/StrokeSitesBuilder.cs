using RobotPainter.Core;
using SharpVoronoiLib;

namespace RobotPainter.Calculations.StrokeGeneration
{
    public static class StrokeSitesBuilder
    {
        public class Options
        {
            public double xResizeCoeff;
            public double yResizeCoeff;

            public double CanvasMaxStrokeLength = 80.0;
            public double L_tol = 3.5;
            public double MaxNormAngle = 30.0;
            public double MaxBrushAngle = 30.0;
        }

        public static StrokeSites GenerateStrokeSites(StrokeGenerator parent, VoronoiSite starting_site, Options options)
        {
            var stroke_sites = new StrokeSites()
            {
                strokeGenerator = parent,
                involvedSites = [ starting_site ],
                startingSite = starting_site,
                startingCentroid = starting_site.Centroid
            };

            double curr_length = 0.0;

            //forward expansion
            VoronoiSite last_site = stroke_sites.startingSite;
            var last_centroid = last_site.Centroid;

            PointD prev_v = new PointD(0.0, 0.0);

            while (curr_length < options.CanvasMaxStrokeLength)
            {
                int lc_ix = Convert.ToInt32(last_centroid.X);
                int lc_iy = Convert.ToInt32(last_centroid.Y);

                double vx = stroke_sites.strokeGenerator.u[lc_ix, lc_iy];
                double vy = stroke_sites.strokeGenerator.v[lc_ix, lc_iy];

                PointD norm_v = new PointD(vx, vy);

                VoronoiSite next_site = GetAdjasentSite(stroke_sites, last_site, norm_v, prev_v, options);

                if (!IsValidSite(stroke_sites, next_site))
                    break;
                var next_centroid = next_site.Centroid;
                if (!IsValidExpand(stroke_sites, last_centroid, next_centroid, curr_length, options))
                    break;

                curr_length += Math.Sqrt(Math.Pow(next_centroid.X - last_centroid.X, 2) + Math.Pow(next_centroid.Y - last_centroid.Y, 2));
                stroke_sites.involvedSites.Add(next_site);

                prev_v.x = next_centroid.X - last_centroid.X;
                prev_v.y = next_centroid.Y - last_centroid.Y;

                last_site = next_site;
                last_centroid = next_centroid;
            }

            //backwards expansion
            last_site = stroke_sites.startingSite;
            last_centroid = last_site.Centroid;
            if (stroke_sites.involvedSites.Count > 1)
            {
                prev_v.x = stroke_sites.involvedSites[0].X - stroke_sites.involvedSites[1].X;
                prev_v.y = stroke_sites.involvedSites[0].Y - stroke_sites.involvedSites[1].Y;

            }
            else
            {
                prev_v.x = 0;
                prev_v.y = 0;
            }
            while (curr_length < options.CanvasMaxStrokeLength)
            {
                int lc_ix = Convert.ToInt32(last_centroid.X);
                int lc_iy = Convert.ToInt32(last_centroid.Y);

                double vx = -stroke_sites.strokeGenerator.u[lc_ix, lc_iy];
                double vy = -stroke_sites.strokeGenerator.v[lc_ix, lc_iy];

                PointD norm_v = new PointD(vx, vy);

                VoronoiSite next_site = GetAdjasentSite(stroke_sites, last_site, norm_v, prev_v, options);

                if (!IsValidSite(stroke_sites, next_site))
                    break;
                var next_centroid = next_site.Centroid;
                if (!IsValidExpand(stroke_sites, last_centroid, next_centroid, curr_length, options))
                    break;

                curr_length += Math.Sqrt(Math.Pow((next_centroid.X - last_centroid.X) * options.xResizeCoeff, 2) + Math.Pow((next_centroid.Y - last_centroid.Y) * options.yResizeCoeff, 2));
                stroke_sites.involvedSites.Insert(0, next_site);

                prev_v.x = next_centroid.X - last_centroid.X;
                prev_v.y = next_centroid.Y - last_centroid.Y;

                last_site = next_site;
                last_centroid = next_centroid;
            }
            return stroke_sites;
        }

        private static bool IsValidExpand(StrokeSites stroke_sties, VoronoiPoint last_centroid, VoronoiPoint next_centroid, double curr_length, Options options)
        {
            int nc_ix = Convert.ToInt32(next_centroid.X);
            int nc_iy = Convert.ToInt32(next_centroid.Y);

            double main_L = stroke_sties.MainColor.L;
            double next_c_L = stroke_sties.strokeGenerator.image.GetPixel(nc_ix, nc_iy).L;

            if (Math.Abs(next_c_L - main_L) > options.L_tol)
            {
                return false;
            }

            double add_length = Math.Sqrt(Math.Pow((next_centroid.X - last_centroid.X) * options.xResizeCoeff, 2) + Math.Pow((next_centroid.Y - last_centroid.Y) * options.yResizeCoeff, 2));
            if (curr_length + add_length > options.CanvasMaxStrokeLength)
            {
                return false;
            }

            return true;
        }

        private static bool IsValidSite(StrokeSites stroke_sites, VoronoiSite site)
        {
            return site != null
                && !stroke_sites.involvedSites.Contains(site)
                && !stroke_sites.strokeGenerator.IsSiteReserved(site)
                && stroke_sites.strokeGenerator.IsSiteToPaint(site);
        }

        private static VoronoiSite GetAdjasentSite(StrokeSites stroke_sites, VoronoiSite site, PointD norm, PointD prev_v, Options options)
        {
            var candidates = new Dictionary<VoronoiSite, double>();
            //var neighbors = site.Neighbours;
            var edges = site.Cell;
            var centroid = site.Centroid;
            foreach (var edge in edges)
            {
                if (edge.Right == null || edge.Left == null)
                    continue;

                var neighbor = edge.Right == site ? edge.Left : edge.Right;

                var neighbor_c = neighbor.Centroid;
                var v_n = new PointD(neighbor_c.X - centroid.X, neighbor_c.Y - centroid.Y);

                double brush_angle = Geometry.Norm(prev_v) != 0 ? Geometry.CalculateAngleDeg(v_n, prev_v) : 0.0;
                double norm_angle = Geometry.CalculateAngleDeg(v_n, norm);

                if(brush_angle > options.MaxBrushAngle || norm_angle > options.MaxNormAngle)
                    continue;

                //on a triangle formed by connecting edges (in case when the outside angle between them < 90)
                //check middle point of an opposite side to that angle
                var triag_edges1 = edge.Neighbours.Where(e => e.Start == edge.Start || e.End == edge.Start).ToList();
                var triag_edges2 = edge.Neighbours.Where(e => e.Start == edge.End || e.End == edge.End).ToList();
                if (triag_edges1.Count != 2 || triag_edges2.Count != 2)
                    throw new Exception("idk");

                PointD triag_1_p1 = VoronoiPointToPointD(triag_edges1[0].Start == edge.Start ? triag_edges1[0].End : triag_edges1[0].Start);
                PointD triag_1_p2 = VoronoiPointToPointD(triag_edges1[1].Start == edge.Start ? triag_edges1[1].End : triag_edges1[1].Start);
                PointD triag_2_p1 = VoronoiPointToPointD(triag_edges2[0].Start == edge.End ? triag_edges2[0].End : triag_edges2[0].End);
                PointD triag_2_p2 = VoronoiPointToPointD(triag_edges2[1].Start == edge.End ? triag_edges2[1].End : triag_edges2[1].End);
                PointD e_start = VoronoiPointToPointD(edge.Start);
                PointD e_end = VoronoiPointToPointD(edge.End);


                bool outside_color_variance = true;
                if (Geometry.Dot(triag_1_p1 - e_start, triag_1_p2 - e_start) > 0)
                {
                    PointD p1 = new PointD((triag_1_p1.x + triag_1_p2.x) / 2.0, (triag_1_p1.y + triag_1_p2.y) / 2.0);
                    int x = Convert.ToInt32(p1.x);
                    int y = Convert.ToInt32(p1.y);
                    if(Math.Abs(stroke_sites.strokeGenerator.image.GetPixel(x, y).L - stroke_sites.MainColor.L) > options.L_tol)
                    {
                        outside_color_variance = false;
                    }
                }
                if (Geometry.Dot(triag_2_p1 - e_end, triag_2_p2 - e_end) > 0)
                {
                    PointD p2 = new PointD((triag_2_p1.x + triag_2_p2.x) / 2.0, (triag_2_p1.y + triag_2_p2.y) / 2.0);
                    int x = Convert.ToInt32(p2.x);
                    int y = Convert.ToInt32(p2.y);
                    if (Math.Abs(stroke_sites.strokeGenerator.image.GetPixel(x, y).L - stroke_sites.MainColor.L) > options.L_tol)
                    {
                        outside_color_variance = false;
                    }
                }

                if (outside_color_variance)
                {
                    candidates.Add(neighbor, norm_angle);
                }
            }
            if (candidates.Count == 0)
            {
                return null;
            }
            var result = candidates.OrderBy(c => c.Value).First();
            return result.Key;
        }

        public static PointD VoronoiPointToPointD(VoronoiPoint p)
        {
            return new PointD(p.X, p.Y);
        }
    }
}
