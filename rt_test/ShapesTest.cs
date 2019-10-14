using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using rt;

namespace rt_test
{
    class TestShape : Shape
    {
        protected override rt.Tuple LocalNormalAt(rt.Tuple pt, rt.Intersection i)
        {
            return pt;
        }

        public Ray SavedRay { get; internal set; }
        protected override Intersections IntersectsInt(Ray r)
        {
            SavedRay = r;
            return new Intersections();
        }

        public override BoundingBox Bounds
        {
            get => new BoundingBox(
                new Point(-1, -1, -1),
                new Point(1, 1, 1));
        }
    }

    [TestClass]
    public class ShapesTest
    {
        [TestMethod]
        public void TheDefaultTransformation()
        {
            var shape = new TestShape();

            Assert.AreEqual(Matrix.Identity(4), shape.Transform);
        }

        [TestMethod]
        public void AssigningATransformation()
        {
            var shape = new TestShape
            {
                Transform = Transform.Translation(2, 3, 4)
            };
            Assert.AreEqual(Transform.Translation(2, 3, 4), shape.Transform);
        }

        [TestMethod]
        public void TheDefaultMaterial()
        {
            var s = new TestShape();
            Assert.AreEqual(new Material(), s.Material);

        }

        [TestMethod]
        public void AssigningAMaterial()
        {
            var m = new Material
            {
                Ambient = 1
            };
            var s = new TestShape
            {
                Material = m
            };
            Assert.AreEqual(m, s.Material);
        }

        [TestMethod]
        public void IntersectingAScaledShapedWithARay()
        {
            var ray = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var shape = new TestShape
            {
                Transform = Transform.Scaling(2, 2, 2)
            };
            var xs = shape.Intersects(ray);
            Assert.AreEqual(new Point(0, 0, -2.5), shape.SavedRay.Origin);
            Assert.AreEqual(new Vector(0, 0, 0.5), shape.SavedRay.Direction);
        }

        [TestMethod]
        public void IntersectingATranslatedShapeWithARay()
        {
            var ray = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var shape = new TestShape
            {
                Transform = Transform.Translation(5, 0, 0)
            };
            var xs = shape.Intersects(ray);

            Assert.AreEqual(new Point(-5, 0, -5), shape.SavedRay.Origin);
            Assert.AreEqual(new Vector(0, 0, 1), shape.SavedRay.Direction);
        }

        [TestMethod]
        public void ComputingTheNormalOnATranslatedShape()
        {
            var shape = new TestShape
            {
                Transform = Transform.Translation(0, 1, 0)
            };
            var n = shape.NormalAt(new Point(0, 1.70711, -0.70711), null);
            Assert.AreEqual(new Vector(0, 0.70711, -0.70711), n);
        }

        [TestMethod]
        public void ComputingTheNormalOnATransformedShape()
        {
            var matrix = Transform.Scaling(1, 0.5, 1) * Transform.RotationZ(Math.PI / 5);
            var shape = new TestShape
            {
                Transform = matrix
            };
            var n = shape.NormalAt(new Point(0, Math.Sqrt(2) / 2, -Math.Sqrt(2) / 2), null);
            Assert.AreEqual(new Vector(0, 0.97014, -0.24254), n);
        }

        [TestMethod]
        public void AShapeHasAParentAttribute()
        {
            var s = new TestShape();
            Assert.IsNull(s.Parent);
        }

        [TestMethod]
        public void ConvertingAPointFromWorldToObjectSpace()
        {
            var g1 = new Group
            {
                Transform = Transform.RotationY(Math.PI / 2)
            };
            var g2 = new Group
            {
                Transform = Transform.Scaling(2, 2, 2)
            };
            g1.Add(g2);
            var s = new Sphere
            {
                Transform = Transform.Translation(5, 0, 0)
            };
            g2.Add(s);

            var p = s.WorldToObject(new Point(-2, 0, -10));
            Assert.AreEqual(new Point(0, 0, -1), p);
        }

        [TestMethod]
        public void ConvertingANormalFromObjectToWorldSpace()
        {
            double rtot = Math.Sqrt(3) / 3.0;
            var g1 = new Group
            {
                Transform = Transform.RotationY(Math.PI / 2)
            };
            var g2 = new Group
            {
                Transform = Transform.Scaling(1, 2, 3)
            };
            g1.Add(g2);

            var s = new Sphere {
                Transform = Transform.Translation(5,0,0)
            };
            g2.Add(s);

            var n = s.NormalToWorld(new Vector(rtot, rtot, rtot));
            Assert.AreEqual(new Vector(0.2857, 0.4286, -0.8571), n);
        }

        [TestMethod]
        public void FindingTheNormalOnAChildObject()
        {
            var g1 = new Group
            {
                Transform = Transform.RotationY(Math.PI / 2)
            };
            var g2 = new Group
            {
                Transform = Transform.Scaling(1, 2, 3)
            };
            g1.Add(g2);

            var s = new Sphere
            {
                Transform = Transform.Translation(5, 0, 0)
            };
            g2.Add(s);

            var n = s.NormalAt(new Point(1.7321, 1.1547, -5.5774), null);
            Assert.AreEqual(new Vector(0.2857, 0.4286, -0.8571), n);
        }

        [TestMethod]
        public void QueryingAShapesBoundingBoxInItsParentsSpace()
        {
            var shape = new Sphere
            {
                Transform = Transform.Translation(1, -3, 5) * Transform.Scaling(0.5, 2, 4)
            };

            var box = shape.ParentSpaceBounds();
            Assert.AreEqual(new Point(0.5, -5, 1), box.Min);
            Assert.AreEqual(new Point(1.5, -1, 9), box.Max);
        }

        [TestMethod]
        public void SubdividingAPrimitiveDoesNothing()
        {
            var shape = new Sphere();
            shape.Divide(1);
            Assert.IsTrue(shape is Sphere);
        }
    }
}
