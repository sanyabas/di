using System.Drawing;
using System.Drawing.Imaging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using TagsCloudVisualisation.Visualizer;

namespace TagsCloudVisualisation
{
    public class TagsCloudVisualizationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var palette = new Palette(Color.White, Color.Black, Color.Brown);
            container.Register(Component.For<Palette>().Instance(palette));
            container.Register(
                Classes.FromThisAssembly()
                    .InNamespace("TagsCloudVisualisation.WordPreparer")
                    .WithService.DefaultInterfaces());
            container.Register(
                Classes.FromThisAssembly()
                    .InNamespace("TagsCloudVisualisation.Layouter")
                    .WithService.DefaultInterfaces());
            container.Register(
                Classes.FromThisAssembly()
                    .InNamespace("TagsCloudVisualisation.Visualizer")
                    .WithService.DefaultInterfaces());
            container.Register(Component.For<ImageFormat>().Instance(ImageFormat.Png));
        }
    }
}