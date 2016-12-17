using System;
using System.Drawing;

namespace TagsCloudVisualisation.Extensions
{
    public static class PointFExtensions
    {
        public static PointF Add(this PointF point, PointF another)
        {
            return new PointF(point.X + another.X, point.Y + another.Y);
        }

        public static PointF Sub(this PointF point, PointF another)
        {
            return new PointF(point.X - another.X, point.Y - another.Y);
        }

        public static PointF RotateAround(this PointF point, PointF center, double angle)
        {
            var movedPoint = point.Sub(center);
            var x = (float)(movedPoint.X * Math.Cos(angle) - movedPoint.Y * Math.Sin(angle));
            var y = (float)(movedPoint.X * Math.Sin(angle) + movedPoint.Y * Math.Cos(angle));
            return new PointF(x, y).Add(center);
        }

        public static bool IsBehindBounds(this PointF point, SizeF bounds)
        {
            return point.X > bounds.Width || point.X < 0 || point.Y > bounds.Height || point.Y < 0;
        }

        public static double GetDistanceTo(this PointF point, PointF other)
        {
            return Math.Sqrt((point.X - other.X) * (point.X - other.X) + (point.Y - other.Y) * (point.Y - other.Y));
        }
    }
}