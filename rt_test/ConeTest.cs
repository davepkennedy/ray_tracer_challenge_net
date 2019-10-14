using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using rt;
using static rt.Constants;

namespace rt_test
{
    [TestClass]
    public class ConeTest
    {
        [TestMethod]
        [DataRow(0, 0, -5,  0, 0, 1,    5,5)]
        [DataRow(0, 0, -5,  1, 1, 1,    8.66025, 8.66025)]
        [DataRow(1, 1, -5,  -0.5,-1,    1, 4.55006, 49.44994)]
        public void IntersectingAConeWithARay(
            double px, double py, double pz,
            double dx, double dy, double dz,
            double t0, double t1)
        {
            var shape = new Cone();
            var direction = new Vector(dx, dy, dz).Normalize();
            var r = new Ray(new Point(px, py, pz), direction);
            var xs = shape.Intersects(r);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(t0, xs[0].T, EPSILON);
            Assert.AreEqual(t1, xs[1].T, EPSILON);
        }

        [TestMethod]
        public void IntersectingAConeWithARayParallelToOneOfItsHalves()
        {
            var shape = new Cone();
            var direction = new Vector(0, 1, 1).Normalize();
            var r = new Ray(new Point(0, 0, -1), direction);
            var xs = shape.Intersects(r);
            Assert.AreEqual(1, xs.Count);
            Assert.AreEqual(0.35355, xs[0].T, EPSILON);
        }

        [TestMethod]
        [DataRow(0, 0, -5,      0, 1, 0,    0)]
        [DataRow(0, 0, -0.25,      0, 1, 1,    2)]
        [DataRow(0, 0, -0.25,      0, 1, 0,    4)]
        public void IntersectingAConesEndCaps(
            double px, double py, double pz,
            double dx, double dy, double dz,
            int count)
        {
            var shape = new Cone
            {
                Minimum = -0.5,
                Maximum = 0.5,
                Closed = true
            };
            var direction = new Vector(dx, dy, dz).Normalize();
            var r = new Ray(new Point(px, py, pz), direction);
            var xs = shape.Intersects(r);
            Assert.AreEqual(count, xs.Count);
        }

        [TestMethod]
        [DataRow(0, 0, 0,       0, 0, 0)]
        [DataRow(1, 1, 1,       1, -1.41421356, 1)]
        [DataRow(-1, -1, 0,     -1, 1, 0)]
        public void ComputingTheNormalVectorOnACone(
            double px, double py, double pz,
            double nx, double ny, double nz)
        {
            var shape = new TestCone();
            var n = shape.TestNormalAt(new Point(px, py, pz));
            Assert.AreEqual(new Vector(nx, ny, nz), n);
        }

        [TestMethod]
        public void AnUnboundedHasHasABOundingBox()
        {
            var shape = new Cone();
            var box = shape.Bounds;
            Assert.AreEqual(Point.MIN, box.Min);
            Assert.AreEqual(Point.MAX, box.Max);
        }

        [TestMethod]
        public void ABoundedHasHasABOundingBox()
        {
            var shape = new Cone
            {
                Minimum = -5,
                Maximum = 3
            };
            var box = shape.Bounds;
            Assert.AreEqual(new Point(-5,-5,-5), box.Min);
            Assert.AreEqual(new Point(5, 3, 5), box.Max);
        }

    }

    class TestCone : Cone
    {
        public rt.Tuple TestNormalAt(rt.Tuple pt)
        {
            return LocalNormalAt(pt,null);
        }
    }
}
