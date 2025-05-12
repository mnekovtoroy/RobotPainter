using RobotPainter.Core;

namespace RobotPainter.Application
{
    public static class ManualPalette
    {
        private static Palette palette = new Palette()
        {
            Colors = new List<ColorLab>()
            {
                new ColorLab(100, 0.00526049995830391, -0.010408184525267927),
                new ColorLab(85.92216328335641, 0.33380134184629595, -4.899097289445908),
                new ColorLab(74.4664368086248, -0.8612887071016151, -4.895170616552158),
                new ColorLab(43.34447699928772, -0.5178637597919034, -5.2847498204258825),
                new ColorLab(1.9192144721665727, 0.0002611069104818675, -0.0005166356362107383)
            }
        };

        public static Palette GetPalette()
        {
            return palette;
        }
    }
}
