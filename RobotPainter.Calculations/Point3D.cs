namespace RobotPainter.Calculations
{
    public struct Point3D : IEquatable<Point3D>
    {
        public double x, y, z;

        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Point3D Add(Point3D a, Point3D b) => new Point3D(a.x + b.x, a.y + b.y, a.z + b.z);

        public static Point3D Substract(Point3D a, Point3D b) => new Point3D(a.x - b.x, a.y - b.y, a.z - b.z);

        public static Point3D Multiply(double a, Point3D b) => new Point3D(a * b.x, a * b.y, a * b.z);

        public static Point3D Divide(Point3D a, double b) => new Point3D(a.x / b, a.y / b, a.z / b);

        public static Point3D operator +(Point3D a, Point3D b) => Add(a, b);

        public static Point3D operator -(Point3D a, Point3D b) => Substract(a, b);

        public static Point3D operator *(double a, Point3D b) => Multiply(a, b);

        public static Point3D operator *(Point3D a, double b) => Multiply(b, a);

        public static Point3D operator /(Point3D a, double b) => Divide(a, b);

        public static bool operator ==(Point3D a, Point3D b) => a.x == b.x && a.y == b.y && a.z == b.z;

        public static bool operator !=(Point3D a, Point3D b) => a.x != b.x || a.y != b.y || a.z != b.z;

        public override readonly bool Equals(object? obj) => obj is Point3D && Equals((Point3D)obj);

        public readonly bool Equals(Point3D other) => this == other;

        public override readonly int GetHashCode() => HashCode.Combine(x.GetHashCode(), y.GetHashCode(), z.GetHashCode());
    }
}