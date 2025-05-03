using RobotPainter.Calculations.ImageProcessing;

namespace RobotPainter.Application.PhotoTransforming
{
    public class PhotoTransformer : IPhotoTransformer
    {
        private Point whitePoint = new Point(3015, 15);
        private Point blackPoint = new Point(1803, 763);

        private Point canvasUpperLeft = new Point(1024, 904);
        private Size canvasSize = new Size(3397, 2542);

        private Rectangle canvasArea;

        public PhotoTransformer()
        {
            canvasArea = new Rectangle(canvasUpperLeft, canvasSize);
        }

        public Bitmap Transform(Bitmap bitmap, int target_width, int target_height)
        {
            //cropping 
            var cropped = bitmap.Clone(canvasArea, bitmap.PixelFormat);

            //resizing
            var resized = new Bitmap(cropped, new Size(target_width, target_height));

            //white balance
            Color perfect_white = bitmap.GetPixel(whitePoint.X, whitePoint.Y);
            Color perfect_black = bitmap.GetPixel(blackPoint.X, blackPoint.Y);
            ImageProcessor.WhiteBalance(resized, perfect_white, perfect_black);

            //idk if its needed
            cropped.Dispose();
            return resized;
        }
    }
}
