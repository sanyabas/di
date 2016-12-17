using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualisation.Layouter
{
    public interface ICLoudLayouter
    {
        List<RectangleF> GetRectanglesLayout();
        Dictionary<RectangleF, string> GetWordLayout();

        RectangleF PutNextRectangle(SizeF rectangleSize);

        RectangleF PutNextWord(string word, int number);

        void PutWords(IEnumerable<KeyValuePair<string, int>> words);

        PointF GetCenter();
    }
}