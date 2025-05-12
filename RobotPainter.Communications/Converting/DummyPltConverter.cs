using RobotPainter.Communications.PltCommands;

namespace RobotPainter.Communications.Converting
{
    public class DummyPltConverter : IPltConverter
    {
        private IColorToCoordConverter _colorToCoord;

        public DummyPltConverter(IColorToCoordConverter color2coord)
        {
            _colorToCoord = color2coord;
        }

        public void SavePlt(List<IPltCommand> commands, string path)
        {
            using (var stream = new StreamWriter(path))
            {
                foreach (var command in commands)
                {
                    stream.Write(command.ToPlt());
                }
            }
        }

        public List<IPltCommand> ToPlt(List<BrushstrokeInfo> strokes)
        {
            var command_list = new List<IPltCommand>();
            foreach(BrushstrokeInfo stroke in strokes)
            {
                command_list.Add(new TakePaintCommand(_colorToCoord.ColorToCoord(stroke.Color)));
            }
            return command_list;
        }
    }
}
