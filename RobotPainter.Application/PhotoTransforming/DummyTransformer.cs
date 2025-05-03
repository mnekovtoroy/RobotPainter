namespace RobotPainter.Application.PhotoTransforming
{
    public class DummyTransformer : IPhotoTransformer
    {
        public Bitmap Transform(Bitmap bitmap, int target_width, int target_height)
        {
            //Thread.Sleep(10000);
            var transformed = new Bitmap(bitmap, target_width, target_height);
            return transformed;
        }
    }
}
