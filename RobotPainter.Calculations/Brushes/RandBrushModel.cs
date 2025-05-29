using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPainter.Calculations.Brushes
{
    public class RandBrushModel : BasicBrushModel
    {
        private Random random = new Random();
        private const double STD = 0.01;

        public new double rfun(double z) => base.rfun(z) + NextDoubleGuassian(0, STD);
        public new double wfun(double z) => base.wfun(z) + NextDoubleGuassian(0, STD);
        public new double bfun(double z) => base.bfun(z) + NextDoubleGuassian(0, STD);

        private double NextDoubleGuassian(double mean, double stdDev)
        {
            double u1 = 1.0 - random.NextDouble(); 
            double u2 = 1.0 - random.NextDouble();
            double rand_norm = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double rand_adj = mean + stdDev * rand_norm; //random normal(mean,stdDev^2)
            return rand_adj;
        }
    }
}
