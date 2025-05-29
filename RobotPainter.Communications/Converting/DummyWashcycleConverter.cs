using RobotPainter.Communications.PltCommands;

namespace RobotPainter.Communications.Converting
{
    public class DummyWashcycleConverter : IPltConverter
    {
        public DummyWashcycleConverter()
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
            foreach (BrushstrokeInfo stroke in strokes)
            {
                AddWashCycle(command_list);
            }
            return command_list;
        }

        private static void AddWashCycle(List<IPltCommand> command_list)
        {
            command_list.Add(new PlaceWasherCommand());
            command_list.Add(new TakeWasherCommand());
            command_list.Add(new PlaceDryerCommand());
            command_list.Add(new TakeDryerCommand());
        }
    }
}
