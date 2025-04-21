using RobotPainter.Calculations.Core;
using RobotPainter.Calculations.Optimization;
using SharpVoronoiLib;

namespace RobotPainter.Calculations
{
    public class LegacyVoronoiStrokeGenerator
    {
        public static List<VoronoiSite> GenerateRandomMesh(int n, int width, int height)
        {
            VoronoiPlane plane = new VoronoiPlane(0, 0, width - 1, height - 1);
            plane.GenerateRandomSites(n, PointGenerationMethod.Uniform);
            plane.Tessellate();
            return plane.Sites;
        }

        public List<VoronoiSite> sites;

        public LabBitmap lbmp;
        public double[,] u;
        public double[,] v;

        //private double[,] L_diff;
        private int[,] mask;

        private int width;
        private int height;

        private readonly IOptimizer _optimizer;

        public LegacyVoronoiStrokeGenerator(int n_strokes, LabBitmap bmp, double[,] u, double[,] v, IOptimizer optimizer)
        {
            this.lbmp = bmp;
            this.u = u;
            this.v = v;
            width = u.GetLength(0);
            height = u.GetLength(1);
            sites = GenerateRandomMesh(n_strokes, width, height);
            
            _optimizer = optimizer;
        }

        public void Optimize1(int iterations)
        {
            for(int k = 0; k < iterations; k++)
            {
                var sites_pull = new List<(double, double)>();
                //optimize every site 1 by 1, pull them all at once
                for(int i = 0; i < sites.Count; i++)
                {
                    var curr_site = sites[0];
                    sites.RemoveAt(0);

                    double new_x, new_y;
                    (new_x, new_y) = _optimizer.Optimize(CostFunction, curr_site.X, curr_site.Y);
                    sites_pull.Add((new_x - curr_site.X, new_y - curr_site.Y));

                    sites.Add(curr_site);
                }
                PullSites(sites_pull);
            }
        }

        public void Optimize2(int iterations)
        {
            for (int k = 0; k < iterations; k++)
            {
                var sites_pull = new List<(double, double)>();
                //optimizing countering with pulling
                for (int i = 0; i < sites.Count; i++)
                {
                    var curr_site = sites[0];
                    sites.RemoveAt(0);

                    double new_x, new_y;
                    (new_x, new_y) = _optimizer.Optimize(CostFunction, curr_site.X, curr_site.Y);
                    sites_pull.Add((new_x - curr_site.X, new_y - curr_site.Y));

                    sites.Add(curr_site);
                }
                PullSites(sites_pull);
                PerformLPull(str_Lpull: 0.0, str_centroid: 0.7);
            }
        }

        public void Optimize3(int iterations)
        {
            for (int k = 0; k < iterations; k++)
            {
                var sites_pull = new List<(double, double)>();
                //optimizing countering with pulling
                for (int i = 0; i < sites.Count; i++)
                {
                    var curr_site = sites[0];
                    sites.RemoveAt(0);

                    double new_x, new_y;
                    (new_x, new_y) = _optimizer.Optimize(CostFunction, curr_site.X, curr_site.Y);
                    sites_pull.Add(((new_x - curr_site.X), (new_y - curr_site.Y)));

                    sites.Add(curr_site);
                }
                PullSites(sites_pull);
                PerformLPull(str_Lpull: 0.3, str_centroid: 0.3);
            }
        }

        public void Optimize5(int iterations)
        {
            for (int k = 0; k < iterations; k++)
            {
                //optimizing countering with pulling
                for (int i = 0; i < sites.Count; i++)
                {
                    var curr_site = sites[0];
                    sites.RemoveAt(0);

                    double new_x, new_y;
                    (new_x, new_y) = _optimizer.Optimize(CostFunction, curr_site.X, curr_site.Y);

                    sites.Add(new VoronoiSite(new_x, new_y));
                }
            }
            VoronoiPlane.TessellateOnce(sites, 0, 0, width - 1, height - 1);
            mask = null;
        }

        public double CostFunction(double x, double y)
        {
            //cost = (1 + direction error + shape error) * color error
            var site = new VoronoiSite(x, y);
            var opt_sites = sites.Select(s => new VoronoiSite(s.X, s.Y)).ToList();
            opt_sites.Add(site);
            VoronoiPlane.TessellateOnce(opt_sites, 0, 0, width - 1, height - 1);

            //out of bounds cost penalty
            double penalty = 0.0;
            if(x < 0 || x > (width - 1) || y < 0 || y > (height - 1))
            {
                penalty += 1000;
            }

            if(penalty > 0)
            {
                return penalty;
            }

            double cost = (0.5 + 1.5 * DirectionalCost(site) + 3.0 * ShapeCost(site)) * ColorCost(site); // 1,2
            //double cost = 100.0 * DirectionalCost(site) + 100.0 * ShapeCost(site); // 3
            //double cost = ColorCost(site) + 50.0 * DirectionalCost(site) + 50 * ShapeCost(site); //4
            return cost;
        }

        public double ColorCost(VoronoiSite site)
        {
            int p_count;
            var Lavg = CalculateSiteLAvg(site, out p_count);
            return CalculateSiteLStd(site, Lavg, p_count);
        }

        public double DirectionalCost(VoronoiSite site)
        {
            //normilized vector of the longest diameter
            VoronoiPoint p1, p2;
            (p1, p2) = FindLongestDiameter(site);
            double d = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            double v_x = (p2.X - p1.X) / d;
            double v_y = (p2.Y - p1.Y) / d;
            //norm to L vector in centroid
            double vn_x = u[Convert.ToInt32(site.Centroid.X), Convert.ToInt32(site.Centroid.Y)];
            double vn_y = v[Convert.ToInt32(site.Centroid.X), Convert.ToInt32(site.Centroid.Y)];
            //scalar multiplication
            double scalar = v_x * vn_x + v_y * vn_y;
            //calculating cost
            double cost = 1.0 - Math.Abs(scalar);
            return cost;
        }

        public double ShapeCost(VoronoiSite site)
        {
            //area of enclosing circle
            VoronoiPoint p1, p2;
            (p1, p2) = FindLongestDiameter(site);
            double d = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            double area_c = Math.PI * d * d / 4.0;
            //area of the polygon
            double area_p = CalculateArea(site);
            //shape cost
            double cost = area_p / area_c;
            return cost;
        }

        private static (VoronoiPoint, VoronoiPoint) FindLongestDiameter(VoronoiSite site)
        {
            var points = site.ClockwisePoints.ToList();

            double max_d = -1.0;
            int max_p1 = -1, max_p2 = -1;
            for(int i  = 0; i < points.Count - 2; i++)
            {
                for(int j = i + 2; j < points.Count; j++)
                {
                    double d = Math.Sqrt(Math.Pow(points[j].X - points[i].X, 2) + Math.Pow(points[j].Y - points[i].Y, 2));
                    if(d > max_d)
                    {
                        max_d = d;
                        max_p1 = i;
                        max_p2 = j;
                    }
                }
            }
            return (points[max_p1], points[max_p2]);
        }

        private static double CalculateArea(VoronoiSite site)
        {
            //formula: https://erkaman.github.io/posts/area_convex_polygon.html
            double area = 0.0;
            var points = site.ClockwisePoints.Reverse().ToList();
            for(int i = 0; i < points.Count;  i++)
            {
                int j = (i + 1) % points.Count;
                area += points[i].X * points[j].Y - points[j].X * points[i].Y;
            }
            area /= 2.0;
            return area;
        }

        private double[] CalculateAllSitesLAvg(out int[] sites_pixel_count)
        {
            if(mask == null)
            {
                throw new Exception("mask must be pre-computed");
            }

            double [] sites_Lavg = new double[sites.Count];
            sites_pixel_count = new int[sites.Count];
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    sites_Lavg[mask[i, j]] += lbmp.GetPixel(i, j).L;
                    sites_pixel_count[mask[i, j]]++;
                }
            }
            for(int i = 0; i < sites.Count; i++)
            {
                sites_Lavg[i] = sites_Lavg[i] / sites_pixel_count[i];
            }
            return sites_Lavg;
        }

        private double CalculateSiteLAvg(VoronoiSite site, out int site_pixel_count)
        {
            //deteremning bounding box
            int xmax, xmin, ymax, ymin;
            xmax = Convert.ToInt32(site.Points.Max(p => p.X));
            xmin = Convert.ToInt32(site.Points.Min(p => p.X));
            ymax = Convert.ToInt32(site.Points.Max(p => p.Y));
            ymin = Convert.ToInt32(site.Points.Min(p => p.Y));

            double Lavg = 0.0;
            site_pixel_count = 0;
            for (int i = xmin; i <= xmax; i++)
            {
                for (int j = ymin; j <= ymax; j++)
                {
                    if(site.Contains(i,j))
                    {
                        Lavg += lbmp.GetPixel(i, j).L;
                        site_pixel_count++;
                    }
                }
            }
            for (int i = 0; i < sites.Count; i++)
            {
                Lavg = Lavg / site_pixel_count;
            }
            return Lavg;
        }

        private double CalculateSiteLStd(VoronoiSite site, double Lavg, int p_count)
        {
            //deteremning bounding box
            int xmax, xmin, ymax, ymin;
            xmax = Convert.ToInt32(site.Points.Max(p => p.X));
            xmin = Convert.ToInt32(site.Points.Min(p => p.X));
            ymax = Convert.ToInt32(site.Points.Max(p => p.Y));
            ymin = Convert.ToInt32(site.Points.Min(p => p.Y));

            double error = 0.0;
            for(int i = xmin; i <= xmax; i++)
            {
                for(int j = ymin; j <= ymax; j++)
                {
                    if(site.Contains(i,j))
                    {
                        error += Math.Pow(lbmp.GetPixel(i, j).L - Lavg, 2) / p_count;
                    }
                }
            }
            error = Math.Sqrt(error);
            return error;
        }

        private List<(double, double)> CalculateSitesLPull(double[] sites_Lavg, int[] sites_pixel_count)
        {
            if (mask == null)
            {
                throw new Exception("mask must be pre-computed");
            }

            //var pull = new List<(double, double)>(sites.Count);
            List<(double, double)> Lpull = sites.Select(s => (0.0, 0.0)).ToList();
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    double L_diff = Math.Abs(lbmp.GetPixel(i, j).L - sites_Lavg[mask[i, j]]);
                    double pull_x = -(i - sites[mask[i, j]].X) * (L_diff / sites_pixel_count[mask[i, j]]);
                    double pull_y = -(j - sites[mask[i, j]].Y) * (L_diff / sites_pixel_count[mask[i, j]]);
                    Lpull[mask[i, j]] = (Lpull[mask[i,j]].Item1 + pull_x, Lpull[mask[i, j]].Item2 + pull_y);
                }
            }
            return Lpull;
        }

        private void PullSites(List<(double, double)> sites_pull)
        {
            var new_sites = new List<VoronoiSite>();
            for(int i = 0;i < sites.Count; i++)
            {
                new_sites.Add(new VoronoiSite(sites[i].X + sites_pull[i].Item1, sites[i].Y + sites_pull[i].Item2));
            }
            sites = new_sites;
            VoronoiPlane.TessellateOnce(sites, 0, 0, width - 1, height - 1);
            mask = null;
        }

        public void PerformLPull(double str_Lpull = 0.2, double str_centroid = 0.5)
        {
            int[] sites_pixel_count;
            MaskPointsToNearestSites();
            var sites_Lavg = CalculateAllSitesLAvg(out sites_pixel_count);
            var Lpull = CalculateSitesLPull(sites_Lavg, sites_pixel_count);
            for (int i = 0; i < sites.Count; i++)
            {
                //pull to centroid + away from color diff
                double pull_x = str_Lpull * Lpull[i].Item1 + str_centroid * (sites[i].Centroid.X - sites[i].X);
                double pull_y = str_Lpull * Lpull[i].Item2 + str_centroid * (sites[i].Centroid.X - sites[i].X);
                Lpull[i] = (pull_x, pull_y);
            }
            PullSites(Lpull);
        }

        public int[,] MaskPointsToNearestSites()
        {
            if(mask != null)
            {
                return mask;
            }
            /*Consider alternative solution:
             1. find which site contains [0,0]
             2. for every next point, only check neighboring sites*/
            int[,] result = new int[width, height];
            //mask every point 
            for(int x = 0; x <  width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    result[x, y] = -1;
                    for(int i = 0; i < sites.Count; i++)
                    {
                        if (sites[i].Contains(x, y))
                        {
                            result[x, y] = i;
                            break;
                        }
                    }
                    if (result[x, y] == -1)
                    {
                        double min_d = double.MaxValue;
                        for (int i = 0; i < sites.Count; i++)
                        {
                            double d = Math.Sqrt(Math.Pow(sites[i].X - x, 2) + Math.Pow(sites[i].Y - y, 2));
                            if (d < min_d)
                            {
                                result[x, y] = i;
                                min_d = d;
                            }
                        }
                    }
                }
            }
            mask = result;
            return mask;
        }
    }
}
