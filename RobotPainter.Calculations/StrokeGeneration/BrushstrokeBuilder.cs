using RobotPainter.Calculations.Brushes;
using RobotPainter.Core;
using SharpVoronoiLib;

namespace RobotPainter.Calculations.StrokeGeneration
{
    public static class BrushstrokeBuilder
    {
        public class Options
        {
            public double xResizeCoeff;
            public double yResizeCoeff;

            public IBrushModel BrushModel = new BasicBrushModel();

            public double MaxWidth = 7.0;
            public double Overlap = 1.0;
            public double StartOverheadCoeff = 1.5;
            public double EndOverheadCoeff = 1.2;
            public double SafeHeight = 2.0;
            public double StartRunawayAngle = 30.0;
            public double EndRunawayAngle = 80.0;
        }

        public static Brushstroke GenerateBrushstroke(StrokeSites stroke_sites, Options options)
        {
            var desired_path = GetDesiredPath(stroke_sites, options);
            var root_path = GetBrushRootPath(desired_path, options);
            var brushstroke = new Brushstroke()
            {
                brushModel = options.BrushModel,
                RootPath = root_path,
                DesiredPath = desired_path,
                Color = stroke_sites.MainColor
            };
            return brushstroke;
        }

        private static List<Point3D> GetBrushRootPath(List<Point3D> desired_path, Options options)
        {
            return options.BrushModel.CalculateBrushRootPath(desired_path);
        }

        private static List<Point3D> GetDesiredPath(StrokeSites stroke_sites, Options options)
        {
            var desired_path = BaseDesiredPath(stroke_sites, options);
            desired_path = ResizeXYcoords(desired_path, options);
            desired_path = AddRunaways(desired_path, options);
            return desired_path;
        }

        private static List<Point3D> BaseDesiredPath(StrokeSites stroke_sites, Options options)
        {
            if (stroke_sites.involvedSites.Count == 1)
                return SingleSiteBaseDesiredPath(stroke_sites, options);

            var result = new List<Point3D>();

            for (int i = 0; i < stroke_sites.involvedSites.Count; i++)
            {
                var site_c = stroke_sites.involvedSites[i].Centroid;
                double r = FindDesiredR(stroke_sites.involvedSites[i], options);
                double z = options.BrushModel.CalculateZCoordinate(r);
                result.Add(new Point3D(site_c.X, site_c.Y, z));
            }

            //adding start and end points
            //start
            PointD p0 = new PointD(result[0].x, result[0].y);
            PointD p1 = new PointD(result[1].x, result[1].y);
            PointD v = p0 - p1;
            v = v / Geometry.Norm(v);
            double s = FindDesiredR(stroke_sites.involvedSites[0], options);
            PointD ps = p0 + v * s * options.StartOverheadCoeff;
            result.Insert(0, new Point3D(ps.x, ps.y, 0.0));

            //end
            PointD pn = new PointD(result[result.Count - 1].x, result[result.Count - 1].y);
            PointD pn_ = new PointD(result[result.Count - 2].x, result[result.Count - 2].y);
            v = pn - pn_;
            v = v / Geometry.Norm(v);
            s = FindDesiredR(stroke_sites.involvedSites[0], options);
            PointD pe = pn + v * s * options.EndOverheadCoeff;
            result.Add(new Point3D(pe.x, pe.y, 0.0));

            return result;
        }

        private static List<Point3D> SingleSiteBaseDesiredPath(StrokeSites stroke_reg, Options options)
        {
            var result = new List<Point3D>();

            var v = stroke_reg.StartingNorm;

            if (Geometry.Norm(v) == 0)
            {
                v.x = options.xResizeCoeff >= options.yResizeCoeff ? 1.0 : 0.0;
                v.y = options.xResizeCoeff < options.yResizeCoeff ? 1.0 : 0.0;
            } else
            {
                v = v / Geometry.Norm(v);
            }


            var site = stroke_reg.startingSite;
            var centroid = stroke_reg.startingCentroid;
            double r = FindDesiredR(site, options);
            double z = options.BrushModel.CalculateZCoordinate(r);

            var pm = new PointD(centroid.X, centroid.Y);

            //shorter edge should be the starting one
            var edge1 = GetIntersectingEdge(site, pm, pm - v);
            var edge2 = GetIntersectingEdge(site, pm, pm + v);
            PointD v1 = new PointD(edge1.End.X - edge1.Start.X, edge1.End.Y - edge1.Start.Y);
            PointD v2 = new PointD(edge2.End.X - edge2.Start.X, edge2.End.Y - edge2.Start.Y);
            PointD vp = Geometry.RotateCounterClockwise(v, 90);
            if (Geometry.CalculateAngleDeg(v1, vp) > 90)
                v1 = -v1;
            if (Geometry.CalculateAngleDeg(v2, vp) > 90)
                v2 = -v2;
            double l1 = Geometry.Dot(v1, vp);
            double l2 = Geometry.Dot(v2, vp);
            var s_edge = l1 < l2 ? edge1 : edge2;
            var e_edge = l1 >= l2 ? edge1 : edge2;
            if (l1 >= l2)
                v = -v;

            //start
            var ps_0 = Geometry.GetRaySegmentIntersectionPoint(pm.x, pm.y, -v.x, -v.y, s_edge.Start.X, s_edge.Start.Y, s_edge.End.X, s_edge.End.Y).Value;
            PointD ps = pm + (ps_0 - pm) * options.StartOverheadCoeff;
            result.Add(new Point3D(ps.x, ps.y, 0.0));
            //result.Add(new Point3D(ps_0.x, ps_0.y, z / 2.0));

            //middle
            result.Add(new Point3D(pm.x, pm.y, z));

            //end
            var pe_0 = Geometry.GetRaySegmentIntersectionPoint(pm.x, pm.y, v.x, v.y, e_edge.Start.X, e_edge.Start.Y, e_edge.End.X, e_edge.End.Y).Value;
            PointD pe = pm + (pe_0 - pm) * options.EndOverheadCoeff;
            //result.Add(new Point3D(pe_0.x, pe_0.y, z / 2.0));
            result.Add(new Point3D(pe.x, pe.y, 0.0));

            return result;
        }

        public static List<Point3D> ResizeXYcoords(List<Point3D> points, Options options)
        {
            return points.Select(p => new Point3D(p.x * options.xResizeCoeff, p.y * options.yResizeCoeff, p.z)).ToList();
        }

        private static VoronoiEdge GetIntersectingEdge(VoronoiSite site, PointD p0, PointD p1)
        {
            var edges = site.Cell;
            foreach(var edge in edges)
            {
                if(Geometry.CheckRaySegmentIntersection(
                    p1.x, p1.y, p1.x - p0.x, p1.y - p0.y,
                    edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y))
                    { return edge; }
            }
            return null;
        }

        private static List<Point3D> AddRunaways(List<Point3D> list, Options options)
        {
            double start_length, end_length;
            start_length = (options.StartRunawayAngle == 90.0 || options.StartRunawayAngle == 0.0) ?
                0.0 :
                options.SafeHeight / Math.Tan(options.StartRunawayAngle * Math.PI / 180.0);
            end_length = (options.EndRunawayAngle == 90.0 || options.EndRunawayAngle == 0.0) ? 
                0.0 :
                options.SafeHeight / Math.Tan(options.EndRunawayAngle * Math.PI / 180.0);

            double d = Math.Sqrt(Math.Pow(list[0].x - list[1].x, 2) + Math.Pow(list[0].y - list[1].y, 2));
            PointD start_v = new PointD((list[0].x - list[1].x) / d, (list[0].y - list[1].y) / d);

            d = Math.Sqrt(Math.Pow(list[list.Count - 1].x - list[list.Count - 2].x, 2) + Math.Pow(list[list.Count - 1].y - list[list.Count - 2].y, 2));
            PointD end_v = new PointD((list[list.Count - 1].x - list[list.Count - 2].x) / d, (list[list.Count - 1].y - list[list.Count - 2].y) / d);

            PointD start_runaway = (new PointD(list[0].x, list[0].y)) + start_length * start_v;
            PointD end_runaway = (new PointD(list[list.Count - 1].x, list[list.Count - 1].y)) + end_length * end_v;

            list.Insert(0, new Point3D(start_runaway.x, start_runaway.y, options.SafeHeight));
            list.Add(new Point3D(end_runaway.x, end_runaway.y, options.SafeHeight));

            return list;
        }

        private static double FindDesiredR(VoronoiSite site, Options options)
        {
            double desired_r = -1.0;

            var points = site.Points;
            var centroid = site.Centroid;
            PointD p_centroid = new PointD(centroid.X, centroid.Y);
            foreach(var p in points)
            {
                PointD curr_p = new PointD(p.X, p.Y);
                double r = Geometry.Norm(curr_p - p_centroid);
                if(r > desired_r)
                {
                    desired_r = r;
                }
            }
            if (desired_r == -1)
            {
                throw new Exception("Cant find desired radius");
            }
            desired_r += options.Overlap;
            double max_r = options.MaxWidth / 2.0;
            desired_r = desired_r < max_r ? desired_r : max_r;
            return desired_r;
        }

        private static PointD FindMinAnglePoint(VoronoiSite site)
        {
            var angle_shared_point = new PointD(); // dummy
            //find the sharpest angle in first / last region - that would be starting point
            double min_angle = double.MaxValue;
            //first region
            List<VoronoiEdge> edges = site.ClockwiseCell.ToList();
            for (int i = 0; i < edges.Count; i++)
            {
                //get angle vectors
                PointD v1, v2, shared_point;
                (v1, v2) = GetAngleVectors(edges[i], edges[(i + 1) % edges.Count], out shared_point);
                //calculate the angle
                double angle = Geometry.CalculateAngleDeg(v1, v2);
                if (angle < min_angle)
                {
                    min_angle = angle;
                    angle_shared_point = shared_point;
                }
            }
            return angle_shared_point;
        }

        private static double FindNonAdjMinAngle(VoronoiSite site, VoronoiSite adj_site, out PointD angle_shared_point)
        {
            angle_shared_point = new PointD(); // dummy
            //find the sharpest angle in first / last region - that would be starting point
            double min_angle = double.MaxValue;
            //first region
            List<VoronoiEdge> edges = site.ClockwiseCell.ToList();
            for (int i = 0; i < edges.Count; i++)
            {
                //we dont want an angle on the connecting edge
                if (IsEdgeConnecting(edges[i], site, adj_site) ||
                    IsEdgeConnecting(edges[(i + 1) % edges.Count], site, adj_site))
                {
                    continue;
                }

                //get angle vectors
                PointD v1, v2, shared_point;
                (v1, v2) = GetAngleVectors(edges[i], edges[(i + 1) % edges.Count], out shared_point);
                //calculate the angle
                double angle = Geometry.CalculateAngleDeg(v1, v2);
                if (angle < min_angle)
                {
                    min_angle = angle;
                    angle_shared_point = shared_point;
                }
            }
            return min_angle;
        }        

        private static bool IsEdgeConnecting(VoronoiEdge edge, VoronoiSite site1, VoronoiSite site2)
        {
            return (edge.Left != null && 
                edge.Right != null) && 
                ((edge.Right == site1 && edge.Left == site2) || (edge.Right == site2 && edge.Left == site1));
        }

        private static (PointD, PointD) GetAngleVectors(VoronoiEdge edge1, VoronoiEdge edge2, out PointD shared_point)
        {
            HashSet<PointD> points = new HashSet<PointD>();
            shared_point = new PointD();
            List<PointD> all_points = [
                new PointD(edge1.Start.X, edge1.Start.Y),
                    new PointD(edge1.End.X, edge1.End.Y),
                    new PointD(edge2.Start.X, edge2.Start.Y),
                    new PointD(edge2.End.X, edge2.End.Y),
                ];
            for (int j = 0; j < all_points.Count; j++)
            {
                if (points.Contains(all_points[j]))
                {
                    shared_point = all_points[j];
                    points.Remove(shared_point);
                }
                points.Add(all_points[j]);
            }
            PointD p1 = points.ToList().First();
            PointD p2 = points.ToList().Last();
            PointD v1 = p1 - shared_point;
            PointD v2 = p2 - shared_point;
            return (v1, v2);
        }
    }
}
