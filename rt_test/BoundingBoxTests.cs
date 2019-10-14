using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using rt;
using static rt.Constants;

namespace rt_test
{
    [TestClass]
    public class BoundingBoxTests
    {
        [TestMethod]
        public void CreatingAnEmptyBoundingBox()
        {
            var box = new BoundingBox();
            Assert.AreEqual(Double.PositiveInfinity, Double.PositiveInfinity);
            Assert.AreEqual(Point.MAX, box.Min);
            Assert.AreEqual(Point.MIN, box.Max);
        }

        [TestMethod]
        public void CreatingABoundingBoxWithVolume()
        {
            var box = new BoundingBox(
                new Point(-1, -2, -3),
                new Point(1, 2, 3));
            Assert.AreEqual(new Point(-1, -2, -3), box.Min);
            Assert.AreEqual(new Point(1, 2, 3), box.Max);
        }

        [TestMethod]
        public void AddingPointsToAnEmptyBoundingBox()
        {
            var box = new BoundingBox();
            box.Add(new Point(-5, 2, 0));
            box.Add(new Point(7, 0, -3));

            Assert.AreEqual(new Point(-5,0,-3), box.Min);
            Assert.AreEqual(new Point(7, 2, 0), box.Max);
        }

        [TestMethod]
        public void AddingOneBoundingBoxToAnother()
        {
            var box1 = new BoundingBox(new Point(-5, -2, 0), new Point(7, 4, 4));
            var box2 = new BoundingBox(new Point(8, -7, -2), new Point(14, 2, 8));

            box1.Add(box2);

            Assert.AreEqual(new Point(-5, -7, -2), box1.Min);
            Assert.AreEqual(new Point(14, 4, 8), box1.Max);
        }

        [TestMethod]
        [DataRow(5, -2, 0, true)]
        [DataRow(11, 4, 7, true)]
        [DataRow(8, 1, 3, true)]
        [DataRow(3, 0, 3, false)]
        [DataRow(8, -4, 3, false)]
        [DataRow(8, 1, -1, false)]
        [DataRow(13, 1, 3, false)]
        [DataRow(8, 5, 3, false)]
        [DataRow(8, 1, 8, false)]
        public void CheckingToSeeIfABoxContainsAGivenPoint (
            double px, double py, double pz, bool expect
            )
        {
            var box = new BoundingBox(new Point(5, -2, 0), new Point(11, 4, 7));
            var p = new Point(px, py, pz);
            Assert.AreEqual(expect, box.Contains(p));
        }

        [TestMethod]
        [DataRow(5, -2, 0, 11, 4, 7, true)]
        [DataRow(6, -1, 1,10, 3, 6, true)]
        [DataRow(4, -3, -1, 10, 3, 6, false)]
        [DataRow(6, -1, 1, 12, 5, 8, false)]
        public void CheckingToSeeIfABoxContainsAGivenBox (
            double minx, double miny, double minz,
            double maxx, double maxy, double maxz,
            bool expect)
        {
            var box = new BoundingBox(new Point(5, -2, 0), new Point(11, 4, 7));
            var box2 = new BoundingBox(new Point(minx, miny, minz), new Point(maxx, maxy, maxz));
            Assert.AreEqual(expect, box.Contains(box2));
        }

        [TestMethod]
        public void TransformingABoundingBox()
        {
            var box = new BoundingBox(new Point(-1, -1, -1), new Point(1, 1, 1));
            var matrix = Transform.RotationX(Math.PI / 4) * Transform.RotationY(Math.PI / 4);
            var box2 = box.Transform(matrix);
            Assert.AreEqual(new Point(-1.4142, -1.7071, -1.7071), box2.Min);
            Assert.AreEqual(new Point(1.4142, 1.7071, 1.7071), box2.Max);
        }

        [TestMethod]
        [DataRow(5, 0.5, 0, -1, 0, 0, true)]
        [DataRow(-5, 0.5, 0, 1, 0, 0, true)]
        [DataRow(0.5, 5, 0,0, -1, 0, true)]
        [DataRow(0.5, -5, 0,0, 1, 0, true)]
        [DataRow(0.5, 0, 5,0, 0, -1, true)]
        [DataRow(0.5, 0, -5, 0, 0, 1, true)]
        [DataRow(0, 0.5, 0,0, 0, 1, true)]
        [DataRow(-2, 0, 0, 2, 4, 6, false)]
        [DataRow(0, -2, 0, 6, 2, 4, false)]
        [DataRow(0, 0, -2, 4, 6, 2, false)]
        [DataRow(2, 0, 2, 0, 0, -1, false)]
        [DataRow(0, 2, 2, 0, -1, 0, false)]
        [DataRow(2, 2, 0, -1, 0, 0, false)]
        public void IntersectingARayWithABoundingBoxAtTheOrigin(
            double px, double py, double pz,
            double dx, double dy, double dz,
            bool result)
        {
            var box = new BoundingBox(new Point(-1, -1, -1), new Point(1, 1, 1));
            var direction = new Vector(dx, dy, dz).Normalize();
            var r = new Ray(new Point(px, py, pz), direction);

            Assert.AreEqual(result, box.Intersects(r));
        }

        [TestMethod]
        [DataRow(15, 1, 2, -1, 0, 0, true)]
        [DataRow(-5, -1, 4, 1, 0, 0, true)]
        [DataRow(7, 6, 5, 0, -1, 0, true)]
        [DataRow(9, -5, 6, 0, 1, 0, true)]
        [DataRow(8, 2, 12, 0, 0, -1, true)]
        [DataRow(6, 0, -5, 0, 0, 1, true)]
        [DataRow(8, 1, 3.5, 0, 0, 1, true)]
        [DataRow(9, -1, -8, 2, 4, 6, false)]
        [DataRow(8, 3, -4, 6, 2, 4, false)]
        [DataRow(9, -1, -2, 4, 6, 2, false)]
        [DataRow(4, 0, 9, 0, 0, -1, false)]
        [DataRow(8, 6, -1, 0, -1, 0, false)]
        [DataRow(12, 5, 4, -1, 0, 0, false)]
        public void IntersectingARayWithANonCubicBoundingBox(
            double px, double py, double pz,
            double dx, double dy, double dz,
            bool result)
        {
            var box = new BoundingBox(new Point(5, -2, 0), new Point(11, 4, 7));
            var direction = new Vector(dx, dy, dz).Normalize();
            var r = new Ray(new Point(px, py, pz), direction);
            Assert.AreEqual(result, box.Intersects(r));
        }

        [TestMethod]
        public void SplittingAPerfectCube() {
            var box = new BoundingBox(new Point(-1, -4, -5), new Point(9, 6, 5));
            (var left, var right) = box.Split();
            Assert.AreEqual(new Point(-1, -4, -5), left.Min);
            Assert.AreEqual(new Point(4, 6, 5), left.Max);
            Assert.AreEqual(new Point(4, -4, -5), right.Min);
            Assert.AreEqual(new Point(9, 6, 5), right.Max);
        }

        [TestMethod]
        public void SplittingAnXWideBox() {
            var box = new BoundingBox(new Point(-1, -2, -3), new Point(9, 5.5, 3));
            (var left, var right) = box.Split();
            Assert.AreEqual(new Point(-1, -2, -3), left.Min);
            Assert.AreEqual(new Point(4, 5.5, 3), left.Max);
            Assert.AreEqual(new Point(4, -2, -3), right.Min);
            Assert.AreEqual(new Point(9, 5.5, 3), right.Max);
        }

        [TestMethod]
        public void SplittingAYWideBox() {
            var box = new BoundingBox(new Point(-1, -2, -3), new Point(5, 8, 3));
            (var left, var right) = box.Split();
            Assert.AreEqual(new Point(-1, -2, -3), left.Min);
            Assert.AreEqual(new Point(5, 3, 3), left.Max);
            Assert.AreEqual(new Point(-1, 3, -3), right.Min);
            Assert.AreEqual(new Point(5, 8, 3), right.Max);
        }

        [TestMethod]
        public void SplittingAZWideBox()
        {
            var box = new BoundingBox(new Point(-1, -2, -3), new Point(5, 3, 7));
            (var left, var right) = box.Split();
            Assert.AreEqual(new Point(-1, -2, -3), left.Min);
            Assert.AreEqual(new Point(5, 3, 2), left.Max);
            Assert.AreEqual(new Point(-1, -2, 2), right.Min);
            Assert.AreEqual(new Point(5, 3, 7), right.Max);
        }
    }
}
