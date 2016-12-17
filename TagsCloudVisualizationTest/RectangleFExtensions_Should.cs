using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualisation.Extensions;

namespace TagsCloudVisualizationTest
{
    [TestFixture]
    public class RectangleFExtensions_Should
    {
        [Test]
        [TestCase(0, 0, 10, 10, 20, 20, false)]
        [TestCase(0, 0, 20, 15, 10, 10, true)]
        [TestCase(0, 0, 10, 10, 10, 10, false)]
        public void CheckIfRectangleBeyondBounds(float rectX, float rectY, float width, float height, float boundWidth, float boundHeight, bool expected)
        {
            var rectangle = new RectangleF(rectX, rectY, width, height);
            var bounds = new SizeF(boundWidth, boundHeight);
            var isBeyond = rectangle.IsBehindBounds(bounds);
            isBeyond.Should().Be(expected);
        }
    }

    [TestFixture]
    public class Rectangle_IntersectionCheck_Should
    {
        private RectangleF rectangle;

        [SetUp]
        public void SetUp()
        {
            rectangle = new RectangleF(0, 0, 10, 10);
        }

        [Test]
        public void NotIntersect_WithNoRectangles()
        {
            var rectangles = new RectangleF[0];
            AssertIntersection(rectangle, rectangles, false);
        }

        [Test]
        public void Intersect_WithItself()
        {
            var rectangles = new[] { rectangle };
            AssertIntersection(rectangle, rectangles, true);
        }

        [Test]
        public void Intersect_WithBiggerRectangle()
        {
            var rectangles = new[] { new RectangleF(-20, -20, 100, 100) };
            AssertIntersection(rectangle, rectangles, true);
        }

        [Test]
        public void Intersect_WithSeveralRectangles()
        {
            var rectangles = new[]
                {new RectangleF(0, 0, 1, 1), new RectangleF(-5, -5, 2, 2), new RectangleF(5, 5, 7, 7)};
            AssertIntersection(rectangle, rectangles, true);
        }

        private void AssertIntersection(RectangleF checkedRectangle, IEnumerable<RectangleF> rectangles, bool expected)
        {
            var result = checkedRectangle.IntersectsWith(rectangles);
            result.Should().Be(expected);
        }
    }
}