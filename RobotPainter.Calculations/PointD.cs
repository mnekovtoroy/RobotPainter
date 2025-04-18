namespace RobotPainter.Calculations
{
    public struct PointD : IEquatable<PointD>
    {
        public double x, y;

        public PointD(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static PointD Add(PointD a, PointD b) => new PointD(a.x + b.x, a.y + b.y);

        public static PointD Substract(PointD a, PointD b) => new PointD(a.x - b.x, a.y - b.y);

        public static PointD operator +(PointD a, PointD b) => Add(a, b);

        public static PointD operator -(PointD a, PointD b) => Substract(a, b);

        public static bool operator ==(PointD a, PointD b) => a.x == b.x && a.y == b.y;

        public static bool operator !=(PointD a, PointD b) => a.x != b.x || a.y != b.y;

        public override readonly bool Equals(object? obj) => obj is PointD && Equals((PointD)obj);

        public readonly bool Equals(PointD other) => this == other;

        public override readonly int GetHashCode() => HashCode.Combine(x.GetHashCode(), y.GetHashCode());
    }
}