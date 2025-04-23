using RobotPainter.Core;

namespace RobotPainter.Communications.PltCommands
{
    public class TakePaintCommand : IPltCommand
    {
        PointD ColorPosition { get; set; }

        public TakePaintCommand(PointD color_position)
        {
            ColorPosition = color_position;
        }

        public string ToPlt()
        {
            return $"TP{ColorPosition.x},{ColorPosition.y};";
        }
    }
}
