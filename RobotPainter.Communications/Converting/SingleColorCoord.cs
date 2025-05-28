using RobotPainter.Core;

namespace RobotPainter.Communications.Converting
{
    public class SingleColorCoord : IColorToCoordConverter
    {
        public PointD ColorToCoord(ColorLab color)
        {
            return new PointD(0, 0);
        }
    }
}
