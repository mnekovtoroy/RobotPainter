using RobotPainter.Calculations.Brushes;
using System.Drawing;

namespace RobotPainter.Calculations
{
    public class RobotPainter
    {
        Bitmap _targetImage;
        IBrushModel _brush;
        Palette _pallete;



        public RobotPainter(Bitmap targetImage, IBrushModel brush, Palette pallete)
        {
            _targetImage = targetImage;
            _brush = brush;
            _pallete = pallete;
        }

        
    }
}