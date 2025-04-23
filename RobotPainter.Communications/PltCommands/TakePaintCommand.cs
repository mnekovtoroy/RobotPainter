namespace RobotPainter.Communications.PltCommands
{
    public class TakePaintCommand : IPltCommand
    {
        double x,y;

        public string ToPlt()
        {
            throw new NotImplementedException();
            return $"TP{x},{y};";
        }
    }
}
