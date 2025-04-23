using RobotPainter.Calculations.Brushes;
using RobotPainter.Core;
using RobotPainter.Calculations.StrokeGeneration;
using System.Drawing;

namespace RobotPainter.Calculations
{
    public class RobotPainterCalculator
    {
        private Bitmap _targetImage;
        private IBrushModel _brush;

        private double canvasWidth;
        private double canvasHeight;

        private LabBitmap targetLabBitmap;

        private StrokeSitesBuilder.Options strokeSiteBuilderOptions;
        private BrushstrokeBuilder.Options brushstrokeBuilderOptions;

        public RobotPainterCalculator(Bitmap targetImage, double canvas_width, double canvas_height, IBrushModel brush)
        {
            _targetImage = targetImage;
            _brush = brush;
            canvasWidth = canvas_width;
            canvasHeight = canvas_height;

            targetLabBitmap = new LabBitmap(_targetImage);

            strokeSiteBuilderOptions = new StrokeSitesBuilder.Options()
            {
                xResizeCoeff = canvasWidth / targetLabBitmap.Width,
                yResizeCoeff = canvasHeight / targetLabBitmap.Height
            };
            brushstrokeBuilderOptions = new BrushstrokeBuilder.Options()
            {
                xResizeCoeff = canvasWidth / targetLabBitmap.Width,
                yResizeCoeff = canvasHeight / targetLabBitmap.Height
            };
        }

        private StrokeGenerator strokeGenerator;
        public void InitializeStrokeGenerator(int n_voronoi, StrokeGenerator.Options options)
        {
            //int n_voronoi = StrokeGenerator.CalculateDesiredVoronoiN(target_stroke_width);
            strokeGenerator = new StrokeGenerator(targetLabBitmap, n_voronoi, options);
        }

        public void SetStrokeBuilderOptions(StrokeSitesBuilder.Options options)
        {
            strokeSiteBuilderOptions = options;
            strokeSiteBuilderOptions.xResizeCoeff = canvasWidth / targetLabBitmap.Width;
            strokeSiteBuilderOptions.yResizeCoeff = canvasHeight / targetLabBitmap.Height;  
        }

        public void SetBrushstrokeBuilderOptions(BrushstrokeBuilder.Options options)
        {
            brushstrokeBuilderOptions = options;
            brushstrokeBuilderOptions.xResizeCoeff = canvasWidth / targetLabBitmap.Width;
            brushstrokeBuilderOptions.yResizeCoeff = canvasHeight / targetLabBitmap.Height;
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
            var stroke_sites = strokeGenerator.GetNextStrokeSites(strokeSiteBuilderOptions);
            var brushstroke = BrushstrokeBuilder.GenerateBrushstroke(stroke_sites, brushstrokeBuilderOptions);
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
    }
}