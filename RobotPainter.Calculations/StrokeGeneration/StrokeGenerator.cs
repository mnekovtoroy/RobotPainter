using RobotPainter.Calculations.Optimization;
using SharpVoronoiLib;
using System.Threading.Tasks;

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

        private Dictionary<VoronoiSite, BrushstrokeRegions> siteToStroke;

        public LabBitmap image;
        public double[,] u;
        public double[,] v;

        private int width;
        private int height;

        public double real_height_mm;
        public double real_width_mm;

        public double real_stroke_max_length_mm;
        //public double StrokeMaxLength { get { return real_stroke_max_length_mm * width / real_width_mm; } }
        public double StrokeMaxLength = 80;

        public double Ltol = 3.5;

        private readonly IOptimizer _optimizer;

        public List<BrushstrokeRegions> strokes = new List<BrushstrokeRegions>();

        public StrokeGenerator(LabBitmap target_image, int n_voronoi, IOptimizer optimizer, int n_rolling_avg = 7)
        {
            image = target_image;
            (u,v) = ImageProcessor.LNormWithRollAvg(image, n_rolling_avg);
            width = image.Width;
            height = image.Height;
            _optimizer = optimizer;

            sites = GenerateRandomRelaxedMesh(n_voronoi, width, height);
            sites = sites.OrderByDescending(s => image.GetPixel(Convert.ToInt32(s.Centroid.X), Convert.ToInt32(s.Centroid.Y)).L).ToList();
            unassigned_sites = sites.Select(x => x).ToList();
            siteToStroke = new Dictionary<VoronoiSite, BrushstrokeRegions>();
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
        public void CalculateStorkes()
        {
            ClearSitesList();
            siteToStroke.Clear();
            strokes.Clear();
            while(!AreAllSitesAssigned())
            {
                GetNextBrushstroke();
            }
        }

        public BrushstrokeRegions GetNextBrushstroke()
        {
            if(AreAllSitesAssigned())
            {
                return null;
            }
            var curr_site = unassigned_sites[0];
            var stroke = GenerateBrushstroke(curr_site);
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

        private BrushstrokeRegions GenerateBrushstroke(VoronoiSite startin_point)
        {
            var stroke = new BrushstrokeRegions(this, startin_point);
            stroke.GenerateStroke(StrokeMaxLength, 1.0, Ltol);
            return stroke;
        }

        private LabBitmap prev_feedback;
        public void ApplyFeedback(LabBitmap feedback)
        {
            if(prev_feedback == null)
            {
                prev_feedback = feedback;
                return;
            }
            //find the difference mask
            var difference = LabBitmap.CalculateDifference(feedback, prev_feedback);
            bool[,] difference_mask = GetDifferenceMask(difference);

            //find the sites to optimize
            var affected_sites = GetSitesByMask(difference_mask);
            var sites_to_optimize = GetUnassignedSitesAndNeighbors(affected_sites);

            //optimize the sites
            OptimizeSitesToMaskPreservingAssigned(sites_to_optimize, difference_mask);
        }

        public bool[,] GetDifferenceMask(ColorLab[,] difference)
        {
            bool[,] difference_mask = new bool[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    double diff = Geometry.Norm(new Point3D(difference[i, j].L, difference[i, j].a, difference[i, j].b));
                    difference_mask[i, j] = diff > 1e-5; //if diff is more than some small error
                }
            }
            return difference_mask;
        }

        private List<VoronoiSite> GetSitesByMask(bool[,] difference_mask)
        {
            var sites_mask = GetVoronoiMask();
            HashSet<VoronoiSite> masked_sites = new HashSet<VoronoiSite>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (difference_mask[i, j] && !masked_sites.Contains(sites[sites_mask[i,j]]))
                    {
                        masked_sites.Add(sites[sites_mask[i, j]]);
                    }
                }
            }
            return masked_sites.ToList();
        }

        private List<VoronoiSite> GetUnassignedSitesAndNeighbors(List<VoronoiSite> sites_list)
        {
            HashSet<VoronoiSite> result = new HashSet<VoronoiSite>();
            foreach(var site in sites_list)
            {
                if(!IsSiteReserved(site) && !result.Contains(site))
                {
                    result.Add(site);
                }
                foreach(var neighbor in site.Neighbours)
                {
                    if (!IsSiteReserved(neighbor) && !result.Contains(neighbor))
                    {
                        result.Add(neighbor);
                    }
                }
            }
            return result.ToList();
        }

        private void OptimizeSitesToMaskPreservingAssigned(List<VoronoiSite> sites_to_optimize, bool[,] difference_mask)
        {
            throw new NotImplementedException();
        }

        public bool IsSiteReserved(VoronoiSite site)
        {
            return !unassigned_sites.Contains(site);
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