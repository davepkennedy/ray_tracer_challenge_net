using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt;
using static rt.Constants;

namespace rt_test
{
    [TestClass]
    public class WorldTests
    {
        [TestMethod]
        public void CreatingAWorld()
        {
            var w = new World();
            Assert.IsTrue(w.Empty);
            Assert.IsNull(w.Light);
        }

        [TestMethod]
        public void TheDefaultWorld()
        {
            var light = new PointLight(new Point(-10, 10, -10), new Color(1, 1, 1));
            var s1 = new Sphere();
            s1.Material.Color = new Color(0.8f, 1.0f, 0.6f);
            s1.Material.Diffuse = 0.7f;
            s1.Material.Specular = 0.2f;
            var s2 = new Sphere();
            s2.Transform = Transform.Scaling(0.5f, 0.5f, 0.5f);

            var w = World.Default();
            Assert.AreEqual(light, w.Light);
            Assert.IsFalse(w.Empty);
            Assert.IsTrue(w.Contains(s1));
            Assert.IsTrue(w.Contains(s2));
        }

        [TestMethod]
        public void IntersectTheWorldWithARay()
        {
            var w = World.Default();
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = w.Intersect(r);
            Assert.AreEqual(4, xs.Count);
            Assert.AreEqual(4, xs[0].T);
            Assert.AreEqual(4.5, xs[1].T);
            Assert.AreEqual(5.5, xs[2].T);
            Assert.AreEqual(6, xs[3].T);
        }

        [TestMethod]
        public void TheColorWhenARayMisses()
        {
            var w = World.Default();
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 1, 0));
            Assert.AreEqual(new Color(0, 0, 0), w.ColorAt(r,5));
        }

        [TestMethod]
        public void TheColorWhenARayHits()
        {
            var w = World.Default();
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            Assert.AreEqual(new Color(0.38066f, 0.47583f, 0.2855f), w.ColorAt(r,5));
        }

        [TestMethod]
        public void TheColorWithAnIntersectionBehindTheRay()
        {
            var w = World.Default();
            var outer = w[0];
            outer.Material.Ambient = 1;
            var inner = w[1];
            inner.Material.Ambient = 1;
            var r = new Ray(new Point(0, 0, 0.75f), new Vector(0, 0, -1));
            var c = w.ColorAt(r,5);
            Assert.AreEqual(inner.Material.Color, c);
        }

        [TestMethod]
        public void ThereIsNoShadowWhenNothingIsCollinearWithPointAndLight()
        {
            var world = World.Default();
            var p = new Point(0, 10, 0);
            Assert.IsFalse(world.IsShadowed(p));
        }

        [TestMethod]
        public void TheWhenAnObjectIsBetweenThePointAndTheLight()
        {
            var w = World.Default();
            var p = new Point(10, -10, 10);
            Assert.IsTrue(w.IsShadowed(p));
        }

        [TestMethod]
        public void ThereIsNoShadowWhenAnObjectIsBehindTheLight()
        {
            var world = World.Default();
            var p = new Point(0, 10, 0);
            Assert.IsFalse(world.IsShadowed(p));
        }

        [TestMethod]
        public void ThereIsNoShadowWhenAnObjectIsBehindThePoint()
        {
            var world = World.Default();
            var p = new Point(0, 10, 0);
            Assert.IsFalse(world.IsShadowed(p));
        }

        [TestMethod]
        public void ShadeHitIsGivenAnIntersectionInShadow()
        {
            var w = new World();
            w.Light = new PointLight(new Point(0, 0, -10), new Color(1, 1, 1));
            var s1 = new Sphere();
            w.Add(s1);
            var s2 = new Sphere()
            {
                Transform = Transform.Translation(0, 0, 10)
            };
            w.Add(s2);
            var ray = new Ray(new Point(0, 0, 5), new Vector(0, 0, 1));
            var i = new Intersection(4, s2);
            var comps = i.PrepareComputations(ray);
            var c = w.Shade(comps,5);
            Assert.AreEqual(new Color(0.1f, 0.1f, 0.1f), c);
        }

        [TestMethod]
        public void TheReflectedColorForANonreflectiveMaterial()
        {
            var w = World.Default();
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var shape = w[1];
            shape.Material.Ambient = 1;
            var i = new Intersection(1, shape);
            var comps = i.PrepareComputations(r);
            var color = w.ReflectedColor(comps,5);
            Assert.AreEqual(Color.BLACK, color);
        }

        [TestMethod]
        public void TheReflectedColorForAReflectiveMaterial()
        {
            var w = World.Default();
            var shape = new Plane();
            shape.Material.Reflective = 0.5;
            shape.Transform = Transform.Translation(0, -1, 0);
            w.Add(shape);
            var r = new Ray(new Point(0, 0, -3), new Vector(0, -ROOT_TWO_OVER_TWO, ROOT_TWO_OVER_TWO));
            var i = new Intersection(Math.Sqrt(2), shape);
            var comps = i.PrepareComputations(r);
            var color = w.ReflectedColor(comps,5);
            Assert.AreEqual(new Color(0.19032, 0.2379, 0.14274), color);
        }

        [TestMethod]
        public void ShadeHitWithAReflectiveMaterial()
        {
            var w = World.Default();
            var shape = new Plane();
            shape.Material.Reflective = 0.5;
            shape.Transform = Transform.Translation(0, -1, 0);
            w.Add(shape);
            var r = new Ray(new Point(0, 0, -3), new Vector(0, -ROOT_TWO_OVER_TWO, ROOT_TWO_OVER_TWO));
            var i = new Intersection(Math.Sqrt(2), shape);
            var comps = i.PrepareComputations(r);
            var color = w.Shade(comps,5);
            Assert.AreEqual(new Color(0.87677, 0.92436, 0.82918), color);
        }

        [Timeout (300)]
        [TestMethod]
        public void ColorAtWithMutuallyReflectiveSurfaces()
        {
            var w = new World();
            w.Light = new PointLight(new Point(0, 0, 0), Color.WHITE);

            var lower = new Plane();
            lower.Material.Reflective = 1;
            lower.Transform = Transform.Translation(0, -1, 0);
            w.Add(lower);

            var upper = new Plane();
            upper.Material.Reflective = 1;
            upper.Transform = Transform.Translation(0, 1, 0);
            w.Add(upper);

            var ray = new Ray(new Point(0, 0, 0), new Vector(0, 1, 0));
            w.ColorAt(ray,5);
        }

        [TestMethod]
        public void TheReflectiveColorAtTheMaximumRecursionDepth()
        {
            var w = World.Default();

            var shape = new Plane();
            shape.Material.Reflective = 1;
            shape.Transform = Transform.Translation(0, -1, 0);
            w.Add(shape);

            var r = new Ray(new Point(0, 0, -3), new Vector(0, -ROOT_TWO_OVER_TWO, ROOT_TWO_OVER_TWO));
            var i = new Intersection(ROOT_TWO, shape);
            var comps = i.PrepareComputations(r);
            var color = w.ReflectedColor(comps, 0);
            Assert.AreEqual(Color.BLACK, color);
        }

        [TestMethod]
        public void TheRefractedColorWithAnOpaqueSurface()
        {
            var w = World.Default();
            var shape = w[0];
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = new Intersections(
                new Intersection(4, shape),
                new Intersection(6, shape)
                );
            var comps = xs[0].PrepareComputations(r, xs);
            var c = w.RefractedColor(comps, 5);
            Assert.AreEqual(Color.BLACK, c);
        }

        [TestMethod]
        public void TheRefractedColorAtTheMaximumRecursiveDepth()
        {
            var w = World.Default();
            var shape = w[0];
            shape.Material.Transparency = 1.0;
            shape.Material.RefractiveIndex = 1.5;
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = new Intersections(
                new Intersection(4, shape),
                new Intersection(6, shape));
            var comps = xs[0].PrepareComputations(r, xs);
            var c = w.RefractedColor(comps, 0);
            Assert.AreEqual(Color.BLACK, c);
        }

        [TestMethod]
        public void TheRefractedColorUnderTotalInternalReflection()
        {
            var rtot = Math.Sqrt(2) / 2;
            var w = World.Default();
            var shape = w[0];
            shape.Material.Transparency = 1;
            shape.Material.RefractiveIndex = 1.5;
            var r = new Ray(new Point(0, 0, -rtot), new Vector(0, 1, 0));
            var xs = new Intersections(
                new Intersection(-rtot, shape),
                new Intersection(rtot, shape));
            var comps = xs[1].PrepareComputations(r, xs);
            var c = w.RefractedColor(comps, 5);
            Assert.AreEqual(Color.BLACK, c);
        }

        [TestMethod]
        public void TheRefractedColorWithARefractedRay()
        {
            var w = World.Default();
            var A = w[0];
            A.Material.Ambient = 1;
            A.Material.Pattern = new TestPattern();

            var B = w[1];
            B.Material.Transparency = 1;
            B.Material.RefractiveIndex = 1.5;

            var r = new Ray(new Point(0, 0, 0.1), new Vector(0, 1, 0));
            var xs = new Intersections(
                new Intersection(-0.9899,A),
                new Intersection(-0.4899,B),
                new Intersection(0.4899,B),
                new Intersection(0.9899,A));

            var comps = xs[2].PrepareComputations(r, xs);
            var c = w.RefractedColor(comps, 5);
            Assert.AreEqual(new Color(0, 0.99888, 0.04725), c);
        }

        [TestMethod]
        public void ShadeHitWithATransparentMaterial()
        {
            var w = World.Default();

            var floor = new Plane();
            floor.Transform = Transform.Translation(0, -1, 0);
            floor.Material.Transparency = 0.5;
            floor.Material.RefractiveIndex = 1.5;
            w.Add(floor);

            var ball = new Sphere();
            ball.Material.Color = new Color(1, 0, 0);
            ball.Material.Ambient = 0.5;
            ball.Transform = Transform.Translation(0, -3.5, -0.5);
            w.Add(ball);

            var r = new Ray(new Point(0, 0, -3), new Vector(0, -ROOT_TWO_OVER_TWO, ROOT_TWO_OVER_TWO));
            var xs = new Intersections(new Intersection(ROOT_TWO, floor));
            var comps = xs[0].PrepareComputations(r, xs);
            var color = w.Shade(comps, 5);
            Assert.AreEqual(new Color(0.93642, 0.68642, 0.68642), color);
        }
    }
}
