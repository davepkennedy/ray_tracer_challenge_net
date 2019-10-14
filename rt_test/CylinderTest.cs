using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using rt;
using static rt.Constants;

namespace rt_test
{
    [TestClass]
    public class CylinderTest
    {
        [TestMethod]
        [DataRow(1, 0, 0, 0, 1, 0)]
        [DataRow(0, 0, 0, 0, 1, 0)]
        [DataRow(0, 0, -5, 1, 1, 1)]
        public void ARayMissesACylinder(
            double px, double py, double pz,
            double dx, double dy, double dz)
        {
            var cyl = new Cylinder();
            var direction = new Vector(dx, dy, dz).Normalize();
            var r = new Ray(new Point(px, py, pz), direction);
            var xs = cyl.Intersects(r);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        [DataRow(1,0,-5,0,0,1,5,5)]
        [DataRow(0, 0, -5, 0, 0, 1, 4, 6)]
        [DataRow(0.5, 0, -5, 0.1, 1, 1, 6.80798, 7.08872)]
        public void ARayStrikesACylinder(
            double px, double py, double pz,
            double dx, double dy, double dz,
            double t0, double t1)
        {
            var cyl = new Cylinder();
            var direction = new Vector(dx, dy, dz).Normalize();
            var r = new Ray(new Point(px, py, pz), direction);
            var xs = cyl.Intersects(r);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(t0, xs[0].T, EPSILON);
            Assert.AreEqual(t1, xs[1].T, EPSILON);
        }

        [TestMethod]
        [DataRow(1,0,0,1,0,0)]
        [DataRow(0,5,-1,0,0,-1)]
        [DataRow(0,-2,1,0,0,1)]
        [DataRow(-1,1,0,-1,0,0)]
        public void NormalVectorOnACylinder(
            double px, double py, double pz,
            double nx, double ny, double nz)
        {
            var cyl = new Cylinder();
            Assert.AreEqual(new Vector(nx, ny, nz), cyl.NormalAt(new Point(px, py, pz), null));
        }

        [TestMethod]
        public void TheDefaultMinimumAndMaximumForACylinder()
        {
            var c = new Cylinder();
            Assert.AreEqual(Double.PositiveInfinity, c.Maximum);
            Assert.AreEqual(Double.NegativeInfinity, c.Minimum);
        }

        [TestMethod]
        [DataRow(0, 1.5, 0, 0.1, 1, 0, 0)]
        [DataRow(0, 3, -5, 0, 0, 1, 0)]
        [DataRow(0, 0, -5, 0, 0, 1, 0)]
        [DataRow(0, 2, -5, 0, 0, 1, 0)]
        [DataRow(0, 1, -5, 0, 0, 1, 0)]
        [DataRow(0, 1.5, -2, 0, 0, 1, 2)]
        public void IntersectingAConstrainedCylinder(
            double px, double py, double pz,
            double dx, double dy, double dz,
            int count)
        {
            var cyl = new Cylinder
            {
                Minimum = 1,
                Maximum = 2
            };
            var direction = new Vector(dx, dy, dz).Normalize();
            var r = new Ray(new Point(px, py, pz), direction);
            var xs = cyl.Intersects(r);
            Assert.AreEqual(count, xs.Count);
        }

        [TestMethod]
        public void TheDefaultClosedValueForACylinder()
        {
            var cyl = new Cylinder();
            Assert.IsTrue(!cyl.Closed);
        }

        [TestMethod]
        [DataRow(0, 3, 0,       0, -1, 0,   2)]
        [DataRow(0, 3, -2,      0, -1, 2,   2)]
        [DataRow(0, 4, -2,      0, -1, 1,   2)]
        [DataRow(0, 0, -2,      0, 1, 2,    2)]
        [DataRow(0, -1, -2,     0, 1, 1,    2)]
        public void IntersectingTheCapsOfAClosedCylinder(
            double px, double py, double pz,
            double dx, double dy, double dz,
            int count)
        {
            var cyl = new Cylinder
            {
                Minimum = 1,
                Maximum = 2,
                Closed = true
            };
            var direction = new Vector(dx, dy, dz).Normalize();
            var r = new Ray(new Point(px, py, pz), direction);
            var xs = cyl.Intersects(r);
            Assert.AreEqual(count, xs.Count);
        }

        [TestMethod]
        [DataRow(0,   1, 0,     0, -1,0)]
        [DataRow(0.5, 1, 0,     0, -1, 0)]
        [DataRow(0,   1, 0.5,   0, -1, 0)]
        [DataRow(0,   2, 0,     0, 1, 0)]
        [DataRow(0.5, 2, 0,     0, 1, 0)]
        [DataRow(0,   2, 0.5,   0, 1, 0)]

        public void TheNormalVectorOnACylindersEndCaps(
            double px, double py, double pz,
            double vx, double vy, double vz)
        {
            var cyl = new Cylinder
            {
                Minimum = 1,
                Maximum = 2,
                Closed = true
            };
            var n = cyl.NormalAt(new Point(px, py, pz), null);
            Assert.AreEqual(new Vector(vx, vy, vz), n);
        }

        [TestMethod]
        public void AnUnboundedCylinderHasABoundingBox()
        {
            var shape = new Cylinder();
            var box = shape.Bounds;
            Assert.AreEqual(new Point(-1,Double.NegativeInfinity, -1), box.Min);
            Assert.AreEqual(new Point(1,Double.PositiveInfinity, 1), box.Max);
        }

        [TestMethod]
        public void ABoundedCylinderHasABoundingBox()
        {
            var shape = new Cylinder
            {
                Minimum = -5,
                Maximum = 3
            };
            var box = shape.Bounds;
            Assert.AreEqual(new Point(-1, -5, -1), box.Min);
            Assert.AreEqual(new Point(1, 3, 1), box.Max);
        }
    }
}
