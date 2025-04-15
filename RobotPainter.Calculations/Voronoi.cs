using SharpVoronoiLib;

namespace RobotPainter.Calculations
{
    public class Voronoi
    {
        public static List<VoronoiSite> GenerateRandomMesh(int n, int width, int height)
        {
            VoronoiPlane plane = new VoronoiPlane(0, 0, width - 1, height - 1);
            plane.GenerateRandomSites(n, PointGenerationMethod.Uniform);
            //var start = DateTime.Now;
            List<VoronoiEdge> edges = plane.Tessellate();
            //var end = DateTime.Now;
            //Console.WriteLine($"time to calculate voronoi for {n} points: {Math.Round((end-start).TotalMilliseconds, 3)} ms");
            return plane.Sites;
        }
    }
}
