namespace RobotPainter.Calculations.Optimization
{
    public interface IOptimizer
    {
        public (double, double) Optimize(Func<double, double, double> function, double x0, double y0);
    }
}