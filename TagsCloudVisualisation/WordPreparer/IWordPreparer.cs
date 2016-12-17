using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagsCloudVisualisation.WordPreparer
{
    public interface IWordPreparer
    {
        IEnumerable<string> LoadWords(FileInfo file);
        IOrderedEnumerable<KeyValuePair<string, int>> SortWords(IEnumerable<string> words);
        IOrderedEnumerable<KeyValuePair<string, int>> PrepareWords(int limit);
    }
}