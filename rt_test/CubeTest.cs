using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using rt;

namespace rt_test
{
    [TestClass]
    public class CubeTest
    {
        [TestMethod]
        [DataRow(5, 0.5, 0,   -1, 0, 0,     4, 6)]
        [DataRow(-5, 0.5, 0,    1, 0, 0,    4, 6)]
         
        [DataRow(0.5, 5, 0,     0, -1, 0,   4, 6)]
        [DataRow(0.5, -5, 0,    0, 1, 0,    4, 6)]

        [DataRow(0.5, 0, 5,     0, 0, -1,   4, 6)]
        [DataRow(0.5, 0, -5,    0, 0, 1,    4, 6)]

        [DataRow(0, 0.5, 0,     0, 0, 1,    -1, 1)]
        public void ARayIntersectsACube(
            double px, double py, double pz, 
            double dx, double dy, double dz, 
            double t1, double t2)
        {
            var c = new Cube();
            var r = new Ray(new Point(px,py,pz), new Vector(dx,dy,dz));
            var xs = c.Intersects(r);
            Assert.AreEqual(t1,xs[0].T);
            Assert.AreEqual(t2,xs[1].T);
        }

        [TestMethod]
        [DataRow(-2, 0, 0, 0.2673, 0.5345, 0.8018)]
        [DataRow(0, -2, 0, 0.8018, 0.2673, 0.5345)]
        [DataRow(0, 0, -2, 0.5345, 0.8018, 0.2673)]

        [DataRow(2, 0, 2, 0, 0, -1)]
        [DataRow(0, 2, 2, 0, -1, 0)]
        [DataRow(2, 2, 0, -1, 0, 0)]
        public void ARayMissesACube(
            double px, double py, double pz,
            double dx, double dy, double dz)
        {
            var c = new Cube();
            var r = new Ray(new Point(px, py, pz), new Vector(dx, dy, dz));
            var xs = c.Intersects(r);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        [DataRow(1, 0.5, -0.8,      1, 0, 0)]
        [DataRow(-1, -0.2, 0.9,     -1, 0, 0)]

        [DataRow(-0.4, 1, -0.1,     0, 1, 0)]
        [DataRow(0.3, -1, -0.7,     0, -1, 0)]

        [DataRow(-0.6, 0.3, 1,      0, 0, 1)]
        [DataRow(0.4, 0.4, -1,      0, 0, -1)]

        [DataRow(1, 1, 1,           1, 0, 0)]
        [DataRow(-1, -1, -1,        -1, 0, 0)]
        public void TheNormalOnTheSurfaceOfACube(
            double px, double py, double pz,
            double nx, double ny, double nz)
        {
            var c = new Cube();
            var normal = c.NormalAt(new Point(px, py, pz), null);
            Assert.AreEqual(new Vector(nx, ny, nz), normal);

        }

        [TestMethod]
        public void CubeHasABoundingBox()
        {
            var shape = new Cube();
            var box = shape.Bounds;
            Assert.AreEqual(new Point(-1, -1, -1), box.Min);
            Assert.AreEqual(new Point(1, 1, 1), box.Max);
        }
    }
}
