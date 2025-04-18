using RobotPainter.Calculations.Brushes;
using SharpVoronoiLib;
using SharpVoronoiLib.Exceptions;
using System.ComponentModel;
using System.Drawing;

namespace RobotPainter.Calculations.StrokeGeneration
{
    public class BrushstrokeDrawer
    {
        private readonly BasicBrushModel brushModel;

        public BrushstrokeDrawer()
        {

        }

        public LabBitmap DrawBrushstroke(BrushstrokeRegions stroke_reg, int width, int height, double real_width, double real_height)
        {
            return null;
        }

        //to do: 1-region strokes
        public List<Point3D> CalculateDesiredPath(BrushstrokeRegions stroke_reg)
        {
            if(stroke_reg.involvedSites.Count == 1)
            {
                return CalculateSingleSiteDesiredPath(stroke_reg.involvedSites[0]);
            }

            var result = new List<Point3D>();
            //finding starting point
            PointD p1, p2;
            double angle1 = FindNonAdjMinAngle(stroke_reg.involvedSites[0], stroke_reg.involvedSites[1], out p1);
            double angle2 = FindNonAdjMinAngle(stroke_reg.involvedSites[stroke_reg.involvedSites.Count - 2], stroke_reg.involvedSites[stroke_reg.involvedSites.Count - 1], out p2);

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

                double r = FindDesiredR(previous_point, current_point, next_point, sites[i]);
                //calculating z
                double z = brushModel.z(r);
                result.Add(new Point3D(current_point.x, current_point.y, z));
            }
            //adding 2 last points
            var prelast_c = sites[sites.Count - 2].Centroid;
            var last_c = sites[sites.Count - 1].Centroid;
            double last_r = FindDesiredR(new PointD(prelast_c.X, prelast_c.Y), new PointD(last_c.X, last_c.Y), ending_point, sites.Last());
            result.Add(new Point3D(last_c.X, last_c.Y, brushModel.z(last_r)));
            result.Add(new Point3D(ending_point.x, ending_point.y, 0.0));

            return AddRunaways(result);
        }

        private List<Point3D> CalculateSingleSiteDesiredPath(VoronoiSite site)
        {
            var result = new List<Point3D>();
            var min_angle_p = FindMinAnglePoint(site);
            result.Add(new Point3D(min_angle_p.x, min_angle_p.y, 0.0));

            throw new NotImplementedException();

            return AddRunaways(result);
        }

        private List<Point3D> AddRunaways(List<Point3D> list)
        {
            return list;
        }

        private double FindDesiredR(PointD p0, PointD p1, PointD p2, VoronoiSite site)
        {
            PointD bisector_v = Geometry.GetBisectorVector(p0, p1, p2);

            double desired_r = double.MaxValue;
            var edges = site.Cell;

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
                    double r = Math.Sqrt(Math.Pow(intersection1.Value.x - p1.x, 2) + Math.Pow(intersection1.Value.y - p1.y, 2));
                    if (r < desired_r)
                    {
                        desired_r = r;
                    }
                }
                if (intersection2.HasValue)
                {
                    double r = Math.Sqrt(Math.Pow(intersection2.Value.x - p1.x, 2) + Math.Pow(intersection2.Value.y - p1.y, 2));
                    if (r < desired_r)
                    {
                        desired_r = r;
                    }
                }
            }
            if(desired_r == double.MaxValue)
            {
                throw new Exception("Cant find desired radius");
            }
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
