using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using rt;

namespace rt_test
{
    class TestPattern : Pattern
    {
        public override Color PatternAt(Point pt)
        {
            return new Color(pt.x, pt.y, pt.z);
        }
    }

    [TestClass]
    public class PatternsTest
    {
        [TestMethod]
        public void TheDefaultPatternTransformation()
        {
            var pattern = new TestPattern();
            Assert.AreEqual(Matrix.Identity(4), pattern.Transform);
        }
        [TestMethod]
        public void ApplyingAPatternTransformation()
        {
            var transformation = Transform.Translation(1, 2, 3);
            var pattern = new TestPattern
            {
                Transform = transformation
            };
            Assert.AreEqual(transformation, pattern.Transform);
        }

        [TestMethod]
        public void APatternWithAnObjectTransformation()
        {
            var shape = new Sphere
            {
                Transform = Transform.Scaling(2, 2, 2)
            };
            var pattern = new TestPattern();
            var c = pattern.PatternAtObject(shape, new Point(2, 3, 4));
            Assert.AreEqual(new Color(1, 1.5, 2), c);
        }

        [TestMethod]
        public void APatternWithAPatternTransformation()
        {
            var shape = new Sphere();
            var pattern = new TestPattern
            {
                Transform = Transform.Scaling(2, 2, 2)
            };
            var c = pattern.PatternAtObject(shape, new Point(2, 3, 4));
            Assert.AreEqual(new Color(1, 1.5, 2), c);
        }

        [TestMethod]
        public void APatternWithBothAnObjectAndPatternTransformation()
        {
            var shape = new Sphere
            {
                Transform = Transform.Scaling(2, 2, 2)
            };
            var pattern = new TestPattern
            {
                Transform = Transform.Translation(0.5, 1, 1.5)
            };
            var c = pattern.PatternAtObject(shape, new Point(2.5, 3, 3.5));
            Assert.AreEqual(new Color(0.75, 0.5, 0.25), c);
        }

        [TestMethod]
        public void CreatingAStripePattern()
        {
            var pattern = new StripePattern(Color.WHITE, Color.BLACK);
            Assert.AreEqual(Color.WHITE, pattern.a);
            Assert.AreEqual(Color.BLACK, pattern.b);
        }

        [TestMethod]
        public void AStripePatternIsConstantInY()
        {
            var shape = new Sphere();
            var pattern = new StripePattern(Color.WHITE, Color.BLACK);
            Assert.AreEqual(Color.WHITE, pattern.PatternAtObject(shape, new Point(0, 0, 0)));
            Assert.AreEqual(Color.WHITE, pattern.PatternAtObject(shape, new Point(0, 1, 0)));
            Assert.AreEqual(Color.WHITE, pattern.PatternAtObject(shape, new Point(0, 2, 0)));
        }

        [TestMethod]
        public void AStripePatternIsConstantInZ()
        {
            var shape = new Sphere();
            var pattern = new StripePattern(Color.WHITE, Color.BLACK);
            Assert.AreEqual(Color.WHITE, pattern.PatternAtObject(shape, new Point(0, 0, 0)));
            Assert.AreEqual(Color.WHITE, pattern.PatternAtObject(shape, new Point(0, 0, 1)));
            Assert.AreEqual(Color.WHITE, pattern.PatternAtObject(shape, new Point(0, 0, 2)));
        }

        [TestMethod]
        public void AStripePatternIsAlternatesInX()
        {
            var shape = new Sphere();
            var pattern = new StripePattern(Color.WHITE, Color.BLACK);
            Assert.AreEqual(Color.WHITE, pattern.PatternAtObject(shape, new Point(0, 0, 0)));
            Assert.AreEqual(Color.WHITE, pattern.PatternAtObject(shape, new Point(0.9, 0, 0)));
            Assert.AreEqual(Color.BLACK, pattern.PatternAtObject(shape, new Point(1, 0, 0)));
            Assert.AreEqual(Color.BLACK, pattern.PatternAtObject(shape, new Point(-0.1, 0, 0)));
            Assert.AreEqual(Color.BLACK, pattern.PatternAtObject(shape, new Point(-1, 0, 0)));
            Assert.AreEqual(Color.WHITE, pattern.PatternAtObject(shape, new Point(-1.1, 0, 0)));
        }

        [TestMethod]
        public void StripesWithAnObjectTransformation()
        {
            var shape = new Sphere
            {
                Transform = Transform.Scaling(2, 2, 2)
            };
            var pattern = new StripePattern(Color.WHITE, Color.BLACK);
            var c = pattern.PatternAtObject(shape, new Point(1.5, 0, 0));
            Assert.AreEqual(Color.WHITE, c);
        }

        [TestMethod]
        public void StripesWithAPatternTransformation()
        {
            var shape = new Sphere();
            var pattern = new StripePattern(Color.WHITE, Color.BLACK)
            {
                Transform = Transform.Scaling(2, 2, 2)
            };
            var c = pattern.PatternAtObject(shape, new Point(1.5, 0, 0));
            Assert.AreEqual(Color.WHITE, c);
        }

        [TestMethod]
        public void StripesWithBothObjectAndPatternTransformation()
        {
            var shape = new Sphere
            {
                Transform = Transform.Scaling(2, 2, 2)
            };

            var pattern = new StripePattern(Color.WHITE, Color.BLACK)
            {
                Transform = Transform.Scaling(2, 2, 2)
            };

            var c = pattern.PatternAtObject(shape, new Point(1.5, 0, 0));
            Assert.AreEqual(Color.WHITE, c);
        }

        [TestMethod]
        public void AGradientLinearlyInterpolatesBetweenTwoColors()
        {
            var pattern = new GradientPattern(Color.WHITE, Color.BLACK);
            Assert.AreEqual(Color.WHITE, pattern.PatternAt(new Point(0, 0, 0)));
            Assert.AreEqual(new Color(0.75,0.75,0.75), pattern.PatternAt(new Point(0.25, 0, 0)));
            Assert.AreEqual(new Color(0.5, 0.5, 0.5), pattern.PatternAt(new Point(0.5, 0, 0)));
            Assert.AreEqual(new Color(0.25, 0.25, 0.25), pattern.PatternAt(new Point(0.75, 0, 0)));
        }

        [TestMethod]
        public void ARingShouldExtendInBothXAndZ()
        {
            var pattern = new RingPattern(Color.WHITE, Color.BLACK);
            Assert.AreEqual(Color.WHITE, pattern.PatternAt(new Point(0, 0, 0)));
            Assert.AreEqual(Color.BLACK, pattern.PatternAt(new Point(1, 0, 0)));
            Assert.AreEqual(Color.BLACK, pattern.PatternAt(new Point(0, 0, 1)));
            Assert.AreEqual(Color.BLACK, pattern.PatternAt(new Point(0.708, 0, 0.708)));
        }

        [TestMethod]
        public void CheckersShouldExtendInX()
        {
            var pattern = new CheckersPattern(Color.WHITE, Color.BLACK);
            Assert.AreEqual(Color.WHITE, pattern.PatternAt(new Point(0, 0, 0)));
            Assert.AreEqual(Color.WHITE, pattern.PatternAt(new Point(0.99, 0, 0)));
            Assert.AreEqual(Color.BLACK, pattern.PatternAt(new Point(1.01, 0, 0)));
        }

        [TestMethod]
        public void CheckersShouldExtendInY()
        {
            var pattern = new CheckersPattern(Color.WHITE, Color.BLACK);
            Assert.AreEqual(Color.WHITE, pattern.PatternAt(new Point(0, 0, 0)));
            Assert.AreEqual(Color.WHITE, pattern.PatternAt(new Point(0, 0.99, 0)));
            Assert.AreEqual(Color.BLACK, pattern.PatternAt(new Point(0, 1.01, 0)));
        }

        [TestMethod]
        public void CheckersShouldExtendInZ()
        {
            var pattern = new CheckersPattern(Color.WHITE, Color.BLACK);
            Assert.AreEqual(Color.WHITE, pattern.PatternAt(new Point(0, 0, 0)));
            Assert.AreEqual(Color.WHITE, pattern.PatternAt(new Point(0, 0, 0.99)));
            Assert.AreEqual(Color.BLACK, pattern.PatternAt(new Point(0, 0, 1.01)));
        }
    }
}
