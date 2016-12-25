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

        public Result<IEnumerable<string>> LoadWords(FileInfo file)
        {
            var words =
                Result.Of<IEnumerable<string>>(() => File.ReadAllLines(file.FullName))
                    .RefineError("Couldn't read words from file");
            return words;
        }

        public IOrderedEnumerable<KeyValuePair<string, int>> CountWordFrequency(IEnumerable<string> words)
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

        public Result<IOrderedEnumerable<KeyValuePair<string, int>>> PrepareWords(int number)
        {
            return LoadWords(WordsFile)
                .Then(w => w.Where(word => word.Length > 3).Select(word => word.ToLower()))
                .Then(CountWordFrequency)
                .Then(w => w.Take(number).OrderByDescending(pair => pair.Value));
        }
    }
}