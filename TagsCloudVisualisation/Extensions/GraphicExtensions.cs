using System.Drawing;

namespace TagsCloudVisualisation.Extensions
{
    public static class GraphicExtensions
    {
        public static Font GetBiggestFont(this Graphics graphics, string word, int maxFontSize, RectangleF bounds)
        {
            var font = new Font("Calibri", maxFontSize);
            var size = graphics.MeasureString(word, font);
            var fontSize = maxFontSize - 1;
            while (size.IsBiggerThan(bounds.Size))
            {
                font = new Font("Calibri", fontSize);
                size = graphics.MeasureString(word, font);
                fontSize--;
            }
            return font;
        }
    }
}