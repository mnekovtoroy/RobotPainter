using RobotPainter.Calculations.Brushes;
using RobotPainter.Core;
using RobotPainter.Calculations.StrokeGeneration;
using System.Drawing;
using RobotPainter.Calculations.Clustering;

namespace RobotPainter.Calculations
{
    public class RobotPainterCalculator
    {
        public class LayerOptions
        {

            //StrokeGenerator
            public double ErrorTolerance;
            public int RelaxationIterations;
            public int LpullIterations;
            public double LpullMaxStep;
            public int RollingAverageN;

            //StrokeSitesBuilder
            public double CanvasMaxStrokeLength;
            public double L_tol;
            public double MaxNormAngle;
            public double MaxBrushAngle;

            //BrushstrokeBuilder
            public IBrushModel BrushModel;
            public double MaxWidth;
            public double Overlap;
            public double StartOverheadCoeff;
            public double EndOverheadCoeff;
            public double SafeHeight;
            public double StartRunawayAngle;
            public double EndRunawayAngle;
        }

        private Bitmap _targetImage;

        private double canvasWidth;
        private double canvasHeight;

        private LabBitmap targetLabBitmap;
        private LabBitmap lastFeedback;

        public Palette SavedPalette { get; set; }

        private bool[,] isPaintedOn;
        private double[,] colorError;

        private int _currLayer = 0;
        public int CurrLayer { get { return _currLayer; } }
        public int NumOfLayers { get { return AllLayersOptions == null ? 0 : AllLayersOptions.Count; } }

        public List<LayerOptions> AllLayersOptions { get; set; }

        public RobotPainterCalculator(Bitmap targetImage, double canvas_width, double canvas_height)
        {
            _targetImage = targetImage;
            canvasWidth = canvas_width;
            canvasHeight = canvas_height;

            targetLabBitmap = new LabBitmap(_targetImage);
            isPaintedOn = new bool[targetLabBitmap.Width, targetLabBitmap.Height];
            colorError = new double[targetLabBitmap.Width, targetLabBitmap.Height];

            AllLayersOptions = new List<LayerOptions>();
        }

        public void SetInitialCanvas(Bitmap initial_canvas)
        {
            lastFeedback = new LabBitmap(initial_canvas);
        }

        private StrokeGenerator strokeGenerator;
        public void InitializeStrokeGenerator()
        {
            int n_voronoi = StrokeGenerator.CalculateDesiredVoronoiN(canvasWidth, canvasHeight, AllLayersOptions[CurrLayer].MaxWidth, AllLayersOptions[CurrLayer].Overlap);
            strokeGenerator = new StrokeGenerator(
                targetLabBitmap,
                n_voronoi,
                isPaintedOn,
                colorError,
                MapStrokeGeneratorOptions(AllLayersOptions[CurrLayer]));
        }

        public List<Brushstroke> GetAllBrushstrokes()
        {
            if(strokeGenerator == null)
            {
                throw new Exception("Stroke Generator must be initialized first. Call RobotPainterCalculator.InintializeStrokeGenerator to do that.");
            }
            var result = new List<Brushstroke>();
            while(!EndOfStrokes())
            {
                result.Add(GetNextBrushstroke());
            }
            return result;
        }

        public Brushstroke GetNextBrushstroke()
        {
            if (strokeGenerator == null)
            {
                throw new Exception("Stroke Generator must be initialized first. Call RobotPainterCalculator.InintializeStrokeGenerator to do that.");
            }
            if (EndOfStrokes())
            {
                return null;
            }
            var stroke_sites = strokeGenerator.GetNextStrokeSites(MapStrokeSitesBuilderOptions(AllLayersOptions[CurrLayer]));
            var brushstroke = BrushstrokeBuilder.GenerateBrushstroke(stroke_sites, MapBrushstrokeBuilderOptions(AllLayersOptions[CurrLayer]));
            return brushstroke;
        }

        public bool EndOfStrokes()
        {
            if (strokeGenerator == null)
            {
                throw new Exception("Stroke Generator must be initialized first. Call RobotPainterCalculator.InintializeStrokeGenerator to do that.");
            }
            return strokeGenerator.AreAllSitesAssigned();
        }

        public void ApplyPalette(List<Brushstroke> brushstrokes)
        {
            if (SavedPalette == null)
                throw new Exception("Palette must be set first.");
            foreach(var stroke in brushstrokes)
            {
                ApplyPalette(stroke);
            }
        }

        public void ApplyPalette(Brushstroke brushstroke)
        {
            brushstroke.Color = SavedPalette.Apply(brushstroke.Color);
        }

        public Palette CreatePalette(int n_colors, IClusterer<ColorLab> clusterer = null)
        {
            if (strokeGenerator == null)
                throw new Exception("Stroke generator must be initialized first.");

            if (clusterer == null)
                clusterer = new KMeansClustering();

            var colors = strokeGenerator.sites.Select(s =>
            {
                var centroid = s.Centroid;
                return targetLabBitmap.GetPixel(Convert.ToInt32(centroid.X), Convert.ToInt32(centroid.Y));
            }).ToList();

            var palette_colors = clusterer.FindClusters(colors, n_colors);
            return new Palette() { Colors = palette_colors };
        }

        public bool AdvanceLayer()
        {
            if (CurrLayer >= AllLayersOptions.Count - 1)
                return false;

            //initialize stroke generator for that layer
            _currLayer++;
            return true;
        }

        public void ApplyFeedback(Bitmap feedback)
        {
            if(lastFeedback == null)
            {
                throw new Exception("Initial canvas state must be set. Call RobotPainterCalculator.SetInitialCanvas to do that");
            }

            var new_feedback = new LabBitmap(feedback);

            const double margin_of_error = 0.5;

            for(int i = 0; i < feedback.Width; i++)
            {
                for(int j = 0; j < feedback.Height; j++)
                {
                    //checking if a pixel been painted on
                    if (!isPaintedOn[i,j] && 
                        lastFeedback.GetPixel(i, j).DeltaE76(new_feedback.GetPixel(i, j)) > margin_of_error)
                    {
                        isPaintedOn[i, j] = true;
                    }

                    //calculating color error
                    var targret_color = SavedPalette == null ? targetLabBitmap.GetPixel(i, j) : SavedPalette.Apply(targetLabBitmap.GetPixel(i, j));
                    var feedback_color = SavedPalette == null ? new_feedback.GetPixel(i, j) : SavedPalette.Apply(new_feedback.GetPixel(i, j));
                    colorError[i, j] = targret_color.DeltaE76(feedback_color);
                }
            }
            lastFeedback = new_feedback;
        }

        private StrokeGenerator.Options MapStrokeGeneratorOptions(LayerOptions layerOptions)
        {
            return new StrokeGenerator.Options()
            {
                ErrorTolerance = layerOptions.ErrorTolerance,
                RelaxationIterations = layerOptions.RelaxationIterations,
                LpullIterations = layerOptions.LpullIterations,
                LpullMaxStep = layerOptions.LpullMaxStep,
                RollingAverageN = layerOptions.RollingAverageN
            };
        }

        private StrokeSitesBuilder.Options MapStrokeSitesBuilderOptions(LayerOptions layerOptions)
        {
            return new StrokeSitesBuilder.Options()
            {
                CanvasMaxStrokeLength = layerOptions.CanvasMaxStrokeLength,
                L_tol = layerOptions.L_tol,
                MaxNormAngle = layerOptions.MaxNormAngle,
                MaxBrushAngle = layerOptions.MaxBrushAngle,

                xResizeCoeff = canvasWidth / targetLabBitmap.Width,
                yResizeCoeff = canvasHeight / targetLabBitmap.Height
            };
        }

        private BrushstrokeBuilder.Options MapBrushstrokeBuilderOptions(LayerOptions layerOptions)
        {
            return new BrushstrokeBuilder.Options()
            {
                BrushModel = layerOptions.BrushModel,
                MaxWidth = layerOptions.MaxWidth,
                Overlap = layerOptions.Overlap,
                StartOverheadCoeff = layerOptions.StartOverheadCoeff,
                EndOverheadCoeff = layerOptions.EndOverheadCoeff,
                SafeHeight = layerOptions.SafeHeight,
                StartRunawayAngle = layerOptions.StartRunawayAngle,
                EndRunawayAngle = layerOptions.EndRunawayAngle,

                xResizeCoeff = canvasWidth / targetLabBitmap.Width,
                yResizeCoeff = canvasHeight / targetLabBitmap.Height
            };
        }

        public static LayerOptions CreateLayerOptions(
            StrokeGenerator.Options sg_options = null,
            StrokeSitesBuilder.Options ssb_options = null, 
            BrushstrokeBuilder.Options bsb_options = null)
        {
            var layerOptions = new LayerOptions();
            StrokeGenerator.Options sg_from = sg_options == null ? new StrokeGenerator.Options() : sg_options;
            StrokeSitesBuilder.Options ssb_from = ssb_options == null ? new StrokeSitesBuilder.Options() : ssb_options;
            BrushstrokeBuilder.Options bsb_from = bsb_options == null ? new BrushstrokeBuilder.Options() : bsb_options;

            layerOptions.ErrorTolerance = sg_from.ErrorTolerance;
            layerOptions.RelaxationIterations = sg_from.RelaxationIterations;
            layerOptions.LpullIterations = sg_from.LpullIterations;
            layerOptions.LpullMaxStep = sg_from.LpullMaxStep;
            layerOptions.RollingAverageN = sg_from.RollingAverageN;

            layerOptions.CanvasMaxStrokeLength = ssb_from.CanvasMaxStrokeLength;
            layerOptions.L_tol = ssb_from.L_tol;
            layerOptions.MaxNormAngle = ssb_from.MaxNormAngle;
            layerOptions.MaxBrushAngle = ssb_from.MaxBrushAngle;

            layerOptions.BrushModel = bsb_from.BrushModel;
            layerOptions.MaxWidth = bsb_from.MaxWidth;
            layerOptions.Overlap = bsb_from.Overlap;
            layerOptions.StartOverheadCoeff = bsb_from.StartOverheadCoeff;
            layerOptions.EndOverheadCoeff = bsb_from.EndOverheadCoeff;
            layerOptions.SafeHeight = bsb_from.SafeHeight;
            layerOptions.StartRunawayAngle = bsb_from.StartRunawayAngle;
            layerOptions.EndRunawayAngle = bsb_from.EndRunawayAngle;

            return layerOptions;
        }
    }
}