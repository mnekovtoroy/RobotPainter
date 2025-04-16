using RobotPainter.Calculations.Optimization;
using SharpVoronoiLib;

namespace RobotPainter.Calculations
{
    public class VoronoiStrokeGenerator
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
        private double[] sites_Lavg;
        private int[] sites_pixel_count;

        private int width;
        private int height;

        private readonly IOptimizer _optimizer;

        public VoronoiStrokeGenerator(int n_strokes, LabBitmap bmp, double[,] u, double[,] v, IOptimizer optimizer)
        {
            this.lbmp = bmp;
            this.u = u;
            this.v = v;
            width = u.GetLength(0);
            height = u.GetLength(1);
            sites = GenerateRandomMesh(n_strokes, width, height);
            
            _optimizer = optimizer;
        }

        public void Optimize(int k)
        {

        }

        private double CostFunction(double x, double y)
        {
            //cost = color_err + direction_err + form_err + constraints
            var site = new VoronoiSite(x, y);
            sites.Add(site);
            VoronoiPlane.TessellateOnce(sites, 0, 0, width - 1, height - 1);
            double w1 = 0.33, w2 = 0.33, w3 = 0.33;
            double cost = w1 * ColorCost(site) + w2 * DirectionalCost(site) + w3 * ShapeCost(site);
            sites.Remove(site);
            return cost;
        }

        private double ColorCost(VoronoiSite site)
        {
            throw new NotImplementedException();
        }

        private double DirectionalCost(VoronoiSite site)
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

        private double ShapeCost(VoronoiSite site)
        {
            //area of enclosing circle
            VoronoiPoint p1, p2;
            (p1, p2) = FindLongestDiameter(site);
            double d = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            double area_c = Math.PI * d * d / 4.0;
            //area of the polygon
            double area_p = CalculateArea(site);
            //shape cost
            double cost = 1.0 - area_p / area_c;
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

        private void CalculateSitesLAvg()
        {
            if(mask == null)
            {
                throw new Exception("mask must be pre-computed");
            }

            sites_Lavg = new double[sites.Count];
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
        }

        private List<(double, double)> CalculateSitesLPull()
        {
            if (mask == null || sites_Lavg == null || sites_pixel_count == null)
            {
                throw new Exception("mask, Lavg and sites_pixel_count must be pre-computed");
            }

            var pull = new List<(double, double)>(sites.Count);
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    double L_diff = Math.Abs(lbmp.GetPixel(i, j).L - sites_Lavg[mask[i, j]]);
                    double pull_x = i * (L_diff / sites_pixel_count[mask[i, j]]);
                    double pull_y = i * (L_diff / sites_pixel_count[mask[i, j]]);
                    pull[i] = (pull[i].Item1 + pull_x, pull[i].Item2 + pull_y);
                }
            }
            return pull;
        }

        public int[,] MaskPointsToNearestSites()
        {
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
