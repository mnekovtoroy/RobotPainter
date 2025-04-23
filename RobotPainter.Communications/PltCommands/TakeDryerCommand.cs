namespace RobotPainter.Communications.PltCommands
{
    public class TakeDryerCommand : IPltCommand
    {
        public string ToPlt()
        {
            return "TD;";
        }
    }
}
