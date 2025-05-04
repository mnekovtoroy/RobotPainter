using RobotPainter.Core;

namespace RobotPainter.Application
{
    public static class ManualPalette
    {
        private static Palette palette = new Palette()
        {
            Colors = new List<ColorLab>()
            {
                new ColorLab(0, 0, 0),
                new ColorLab(15, 0, 0),
                new ColorLab(30, 0, 0),
                new ColorLab(45, 0, 0),
                new ColorLab(60, 0, 0),
                new ColorLab(70, 0, 0),
                new ColorLab(80, 0, 0),
                new ColorLab(87, 0, 0),
                new ColorLab(94, 0, 0),
                new ColorLab(100, 0, 0),
            }
        };

        public static Palette GetPalette()
        {
            return palette;
        }
    }
}
