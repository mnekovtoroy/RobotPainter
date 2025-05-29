using RobotPainter.Communications.PltCommands;

namespace RobotPainter.Communications.Converting
{
    public class DummyStrokeConverter : IPltConverter
    {
        public DummyStrokeConverter()
        {
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

            foreach (var stroke in strokes)
            {
                command_list.Add(new BrushstrokeCommand(stroke.RootPath));
            }
            return command_list;
        }
    }
}
