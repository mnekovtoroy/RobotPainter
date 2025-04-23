using RobotPainter.Core;

namespace RobotPainter.Communications.Converting
{
    public interface IColorToCoordConverter
    {
        public PointD ColorToCoord(ColorLab color);
    }
}
