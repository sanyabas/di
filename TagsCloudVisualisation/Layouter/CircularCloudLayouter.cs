using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TagsCloudVisualisation.Extensions;

namespace TagsCloudVisualisation.Layouter
{
    public class CircularCloudLayouter : ICloudLayouter
    {
        private const float SpiralRotationAngle = (float)(-Math.PI / 10);
        private const double BoundRotationAngle = -Math.PI / 6;
        private const int Delta = 5;
        private const int RotationLimit = 12;
        private static readonly float SpiralLengthDelta = (float)Math.Sin(Math.PI / 4);
        private readonly SizeF bounds;
        private readonly PointF center;
        private PointF previousRadiusPoint;
        private readonly List<RectangleF> rectangles;
        private static readonly SizeF FirstRectangle = new SizeF(200, 100);
        private int biggestFrequency;
        private readonly Dictionary<RectangleF, string> wordsLayout;

        public CircularCloudLayouter(PointF center)
        {
            this.center = center;
            rectangles = new List<RectangleF>();
            wordsLayout = new Dictionary<RectangleF, string>();
            bounds = new SizeF(center.X * 2, center.Y * 2);
        }

        public CircularCloudLayouter() : this(new PointF(400, 300))
        {
        }

        public List<RectangleF> GetRectanglesLayout()
        {
            return rectangles;
        }

        public Dictionary<RectangleF, string> GetWordLayout()
        {
            return wordsLayout;
        }

        public Result<RectangleF> PutNextRectangle(SizeF rectangleSize)
        {
            Result<RectangleF> result;
            if (wordsLayout.Count == 0 && rectangles.Count == 0)
            {
                var delta = new PointF(rectangleSize.Width / 2, rectangleSize.Height / 2);
                var placingPoint = center.Sub(delta);
                result = new RectangleF(placingPoint, rectangleSize);
            }
            else
            {
                result = FindSuitablePoint(rectangleSize)
                    .Then(point => previousRadiusPoint = point)
                    .Then(point => GetProcessedRectangle(point, rectangleSize));
            }
            result.Then(rect => rectangles.Add(rect));
            return result;
        }

        public Result<None> PutWords(IEnumerable<KeyValuePair<string, int>> words)
        {
            Result<None> result = Result.Ok();
            foreach (var word in words)
            {
                result = PutNextWord(word.Key, word.Value);
                if (!result.IsSuccess)
                    break;
            }
            return result;
        }

        public Result<None> PutNextWord(string word, int number)
        {
            if (rectangles.Count == 0)
            {
                biggestFrequency = number;
            }
            var factor = (float)(number * 1.25 / biggestFrequency);
            var currentSize = new SizeF(FirstRectangle.Width * factor, FirstRectangle.Height * factor);
            var rectangle = PutNextRectangle(currentSize)
                .Then(rect => wordsLayout.Add(rect, word))
                .RefineError($"Couldn't place word {word}");
            return rectangle;
        }

        public PointF GetCenter()
        {
            return center;
        }

        private Result<PointF> GetNextSpiralPoint(PointF previousPoint)
        {
            if (rectangles.Count == 1)
            {
                var previousRectangle = rectangles.Last();
                return new PointF(previousRectangle.Right, previousRectangle.Top);
            }
            var result = RotateAndIncreaseRadius(previousPoint);
            var number = 0;
            while (result.IsBehindBounds(bounds) && (number <= RotationLimit))
            {
                number++;
                result = result.RotateAround(center, BoundRotationAngle);
            }
            return result.IsBehindBounds(bounds) ? Result.Fail<PointF>("Couldn't place a point") : result;
        }

        private PointF RotateAndIncreaseRadius(PointF previousPoint)
        {
            var rotated = previousPoint.RotateAround(center, SpiralRotationAngle);
            var relativeToCenter = rotated.Sub(center);
            var xShift = Math.Sign(relativeToCenter.X) * SpiralLengthDelta;
            var yShift = Math.Sign(relativeToCenter.Y) * SpiralLengthDelta;
            var shift = new PointF(xShift, yShift);
            var result = relativeToCenter.Add(shift).Add(center);
            return result;
        }

        private Result<RectangleF> GetProcessedRectangle(PointF placingPoint, SizeF rectangleSize)
        {
            var tempRectangle = new RectangleF(placingPoint, rectangleSize);
            var tempResult = ShiftToCenter(tempRectangle)
                .Then(TryRotateInBounds)
                .Then(ShiftToCenter);
            return tempResult;
        }

        private Result<PointF> FindSuitablePoint(SizeF rectangleSize)
        {
            Result<bool> intersects;
            while (true)
            {
                intersects = GetNextSpiralPoint(previousRadiusPoint)
                    .Then(point => previousRadiusPoint = point)
                    .Then(point => new RectangleF(point, rectangleSize))
                    .Then(rect => rect.IntersectsWith(rectangles));
                if (intersects.Value && intersects.IsSuccess)
                    continue;
                break;
            }

            return intersects.IsSuccess ? previousRadiusPoint : Result.Fail<PointF>(intersects.Error);
        }

        private Result<RectangleF> ShiftToCenter(RectangleF initialPoint)
        {
            var shiftedByDiagonal = ShiftToCenter(initialPoint, GetShiftToCenter, rect => true);
            var shiftedByX = ShiftToCenter(shiftedByDiagonal, GetShiftToCenterByX,
                rect => Math.Abs(rect.X - center.X) > 5 && Math.Abs(rect.X - center.X) < center.X);
            var shiftedByY = ShiftToCenter(shiftedByX, GetShiftToCenterByY, rect => Math.Abs(rect.Y - center.Y) > 5);
            return shiftedByY;
        }

        private RectangleF ShiftToCenter(RectangleF initial, Func<RectangleF, PointF> getShift,
            Func<RectangleF, bool> condition)
        {
            var shift = getShift(initial);
            var tempResult = new RectangleF(initial.Location.Add(shift), initial.Size);
            while (!tempResult.IntersectsWith(rectangles) && condition(tempResult))
            {
                shift = getShift(tempResult);
                tempResult = new RectangleF(tempResult.Location.Add(shift), tempResult.Size);
            }
            return new RectangleF(tempResult.Location.Sub(shift), tempResult.Size);
        }

        private PointF GetShiftToCenter(RectangleF rectangle)
        {
            var dx = rectangle.X < center.X ? Delta : -Delta;
            var dy = rectangle.Y < center.Y ? Delta : -Delta;
            return new PointF(dx, dy);
        }

        private PointF GetShiftToCenterByX(RectangleF rectangle)
        {
            var dx = rectangle.X < center.X ? Delta : -Delta;
            return new PointF(dx, 0);
        }

        private PointF GetShiftToCenterByY(RectangleF rectangle)
        {
            var dy = rectangle.Y < center.Y ? Delta : -Delta;
            return new PointF(0, dy);
        }

        private Result<RectangleF> TryRotateInBounds(RectangleF rectangle)
        {
            var number = 0;
            while ((rectangle.IntersectsWith(rectangles) || rectangle.IsBehindBounds(bounds)) &&
                   (number < RotationLimit))
            {
                number++;
                rectangle = new RectangleF(rectangle.Location.RotateAround(center, BoundRotationAngle), rectangle.Size);
            }
            Result<RectangleF> result;
            if (rectangle.IntersectsWith(rectangles) || rectangle.IsBehindBounds(bounds))
                result = Result.Fail<RectangleF>("Couldn't place rectangke");
            else result = rectangle.AsResult();
            return result;
        }
    }
}