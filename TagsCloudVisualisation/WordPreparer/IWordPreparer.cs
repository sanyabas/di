using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagsCloudVisualisation.WordPreparer
{
    public interface IWordPreparer
    {
        Result<IEnumerable<string>> LoadWords(FileInfo file);
        IOrderedEnumerable<KeyValuePair<string, int>> CountWordFrequency(IEnumerable<string> words);
        Result<IOrderedEnumerable<KeyValuePair<string, int>>> PrepareWords(int limit);
    }
}