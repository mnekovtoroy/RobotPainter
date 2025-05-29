using RobotPainter.Communications.PltCommands;
using RobotPainter.Core;

namespace RobotPainter.Communications.Converting
{
    public class StrokeOnlyPlt : IPltConverter
    {
        public int maxConsecStrokes = 10;

        public int consecStrokes = 0;
        public ColorLab last_color = new ColorLab(double.MaxValue, double.MaxValue, double.MaxValue);

        public IColorToCoordConverter _colorToCoord = new SingleColorCoord();

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
                if (stroke.Color != last_color || consecStrokes == maxConsecStrokes)
                {
                    command_list.Add(new TakePaintCommand(_colorToCoord.ColorToCoord(stroke.Color)));
                    last_color = stroke.Color;
                    consecStrokes = 0;
                }
                command_list.Add(new BrushstrokeCommand(stroke.RootPath));
                consecStrokes++;
            }
            return command_list;
        }
    }
}
