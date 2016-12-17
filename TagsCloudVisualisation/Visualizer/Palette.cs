using System.Drawing;

namespace TagsCloudVisualisation.Visualizer
{
    public class Palette
    {
        public Palette(Color background, Color wordColor, Color rectangleColor)
        {
            Background = background;
            WordColor = wordColor;
            RectangleColor = rectangleColor;
        }

        public Color Background { get; }
        public Color WordColor { get; }
        public Color RectangleColor { get; }
    }
}