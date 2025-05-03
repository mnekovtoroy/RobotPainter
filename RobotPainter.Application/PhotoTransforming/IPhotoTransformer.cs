namespace RobotPainter.Application.PhotoTransforming
{
    public interface IPhotoTransformer
    {
        public Bitmap Transform(Bitmap bitmap, int target_width, int target_height);
    }
}
