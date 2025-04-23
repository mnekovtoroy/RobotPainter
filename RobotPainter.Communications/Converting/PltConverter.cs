using RobotPainter.Communications.PltCommands;

namespace RobotPainter.Communications.Converting
{
    public class PltConverter : IPltConverter
    {
        public void SavePlt(List<IPltCommand> commands, string path)
        {
            throw new NotImplementedException();
        }

        public List<IPltCommand> ToPlt(List<BrushstrokeInfo> strokes)
        {
            throw new NotImplementedException();
        }
    }
}
