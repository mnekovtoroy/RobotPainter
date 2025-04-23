using RobotPainter.Communications.PltCommands;

namespace RobotPainter.Communications.Converting
{
    public interface IPltConverter
    {
        List<IPltCommand> ToPlt(List<BrushstrokeInfo> strokes);

        public void SavePlt(List<IPltCommand> commands, string path);
    }
}
