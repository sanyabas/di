using System.Drawing;

namespace TagsCloudVisualisation.Extensions
{
    public static class SizeFExtensions
    {
        public static bool IsBiggerThan(this SizeF size, SizeF other)
        {
            return size.Width > other.Width+1 || size.Height > other.Height+1;
        }
    }
}