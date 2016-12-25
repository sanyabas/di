using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualisation.Layouter
{
    public interface ICloudLayouter
    {
        List<RectangleF> GetRectanglesLayout();
        Dictionary<RectangleF, string> GetWordLayout();

        Result<RectangleF> PutNextRectangle(SizeF rectangleSize);

        Result<None> PutNextWord(string word, int number);

        Result<None> PutWords(IEnumerable<KeyValuePair<string, int>> words);

        PointF GetCenter();
    }
}