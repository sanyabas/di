using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudVisualisation.Visualizer
{
    public interface ICloudVisualizer
    {
        Result<Bitmap> Visualize(Dictionary<RectangleF, string> layout);

        Result<None> VisualizeAndSave(Dictionary<RectangleF, string> layout, string filename);

        Result<None> VisualizeAndSave(IEnumerable<RectangleF> layout, string filename);
    }
}