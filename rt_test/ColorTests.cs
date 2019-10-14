using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt;
using static rt.Constants;

namespace rt_test
{
    [TestClass]
    public class ColorTests
    {
        [TestMethod]
        public void ColorsAreRGBTuples()
        {
            var c = new Color(-0.5f, 0.4f, 1.7f);
            Assert.AreEqual(-0.5f, c.Red);
            Assert.AreEqual(0.4f, c.Green);
            Assert.AreEqual(1.7f, c.Blue);
        }

        [TestMethod]
    public void AddingColors()
    {
        var a = new Color(0.9f, 0.6f, 0.75f);
        var b = new Color(0.7f, 0.1f, 0.25f);
        var c = a + b;
        Assert.AreEqual(1.6f, c.Red, EPSILON);
        Assert.AreEqual(0.7f, c.Green, EPSILON);
        Assert.AreEqual(1.0f, c.Blue, EPSILON);
    }

        [TestMethod]
    public void SubtractingColors()
    {
        var a = new Color(0.9f, 0.6f, 0.75f);
        var b = new Color(0.7f, 0.1f, 0.25f);
        var c = a - b;
        Assert.AreEqual(0.2f, c.Red, EPSILON);
        Assert.AreEqual(0.5f, c.Green, EPSILON);
        Assert.AreEqual(0.5f, c.Blue, EPSILON);
    }

        [TestMethod]
    public void MultiplyByScalar()
    {
        var a = new Color(0.2f, 0.3f, 0.4f);
        var b = a * 2;

        Assert.AreEqual(0.4f, b.Red);
        Assert.AreEqual(0.6f, b.Green);
        Assert.AreEqual(0.8f, b.Blue);
    }
        [TestMethod]
    public void MultiplyByColor()
    {
        var a = new Color(1.0f, 0.2f, 0.4f);
        var b = new Color(0.9f, 1.0f, 0.1f);
        var c = a * b;
        Assert.AreEqual(0.9f, c.Red, EPSILON);
        Assert.AreEqual(0.2f, c.Green, EPSILON);
        Assert.AreEqual(0.04f, c.Blue, EPSILON);
    }
}
}
