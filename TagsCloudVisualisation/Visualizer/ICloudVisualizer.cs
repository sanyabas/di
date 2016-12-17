using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudVisualisation.Visualizer
{
    public interface ICloudVisualizer
    {
        Bitmap Visualize(Dictionary<RectangleF, string> layout);

        void VisualizeAndSave(Dictionary<RectangleF, string> layout, string filename);

        void VisualizeAndSave(IEnumerable<RectangleF> layout, string filename);
    }
}