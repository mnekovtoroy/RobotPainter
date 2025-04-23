namespace RobotPainter.Communications.PltCommands
{
    public class PlaceBrushCommand : IPltCommand
    {
        public string ToPlt()
        {
            return "PB;";
        }
    }
}
