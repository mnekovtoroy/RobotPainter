namespace RobotPainter.Calculations.Optimization
{
    public class GradDescent : IOptimizer
    {
        private static (double, double) Grad(Func<double, double, double> function, double x, double y, double h)
        {
            double dx, dy;
            dx = (function(x + h, y) - function(x - h, y)) / (2 * h);
            dy = (function(x, y + h) - function(x, y - h)) / (2 * h);
            return (dx, dy);
        }

        /// <summary>
        /// Maximum iterations.
        /// </summary>
        public int kmax { get; set; } = 1000;
        /// <summary>
        /// Maximum step distance in a single iteration.
        /// </summary>
        public double max_step { get; set; } = 1.0;
        /// <summary>
        /// Tolerance condition to stop optimizing.
        /// </summary>
        public double tol { get; set; } = 1e-3;
        /// <summary>
        /// Alpha.
        /// </summary>
        public double a { get; set; } = 0.05;
        /// <summary>
        /// Rate of decrease of alpha.
        /// </summary>
        public double l { get; set; } = 1.2;
        /// <summary>
        /// h for finite differences to find gradient.
        /// </summary>
        public double h { get; set; } = 0.1;

        public GradDescent()
        {

        }

        /// <summary>
        /// Finds lowest point using gradient descent.
        /// </summary>
        /// <param name="function">Target function.</param>
        /// <param name="x0">Starting x.</param>
        /// <param name="y0">Starting y.</param>
        /// <returns>(x, y) minimum point.</returns>
        public (double, double) Optimize(Func<double,double,double> function, double x0, double y0)
        {
            double xc = x0, yc = y0;

            double ac = a;

            for (int i = 0; i < kmax; i++)
            {
                double xgrad, ygrad;
                (xgrad, ygrad) = Grad(function, xc, yc, h);
                
                //checking for max step
                if (Math.Sqrt(Math.Pow(ac * xgrad, 2) + Math.Pow(ac * ygrad, 2)) > max_step)
                {
                    xgrad = xgrad * (max_step / Math.Sqrt(Math.Pow(ac * xgrad, 2) + Math.Pow(ac * ygrad, 2)));
                    ygrad = ygrad * (max_step / Math.Sqrt(Math.Pow(ac * xgrad, 2) + Math.Pow(ac * ygrad, 2)));
                }

                double xnext = xc + ac * -xgrad;
                double ynext = yc + ac * -ygrad;

                //decreasing step
                if (function.Invoke(xnext, ynext) < function.Invoke(xc, yc))
                {
                    ac /= l;
                }

                double delta = Math.Sqrt(Math.Pow(xnext - xc, 2) + Math.Pow(ynext - yc, 2));

                xc = xnext; yc = ynext;

                //checking for exit condition
                if (delta < tol)
                {
                    break;
                }
            }

            return (xc, yc);
        }
    }
}
