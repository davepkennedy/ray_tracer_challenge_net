using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt;

namespace rt_test
{
    [TestClass]
    public class SphereTests
    {

        [TestMethod]
        public void IntersectingAScaledSphereWithARay()
        {
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var s = new Sphere();
            s.Transform = Transform.Scaling (2, 2, 2);

            var xs = s.Intersects(r);

            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(3.0f, xs[0].T);
            Assert.AreEqual(7.0f, xs[1].T);
        }

        [TestMethod]
        public void IntersectingATranslatedSphereWithARay()
        {
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var s = new Sphere();
            s.Transform = Transform.Translation (5, 0, 0);

            var xs = s.Intersects(r);

            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void TheNormalOnASphereAtAPointOnTheXAxis()
        {
            var s = new Sphere();
            var n = s.NormalAt(new Point(1, 0, 0), null);
            Assert.AreEqual(new Vector(1, 0, 0), n);
        }

        [TestMethod]
        public void TheNormalOnASphereAtAPointOnTheYAxis()
        {
            var s = new Sphere();
            var n = s.NormalAt(new Point(0, 1, 0), null);
            Assert.AreEqual(new Vector(0, 1, 0), n);
        }

        [TestMethod]
        public void TheNormalOnASphereAtAPointOnTheZAxis()
        {
            var s = new Sphere();
            var n = s.NormalAt(new Point(0, 0, 1), null);
            Assert.AreEqual(new Vector(0, 0, 1), n);
        }

        [TestMethod]
        public void TheNormalOnASphereAtANonAxisPoint()
        {
            double rtot = Math.Sqrt(3.0) / 3.0f;
            var s = new Sphere();
            var n = s.NormalAt(new Point(rtot, rtot, rtot), null);
            Assert.AreEqual(new Vector(rtot, rtot, rtot), n);
        }

        [TestMethod]
        public void TheNormalIsANormalizedVector()
        {
            double rtot = Math.Sqrt(3.0) / 3.0f;
            var s = new Sphere();
            var n = s.NormalAt(new Point(rtot, rtot, rtot), null);
            Assert.AreEqual(n, n.Normalize());
        }

        [TestMethod]
        public void ComputingTheNormalOnATranslatedSphere()
        {
            var s = new Sphere();
            s.Transform = Transform.Translation(0, 1, 0);
            var n = s.NormalAt(new Point(0, 1.70711f, -0.70711f), null);
            Assert.AreEqual(new Vector(0, 0.70711f, -0.70711f), n);
        }

        [TestMethod]
        public void ComputingTheNormalOnATransformedSphere()
        {
            double rtot = Math.Sqrt(2) / 2.0f;
            var s = new Sphere();
            var m = Transform.Scaling (1, 0.5f, 1) * Transform.RotationZ((Math.PI / 5.0));
            s.Transform = m;
            var n = s.NormalAt(new Point(0, rtot, -rtot), null);
            Assert.AreEqual(new Vector(0, 0.97014f, -0.24254f), n);
        }

        [TestMethod]
        public void HelperForCreatingASphereWithAGlassyMaterial()
        {
            var s = Sphere.Glass();
            Assert.AreEqual(Matrix.Identity(4), s.Transform);
            Assert.AreEqual(1, s.Material.Transparency);
            Assert.AreEqual(1.5, s.Material.RefractiveIndex);

        }

        [TestMethod]
        public void ASphereHasABoundingBox()
        {
            var shape = new Sphere();
            var box = shape.Bounds;
            Assert.AreEqual(new Point(-1, -1, -1), box.Min);
            Assert.AreEqual(new Point(1, 1, 1), box.Max);
        }
    }
}
