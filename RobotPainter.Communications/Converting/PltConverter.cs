using RobotPainter.Communications.PltCommands;
using RobotPainter.Core;

namespace RobotPainter.Communications.Converting
{
    public class PltConverter : IPltConverter
    {
        public int maxConsecStrokes;
        public int maxWithoutWashcycle;

        public int consecStrokes = 0;
        public int withousWashcycle = 0;
        public ColorLab last_color = new ColorLab(double.MaxValue, double.MaxValue, double.MaxValue);

        private IColorToCoordConverter _colorToCoord;

        public PltConverter(IColorToCoordConverter color2coord, int max_consec_strokes = 3, int max_without_washcycle = 6)
        {
            _colorToCoord = color2coord;
            this.maxConsecStrokes = max_consec_strokes;
            this.maxWithoutWashcycle = max_without_washcycle;
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

            foreach(var stroke in strokes)
            {
                if(stroke.Color != last_color || withousWashcycle == maxWithoutWashcycle)
                {
                    AddWashCycle(command_list);
                    withousWashcycle = 0;
                }
                if(stroke.Color != last_color || consecStrokes == maxConsecStrokes || withousWashcycle == 0)
                {                    
                    command_list.Add(new TakePaintCommand(_colorToCoord.ColorToCoord(stroke.Color)));
                    last_color = stroke.Color;
                    consecStrokes = 0;
                }
                command_list.Add(new BrushstrokeCommand(stroke.RootPath));
                consecStrokes++;
                withousWashcycle++;
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
