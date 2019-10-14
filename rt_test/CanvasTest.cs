using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt;

namespace rt_test
{
    [TestClass]
    public class CanvasTest
    {
        [TestMethod]
        public void CreatingACanvas()
        {
            var color = new Color(0, 0, 0);
            var canvas = new Canvas(10, 20);
            Assert.AreEqual(10, canvas.Width);
            Assert.AreEqual(20, canvas.Height);

            foreach (var pixel in canvas)
            {
                Assert.AreEqual(color, pixel);
            }
        }

        [TestMethod]
        public void WritePixelToCanvas()
        {
            var red = new Color(1, 0, 0);
            var canvas = new Canvas(10, 20);
            canvas[2, 3] = red;
            Assert.AreEqual(red, canvas[2, 3]);
        }

        [TestMethod]
        public void ConstructingThePPMHeader()
        {
            var c = new Canvas(5, 3);
            var ppm = c.toPpm();
            var writer = new StringWriter();
            writer.WriteLine("P3");
            writer.WriteLine("5 3");
            writer.WriteLine("255");
            var header = writer.ToString();
            Assert.AreEqual(header, ppm.Substring(0, header.Length));
        }

        [TestMethod]
        public void ConstructingThePPMPixelData()
        {
            var c = new Canvas(5, 3);
            var c1 = new Color(1.5f, 0, 0);
            var c2 = new Color(0, 0.5f, 0);
            var c3 = new Color(-0.5f, 0, 1);

            c[0,0] = c1;
            c[2,1] = c2;
            c[4,2] = c3;

            var buf = new StringReader(c.toPpm());

            for (int i = 0; i < 3; i++)
            {
                buf.ReadLine();
            }
            Assert.AreEqual("255 0 0 0 0 0 0 0 0 0 0 0 0 0 0", buf.ReadLine());
            Assert.AreEqual("0 0 0 0 0 0 0 128 0 0 0 0 0 0 0", buf.ReadLine());
            Assert.AreEqual("0 0 0 0 0 0 0 0 0 0 0 0 0 0 255", buf.ReadLine());
        }

        [TestMethod]
        public void PPMEndsWithNewLine()
        {
            var c = new Canvas(5, 3);
            var ppm = c.toPpm();
            Assert.AreEqual('\n', ppm[ppm.Length - 1]);
        }
    }
}
