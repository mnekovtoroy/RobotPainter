using RobotPainter.Calculations.ImageProcessing;
using Emgu.CV;

namespace RobotPainter.Application.PhotoTransforming
{
    public class PhotoTransformer : IPhotoTransformer
    {
        private Point whitePoint = new Point(2926, 20);
        private Point blackPoint = new Point(1696, 728);

        private Point canvasTopLeft = new Point(989, 883);
        private Point canvasTopRight = new Point(4406, 879);
        private Point canvasBottomRight = new Point(4400, 3418);
        private Point canvasBottomLeft = new Point(990, 3440);
        
        
        /*private Point canvasTopLeft = new Point(2909, 2407);
        private Point canvasTopRight = new Point(3766, 2396);
        private Point canvasBottomRight = new Point(3782, 3261);
        private Point canvasBottomLeft = new Point(2921, 3271);*/

        private PointF[] canvas_bounds;

        public PhotoTransformer()
        {
            canvas_bounds = new PointF[]
            {
                new PointF(canvasTopLeft.X, canvasTopLeft.Y),
                new PointF(canvasTopRight.X, canvasTopRight.Y),
                new PointF(canvasBottomRight.X, canvasBottomRight.Y),
                new PointF(canvasBottomLeft.X, canvasBottomLeft.Y),
            };
        }

        public Bitmap Transform(Bitmap bitmap, int target_width, int target_height)
        {
            var target_bounds = new PointF[]
            {
                new Point(0, 0),
                new Point(target_width - 1, 0),
                new Point(target_width - 1, target_height - 1),
                new Point(0, target_height - 1)
            };
            Color perfect_white = bitmap.GetPixel(whitePoint.X, whitePoint.Y);
            Color perfect_black = bitmap.GetPixel(blackPoint.X, blackPoint.Y);
            Size target_size = new Size(target_width, target_height);

            //transforming
            Bitmap transformed;
            using (var orig = bitmap.ToMat())
            using (var target = new Mat())
            {
                var matrix = CvInvoke.GetPerspectiveTransform(canvas_bounds, target_bounds);

                CvInvoke.WarpPerspective(orig, target, matrix, target_size);

                transformed = target.ToBitmap();
            }
            //white balance
            ImageProcessor.WhiteBalance(transformed, perfect_white, perfect_black);

            return transformed;
        }
    }
}
