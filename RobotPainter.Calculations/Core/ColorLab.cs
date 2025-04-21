using System.Drawing;

namespace RobotPainter.Calculations.Core
{
    public struct ColorLab
    {
        public double L;
        public double a;
        public double b;

        public ColorLab(double L, double a, double b)
        {
            this.L = L;
            this.a = a;
            this.b = b;
        }

        public static ColorLab FromRgb(Color color)
        {
            //formulas: https://www.easyrgb.com/en/math.php
            //rgb to xyz
            double R = pivotRgb(color.R / 255.0);
            double G = pivotRgb(color.G / 255.0);
            double B = pivotRgb(color.B / 255.0);


            double x = R * 0.4124 + G * 0.3576 + B * 0.1805;
            double y = R * 0.2126 + G * 0.7152 + B * 0.0722;
            double z = R * 0.0193 + G * 0.1192 + B * 0.9505;


            //xyz to L*a*b*
            double ref_X = 95.047; // Observer= 2°, Illuminant= D65
            double ref_Y = 100.000;
            double ref_Z = 108.883;

            x = pivotXyz(x / ref_X);
            y = pivotXyz(y / ref_Y);
            z = pivotXyz(z / ref_Z);

            double L = 116 * y - 16;
            double a = 500 * (x - y);
            double b = 200 * (y - z);

            return new ColorLab(L, a, b);
        }

        public Color ToRgb()
        {
            //L*a*b* to xyz
            double ref_X = 95.047; // Observer= 2°, Illuminant= D65
            double ref_Y = 100.000;
            double ref_Z = 108.883;

            double y = (L + 16) / 116.0;
            double x = a / 500.0 + y;
            double z = y - b / 200.0;

            x = reverse_pivotXyz(x) * ref_X;
            y = reverse_pivotXyz(y) * ref_Y;
            z = reverse_pivotXyz(z) * ref_Z;

            //xyz to rgb
            x /= 100.0;
            y /= 100.0;
            z /= 100.0;

            double R = x * 3.2406 + y * -1.5372 + z * -0.4986;
            double G = x * -0.9689 + y * 1.8758 + z * 0.0415;
            double B = x * 0.0557 + y * -0.2040 + z * 1.0570;

            R = reverse_pivotRgb(R) * 255;
            G = reverse_pivotRgb(G) * 255;
            B = reverse_pivotRgb(B) * 255;

            var c = Color.FromArgb(Convert.ToInt32(R), Convert.ToInt32(G), Convert.ToInt32(B));
            return c;
        }

        private static double pivotRgb(double n)
        {
            return 100.0 * (n > 0.04045 ? Math.Pow((n + 0.055) / 1.055, 2.4) : n / 12.92);
        }

        private static double pivotXyz(double n)
        {
            return n > 0.008856 ? Math.Cbrt(n) : 7.787 * n + 16.0 / 116.0;
        }

        private static double reverse_pivotRgb(double n)
        {
            return n > 0.0031308 ? 1.055 * Math.Pow(n, 1.0 / 2.4) - 0.055 : 12.92 * n;
        }

        private static double reverse_pivotXyz(double n)
        {
            return Math.Pow(n, 3) > 0.008856 ? Math.Pow(n, 3) : (n - 16.0 / 116.0) / 7.787;
        }

        public double DeltaE76(ColorLab c)
        {
            double dist = Math.Sqrt(
                Math.Pow(c.L - L, 2) +
                Math.Pow(c.a - a, 2) +
                Math.Pow(c.b - b, 2));
            return dist;
        }

        public double DeltaE2000(ColorLab c)
        {
            double wL = 1;
            double wC = 0.045;
            double wH = 0.015;

            double L1 = L; double L2 = c.L;
            double a1 = a; double a2 = c.a;
            double b1 = b; double b2 = c.b;

            double xC1 = Math.Sqrt(a1 * a1 + b1 * b1);
            double xC2 = Math.Sqrt(a2 * a2 + b2 * b2);
            double xCX = (xC1 + xC2) / 2;
            double xGX = 0.5 * (1 - Math.Sqrt(Math.Pow(xCX, 7) / (Math.Pow(xCX, 7) + Math.Pow(25, 7))));
            double xNN = (1 + xGX) * a1;
            xC1 = Math.Sqrt(xNN * xNN + b1 * b1);
            double xH1 = CieLab2Hue(xNN, b1);
            xNN = (1 + xGX) * a2;
            xC2 = Math.Sqrt(xNN * xNN + b2 * b2);
            double xH2 = CieLab2Hue(xNN, b2);
            double xDL = L2 - L1;
            double xDC = xC2 - xC1;
            double xDH;
            if(xC1 * xC2 == 0)
            {
                xDH = 0;
            } else
            {
                xNN = Math.Round(xH2 - xH1, 12);
                if(Math.Abs(xNN) <= 180)
                {
                    xDH = xH2 - xH1;
                } else
                {
                    if (xNN > 180)
                        xDH = xH2 - xH1 - 360;
                    else
                        xDH = xH2 - xH1 + 360;
                }
            }

            xDH = 2 * Math.Sqrt(xC1 * xC2) * Math.Sin(xDH / 2 * Math.PI / 180.0);
            double xLX = (L1 + L2) / 2;
            double xCY = (xC1 + xC2) / 2;
            double xHX;
            if (xC1 * xC2 == 0)
            {
                xHX = xH1 + xH2;
            }
            else
            {
                xNN = Math.Abs(Math.Round(xH1 - xH2, 12));
                if (xNN > 180)
                {
                    if (xH2 + xH1 < 360) 
                        xHX = xH1 + xH2 + 360;
                    else 
                        xHX = xH1 + xH2 - 360;
                }
                else
                {
                    xHX = xH1 + xH2;
                }
                xHX /= 2;
            }
            double xTX = 1 - 0.17 * Math.Cos((xHX - 30) * Math.PI / 180.0)
                           + 0.24 * Math.Cos(2 * xHX * Math.PI / 180.0)
                           + 0.32 * Math.Cos((3 * xHX + 6) * Math.PI / 180.0)
                           - 0.20 * Math.Cos((4 * xHX - 63) * Math.PI / 180.0);
            double xPH = 30 * Math.Exp(-((xHX - 275) / 25) * ((xHX - 275) / 25));
            double xRC = 2 * Math.Sqrt(Math.Pow(xCY, 7) / (Math.Pow(xCY, 7) + Math.Pow(25, 7)));
            double xSL = 1 + 0.015 * ((xLX - 50) * (xLX - 50))
                     / Math.Sqrt(20 + (xLX - 50) * (xLX - 50));

            double xSC = 1 + 0.045 * xCY;
            double xSH = 1 + 0.015 * xCY * xTX;
            double xRT = -Math.Sin(2 * xPH * Math.PI / 180.0) * xRC;
            xDL = xDL / (wL * xSL);
            xDC = xDC / (wC * xSC);
            xDH = xDH / (wH * xSH);

            double dE = Math.Sqrt(xDL*xDL + xDC*xDC + xDH*xDH + xRT*xDC*xDH);
            return dE;
        }

        private double CieLab2Hue(double var_a, double var_b)
        {
            double var_bias = 0;
            if (var_a >= 0 && var_b == 0) return 0;
            if (var_a < 0 && var_b == 0) return 180;
            if (var_a == 0 && var_b > 0) return 90;
            if (var_a == 0 && var_b < 0) return 270;
            if (var_a > 0 && var_b > 0) var_bias = 0;
            if (var_a < 0) var_bias = 180;
            if (var_a > 0 && var_b < 0) var_bias = 360;
            return Math.Atan(var_b / var_a) * 180.0 / Math.PI + var_bias;
        }

        public static ColorLab Substract(ColorLab a, ColorLab b) => new ColorLab(a.L - b.L, a.a - b.a, a.b - b.b);

        public static ColorLab operator -(ColorLab a, ColorLab b) => Substract(a, b);
    }
}
