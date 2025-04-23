namespace RobotPainter.Communications.PltCommands
{
    public class PlaceWasherCommand : IPltCommand
    {
        public string ToPlt()
        {
            return "BW;";
        }
    }
}
