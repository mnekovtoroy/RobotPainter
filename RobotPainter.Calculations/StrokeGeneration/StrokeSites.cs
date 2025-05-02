using RobotPainter.Core;
using SharpVoronoiLib;

namespace RobotPainter.Calculations.StrokeGeneration
{
    public class StrokeSites
    {
        public StrokeGenerator strokeGenerator;

        public List<VoronoiSite> involvedSites;

        public VoronoiSite startingSite;
        public VoronoiPoint startingCentroid;

        public ColorLab MainColor { get { return strokeGenerator.image.GetPixel(Convert.ToInt32(Math.Floor(startingCentroid.X)), Convert.ToInt32(Math.Floor(startingCentroid.Y))); } }

        public PointD StartingNorm { 
            get { 
                return new PointD(
                    strokeGenerator.u[Convert.ToInt32(Math.Floor(startingCentroid.X)), Convert.ToInt32(Math.Floor(startingCentroid.Y))], 
                    strokeGenerator.v[Convert.ToInt32(Math.Floor(startingCentroid.X)), Convert.ToInt32(Math.Floor(startingCentroid.Y))]); 
            } 
        }

        public StrokeSites()
        {

        }
    }
}