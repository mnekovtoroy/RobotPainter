using RobotPainter.Core;

namespace RobotPainter.Communications.PltCommands
{
    public class TakePaintCommand : IPltCommand
    {
        private static int scf = 40; // scale to PLT

        public PointD ColorPosition { get; set; }

        public TakePaintCommand(PointD color_position)
        {
            ColorPosition = color_position;
        }

        public string ToPlt()
        {
            return $"TP{Convert.ToInt32(ColorPosition.x * scf)},{Convert.ToInt32(ColorPosition.y * scf)};";
        }
    }
}
