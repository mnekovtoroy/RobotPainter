using RobotPainter.Calculations.Optimization;
using SharpVoronoiLib;

namespace RobotPainter.Calculations.StrokeGeneration
{
    public class StrokeGenerator
    {
        public static List<VoronoiSite> GenerateRandomRelaxedMesh(int n, int width, int height, int relax_iterations = 3)
        {
            VoronoiPlane plane = new VoronoiPlane(0, 0, width - 1, height - 1);
            plane.GenerateRandomSites(n, PointGenerationMethod.Uniform);
            plane.Tessellate();
            plane.Relax(relax_iterations);
            return plane.Sites;
        }

        public List<VoronoiSite> sites;

        private Dictionary<VoronoiSite, Brushstroke> siteToStroke;

        public LabBitmap image;
        public double[,] u;
        public double[,] v;

        private int width;
        private int height;

        public double real_height_mm;
        public double real_width_mm;

        public double real_stroke_max_length_mm;
        public double StrokeMaxLength { get { return real_stroke_max_length_mm * width / real_width_mm; } }

        public double Ltol = 0.5;

        private readonly IOptimizer _optimizer;

        private List<Brushstroke> strokes = new List<Brushstroke>();

        public StrokeGenerator(LabBitmap target_image, int n_voronoi, IOptimizer optimizer, int n_rolling_avg = 7)
        {
            image = target_image;
            (u,v) = ImageProcessor.LNormWithRollAvg(image, n_rolling_avg);
            width = image.Width;
            height = image.Height;
            _optimizer = optimizer;

            sites = GenerateRandomRelaxedMesh(n_voronoi, width, height);
            siteToStroke = new Dictionary<VoronoiSite, Brushstroke>();
        }

        public void CalculateStorkes()
        {
            ClearSitesList();
            siteToStroke.Clear();
            var unprocessed_sites = sites.Select(x => x).ToList();
            while(unprocessed_sites.Count > 0)
            {
                var curr_site = unprocessed_sites[0];
                var stroke = GenerateBrushstroke(curr_site);
                //reserving the sites
                foreach (var site in stroke.involvedSites) {
                    unprocessed_sites.Remove(site);
                    siteToStroke.Add(site, stroke);
                }
            }
        }

        private Brushstroke GenerateBrushstroke(VoronoiSite startin_point)
        {
            var stroke = new Brushstroke(this, startin_point);
            stroke.GenerateStroke(StrokeMaxLength, 1.0, Ltol);
            return stroke;
        }

        public bool IsSiteReserved(VoronoiSite site)
        {
            return siteToStroke.ContainsKey(site);
        }

        private void ClearSitesList()
        {
            sites.RemoveAll(x => x.Cell == null);
        }

        private List<(double, double)> CalculateSitesLPull()
        {
            List<(double, double)> Lpull = new List<(double, double)>();
            foreach(var site in sites)
            {
                double x_pull = 0.0, y_pull = 0.0;
                //possible optimization: use site.Cell instead of points
                var points = site.Points;
                var centroid = site.Centroid;
                int cx = Convert.ToInt32(centroid.X);
                int cy = Convert.ToInt32(centroid.Y);
                foreach(var point in site.Points)
                {
                    int px = Convert.ToInt32(point.X);
                    int py = Convert.ToInt32(point.Y);
                    double L_diff = Math.Abs(image.GetPixel(px, py).L - image.GetPixel(cx, cy).L);
                    x_pull += -(point.X - centroid.X) * L_diff / points.Count();
                    y_pull += -(point.Y - centroid.Y) * L_diff / points.Count();
                }
                Lpull.Add((x_pull, y_pull));
            }
            return Lpull;
        }

        private void PullSites(List<(double, double)> sites_pull, double strength = 1.0)
        {
            var new_sites = new List<VoronoiSite>();
            for (int i = 0; i < sites.Count; i++)
            {
                new_sites.Add(new VoronoiSite(sites[i].X + strength * sites_pull[i].Item1, sites[i].Y + strength * sites_pull[i].Item2));
            }
            sites = new_sites;
            VoronoiPlane.TessellateOnce(sites, 0, 0, width - 1, height - 1);
        }

        public void PerformLPull(double str_Lpull = 0.2, double str_centroid = 0.5)
        {
            var Lpull = CalculateSitesLPull();
            for (int i = 0; i < sites.Count; i++)
            {
                //pull to centroid + away from color diff
                var centroid = sites[i].Centroid;
                double pull_x = str_Lpull * Lpull[i].Item1 + str_centroid * (centroid.X - sites[i].X);
                double pull_y = str_Lpull * Lpull[i].Item2 + str_centroid * (centroid.X - sites[i].X);
                Lpull[i] = (pull_x, pull_y);
            }
            PullSites(Lpull);
        }

        public LabBitmap GetColoredStrokeMap()
        {
            var result = new LabBitmap(width, height);
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    bool value_found = false;
                    for (int i = 0; i < sites.Count; i++)
                    {
                        if (sites[i].Contains(x, y))
                        {
                            result.SetPixel(x, y, siteToStroke[sites[i]].MainColor);
                            value_found = true;
                            break;
                        }
                    }
                    if (!value_found)
                    {
                        double min_d = double.MaxValue;
                        for (int i = 0; i < sites.Count; i++)
                        {
                            double d = Math.Sqrt(Math.Pow(sites[i].X - x, 2) + Math.Pow(sites[i].Y - y, 2));
                            if (d < min_d)
                            {
                                result.SetPixel(x, y, siteToStroke[sites[i]].MainColor);
                                min_d = d;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}