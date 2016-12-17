using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualisation.Extensions
{
    public static class RectangleFExtensions
    {
        public static bool IsBehindBounds(this RectangleF rectangle, SizeF bounds)
        {
            return rectangle.Bottom > bounds.Height || rectangle.Top < 0 || rectangle.Right > bounds.Width ||
                   rectangle.Left < 0;
        }

        public static bool IntersectsWith(this RectangleF rectangle, IEnumerable<RectangleF> others)
        {
            return others.Any(rect => rect.IntersectsWith(rectangle));
        }

    }
}