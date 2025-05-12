using RobotPainter.Core;

namespace RobotPainter.Communications.Converting
{
    public class ManualColorToCoord : IColorToCoordConverter
    {
        private Dictionary<ColorLab, PointD> colorToCoord;

        public ManualColorToCoord(List<ColorLab> colors, PointD p0, double dx, double dy, int width, int height)
        {
            colorToCoord = new Dictionary<ColorLab, PointD>();
            for(int i =  0; i < colors.Count; i++)
            {
                colorToCoord.Add(colors[i], p0 + new PointD(dx * (i % width), dy * (i / width)));
            }
        }

        public PointD ColorToCoord(ColorLab color)
        {
            return colorToCoord[color];
        }
    }
}
