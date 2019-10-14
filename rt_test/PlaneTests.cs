using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

using rt;

namespace rt_test
{
    [TestClass]
    public class PlaneTests
    {
        [TestMethod]
        public void TheNormalOfAPlaneIsConstantEverywhere()
        {
            var plane = new Plane();
            var n1 = plane.NormalAt(new Point(0, 0, 0), null);
            var n2 = plane.NormalAt(new Point(10, 0, -10), null);
            var n3 = plane.NormalAt(new Point(-5, 0, 150), null);
            Assert.AreEqual(new Vector(0, 1, 0), n1);
            Assert.AreEqual(new Vector(0, 1, 0), n2);
            Assert.AreEqual(new Vector(0, 1, 0), n3);
        }

        [TestMethod]
        public void IntersectARayParallelToThePlane()
        {
            var plane = new Plane();
            var ray = new Ray(new Point(0, 10, 0), new Vector(0, 0, 1));
            var xs = plane.Intersects(ray);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void IntersectsWithACoplanarRay()
        {
            var plane = new Plane();
            var ray = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var xs = plane.Intersects(ray);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void ARayIntersectingAPlaneFromAbove()
        {
            var plane = new Plane();
            var ray = new Ray(new Point(0, 1, 0), new Vector(0, -1, 0));
            var xs = plane.Intersects(ray);
            Assert.AreEqual(1, xs.Count);
            Assert.AreEqual(1, xs[0].T);
            Assert.AreEqual(plane, xs[0].Shape);

            var comps = xs.Hit().PrepareComputations(ray, xs);
            Debug.WriteLine($"Over {comps.OverPoint} Under {comps.UnderPoint}");
        }

        [TestMethod]
        public void ARayIntersectingAPlaneFromBelow()
        {
            var plane = new Plane();
            var ray = new Ray(new Point(0, -1, 0), new Vector(0, 1, 0));
            var xs = plane.Intersects(ray);
            Assert.AreEqual(1, xs.Count);
            Assert.AreEqual(1, xs[0].T);
            Assert.AreEqual(plane, xs[0].Shape);

            var comps = xs.Hit().PrepareComputations(ray, xs);
            Debug.WriteLine($"Over {comps.OverPoint} Under {comps.UnderPoint}");
        }

        [TestMethod]
        public void APlaneHasABoundingBox()
        {
            var shape = new Plane();
            var box = shape.Bounds;
            Assert.AreEqual(new Point(Double.NegativeInfinity, 0, Double.NegativeInfinity), box.Min);
            Assert.AreEqual(new Point(Double.PositiveInfinity, 0, Double.PositiveInfinity), box.Max);
        }
    }
}
