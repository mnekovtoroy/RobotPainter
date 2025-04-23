using RobotPainter.Communications.PltCommands;
using RobotPainter.Core;

namespace RobotPainter.Communications.Converting
{
    public class PltConverter : IPltConverter
    {
        public int maxConsecStrokes;
        public int maxWithoutWashcycle;

        private IColorToCoordConverter _colorToCoord;

        public PltConverter(IColorToCoordConverter color2coord, int max_consec_strokes = 5, int max_without_washcycle)
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

            ColorLab last_color = new ColorLab(double.MaxValue, double.MaxValue, double.MaxValue);
            int since_last_take_color = 0;
            int since_last_wash_cycle = 0;
            foreach(var stroke in strokes)
            {
                if(stroke.Color != last_color || since_last_wash_cycle == maxWithoutWashcycle)
                {
                    AddWashCycle(command_list);
                    since_last_wash_cycle = 0;
                }
                if(stroke.Color != last_color || since_last_take_color == maxConsecStrokes)
                {                    
                    command_list.Add(new TakePaintCommand(_colorToCoord.ColorToCoord(stroke.Color)));
                    last_color = stroke.Color;
                    since_last_take_color = 0;
                }
                command_list.Add(new BrushstrokeCommand(stroke.RootPath));
                since_last_take_color++;
                since_last_wash_cycle++;
            }
            AddWashCycle(command_list);
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
