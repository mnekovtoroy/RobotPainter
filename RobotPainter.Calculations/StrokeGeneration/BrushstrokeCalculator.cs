using RobotPainter.Calculations.Brushes;
using SharpVoronoiLib;
using SharpVoronoiLib.Exceptions;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;

namespace RobotPainter.Calculations.StrokeGeneration
{
    public class BrushstrokeCalculator
    {
        private readonly IBrushModel _brushModel;

        public readonly double xResizeCoeff;
        public readonly double yResizeCoeff;

        public BrushstrokeCalculator(double x_resize_coeff, double y_resize_coeff, IBrushModel brushModel)
        {
            xResizeCoeff = x_resize_coeff;
            yResizeCoeff = y_resize_coeff;
            _brushModel = brushModel;
        }

        public List<Point3D> GetBrushPath(BrushstrokeRegions stroke_reg)
        {
            var desired_path = SimpleCalculateDesiredPath(stroke_reg);
            desired_path = ResizeXYcoords(desired_path);
            desired_path = AddRunaways(desired_path);
            var brush_path = _brushModel.CalculateBrushRootPath(desired_path);
            return brush_path;
        }

        public List<Point3D> GetDesiredPath(BrushstrokeRegions stroke_reg)
        {
            var desired_path = SimpleCalculateDesiredPath(stroke_reg);
            desired_path = ResizeXYcoords(desired_path);
            return desired_path;
        }

        private List<Point3D> CalculateDesiredPath(BrushstrokeRegions stroke_reg)
        {
            if(stroke_reg.involvedSites.Count == 1)
            {
                return CalculateSingleSiteDesiredPath(stroke_reg.involvedSites[0]);
            }

            var result = new List<Point3D>();
            //finding starting point
            PointD p1, p2;
            double angle1 = FindNonAdjMinAngle(stroke_reg.involvedSites[0], stroke_reg.involvedSites[1], out p1);
            double angle2 = FindNonAdjMinAngle(stroke_reg.involvedSites[stroke_reg.involvedSites.Count - 1], stroke_reg.involvedSites[stroke_reg.involvedSites.Count - 2], out p2);

            PointD starting_point, ending_point;
            if(angle1 <= angle2)
            {
                starting_point = p1;
                ending_point = p2;
            } else
            {
                stroke_reg.involvedSites.Reverse();
                starting_point = p2;
                ending_point = p1;
            }
            result.Add(new Point3D(starting_point.x, starting_point.y, 0.0));

            var sites = stroke_reg.involvedSites;
            for (int i = 0; i <  sites.Count - 1; i++)
            {
                //calculate z component
                //first, find desired radius
                var curr_c = sites[i].Centroid;
                var next_c = sites[i + 1].Centroid;

                PointD previous_point = new PointD(result.Last().x, result.Last().y);
                PointD current_point = new PointD(curr_c.X, curr_c.Y);
                PointD next_point = new PointD(next_c.X, next_c.Y);

                double r = FindDesiredR(/*previous_point, current_point, next_point,*/ sites[i]);
                //calculating z
                double z = _brushModel.CalculateZCoordinate(r);
                result.Add(new Point3D(current_point.x, current_point.y, z));
            }
            //adding 2 last points
            var prelast_c = sites[sites.Count - 2].Centroid;
            var last_c = sites[sites.Count - 1].Centroid;

            //last point in the middle of the edge
            /*VoronoiEdge edge = GetIntersectingEdge(sites[sites.Count - 1], new PointD(prelast_c.X, prelast_c.Y), new PointD(last_c.X, last_c.Y));
            if (edge == null) throw new ArgumentException("couldnt find edge intersecting ray");
            PointD last_p = new PointD((edge.Start.X + edge.End.X) / 2.0, (edge.Start.Y + edge.End.Y) / 2.0);*/

            double last_r = FindDesiredR(/*new PointD(prelast_c.X, prelast_c.Y), new PointD(last_c.X, last_c.Y), ending_point,*/ sites.Last());
            result.Add(new Point3D(last_c.X, last_c.Y, _brushModel.CalculateZCoordinate(last_r)));
            result.Add(new Point3D(ending_point.x, ending_point.y, 0.0));

            return result;
        }

        private List<Point3D> SimpleCalculateDesiredPath(BrushstrokeRegions stroke_reg)
        {
            if (stroke_reg.involvedSites.Count == 1) 
                return SimpleCalculateSingleSiteDesiredPath(stroke_reg);

            var result = new List<Point3D>();

            for (int i = 0; i < stroke_reg.involvedSites.Count; i++)
            {
                var site_c = stroke_reg.involvedSites[i].Centroid;
                double r = FindDesiredR(stroke_reg.involvedSites[i]);
                double z = _brushModel.CalculateZCoordinate(r);
                result.Add(new Point3D(site_c.X, site_c.Y, z));
            }

            //adding start and end points
            //start
            PointD p0 = new PointD(result[0].x, result[0].y);
            PointD p1 = new PointD(result[1].x, result[1].y);
            PointD v = p0 - p1;
            v = v / Geometry.Norm(v);
            double s = FindDesiredR(stroke_reg.involvedSites[0]);
            double start_coeff = 1.2;
            PointD ps = p0 + v * s * start_coeff;
            result.Insert(0, new Point3D(ps.x, ps.y, 0.0));

            //end
            PointD pn = new PointD(result[result.Count - 1].x, result[result.Count - 1].y);
            PointD pn_ = new PointD(result[result.Count - 2].x, result[result.Count - 2].y);
            v = pn - pn_;
            v = v / Geometry.Norm(v);
            s = FindDesiredR(stroke_reg.involvedSites[0]);
            double end_coeff = 1.2;
            PointD pe = pn + v * s * end_coeff;
            result.Add(new Point3D(pe.x, pe.y, 0.0));

            return result;
        }

        private List<Point3D> CalculateSingleSiteDesiredPath(VoronoiSite site)
        {
            var result = new List<Point3D>();
            var p0 = FindMinAnglePoint(site);

            var centroid = site.Centroid;
            PointD p1 = new PointD(centroid.X, centroid.Y);

            VoronoiEdge edge = GetIntersectingEdge(site, p0, p1);
            if (edge == null)
            {
                throw new ArgumentException("couldnt find edge intersecting ray");
            }
            PointD p2 = new PointD((edge.Start.X + edge.End.X) / 2.0, (edge.Start.Y + edge.End.Y) / 2.0);

            double p1_r = FindDesiredR(/*p0, p1, p2,*/ site);

            result.Add(new Point3D(p0.x, p0.y, 0.0));
            result.Add(new Point3D(p1.x, p1.y, _brushModel.CalculateZCoordinate(p1_r)));
            result.Add(new Point3D(p2.x, p2.y, 0.0));
            return result;
        }

        private List<Point3D> SimpleCalculateSingleSiteDesiredPath(BrushstrokeRegions stroke_reg)
        {
            var result = new List<Point3D>();

            var v = stroke_reg.StartingNorm;

            if(Geometry.Norm(v) == 0)
            {
                v.x = xResizeCoeff >= yResizeCoeff ? 1.0 : 0.0;
                v.y = xResizeCoeff < yResizeCoeff ? 1.0 : 0.0;
            } else
            {
                v = v / Geometry.Norm(v);
            }


            var site = stroke_reg.startingSite;
            var centroid = stroke_reg.startingCentroid;
            double r = FindDesiredR(site);

            var pm = new PointD(centroid.X, centroid.Y);
            //start
            double start_coeff = 1.2;
            PointD ps = pm - v * r * start_coeff;
            result.Add(new Point3D(ps.x, ps.y, 0.0));

            //middle
            double z = _brushModel.CalculateZCoordinate(r);
            result.Add(new Point3D(pm.x, pm.y, z));

            //end
            double end_coeff = 1.2;
            PointD pe = pm + v * r * end_coeff;
            result.Add(new Point3D(pe.x, pe.y, 0.0));

            return result;
        }

        public List<Point3D> ResizeXYcoords(List<Point3D> points)
        {
            return points.Select(p => new Point3D(p.x * xResizeCoeff, p.y * yResizeCoeff, p.z)).ToList();
        }

        private VoronoiEdge GetIntersectingEdge(VoronoiSite site, PointD p0, PointD p1)
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

        private List<Point3D> AddRunaways(List<Point3D> list, double start_angle = 30.0, double end_angle = 60.0, double safe_height = 2.0)
        {
            double start_length, end_length;
            start_length = (start_angle == 90.0 || start_angle == 0.0) ? 0.0 : safe_height / Math.Tan(start_angle * Math.PI / 180.0);
            end_length = (end_angle == 90.0 || end_angle == 0.0) ? 0.0 : safe_height / Math.Tan(end_angle * Math.PI / 180.0);

            double d = Math.Sqrt(Math.Pow(list[0].x - list[1].x, 2) + Math.Pow(list[0].y - list[1].y, 2));
            PointD start_v = new PointD((list[0].x - list[1].x) / d, (list[0].y - list[0].y) / d);

            d = Math.Sqrt(Math.Pow(list[list.Count - 1].x - list[list.Count - 2].x, 2) + Math.Pow(list[list.Count - 1].y - list[list.Count - 2].y, 2));
            PointD end_v = new PointD((list[list.Count - 1].x - list[list.Count - 2].x) / d, (list[list.Count - 1].y - list[list.Count - 2].y) / d);

            PointD start_runaway = (new PointD(list[0].x, list[0].y)) + start_length * start_v;
            PointD end_runaway = (new PointD(list[list.Count - 1].x, list[list.Count - 1].y)) + end_length * end_v;

            list.Insert(0, new Point3D(start_runaway.x, start_runaway.y, safe_height));
            list.Add(new Point3D(end_runaway.x, end_runaway.y, safe_height));

            return list;
        }

        private double FindDesiredR(/*PointD p0, PointD p1, PointD p2,*/ VoronoiSite site)
        {
            //PointD bisector_v = Geometry.GetBisectorVector(p0, p1, p2);

            //double desired_r = double.MaxValue;
            double desired_r = -1.0;
            /*var edges = site.Cell;

            foreach (var edge in edges)
            {

                PointD? intersection1 = Geometry.GetRaySegmentIntersectionPoint(
                    p1.x, p1.y, bisector_v.x, bisector_v.y,
                    edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y);
                PointD? intersection2 = Geometry.GetRaySegmentIntersectionPoint(
                    p1.x, p1.y, bisector_v.x, bisector_v.y,
                    edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y);

                if (intersection1.HasValue)
                {
                    double r = Math.Sqrt(Math.Pow((intersection1.Value.x - p1.x) * xResizeCoeff, 2) + Math.Pow((intersection1.Value.y - p1.y) * yResizeCoeff, 2));
                    if (r > desired_r)
                    {
                        desired_r = r;
                    }
                }
                if (intersection2.HasValue)
                {
                    double r = Math.Sqrt(Math.Pow((intersection2.Value.x - p1.x) * xResizeCoeff, 2) + Math.Pow((intersection2.Value.y - p1.y) * yResizeCoeff, 2));
                    if (r > desired_r)
                    {
                        desired_r = r;
                    }
                }
            }
            if (desired_r == -1)
            {
                throw new Exception("Cant find desired radius");
            }
            double max_r = 3.5;
            desired_r = desired_r < max_r ? desired_r : max_r;*/
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
            double overlap = 0.2;
            desired_r += overlap;
            double max_r = 3.5;
            desired_r = desired_r < max_r ? desired_r : max_r;
            return desired_r;
        }

        private PointD FindMinAnglePoint(VoronoiSite site)
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

        private double FindNonAdjMinAngle(VoronoiSite site, VoronoiSite adj_site, out PointD angle_shared_point)
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

        private bool IsEdgeConnecting(VoronoiEdge edge, VoronoiSite site1, VoronoiSite site2)
        {
            return (edge.Left != null && 
                edge.Right != null) && 
                ((edge.Right == site1 && edge.Left == site2) || (edge.Right == site2 && edge.Left == site1));
        }

        private (PointD, PointD) GetAngleVectors(VoronoiEdge edge1, VoronoiEdge edge2, out PointD shared_point)
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
