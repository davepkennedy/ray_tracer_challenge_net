using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using rt;
using static rt.Constants;

namespace rt_test
{

    [TestClass]
    public class SmoothTrianglesTest
    {
        private Point p1;
        private Point p2;
        private Point p3;

        private Vector n1;
        private Vector n2;
        private Vector n3;

        private SmoothTriangle tri;

        [TestInitialize]
        public void setup()
        {
            p1 = new Point(0, 1, 0);
            p2 = new Point(-1, 0, 0);
            p3 = new Point(1, 0, 0);

            n1 = new Vector(0, 1, 0);
            n2 = new Vector(-1, 0, 0);
            n3 = new Vector(1, 0, 0);

            tri = new SmoothTriangle(p1, p2, p3, n1, n2, n3);
        }

        [TestMethod]
        public void ConstructingASmoothTriangle()
        {
            Assert.AreEqual(p1, tri.P1);
            Assert.AreEqual(p2, tri.P2);
            Assert.AreEqual(p3, tri.P3);

            Assert.AreEqual(n1, tri.N1);
            Assert.AreEqual(n2, tri.N2);
            Assert.AreEqual(n3, tri.N3);
        }

        [TestMethod]
        public void AnIntersectionWithASmoothTriangle()
        {
            var r = new Ray(new Point(-0.2, 0.3, -2), new Vector(0, 0, 1));
            var xs = tri.Intersects(r);

            Assert.AreEqual(0.45, xs[0].U.Value, EPSILON);
            Assert.AreEqual(0.25, xs[0].V.Value, EPSILON);
        }

        [TestMethod]
        public void ASmoothTriangleUsesUVToInterpolateTheNormal()
        {
            var i = new Intersection(1, tri, 0.45, 0.25);
            var n = tri.NormalAt(new Point(0, 0, 0), i);
            Assert.AreEqual(new Vector(-0.5547, 0.83205, 0), n);
        }

        [TestMethod]
        public void PreparingTheNormalOnASmoothTriangle()
        {
            var i = new Intersection(1, tri, 0.45, 0.25);
            var r = new Ray(new Point(-0.2, 0.3, -2), new Vector(0, 0, 1));
            var xs = new Intersections(i);
            var comps = i.PrepareComputations(r, xs);
            Assert.AreEqual(new Vector(-0.5547, 0.83205, 0), comps.NormalV);
        }
    }
}
