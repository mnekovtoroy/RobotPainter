namespace RobotPainter.Calculations.Optimization
{
    public static class Optimizer
    {
        private static (double, double) Grad(Func<double, double, double> function, double x, double y, double h)
        {
            double dx, dy;
            dx = (function(x + h, y) - function(x - h, y)) / (2 * h);
            dy = (function(x, y + h) - function(x, y - h)) / (2 * h);
            return (dx, dy);
        }

        /// <summary>
        /// Finds lowest point using gradient descent.
        /// </summary>
        /// <param name="function">Target function.</param>
        /// <param name="x0">Starting x.</param>
        /// <param name="y0">Starting y.</param>
        /// <param name="kmax">Maximum iterations.</param>
        /// <param name="max_step">Maximum step distance in a single iteration.</param>
        /// <param name="tol">Tolerance condition to stop optimizing.</param>
        /// <param name="a">Alpha.</param>
        /// <param name="l">Rate of decrease of alpha.</param>
        /// <param name="h">h for finite differences to find gradient.</param>
        /// <returns>(x, y) minimum point.</returns>
        public static (double, double) GradDescent(
            Func<double,double,double> function, 
            double x0, double y0, 
            int kmax = 1000, 
            double max_step = 1.0,
            double tol = 1e-3, 
            double a = 0.05,
            double l = 1.2,
            double h = 0.1)
        {
            double xc = x0, yc = y0;

            for (int i = 0; i < kmax; i++)
            {
                double xgrad, ygrad;
                (xgrad, ygrad) = Grad(function, xc, yc, h);
                
                //checking for max step
                if (Math.Sqrt(Math.Pow(a * xgrad, 2) + Math.Pow(a * ygrad, 2)) > max_step)
                {
                    xgrad = xgrad * (max_step / Math.Sqrt(Math.Pow(a * xgrad, 2) + Math.Pow(a * ygrad, 2)));
                    ygrad = ygrad * (max_step / Math.Sqrt(Math.Pow(a * xgrad, 2) + Math.Pow(a * ygrad, 2)));
                }

                double xnext = xc + a * -xgrad;
                double ynext = yc + a * -ygrad;

                //decreasing step
                if (function.Invoke(xnext, ynext) < function.Invoke(xc, yc))
                {
                    a /= l;
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
