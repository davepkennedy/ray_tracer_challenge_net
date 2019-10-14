using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using rt;
using static rt.Constants;

namespace rt_test
{
    [TestClass]
    public class TupleTest
    {

        [TestMethod]
        public void PointHasW_1()
        {
            var p = new Point(4, -4, 3);
            Assert.AreEqual(4.0f, p.x);
            Assert.AreEqual(-4.0f, p.y);
            Assert.AreEqual(3.0f, p.z);
            Assert.AreEqual(1.0f, p.w);
        }

        [TestMethod]
        public void VectorHasW_0()
        {
            var p = new Vector(4, -4, 3);
            Assert.AreEqual(4.0f, p.x);
            Assert.AreEqual(-4.0f, p.y);
            Assert.AreEqual(3.0f, p.z);
            Assert.AreEqual(0.0f, p.w);
        }

        [TestMethod]
        public void AddingTuples()
        {
            var a1 = new Tuple (3, -2, 5, 1);
            var a2 = new Tuple (-2, 3, 1, 0);
            Assert.AreEqual(new Tuple(1, 1, 6, 1), a1 + a2);
        }

        [TestMethod]
        public void SubtractingTuples()
        {
            var p1 = new Point(3, 2, 1);
            var p2 = new Point(5, 6, 7);
            Assert.AreEqual(new Vector(-2, -4, -6), p1 - p2);
        }

        [TestMethod]
        public void NegatingTuples()
        {
            var a = new Tuple (1, -2, 3, -4);
            Assert.AreEqual(new Tuple (-1, 2, -3, 4), -a);
        }

        [TestMethod]
        public void MultiplyByScalar()
        {
            var a = new Tuple (1, -2, 3, -4);
            Assert.AreEqual(new Tuple(3.5f, -7, 10.5f, -14), a * 3.5f);
        }

        [TestMethod]
        public void DividingByScalar()
        {
            var a = new Tuple (1, -2, 3, -4);
            Assert.AreEqual(new Tuple ( 0.5f, -1, 1.5f, -2 ), a / 2);
        }

        [TestMethod]
        public void ComputeMagnitude()
        {
            var cases = new List<Tuple>{
                new Vector(1, 0, 0),
                new Vector(0, 1, 0),
                new Vector(0, 0, 1),
                new Vector(1, 2, 3),
                new Vector(-1, -2, -3),
			};

            var expectations = new List<double>{
                1,
                1,
                1,
                System.Math.Sqrt (14),
                System.Math.Sqrt(14)
            };

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(expectations[i], cases[i].Magnitude());
            }
        }

        [TestMethod]
        public void Normalize_4_0_0()
        {
            var v = new Vector(4, 0, 0);
            Assert.AreEqual(new Vector(1, 0, 0), v.Normalize());
        }

        [TestMethod]
        public void Normalize_1_2_3()
        {
            double d = System.Math.Sqrt(14);
            var v = new Vector(1, 2, 3);
            Assert.AreEqual(new Vector(1 / d, 2 / d, 3 / d), v.Normalize());
        }

        [TestMethod]
        public void MagnitudeOfNormalized()
        {
            var v = new Vector(1, 2, 3);
            var n = v.Normalize();
            Assert.AreEqual(1.0f, n.Magnitude(), EPSILON);
        }

        [TestMethod]
        public void DotProduct()
        {
            var a = new Vector(1, 2, 3);
            var b = new Vector(2, 3, 4);
            Assert.AreEqual(20.0f, Tuple.Dot(a, b));
        }

        [TestMethod]
        public void CrossProduct()
        {
            var a = new Vector(1, 2, 3);
            var b = new Vector(2, 3, 4);

            Assert.AreEqual(new Vector(-1, 2, -1), Tuple.Cross(a, b));
            Assert.AreEqual(new Vector(1, -2, 1), Tuple.Cross(b, a));
        }

        [TestMethod]
        public void ReflectingAVectorApproachingAt45Deg()
        {
            var v = new Vector(1, -1, 0);
            var n = new Vector(0, 1, 0);
            var r = v.ReflectOn(n);
            Assert.AreEqual(new Vector(1, 1, 0), r);
        }

        [TestMethod]
        public void ReflectingAVectorApproachingASlantedSurface()
        {
            double rtot = (System.Math.Sqrt(2) / 2.0);
            var v = new Vector(0, -1, 0);
            var n = new Vector(rtot, rtot, 0);
            var r = v.ReflectOn(n);
            Assert.AreEqual(new Vector(1, 0, 0), r);
        }
    }
}
