using System;
using System.Drawing;
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
            var container = new WindsorContainer();
            container.Register(Component.For<FileInfo>().Instance(new FileInfo(options.WordsFile)));
            container.Install(new TagsCloudVisualizationInstaller());
            var preparer = container.Resolve<IWordPreparer>();
            var layouter = container.Resolve<ICloudLayouter>();
            var visualizer = container.Resolve<ICloudVisualizer>(new { fontName = "Times New Roman" });
            preparer.PrepareWords(options.RectanglesNumber)
                .Then(words => layouter.PutWords(words))
                .Then(words => visualizer.VisualizeAndSave(layouter.GetWordLayout(), options.OutputFileName))
                .OnFail(Console.WriteLine);
        }
    }
}
