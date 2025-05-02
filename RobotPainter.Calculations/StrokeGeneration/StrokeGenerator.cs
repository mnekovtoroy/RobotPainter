using RobotPainter.Core;
using RobotPainter.Calculations.ImageProcessing;
using SharpVoronoiLib;

namespace RobotPainter.Calculations.StrokeGeneration
{
    public class StrokeGenerator
    {
        public class Options
        {
            public double ErrorLeniency = 3.0;

            public int RelaxationIterations = 5;
            public int LpullIterations = 2;
            public double LpullMaxStep = 2.0;
            public int RollingAverageN = 9;
        }

        public static List<VoronoiSite> GenerateRandomRelaxedMesh(int n, int width, int height, int relax_iterations = 3)
        {
            VoronoiPlane plane = new VoronoiPlane(0, 0, width - 1, height - 1);
            plane.GenerateRandomSites(n, PointGenerationMethod.Uniform);
            plane.Tessellate();
            plane.Relax(relax_iterations);
            return plane.Sites;
        }

        public static int CalculateDesiredVoronoiN(double canvas_width, double canvas_height, double stroke_width, double overlap)
        {
            const double safety_overhead = 4;

            double non_overlapped = stroke_width - 2.0 * overlap;
            return Convert.ToInt32(((canvas_width * canvas_height) / (non_overlapped * non_overlapped)) * safety_overhead);
        }

        public List<VoronoiSite> sites;

        private Dictionary<VoronoiSite, StrokeSites> siteToStroke;

        public LabBitmap image;
        public double[,] u;
        public double[,] v;

        Dictionary<VoronoiSite, bool> sitesToPaint;

        private int width;
        private int height;

        Options options;

        public List<StrokeSites> strokes = new List<StrokeSites>();

        public StrokeGenerator(LabBitmap target_image, int n_voronoi, bool[,] is_painted, double[,] error, Options options)
        {
            image = target_image;
            (u,v) = ImageProcessor.LNormWithRollAvg(image, options.RollingAverageN);
            width = image.Width;
            height = image.Height;

            this.options = options;

            sites = GenerateRandomRelaxedMesh(n_voronoi, width, height, options.RelaxationIterations);
            Lfit(options.LpullIterations, options.LpullMaxStep);
            sites = sites.OrderByDescending(s => image.GetPixel(Convert.ToInt32(s.Centroid.X), Convert.ToInt32(s.Centroid.Y)).L).ToList();
            
            sitesToPaint = CalculateSitesToPaint(is_painted, error);
            
            unassigned_sites = sites.Where(s => sitesToPaint[s]).ToList();
            siteToStroke = new Dictionary<VoronoiSite, StrokeSites>();
        }

        private Dictionary<VoronoiSite, bool> CalculateSitesToPaint(bool[,] is_painted, double[,] error)
        {
            var mask = GetVoronoiMask();

            var total_error = new Dictionary<VoronoiSite, double>();
            var pixel_count = new Dictionary<VoronoiSite, int>();

            var result = sites.Select(s => new KeyValuePair<VoronoiSite, bool>(s, false)).ToDictionary();

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    var site = sites[mask[x, y]];

                    if (result[site])
                        continue;

                    if (!is_painted[x,y])
                    {
                        result[site] = true;
                        if(total_error.ContainsKey(site))
                        {
                            total_error.Remove(site);
                            pixel_count.Remove(site);
                        }
                        continue;
                    }

                    if(total_error.ContainsKey(site))
                    {
                        total_error[site] += error[x, y];
                        pixel_count[site]++;
                    }
                    else
                    {
                        total_error.Add(site, error[x, y]);
                        pixel_count.Add(site, 1);
                    }
                }
            }
            foreach(var site_error in total_error)
            {
                var avg_error = site_error.Value / pixel_count[site_error.Key];
                if(avg_error > options.ErrorLeniency)
                {
                    result[site_error.Key] = true;
                }
            }
            return result;
        }

        public void Lfit(int iterations, double max_step_per_i)
        {
            for (int k = 0; k < iterations; k++)
            {
                //optimizing countering with pulling
                for (int i = 0; i < sites.Count; i++)
                {
                    var curr_site = sites[0];
                    sites.RemoveAt(0);

                    double x_pull, y_pull;
                    (x_pull, y_pull) = CalculateSiteLPull(curr_site);

                    double pull_length = Math.Sqrt(x_pull * x_pull + y_pull * y_pull);
                    if (pull_length > max_step_per_i)
                    {
                        x_pull = x_pull * max_step_per_i / pull_length;
                        y_pull = y_pull * max_step_per_i / pull_length;
                    }
                    if (curr_site.X + x_pull <= 0 || curr_site.X + x_pull >= width - 1)
                        x_pull = 0;
                    if (curr_site.Y + y_pull <= 0 || curr_site.Y + y_pull >= width - 1)
                        y_pull = 0;

                    sites.Add(new VoronoiSite(curr_site.X + x_pull, curr_site.Y + y_pull));
                }
                VoronoiPlane plane = new VoronoiPlane(0, 0, width - 1, height - 1);
                plane.SetSites(sites);
                plane.Tessellate();
                plane.Relax(strength: 0.5f);
                
            }
            unassigned_sites = sites.Select(x => x).ToList();
        }

        List<VoronoiSite> unassigned_sites;
        public List<StrokeSites> CalculateAllStorkes(StrokeSitesBuilder.Options strokeSitesBuilderOptions)
        {
            ClearSitesList();
            siteToStroke.Clear();
            strokes.Clear();
            while(!AreAllSitesAssigned())
            {
                GetNextStrokeSites(strokeSitesBuilderOptions);
            }
            return strokes;
        }

        public StrokeSites GetNextStrokeSites(StrokeSitesBuilder.Options strokeSitesBuilderOptions)
        {
            if(AreAllSitesAssigned())
            {
                return null;
            }
            var curr_site = unassigned_sites[0];
            var stroke = GenerateBrushstroke(curr_site, strokeSitesBuilderOptions);
            //reserving the sites
            foreach (var site in stroke.involvedSites)
            {
                unassigned_sites.Remove(site);
                siteToStroke.Add(site, stroke);
            }
            strokes.Add(stroke);
            return stroke;
        }

        public bool AreAllSitesAssigned() => unassigned_sites.Count == 0;

        private StrokeSites GenerateBrushstroke(VoronoiSite starting_site, StrokeSitesBuilder.Options strokeSitesBuilderOptions)
        {
            var stroke = StrokeSitesBuilder.GenerateStrokeSites(this, starting_site, strokeSitesBuilderOptions);
            return stroke;
        }

        public bool IsSiteReserved(VoronoiSite site)
        {
            //return !unassigned_sites.Contains(site);
            return siteToStroke.ContainsKey(site);
        }

        public bool IsSiteToPaint(VoronoiSite site)
        {
            return sitesToPaint.ContainsKey(site) && sitesToPaint[site];
        }

        private void ClearSitesList()
        {
            sites.RemoveAll(x => x.Cell == null);
        }

        private (double, double) CalculateSiteLPull(VoronoiSite site)
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
            return (x_pull, y_pull);
        }

        public LabBitmap GetColoredStrokeMap()
        {
            var result = new LabBitmap(width, height);
            var mask = GetVoronoiMask();
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    result.SetPixel(x, y, siteToStroke[sites[mask[x, y]]].MainColor);
                }
            }
            return result;
        }

        public int[,] GetVoronoiMask(List<VoronoiSite> from_sites = null)
        {
            if(from_sites == null)
            {
                from_sites = sites;
            }

            var result = new int[width, height];
            //find first pixel
            double min_d = double.MaxValue;
            int last_site_i = -1;
            for (int i = 0; i < from_sites.Count; i++)
            {
                double d = Math.Sqrt(Math.Pow(from_sites[i].X, 2) + Math.Pow(from_sites[i].Y, 2));
                if (d < min_d)
                {
                    result[0, 0] = i;
                    last_site_i = i;
                    min_d = d;
                }
            }
            //for every pixel after only check cells that are neighbors to the last found cell
            for(int x = 0; x < width; x++)
            {
                //first go ->
                for(int y = 0; y < height; y++)
                {
                    min_d = Math.Sqrt(Math.Pow(from_sites[last_site_i].X - x, 2) + Math.Pow(from_sites[last_site_i].Y - y, 2));
                    var sites_to_check = from_sites[last_site_i].Neighbours;
                    result[x, y] = last_site_i;
                    foreach (var site in sites_to_check)
                    {
                        double d = Math.Sqrt(Math.Pow(site.X - x, 2) + Math.Pow(site.Y - y, 2));                         
                        if (d < min_d)
                        {
                            int i = from_sites.IndexOf(site);
                            result[x, y] = i;
                            last_site_i = i;
                            min_d = d;
                        }
                    }
                }
                //second go <-
                x++;
                if(x >= width)
                {
                    break;
                }
                for (int y = height - 1; y >= 0; y--)
                {
                    min_d = Math.Sqrt(Math.Pow(from_sites[last_site_i].X - x, 2) + Math.Pow(from_sites[last_site_i].Y - y, 2));
                    var sites_to_check = from_sites[last_site_i].Neighbours;
                    result[x, y] = last_site_i;
                    foreach (var site in sites_to_check)
                    {
                        double d = Math.Sqrt(Math.Pow(site.X - x, 2) + Math.Pow(site.Y - y, 2));
                        if (d < min_d)
                        {
                            int i = from_sites.IndexOf(site);
                            result[x, y] = i;
                            last_site_i = i;
                            min_d = d;
                        }
                    }
                }
            }

            return result;
        }
    }
}