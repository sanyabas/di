using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagsCloudVisualisation.WordPreparer
{
    public class SimpleWordPreparer : IWordPreparer
    {
        public FileInfo WordsFile { get; }

        public SimpleWordPreparer(FileInfo wordsFile)
        {
            this.WordsFile = wordsFile;
        }

        public IEnumerable<string> LoadWords(FileInfo file)
        {
            var words = File.ReadAllLines(file.FullName);
            return words;
        }

        public IOrderedEnumerable<KeyValuePair<string, int>> SortWords(IEnumerable<string> words)
        {
            var dictionary = new Dictionary<string, int>();
            foreach (var word in words)
            {
                if (dictionary.ContainsKey(word))
                    dictionary[word]++;
                else
                    dictionary[word] = 1;
            }
            return dictionary.OrderByDescending(pair => pair.Value);
        }

        public IOrderedEnumerable<KeyValuePair<string, int>> PrepareWords(int number)
        {
            var words = LoadWords(WordsFile).Where(word => word.Length > 3).Select(word => word.ToLower());
            return SortWords(words).Take(number).OrderByDescending(pair => pair.Value);
        }
    }
}