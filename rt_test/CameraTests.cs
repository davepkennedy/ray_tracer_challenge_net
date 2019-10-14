using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static rt.Constants;

using rt;
namespace rt_test
{
    [TestClass]
    public class CameraTests
    {
        [TestMethod]
        public void ConstructingACamera()
        {
            var hsize = 160;
            var vsize = 120;
            var fieldOfView = Math.PI / 2.0;

            var camera = new Camera(hsize, vsize, fieldOfView);

            Assert.AreEqual(hsize, camera.Hsize);
            Assert.AreEqual(vsize, camera.Vsize);
            Assert.AreEqual(fieldOfView, camera.FieldOfView);
            Assert.AreEqual(Matrix.Identity(4), camera.Transform);
        }

        [TestMethod]
        public void PixelSizeForAHorizontalCanvas()
        {
            var camera = new Camera(200, 125, Math.PI / 2);
            Assert.AreEqual(0.01f, camera.PixelSize, EPSILON);
        }

        [TestMethod]
        public void PixelSizeForAVerticalCanvas()
        {
            var camera = new Camera(125, 200, Math.PI / 2);
            Assert.AreEqual(0.01f, camera.PixelSize, EPSILON);
        }

        [TestMethod]
        public void ConstructingARayThroughTheCenterOfTheCanvas()
        {
            var camera = new Camera(201, 101, Math.PI / 2);
            var r = camera.RayForPixel(100, 50);
            Assert.AreEqual(new Point(0, 0, 0), r.Origin);
            Assert.AreEqual(new Vector(0, 0, -1), r.Direction);
        }

        [TestMethod]
        public void ConstructingARayThroughTheCornerOfTheCanvas()
        {
            var camera = new Camera(201, 101, Math.PI / 2);
            var r = camera.RayForPixel(0, 0);
            Assert.AreEqual(new Point(0, 0, 0), r.Origin);
            Assert.AreEqual(new Vector(0.66519f, 0.33259f, -0.66851f), r.Direction);
        }

        [TestMethod]
        public void ConstructingARayWhenTheCanvasIsTransformed()
        {
            double rtot = Math.Sqrt(2) / 2;
            var camera = new Camera(201, 101, Math.PI / 2);
            camera.Transform = Transform.RotationY(Math.PI / 4) * Transform.Translation(0, -2, 5);
            var r = camera.RayForPixel(100, 50);
            Assert.AreEqual(new Point(0, 2, -5), r.Origin);
            Assert.AreEqual(new Vector(rtot, 0, -rtot), r.Direction);
        }

        [TestMethod]
        public void RenderingAWorldWithACamera()
        {
            var w = World.Default();
            var c = new Camera(11,11,Math.PI/2);
            var f = new Point(0, 0, -5);
            var t = new Point(0, 0, 0);
            var u = new Vector(0, 1, 0);
            c.Transform = Transform.View(f, t, u);
            var image = c.Render(w);
            Assert.AreEqual(new Color(0.38066f, 0.47583f, 0.2855f), image[5, 5]);
        }
    }
}
