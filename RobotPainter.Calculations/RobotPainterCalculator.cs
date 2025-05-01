using RobotPainter.Calculations.Brushes;
using RobotPainter.Core;
using RobotPainter.Calculations.StrokeGeneration;
using System.Drawing;

namespace RobotPainter.Calculations
{
    public class RobotPainterCalculator
    {
        public class LayerOptions
        {
            public double ErrorTolerance = 1.0;
            public int NVoronoi = 5000;

            //StrokeGenerator
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

            AllLayersOptions = new List<LayerOptions>();
        }

        private StrokeGenerator strokeGenerator;
        public void InitializeStrokeGenerator()
        {
            //int n_voronoi = StrokeGenerator.CalculateDesiredVoronoiN(target_stroke_width);
            strokeGenerator = new StrokeGenerator(
                targetLabBitmap,
                AllLayersOptions[CurrLayer].NVoronoi,
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

        public bool AdvanceLayer()
        {
            if (CurrLayer >= AllLayersOptions.Count - 1)
                return false;

            //initialize stroke generator for that layer

            return true;
        }

        public void ApplyFeedback(Bitmap feedback)
        {
            Console.WriteLine("applying feedback not impleneted");
        }

        public StrokeGenerator.Options MapStrokeGeneratorOptions(LayerOptions layerOptions)
        {
            return new StrokeGenerator.Options()
            {
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