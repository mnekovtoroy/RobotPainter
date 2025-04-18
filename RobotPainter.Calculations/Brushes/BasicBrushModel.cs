using SharpVoronoiLib;

namespace RobotPainter.Calculations.Brushes
{
    public class BasicBrushModel : IBrushModel
    {
        public readonly static string model_name = "Malevich 6";

        public readonly static PointD[] footprint = [
            new PointD(27.447855, 0.536619),
            new PointD(28.201554, 0.775132),
            new PointD(28.869389, 1.233075),
            new PointD(29.317792, 1.782544),
            new PointD(29.651710, 2.454259),
            new PointD(29.880682, 3.179337),
            new PointD(30.201969, 4.677195),
            new PointD(30.310004, 5.980738),
            new PointD(30.087150, 7.662308),
            new PointD(29.427442, 8.717410),
            new PointD(28.679915, 9.508738),
            new PointD(27.944634, 9.888365),
            new PointD(27.447857, 9.961784)
        ]; 	

        public BasicBrushModel()
        {
            
        }

        public double r(double z)
        {
            double p1 = 0.002054;
            double p2 = -0.05006;
            double p3 = 0.3085;
            double p4 = 0.8559;
            double p5 = 3.122;
            double f = p1 * Math.Pow(z, 4) + p2 * Math.Pow(z, 3) + p3 * Math.Pow(z, 2) + p4 * z + p5;
            return f;
        }

        public double w(double z)
        {
            return 0.6 * z + 1.07;
        }

        public double b(double z)
        {
            double kb = 0.5;
            return kb * z;
        }

        public double z(double r)
        {
            return 0;
        }
    }
}
