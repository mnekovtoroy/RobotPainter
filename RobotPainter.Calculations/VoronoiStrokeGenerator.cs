using RobotPainter.Calculations.Optimization;
using SharpVoronoiLib;

namespace RobotPainter.Calculations
{
    public class VoronoiStrokeGenerator
    {
        public static List<VoronoiSite> GenerateRandomMesh(int n, int width, int height, out List<(double, double)> points)
        {
            VoronoiPlane plane = new VoronoiPlane(0, 0, width - 1, height - 1);
            plane.GenerateRandomSites(n, PointGenerationMethod.Uniform);
            points = plane.Sites.Select(s => (s.X, s.Y)).ToList();
            plane.Tessellate();
            return plane.Sites;
        }

        public List<(double, double)> orig_points;
        public List<VoronoiSite> sites;
        public double[,] u;
        public double[,] v;

        private int width;
        private int height;

        private readonly IOptimizer _optimizer;

        public VoronoiStrokeGenerator(int n_strokes, double[,] u, double[,] v, IOptimizer optimizer)
        {
            this.u = u;
            this.v = v;
            width = u.GetLength(0);
            height = u.GetLength(1);
            sites = GenerateRandomMesh(n_strokes, width, height, out orig_points);
            
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
            double w1 = 1.0, w2 = 1.0, w3 = 1.0;
            double cost = w1 * ColorError(site) + w2 * DirectionalError(site) + w3 * ShapeError(site);
            sites.Remove(site);
            return cost;
        }

        private double ColorError(VoronoiSite site)
        {
            throw new NotImplementedException();
        }

        private double DirectionalError(VoronoiSite site)
        {
            throw new NotImplementedException();
        }

        private double ShapeError(VoronoiSite site)
        {
            throw new NotImplementedException();
        }
    }
}
