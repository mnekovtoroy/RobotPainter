namespace RobotPainter.Communications.PltCommands
{
    public class TakeWasherCommand : IPltCommand
    {
        public string ToPlt()
        {
            return "TW;";
        }
    }
}
