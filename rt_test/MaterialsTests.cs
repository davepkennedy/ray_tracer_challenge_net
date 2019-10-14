using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt;
using static rt.Constants;

namespace rt_test
{
    [TestClass]
    public class MaterialsTests
    {
        [TestMethod]
        public void TheDefaultMaterial()

        {
            var m = new Material();
            Assert.AreEqual(new Color(1, 1, 1), m.Color);
            Assert.AreEqual(0.1, m.Ambient, EPSILON);
            Assert.AreEqual(0.9, m.Diffuse, EPSILON);
            Assert.AreEqual(0.9, m.Specular, EPSILON);
            Assert.AreEqual(200.0, m.Shininess, EPSILON);
        }

        [TestMethod]
        public void LightingWithTheEyeBetweenTheLightAndTheSurface()
        {
            var shape = new Sphere();
            var m = new Material();
            var position = new Point(0, 0, 0);
            var eyev = new Vector(0, 0, -1);
            var normalv = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 0, -10), new Color(1, 1, 1));
            var result = Light.Lighting(m, shape, light, position, eyev, normalv, false);
            Assert.AreEqual(new Color(1.9f, 1.9f, 1.9f), result);
        }

        [TestMethod]
        public void LightingWithTheEyeBetweenTheLightAndTheSurfaceEyeOffset45()
        {
            var shape = new Sphere();
            double rtot = System.Math.Sqrt(2) / 2.0f;
            var m = new Material();
            var position = new Point(0, 0, 0);
            var eyev = new Vector(0, -rtot, -rtot);
            var normalv = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 0, -10), new Color(1, 1, 1));
            var result = Light.Lighting(m, shape, light, position, eyev, normalv, false);
            Assert.AreEqual(new Color(1.0f, 1.0f, 1.0f), result);
        }

        [TestMethod]
        public void LightingWithTheEyeBetweenTheLightAndTheSurfaceLIghtOffset45()
        {
            var shape = new Sphere();
            double rtot = System.Math.Sqrt(2) / 2.0f;
            var m = new Material();
            var position = new Point(0, 0, 0);
            var eyev = new Vector(0, 0, -1);
            var normalv = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 10, -10), new Color(1, 1, 1));
            var result = Light.Lighting(m, shape, light, position, eyev, normalv, false);
            Assert.AreEqual(new Color(0.7364f, 0.7364f, 0.7364f), result);
        }

        [TestMethod]
        public void LightingWithTheEyeInThePathOfTheReflectionVector()

        {
            var shape = new Sphere();
            double rtot = System.Math.Sqrt(2) / 2.0f;
            var m = new Material();
            var position = new Point(0, 0, 0);
            var eyev = new Vector(0, -rtot, -rtot);
            var normalv = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 10, -10), new Color(1, 1, 1));
            var result = Light.Lighting(m, shape, light, position, eyev, normalv, false);
            Assert.AreEqual(new Color(1.6364f, 1.6364f, 1.6364f), result);
        }

        [TestMethod]
        public void LightingWithLightBehindTheSurface()
        {
            var shape = new Sphere();
            var m = new Material();
            var position = new Point(0, 0, 0);
            var eyev = new Vector(0, 0, -1);
            var normalv = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 0, 10), new Color(1, 1, 1));
            var result = Light.Lighting(m, shape, light, position, eyev, normalv, false);
            Assert.AreEqual(new Color(0.1f, 0.1f, 0.1f), result);
        }

        [TestMethod]
        public void LightingWithTheSurfaceInShadow()
        {
            var shape = new Sphere();
            var m = new Material();
            var position = new Point(0, 0, 0);
            var eyev = new Vector(0, 0, -1);
            var normalv = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 0, -10), new Color(1, 1, 1));
            var inShadow = true;
            var result = Light.Lighting(m, shape, light, position, eyev, normalv, inShadow);
            Assert.AreEqual(new Color(0.1f, 0.1f, 0.1f), result);
        }

        [TestMethod]
        public void LightingWithAPatternApplied()
        {
            var m = new Material
            {
                Pattern = new StripePattern(Color.WHITE, Color.BLACK),
                Ambient = 1,
                Diffuse = 0,
                Specular = 0
            };

            var shape = new Sphere();
            var eyev = new Vector(0, 0, -1);
            var normalv = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 0, -10), Color.WHITE);
            var c1 = Light.Lighting(m, shape, light, new Point(0.9, 0, 0), eyev, normalv, false);
            var c2 = Light.Lighting(m, shape, light, new Point(1.1, 0, 0), eyev, normalv, false);
            Assert.AreEqual(Color.WHITE, c1);
            Assert.AreEqual(Color.BLACK, c2);
        }

        [TestMethod]
        public void ReflectivityForTheDefaultMaterial()
        {
            var m = new Material();
            Assert.AreEqual(0.0, m.Reflective);
        }

        [TestMethod]
        public void TransparencyAndRefractiveIndexForTheDefaultWorld()
        {
            var m = new Material();
            Assert.AreEqual(0, m.Transparency);
            Assert.AreEqual(1, m.RefractiveIndex);
        }
    }
}

