using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using rt;

namespace rt_test
{
    [TestClass]
    public class TriangleTest
    {
        [TestMethod]
        public void ConstructingATriangle()
        {
            var p1 = new Point(0, 1, 0);
            var p2 = new Point(-1, 0, 0);
            var p3 = new Point(1, 0, 0);

            var t = new Triangle(p1, p2, p3);

            Assert.AreEqual(p1, t.P1);
            Assert.AreEqual(p2, t.P2);
            Assert.AreEqual(p3, t.P3);

            Assert.AreEqual(new Vector(-1, -1, 0), t.E1);
            Assert.AreEqual(new Vector(1, -1, 0), t.E2);

            Assert.AreEqual(new Vector(0, 0, -1), t.Normal);
        }

        [TestMethod]
        public void NormalOnTriangle()
        {
            var t = new Triangle(
                new Point(0, 1, 0),
                new Point(-1, 0, 0),
                new Point(1, 0, 0)
                );

            var n1 = t.NormalAt(new Point(0, 0.5, 0), null);
            var n2 = t.NormalAt(new Point(-0.5, 0.75, 0), null);
            var n3 = t.NormalAt(new Point(0.5, 0.25, 0), null);

            Assert.AreEqual(t.Normal, n1);
            Assert.AreEqual(t.Normal, n2);
            Assert.AreEqual(t.Normal, n3);
        }

        [TestMethod]
        public void IntersectingARayParallelToTheTriangle()
        {
            var t = new Triangle(
                new Point(0, 1, 0),
                new Point(-1, 0, 0),
                new Point(1, 0, 0)
                );
            var r = new Ray(new Point(0,-1,-2), new Vector(0,1,0));
            var xs = t.Intersects(r);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void ARayMissesTheP1P3Edge()
        {
            var t = new Triangle(
                new Point(0, 1, 0),
                new Point(-1, 0, 0),
                new Point(1, 0, 0)
                );
            var r = new Ray(new Point(1, 1, -2), new Vector(0, 0, 1));
            var xs = t.Intersects(r);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void ARayMissesTheP1P2Edge()
        {
            var t = new Triangle(
                new Point(0, 1, 0),
                new Point(-1, 0, 0),
                new Point(1, 0, 0)
                );
            var r = new Ray(new Point(-1, 1, -2), new Vector(0, 0, 1));
            var xs = t.Intersects(r);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void ARayMissesTheP2P3Edge()
        {
            var t = new Triangle(
                new Point(0, 1, 0),
                new Point(-1, 0, 0),
                new Point(1, 0, 0)
                );
            var r = new Ray(new Point(0, -1, -2), new Vector(0, 0, 1));
            var xs = t.Intersects(r);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void ARayStrikesATriangle()
        {
            var t = new Triangle(
                new Point(0, 1, 0),
                new Point(-1, 0, 0),
                new Point(1, 0, 0)
                );
            var r = new Ray(new Point(0, 0.5, -2), new Vector(0, 0, 1));
            var xs = t.Intersects(r);
            Assert.AreEqual(1, xs.Count);
            Assert.AreEqual(2, xs[0].T);
        }

        [TestMethod]
        public void ATriangleHasABoundingBox()
        {
            var p1 = new Point(-3, 7, 2);
            var p2 = new Point(6,2,-4);
            var p3 = new Point(2,-1,-1);
            var shape = new Triangle(p1, p2, p3);
            var box = shape.Bounds;
            Assert.AreEqual(new Point(-3, -1, -4), box.Min);
            Assert.AreEqual(new Point(6, 7, 2), box.Max);
        }
    }
}
