namespace RobotPainter.Communications.PltCommands
{
    public class PlaceDryerCommand : IPltCommand
    {
        public string ToPlt()
        {
            return "BD;";
        }
    }
}
