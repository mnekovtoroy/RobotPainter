namespace RobotPainter.Application.CustomEventArgs
{
    public class LayerCompletedEventArgs : EventArgs
    {
        public int LayerIndex { get; set; }

        public int TotalLayers { get; set; }
    }
}
