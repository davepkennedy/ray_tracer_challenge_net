using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt;

namespace rt_test
{
    [TestClass]
    public class RayTests
    {
        [TestMethod]
        public void CreateAndQueryARay()
        {
            var origin = new Point(1, 2, 3);
            var direction = new Vector(4, 5, 6);
            var ray = new Ray(origin, direction);

            Assert.AreEqual(origin, ray.Origin);
            Assert.AreEqual(direction, ray.Direction);
        }

        [TestMethod]
        public void ComputingAPointFromADistance()
        {
            var origin = new Point(2, 3, 4);
            var direction = new Vector(1, 0, 0);
            var ray = new Ray(origin, direction);

            Assert.AreEqual(new Point(2, 3, 4), ray.Position(0));
            Assert.AreEqual(new Point(3, 3, 4), ray.Position(1));
            Assert.AreEqual(new Point(1, 3, 4), ray.Position(-1));
            Assert.AreEqual(new Point(4.5f, 3, 4), ray.Position(2.5f));
        }

        [TestMethod]
        public void ARayIntersectsASphereAtTwoPoints()
        {
            var origin = new Point(0, 0, -5);
            var direction = new Vector(0, 0, 1);
            var ray = new Ray(origin, direction);
            var sphere = new Sphere();
            var xs = sphere.Intersects(ray);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(4.0f, xs[0].T);
            Assert.AreEqual(6.0f, xs[1].T);
        }

        [TestMethod]
        public void ARayIntersectsASphereAtATangent()
        {
            var ray = new Ray(new Point(0, 1, -5), new Vector(0, 0, 1));
            var sphere = new Sphere();
            var xs = sphere.Intersects(ray);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(5.0f, xs[0].T);
            Assert.AreEqual(5.0f, xs[1].T);
        }

        [TestMethod]
        public void ARayMissesASphere()
        {
            var ray = new Ray(new Point(0, 2, -5), new Vector(0, 0, 1));
            var sphere = new Sphere();
            var xs = sphere.Intersects(ray);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void ARayOriginatesInsideASphere()
        {
            var ray = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var sphere = new Sphere();
            var xs = sphere.Intersects(ray);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(-1.0f, xs[0].T);
            Assert.AreEqual(1.0f, xs[1].T);
        }

        [TestMethod]
        public void ASphereIsBehindARay()
        {
            var ray = new Ray(new Point(0, 0, 5), new Vector(0, 0, 1));
            var sphere = new Sphere();
            var xs = sphere.Intersects(ray);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(-6.0f, xs[0].T);
            Assert.AreEqual(-4.0f, xs[1].T);
        }

        [TestMethod]
        public void IntersectionEncapsulatesTAndObject()
        {
            double t = 0.5f;
            var o = new Sphere();
            var intersection = new Intersection(t, o);

            Assert.AreEqual(t, intersection.T);
        }

        [TestMethod]
        public void TranslatingARay()
        {
            var r = new Ray(new Point(1, 2, 3), new Vector(0, 1, 0));
            var m = Transform.Translation(3, 4, 5);
            var r2 = r.Transform(m);
            Assert.AreEqual(new Point(4, 6, 8), r2.Origin);
            Assert.AreEqual(new Vector(0, 1, 0), r2.Direction);
        }

        [TestMethod]
        public void ScalingARay()
        {
            var r = new Ray(new Point(1, 2, 3), new Vector(0, 1, 0));
            var m = Transform.Scaling(2, 3, 4);
            var r2 = r.Transform(m);
            Assert.AreEqual(new Point(2, 6, 12), r2.Origin);
            Assert.AreEqual(new Vector(0, 3, 0), r2.Direction);
        }
    }

}
