using System;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualisation.Extensions;

namespace TagsCloudVisualizationTest
{
    [TestFixture]
    public class PointFExtensions_Should
    {
        [Test]
        public void ReturnSamePoint_AddWithZero()
        {
            var initialPoint = new PointF(10, 7);
            var result = initialPoint.Add(new PointF(0, 0));
            result.Should().Be(initialPoint);
        }

        [Test]
        [TestCase(0, 0, 1, 1, 1, 1)]
        [TestCase(-1, -5, 1, 5, 0, 0)]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(-10, -20, 5, 6, -5, -14)]
        [TestCase(10, 30, 15, 11, 25, 41)]
        public void AddPoints(float initX, float initY, float dx, float dy, float expectedX, float expectedY)
        {
            var initialPoint = new PointF(initX, initY);
            var delta = new PointF(dx, dy);
            var expected = new PointF(expectedX, expectedY);
            var actual = initialPoint.Add(delta);
            actual.Should().Be(expected);
        }

        [Test]
        [TestCase(0, 0, 1, 1, -1, -1)]
        [TestCase(-1, -5, -1, -5, 0, 0)]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(-10, -20, 5, 6, -15, -26)]
        [TestCase(10, 30, 5, 11, 5, 19)]
        public void SubPoints(float initX, float initY, float dx, float dy, float expectedX, float expectedY)
        {
            var initialPoint = new PointF(initX, initY);
            var delta = new PointF(dx, dy);
            var expected = new PointF(expectedX, expectedY);
            var actual = initialPoint.Sub(delta);
            actual.Should().Be(expected);
        }

        [Test]
        public void ReturnSamePoint_ReturnOnZeroAngle()
        {
            var initialPoint = new PointF(10, 20);
            var center = new PointF(0, 0);
            var result = initialPoint.RotateAround(center, 0);
            result.Should().Be(initialPoint);
        }

        [Test]
        [TestCase(0, 0, Math.PI, 0, 0, 0, 0)]
        [TestCase(1, 0, Math.PI, -1, 0, -3, 0)]
        [TestCase(1, 0, 2 * Math.PI, -1, 0, 1, 0)]
        [TestCase(1, 0, Math.PI / 2, 0, 0, 0, 1)]
        public void RotatePointsProperly(float initX, float initY, double angle, float centerX, float centerY, float expectedX, float expectedY)
        {
            var initialPoint = new PointF(initX, initY);
            var center = new PointF(centerX, centerY);
            var result = initialPoint.RotateAround(center, angle);
            result.X.Should().BeApproximately(expectedX, 1e-3f);
            result.Y.Should().BeApproximately(expectedY, 1e-3f);
        }
    }
}