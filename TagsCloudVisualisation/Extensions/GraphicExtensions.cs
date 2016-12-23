using System.Drawing;

namespace TagsCloudVisualisation.Extensions
{
    public static class GraphicExtensions
    {
        public static Font GetBiggestFont(this Graphics graphics, string word, string fontName, float maxFontSize, RectangleF bounds)
        {
            var font = new Font(fontName, maxFontSize);
            if (string.IsNullOrEmpty(word))
                return font;
            var size = graphics.MeasureString(word, font);
            var fontSize = maxFontSize - 1;
            while (size.IsBiggerThan(bounds.Size))
            {
                font = new Font(fontName, fontSize);
                size = graphics.MeasureString(word, font);
                fontSize--;
            }
            return font;
        }
    }
}