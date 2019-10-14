using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt;
using static rt.Constants;

namespace rt_test
{

    [TestClass]
    public class IntersectionsTest
    {
        [TestMethod]
        public void TheHitWhenAllIntersectionsHavePositiveT()
        {
            var s = new Sphere();
            var i1 = new Intersection(1, s);
            var i2 = new Intersection(2, s);
            var xs = new Intersections(new List<Intersection> { i2, i1 });
            var i = xs.Hit();
            Assert.AreEqual(i1, i);
        }

        [TestMethod]
        public void TheHitWhenSomeIntersectionsHaveNegativeT()
        {
            var s = new Sphere();
            var i1 = new Intersection(-1, s);
            var i2 = new Intersection(1, s);
            var xs = new Intersections(new List<Intersection> { i2, i1 });
            var i = xs.Hit();
            Assert.AreEqual(i2, i);

        }

        [TestMethod]
        public void TheHitWhenAllIntersectionsHaveNegativeT()
        {
            var s = new Sphere();
            var i1 = new Intersection(-1, s);
            var i2 = new Intersection(-2, s);
            var xs = new Intersections(new List<Intersection> { i2, i1 });
            var i = xs.Hit();
            Assert.IsNull(i);
        }

        [TestMethod]
        public void TheHitIsAllwaysTheLowestNonNegativeIntersection()
        {
            var s = new Sphere();
            var i1 = new Intersection(5, s);
            var i2 = new Intersection(7, s);
            var i3 = new Intersection(-3, s);
            var i4 = new Intersection(2, s);
            var xs = new Intersections(new List<Intersection> { i1, i2, i3, i4 });
            var i = xs.Hit();
            Assert.AreEqual(i4, i);
        }

        [TestMethod]
        public void PrecomputingTheStateOfAnIntersection()
        {
            var r = new Ray(new Point(0,0,-5), new Vector(0,0,1));
            var shape = new Sphere();
            var i = new Intersection(4, shape);

            var comps = i.PrepareComputations(r);
            Assert.AreEqual(i.T, comps.T);
            Assert.AreEqual(i.Shape, comps.Shape);
            Assert.AreEqual(new Point(0, 0, -1), comps.Point);
            Assert.AreEqual(new Vector(0, 0, -1), comps.EyeV);
            Assert.AreEqual(new Vector(0, 0, -1), comps.NormalV);
        }

        [TestMethod]
        public void HitWhenIntersectionOccursOnTheOutside()
        {
            var r = new Ray(new Point(0,0,-5), new Vector(0,0,1));
            var shape = new Sphere();
            var i = new Intersection(4, shape);
            var comps = i.PrepareComputations(r);
            Assert.IsFalse(comps.Inside);
        }

        [TestMethod]
        public void HitWhenIntersectionOccursOnTheInside()
        {
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var shape = new Sphere();
            var i = new Intersection(1, shape);
            var comps = i.PrepareComputations(r);
            Assert.AreEqual(new Point(0,0,1), comps.Point);
            Assert.AreEqual(new Vector(0,0,-1), comps.EyeV);
            Assert.IsTrue(comps.Inside);
            Assert.AreEqual(new Vector(0,0,-1), comps.NormalV);
        }

        [TestMethod]
        public void ShadingAnIntersection()
        {
            var w = World.Default();
            var r = new Ray(new Point(0,0,-5), new Vector(0,0,1));
            var shape = w[0];
            var i = new Intersection(4, shape);
            var comps = i.PrepareComputations(r);
            var c = w.Shade(comps,5);
            Assert.AreEqual(new Color(0.38066f, 0.47583f, 0.2855f), c);
        }

        [TestMethod]
        public void ShadingAnIntersectionFromTheInside()
        {
            var w = World.Default();
            w.Light = new PointLight(new Point(0,0.25f,0), new Color(1,1,1));
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var shape = w[1];
            var i = new Intersection(0.5f, shape);
            var comps = i.PrepareComputations(r);
            var c = w.Shade(comps,5);
            Assert.AreEqual(new Color(0.90498, 0.90498, 0.90498), c);
            //Assert.AreEqual(new Color(0.1, 0.1, 0.1), c);
        }

        [TestMethod]
        public void TheHitShouldOffsetThePoint()
        {
            var ray = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var shape = new Sphere
            {
                Transform = Transform.Translation(0, 0, 1)
            };
            var i = new Intersection(5, shape);
            var comps = i.PrepareComputations(ray);
            Assert.IsTrue(comps.OverPoint.z < -EPSILON / 2);
        }

        [TestMethod]
        public void PrecomputingTheReflectionVector()
        {
            var rtot = Math.Sqrt(2) / 2;
            var shape = new Plane();
            var ray = new Ray(new Point(0, 1, -1), new Vector(0, -rtot, rtot));
            var i = new Intersection(Math.Sqrt(2), shape);
            var comps = i.PrepareComputations(ray);
            Assert.AreEqual(new Vector(0, rtot, rtot), comps.ReflectV);
        }

        [TestMethod]
        [DataRow(0, 1.0, 1.5)]
        [DataRow(1, 1.5, 2.0)]
        [DataRow(2, 2.0, 2.5)]
        [DataRow(3, 2.5, 2.5)]
        [DataRow(4, 2.5, 1.5)]
        [DataRow(5, 1.5, 1.0)]
        public void FindingN1AndN2AtVariousIntersections(int index, double n1, double n2)
        {
            var A = Sphere.Glass();
            A.Transform = Transform.Scaling(2, 2, 2);
            A.Material.RefractiveIndex = 1.5;

            var B = Sphere.Glass();
            B.Transform = Transform.Translation(0, 0, -0.25);
            B.Material.RefractiveIndex = 2.0;

            var C = Sphere.Glass();
            C.Transform = Transform.Translation(0, 0, 0.25);
            C.Material.RefractiveIndex = 2.5;

            var r = new Ray(new Point(0, 0, -4), new Vector(0, 0, 1));
            var xs = new Intersections(new List<Intersection> {
                new Intersection(2,A),
                new Intersection(2.75,B),
                new Intersection(3.25,C),
                new Intersection(4.75,B),
                new Intersection(5.25,C),
                new Intersection(6,A)
            });
            var comps = xs[index].PrepareComputations(r, xs);
            Assert.AreEqual(n1, comps.N1);
            Assert.AreEqual(n2, comps.N2);
        }

        [TestMethod]
        public void TheUnderpointIsOffsetBelowTheSurface()
        {
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var shape = Sphere.Glass();
            shape.Transform = Transform.Translation(0, 0, 1);
            var i = new Intersection(5, shape);
            var xs = new Intersections(i);
            var comps = i.PrepareComputations(r, xs);
            Assert.IsTrue(comps.UnderPoint.z > EPSILON / 2);
            Assert.IsTrue(comps.Point.z < comps.UnderPoint.z);
        }

        [TestMethod]
        public void TheSchlickApproximationUnderTotalInternalReflection()
        {
            var shape = Sphere.Glass();
            var r = new Ray(new Point(0, 0, -ROOT_TWO_OVER_TWO), new Vector(0, 1, 0));
            var xs = new Intersections(
                new Intersection(-ROOT_TWO_OVER_TWO, shape),
                new Intersection(ROOT_TWO_OVER_TWO, shape));
            var comps = xs[1].PrepareComputations(r, xs);
            var reflectance = comps.Schlick();
            Assert.AreEqual(1, reflectance);
        }

        [TestMethod]
        public void TheSchlickApproximationWithAPerpendicularViewingAngle()
        {
            var shape = Sphere.Glass();
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 1, 0));
            var xs = new Intersections(
                new Intersection(-1, shape),
                new Intersection(1, shape));
            var comps = xs[1].PrepareComputations(r, xs);
            var reflectance = comps.Schlick();
            Assert.AreEqual(0.04, reflectance, EPSILON);
        }

        [TestMethod]
        public void TheSchlickApproximationWithSmallAngleAndN2MoreThanN1()
        {
            var shape = Sphere.Glass();
            var r = new Ray(new Point(0, 0.99, -2), new Vector(0, 0, 1));
            var xs = new Intersections(
                new Intersection(1.8589, shape));
            var comps = xs[0].PrepareComputations(r, xs);
            var reflectance = comps.Schlick();
            Assert.AreEqual(0.48873, reflectance, EPSILON);
        }

        [TestMethod]
        public void ShadeHitWithAReflectiveTransparentMaterial()
        {
            var w = World.Default();
            var r = new Ray(new Point(0, 0, -3), new Vector(0, -ROOT_TWO_OVER_TWO, ROOT_TWO_OVER_TWO));

            var floor = new Plane();
            floor.Transform = Transform.Translation(0, -1, 0);
            floor.Material.Reflective = 0.5;
            floor.Material.Transparency = 0.5;
            floor.Material.RefractiveIndex = 1.5;
            w.Add(floor);

            var ball = new Sphere();
            ball.Material.Color = new Color(1, 0, 0);
            ball.Material.Ambient = 0.5;
            ball.Transform = Transform.Translation(0, -3.5, -0.5);
            w.Add(ball);

            var xs = new Intersections(
                new Intersection(ROOT_TWO, floor));
            var comps = xs[0].PrepareComputations(r, xs);
            var color = w.Shade(comps, 5);
            Assert.AreEqual(new Color(0.93391, 0.69643, 0.69243), color);
        }

        [TestMethod]
        public void AnIntersectionCanEncapsulateUAndV()
        {
            var shape = new Triangle(
                new Point(0, 1, 0),
                new Point(-1, 0, 0),
                new Point(1, 0, 0));
            var i = new Intersection(3.5, shape, 0.2, 0.4);
            Assert.AreEqual(0.2, i.U);
            Assert.AreEqual(0.4, i.V);

        }
    }
}
