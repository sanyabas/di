using CommandLine;

namespace TagsCloudVisualisation
{
    public class CommandLineOptions
    {
        [Option('n', "number", DefaultValue = 50)]
        public int RectanglesNumber { get; set; }

        [Option('h', "height", DefaultValue = 30)]
        public int Height { get; set; }

        [Option('w', "width", DefaultValue = 30)]
        public int Width { get; set; }

        [Option('o', "output", DefaultValue = @"main.png")]
        public string OutputFileName { get; set; }

        [Option('f', "words file", DefaultValue = "words.txt")]
        public string WordsFile { get; set; }
    }
}