using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using rt;
using static rt.Constants;

namespace rt_test
{
    [TestClass]
    public class GroupTest
    {
        [TestMethod]
        public void CreatingANewGroup()
        {
            var g = new Group();
            Assert.AreEqual(Matrix.Identity(4), g.Transform);
            Assert.AreEqual(0, g.Count);
        }

        [TestMethod]
        public void AddingAChildToAGroup()
        {
            var g = new Group();
            var s = new TestShape();

            g.Add(s);

            Assert.AreNotEqual(0, g.Count);
            Assert.IsTrue(g.Contains(s));
            Assert.AreEqual(g, s.Parent);
        }

        [TestMethod]
        public void IntersectingARayWithAnEmptyGroup()
        {
            var g = new Group();
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var xs = g.Intersects(r);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void IntesectingARayWithANonEmptyGroup()
        {
            var g = new Group();
            var s1 = new Sphere();
            var s2 = new Sphere
            {
                Transform = Transform.Translation(0, 0, -3)
            };
            var s3 = new Sphere
            {
                Transform = Transform.Translation(5, 0, 0)
            };
            g.Add(s1);
            g.Add(s2);
            g.Add(s3);

            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));

            var xs = g.Intersects(r);
            Assert.AreEqual(4, xs.Count);
            Assert.AreEqual(s2, xs[0].Shape);
            Assert.AreEqual(s2, xs[1].Shape);
            Assert.AreEqual(s1, xs[2].Shape);
            Assert.AreEqual(s1, xs[3].Shape);
        }

        [TestMethod]
        public void IntersectingATransformedGroup()
        {
            var g = new Group
            {
                Transform = Transform.Scaling(2, 2, 2)
            };
            var s = new Sphere
            {
                Transform = Transform.Translation(5, 0, 0)
            };
            g.Add(s);
            var r = new Ray(new Point(10, 0, -10), new Vector(0, 0, 1));
            var xs = g.Intersects(r);
            Assert.AreEqual(2, xs.Count);
        }

        [TestMethod]
        public void AGroupHasABoundingBoxThatContainsItsChildren()
        {
            var s = new Sphere
            {
                Transform = Transform.Translation(2, 5, -3) * Transform.Scaling(2, 2, 2)
            };

            var c = new Cylinder
            {
                Minimum = -2,
                Maximum = 2,
                Transform = Transform.Translation(-4, -1, 4) * Transform.Scaling(0.5, 1, 0.5)
            };

            var shape = new Group();
            shape.Add(s);
            shape.Add(c);

            var box = shape.Bounds;
            Assert.AreEqual(new Point(-4.5, -3, -5), box.Min);
            Assert.AreEqual(new Point(4, 7, 4.5), box.Max);
        }

        [TestMethod]
        public void IntersectingRayAndGroupDoesntTestChildrenIfBoxIsMissed() {
            var child = new TestShape();
            var shape = new Group();
            shape.Add(child);
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 1, 0));
            var xs = shape.Intersects(r);
            Assert.IsNull(child.SavedRay);
        }

        [TestMethod]
        public void IntersectingRayGroupTestsChildrenIfBoxIsHit()
        {
            var child = new TestShape();
            var shape = new Group();
            shape.Add(child);
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = shape.Intersects(r);
            Assert.IsNotNull(child.SavedRay);
        }

        [TestMethod]
        public void PartitioningAGroupsChildren()
        {
            var s1 = new Sphere
            {
                Transform = Transform.Translation(-2, 0, 0)
            };

            var s2 = new Sphere
            {
                Transform = Transform.Translation(2, 0, 0)
            };

            var s3 = new Sphere();
            var g = Group.Of(s1, s2, s3);
            (var left, var right) = g.PartitionChildren();
            Assert.AreEqual(Group.Of(s3), g);
            CollectionAssert.AreEqual(new List<Shape> { s1 }, left);
            CollectionAssert.AreEqual(new List<Shape> { s2 }, right);
        }

        [TestMethod]
        public void CreatingASubGroupFromAListOfChildren()
        {
            var s1 = new Sphere();
            var s2 = new Sphere();
            var g = new Group();
            g.MakeSubgroup(s1, s2);
            Assert.AreEqual(1, g.Count);
            Assert.AreEqual(Group.Of(s1, s2), g[0]);
        }

        [TestMethod]
        public void SubdividingAGroupPartitionsItsChildren()
        {
            var s1 = new Sphere
            {
                Transform = Transform.Translation(-2, -2, 0)
            };

            var s2 = new Sphere
            {
                Transform = Transform.Translation(-2, 2, 0)
            };

            var s3 = new Sphere
            {
                Transform = Transform.Scaling(4, 4, 4)
            };

            var g = Group.Of(s1, s2, s3);
            g.Divide(1);
            Assert.AreEqual(s3, g[0]);

            var subgroup = g[1] as Group;
            Assert.IsNotNull(subgroup);
            Assert.AreEqual(2, subgroup.Count);
            Assert.AreEqual(Group.Of(s1), subgroup[0]);
            Assert.AreEqual(Group.Of(s2), subgroup[1]);
        }

        [TestMethod]
        public void SubdividingAGroupWithTooFewChildren()
        {
            var s1 = new Sphere
            {
                Transform = Transform.Translation(-2, 0, 0)
            };

            var s2 = new Sphere
            {
                Transform = Transform.Translation(2, 1, 0)
            };

            var s3 = new Sphere
            {
                Transform = Transform.Translation(2, -1, 0)
            };

            var subgroup = Group.Of(s1, s2, s3);

            var s4 = new Sphere();

            var g = Group.Of(subgroup, s4);
            g.Divide(3);

            Assert.AreEqual(subgroup, g[0]);
            Assert.AreEqual(s4, g[1]);
            Assert.AreEqual(2, subgroup.Count);
            Assert.AreEqual(Group.Of(s1), subgroup[0]);
            Assert.AreEqual(Group.Of(s2, s3), subgroup[1]);
        }
    }
}
