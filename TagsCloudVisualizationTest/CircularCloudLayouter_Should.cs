using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TagsCloudVisualisation.Extensions;
using TagsCloudVisualisation.Layouter;
using TagsCloudVisualisation.Visualizer;
using TagsCloudVisualisation.WordPreparer;

namespace TagsCloudVisualizationTest
{
    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private ICLoudLayouter layouter;
        private ICloudVisualizer visualizer;
        private Point layoutCenter;
        private const double DensityFactor = 0.6;
        private IWordPreparer wordPreparer;
        private IOrderedEnumerable<KeyValuePair<string, int>> words;

        [SetUp]
        public void SetUp()
        {
            layoutCenter = new Point(400, 300);
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "words.txt");
            var palette = new Palette(Color.White, Color.Black, Color.Brown);
            var container = new WindsorContainer();
            container.Register(Component.For<FileInfo>().Instance(new FileInfo(path)));
            container.Register(Component.For<Palette>().Instance(palette));
            container.Register(Component.For<ImageFormat>().Instance(ImageFormat.Png));
            container.Register(Component.For<IWordPreparer>().ImplementedBy<SimpleWordPreparer>());
            container.Register(Component.For<ICLoudLayouter>().ImplementedBy<CircularCloudLayouter>());
            container.Register(Component.For<ICloudVisualizer>().ImplementedBy<WordCloudVisualizer>());
            wordPreparer = container.Resolve<IWordPreparer>();
            layouter = container.Resolve<ICLoudLayouter>();
            visualizer = container.Resolve<ICloudVisualizer>();
            words = wordPreparer.PrepareWords(200);
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed)
                return;
            var name = TestContext.CurrentContext.Test.Name;
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, $"{name}.png");
            var layout = layouter.GetWordLayout();
            if (layout.Any())
                visualizer.VisualizeAndSave(layouter.GetWordLayout(), path);
            else
                visualizer.VisualizeAndSave(layouter.GetRectanglesLayout(), path);
            TestContext.WriteLine($"The image was saved to {path}");
        }

        [Test]
        public void PlaceInCenter_ZeroRectangle()
        {
            var rectangle = layouter.PutNextRectangle(new Size(0, 0));
            Assert.AreEqual(rectangle.Location.X, layoutCenter.X, 1e-3);
            Assert.AreEqual(rectangle.Location.Y, layoutCenter.Y, 1e-3);
        }

        [Test]
        public void PlaceWithoutIntersection_SeveralWords()
        {
            layouter.PutWords(words);
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "words.bmp");
            visualizer.VisualizeAndSave(layouter.GetWordLayout(), path);
        }

        [Test]
        public void PlaceInCenter_OneRectangle()
        {
            var rectangle = layouter.PutNextRectangle(new Size(100, 100));
            rectangle.Should().Match(rect => ((RectangleF)rect).IntersectsWith(new RectangleF(layoutCenter, new SizeF(0, 0))));

        }

        [Test]
        public void PlaceWithoutIntersection_TwoRectangles()
        {
            layouter.PutNextRectangle(new Size(73, 73));
            layouter.PutNextRectangle(new Size(73, 73));
            CheckIntersection(layouter.GetRectanglesLayout());
        }

        [Test]
        public void PlaceWithoutIntersection_TwoDifferentRectangles()
        {
            layouter.PutNextRectangle(new Size(100, 80));
            layouter.PutNextRectangle(new Size(50, 30));
            CheckIntersection(layouter.GetRectanglesLayout());

        }

        [Test]
        public void PlaceWithoutIntersection_ThreeRectangles()
        {
            layouter.PutNextRectangle(new SizeF(80, 50));
            layouter.PutNextRectangle(new SizeF(70, 70));
            layouter.PutNextRectangle(new SizeF(100, 30));
            CheckIntersection(layouter.GetRectanglesLayout());
        }

        [Test]
        [TestCase(1, TestName = "one")]
        [TestCase(2, TestName = "two")]
        [TestCase(3, TestName = "three")]
        [TestCase(4, TestName = "four")]
        [TestCase(5, TestName = "five")]
        [TestCase(8, TestName = "eight")]
        [TestCase(10, TestName = "ten")]
        [TestCase(20, TestName = "twenty")]
        [TestCase(50, TestName = "fifty")]
        [TestCase(100, TestName = "hundred")]
        public void PlaceWithoutIntersection_RandomRectangles(int rectanglesNumber)
        {
            var random = new Random();
            for (var i = 0; i < rectanglesNumber; i++)
                layouter.PutNextRectangle(new SizeF(random.Next(5, 8) * 10, random.Next(2, 5) * 10));
            CheckIntersection(layouter.GetRectanglesLayout());
        }

        [Test]
        public void PlaceWithoutIntersection_200Squares()
        {
            const int number = 200;
            for (var i = 0; i < number; i++)
            {
                layouter.PutNextRectangle(new SizeF(30, 30));
            }
            CheckIntersection(layouter.GetRectanglesLayout());
        }

        [Test]
        public void MakeDenseCircularLayout()
        {
            for (var i = 0; i < 200; i++)
                layouter.PutNextRectangle(new SizeF(30, 30));
            CheckCircularity(layouter.GetRectanglesLayout());

        }

        private void CheckIntersection(List<RectangleF> rectangles)
        {
            for (var i = 0; i < rectangles.Count - 1; i++)
                for (var j = i + 1; j < rectangles.Count; j++)
                    rectangles[i].Should().Match(rect => !((RectangleF)rect).IntersectsWith(rectangles[j]));
        }

        private void CheckCircularity(List<RectangleF> rectangles)
        {
            var farest = rectangles.OrderByDescending(rect => rect.Location.GetDistanceTo(layoutCenter)).FirstOrDefault();
            var circleRadius = farest.Location.GetDistanceTo(layoutCenter);
            var area = rectangles.Sum(rectangle => rectangle.Width * rectangle.Height);
            var circleArea = circleRadius * circleRadius * Math.PI;
            area.Should().BeGreaterOrEqualTo((float)(DensityFactor * circleArea));
        }
    }
}