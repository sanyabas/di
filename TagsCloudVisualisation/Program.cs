using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommandLine;
using TagsCloudVisualisation.Layouter;
using TagsCloudVisualisation.Visualizer;
using TagsCloudVisualisation.WordPreparer;

namespace TagsCloudVisualisation
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var options = new CommandLineOptions();
            if (!Parser.Default.ParseArguments(args, options))
                return;
            var palette = new Palette(Color.White, Color.Black, Color.Brown);
            var container = new WindsorContainer();
            container.Register(Component.For<FileInfo>().Instance(new FileInfo(options.WordsFile)));
            container.Register(Component.For<IWordPreparer>().ImplementedBy<SimpleWordPreparer>());
            container.Register(Component.For<ICLoudLayouter>().ImplementedBy<CircularCloudLayouter>());
            container.Register(Component.For<ICloudVisualizer>().ImplementedBy<WordCloudVisualizer>());
            container.Register(Component.For<Palette>().Instance(palette));
            container.Register(Component.For<ImageFormat>().Instance(ImageFormat.Png));
            var preparer = container.Resolve<IWordPreparer>();
            var layouter = container.Resolve<ICLoudLayouter>();
            var visualizer = container.Resolve<ICloudVisualizer>(new { fontName = "Times New Roman" });
            layouter.PutWords(preparer.PrepareWords(options.RectanglesNumber));
            visualizer.VisualizeAndSave(layouter.GetWordLayout(), options.OutputFileName);
        }
    }
}
