using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using rt;

namespace rt_test
{
    [TestClass]
    public class CSGTest
    {
        [TestMethod]
        public void CSGIsCreatedWithAnOperationAndTwoShapes()
        {
            var s1 = new Sphere();
            var s2 = new Cube();
            var c = new CSG(Operation.UNION, s1, s2);
            Assert.AreEqual(Operation.UNION, c.Operation);
            Assert.AreEqual(s1, c.Left);
            Assert.AreEqual(s2, c.Right);
            Assert.AreEqual(c, s1.Parent);
            Assert.AreEqual(c, s2.Parent);
        }

        [TestMethod]
        [DataRow("union", true, true, true, false)]
        [DataRow("union", true, true, false, true)]
        [DataRow("union", true, false, true, false)]
        [DataRow("union", true, false, false, true)]
        [DataRow("union", false, true, true, false)]
        [DataRow("union", false, true, false, false)]
        [DataRow("union", false, false, true, true)]
        [DataRow("union", false, false, false, true)]

        [DataRow("intersection", true, true, true, true)]
        [DataRow("intersection", true, true, false, false)]
        [DataRow("intersection", true, false, true, true)]
        [DataRow("intersection", true, false, false, false)]
        [DataRow("intersection", true, true, true, true)]
        [DataRow("intersection", false, true, false, true)]
        [DataRow("intersection", false, false, true, false)]
        [DataRow("intersection", false, false, false, false)]

        [DataRow("difference", true, true, true, false)]
        [DataRow("difference", true, true, false, true)]
        [DataRow("difference", true, false, true, false)]
        [DataRow("difference", true, false, false, true)]
        [DataRow("difference", false, true, true, true)]
        [DataRow("difference", false, true, false, true)]
        [DataRow("difference", false, false, true, false)]
        [DataRow("difference", false, false, false, false)]
        public void EvaluatingTheRuleForACAGOperation(
            string op,
            bool lhit, bool inl, bool inr,
            bool result)
        {
            Assert.AreEqual(result, Operation.OfKind(op).IntersectionAllowed(lhit, inl, inr));
        }

        [TestMethod]
        [DataRow("union", 0, 3)]
        [DataRow("intersection", 1, 2)]
        [DataRow("difference", 0, 1)]
        public void FilteringAListOfIntersections(string operation, int x0, int x1)
        {
            var s1 = new Sphere();
            var s2 = new Cube();

            var c = new CSG(Operation.OfKind(operation), s1, s1);
            var xs = new Intersections(
                new Intersection(1, s1),
                new Intersection(2, s2),
                new Intersection(3, s1),
                new Intersection(4, s2));
            var result = c.FilterIntersections(xs);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(xs[x0], result[0]);
            Assert.AreEqual(xs[x1], result[1]);
        }

        [TestMethod]
        public void ARayMissesACSGObject()
        {
            var c = new CSG(Operation.UNION, new Sphere(), new Cube());
            var r = new Ray(new Point(0, 2, -5), new Vector(0, 0, 1));
            var xs = c.Intersects(r);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void ARayHitsACSGObject()
        {
            var s1 = new Sphere();
            var s2 = new Sphere
            {
                Transform = Transform.Translation(0, 0, 0.5)
            };

            var c = new CSG(Operation.UNION, s1, s2);
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = c.Intersects(r);

            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(4, xs[0].T);
            Assert.AreEqual(s1, xs[0].Shape);
            Assert.AreEqual(6.5, xs[1].T);
            Assert.AreEqual(s2, xs[1].Shape);
        }

        [TestMethod]
        public void ACSGShapeHasABoundingBoxThatContainsItsChildren()
        {
            var left = new Sphere();
            var right = new Sphere
            {
                Transform = Transform.Translation(2, 3, 4)
            };
            var shape = new CSG(Operation.DIFFERENCE, left, right);
            var box = shape.Bounds;
            Assert.AreEqual(new Point(-1, -1, -1), box.Min);
            Assert.AreEqual(new Point(3, 4, 5), box.Max);
        }

        [TestMethod]
        public void IntersectingRayAndCsgDoesntTestChildrenIfBoxIsMissed()
        {
            var left = new TestShape();
            var right = new TestShape();
            var shape = new CSG(Operation.DIFFERENCE, left, right);
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 1, 0));
            var xs = shape.Intersects(r);
            Assert.IsNull(left.SavedRay);
            Assert.IsNull(right.SavedRay);
        }

        [TestMethod]
        public void IntersectingRayAndCsgTestsChildrenIfBoxIsHit()
        {
            var left = new TestShape();
            var right = new TestShape();
            var shape = new CSG(Operation.DIFFERENCE, left, right);
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = shape.Intersects(r);
            Assert.IsNotNull(left.SavedRay);
            Assert.IsNotNull(right.SavedRay);
        }

        [TestMethod]
        public void SubdividingACSGShapeSubdividesItsChildren()
        {
            var s1 = new Sphere
            {
                Transform = Transform.Translation(-1.5, 0, 0)
            };
            var s2 = new Sphere
            {
                Transform = Transform.Translation(1.5, 0, 0)
            };

            var left = Group.Of(s1, s2);

            var s3 = new Sphere
            {
                Transform = Transform.Translation(0, 0, -1.5)
            };

            var s4 = new Sphere
            {
                Transform = Transform.Translation(0, 0, 1.5)
            };

            var right = Group.Of(s3, s4);

            var shape = new CSG(Operation.DIFFERENCE, left, right);
            shape.Divide(1);
            Assert.AreEqual(Group.Of(s1), left[0]);
            Assert.AreEqual(Group.Of(s2), left[1]);
            Assert.AreEqual(Group.Of(s3), right[0]);
            Assert.AreEqual(Group.Of(s4), right[1]);
        }
    }
}
