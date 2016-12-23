using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using TagsCloudVisualisation.Extensions;
using TagsCloudVisualisation.Layouter;

namespace TagsCloudVisualisation.Visualizer
{
    public class WordCloudVisualizer : ICloudVisualizer
    {
        private readonly ImageFormat format;
        private readonly string fontName;
        private readonly Color backgroundColor;
        private readonly Pen visualizationPen;
        private readonly Brush wordBrush;
        private readonly int bitmapWidth;
        private readonly int bitmapHeight;

        public WordCloudVisualizer(ICLoudLayouter layouter, Palette palette, ImageFormat format, string fontName)
        {
            this.format = format;
            this.fontName = fontName;
            var center = layouter.GetCenter();
            bitmapWidth = (int)(center.X * 2);
            bitmapHeight = (int)(center.Y * 2);
            backgroundColor = palette.Background;
            visualizationPen = new Pen(palette.RectangleColor, 3);
            wordBrush = new SolidBrush(palette.WordColor);
        }

        public Result<Bitmap> Visualize(Dictionary<RectangleF, string> layout)
        {

            var bitmap = new Bitmap(bitmapWidth, bitmapHeight);
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(backgroundColor);
            float fontSize = 90;
            var stringFormat = new StringFormat { Alignment = StringAlignment.Center };
            foreach (var word in layout)
            {
                var font = graphics.GetBiggestFont(word.Value, fontName, fontSize, word.Key);
                graphics.DrawString(word.Value, font, wordBrush, word.Key, stringFormat);
                graphics.DrawRectangles(visualizationPen, new[] { word.Key });
                fontSize = font.Size;
            }
            return Result.Ok(bitmap);
        }

        public Result<None> VisualizeAndSave(Dictionary<RectangleF, string> layout, string filename)
        {
            using (var bitmap = Visualize(layout)
                .ReplaceError(error => "Couldn't create image")
                .Then(pic => Save(pic, filename, format)))
                return bitmap;

        }

        public Result<None> Save(Bitmap image, string filename, ImageFormat imageFormat)
        {
            return Result.OfAction(() => image.Save(filename, imageFormat));
        }

        public Result<None> VisualizeAndSave(IEnumerable<RectangleF> layout, string filename)
        {
            return VisualizeAndSave(layout.ToDictionary(rect => rect, rect => ""), filename);
        }
    }
}