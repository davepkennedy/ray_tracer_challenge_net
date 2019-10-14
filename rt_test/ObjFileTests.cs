using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using rt;

namespace rt_test
{
    [TestClass]
    public class ObjFileTests
    {
        [TestMethod]
        public void IgnoringUnrecognizedLines()
        {
            var input = @"
There was a young lady named Bright
Who travelled much faster than light
She set out one day
In a relative way
And returned home the previous night
            ";
            var reader = new StringReader(input);
            var parser = new ObjFileParser(reader);

            Assert.AreEqual(5, parser.Ignored);
        }

        [TestMethod]
        public void VertexData()
        {
            var input = @"
v -1 1 0
v -1.000 0.5000 0.000
v 1 0 0
v 1 1 0
            ";
            var reader = new StringReader(input);
            var parser = new ObjFileParser(reader);
            Assert.AreEqual(new Point(-1, 1, 0), parser.Vertices[0]);
            Assert.AreEqual(new Point(-1, 0.5, 0), parser.Vertices[1]);
            Assert.AreEqual(new Point(1, 0, 0), parser.Vertices[2]);
            Assert.AreEqual(new Point(1, 1, 0), parser.Vertices[3]);
        }

        [TestMethod]
        public void ParsingTriangleFaces()
        {
            var input = @"
v -1 1 0
v -1 0 0
v 1 0 0
v 1 1 0

f 1 2 3
f 1 3 4
            ";
            var reader = new StringReader(input);
            var parser = new ObjFileParser(reader);

            var g = parser.DefaultGroup;
            var t1 = g[0] as Triangle;
            var t2 = g[1] as Triangle;

            Assert.AreEqual(t1.P1, parser.Vertices[0]);
            Assert.AreEqual(t1.P2, parser.Vertices[1]);
            Assert.AreEqual(t1.P3, parser.Vertices[2]);
            Assert.AreEqual(t2.P1, parser.Vertices[0]);
            Assert.AreEqual(t2.P2, parser.Vertices[2]);
            Assert.AreEqual(t2.P3, parser.Vertices[3]);
        }

        [TestMethod]
        public void TriangulatingPolygons()
        {
            var input = @"
v -1 1 0
v -1 0 0
v 1 0 0
v 1 1 0
v 0 2 0

f 1 2 3 4 5
            ";
            var reader = new StringReader(input);
            var parser = new ObjFileParser(reader);

            var g = parser.DefaultGroup;
            var t1 = g[0] as Triangle;
            var t2 = g[1] as Triangle;
            var t3 = g[2] as Triangle;

            Assert.AreEqual(t1.P1, parser.Vertices[0]);
            Assert.AreEqual(t1.P2, parser.Vertices[1]);
            Assert.AreEqual(t1.P3, parser.Vertices[2]);
            Assert.AreEqual(t2.P1, parser.Vertices[0]);
            Assert.AreEqual(t2.P2, parser.Vertices[2]);
            Assert.AreEqual(t2.P3, parser.Vertices[3]);
            Assert.AreEqual(t3.P1, parser.Vertices[0]);
            Assert.AreEqual(t3.P2, parser.Vertices[3]);
            Assert.AreEqual(t3.P3, parser.Vertices[4]);
        }

        [TestMethod]
        public void TrianglesInGroups()
        {
            var input = @"
v -1 1 0
v -1 0 0
v 1 0 0
v 1 1 0

g FirstGroup
f 1 2 3

g SecondGroup
f 1 3 4
";
            var reader = new StringReader(input);
            var parser = new ObjFileParser(reader);

            var g1 = parser["FirstGroup"];
            var g2 = parser["SecondGroup"];

            var t1 = g1[0] as Triangle;
            var t2 = g2[0] as Triangle;

            Assert.AreEqual(t1.P1, parser.Vertices[0]);
            Assert.AreEqual(t1.P2, parser.Vertices[1]);
            Assert.AreEqual(t1.P3, parser.Vertices[2]);
            Assert.AreEqual(t2.P1, parser.Vertices[0]);
            Assert.AreEqual(t2.P2, parser.Vertices[2]);
            Assert.AreEqual(t2.P3, parser.Vertices[3]);
        }

        [TestMethod]
        public void ConvertingAnObjFileToAGroup()
        {
            var input = @"
v -1 1 0
v -1 0 0
v 1 0 0
v 1 1 0

g FirstGroup
f 1 2 3

g SecondGroup
f 1 3 4
";
            var reader = new StringReader(input);
            var parser = new ObjFileParser(reader);

            var g1 = parser["FirstGroup"];
            var g2 = parser["SecondGroup"];

            var g = parser.ToGroup();
            Assert.IsTrue(g.Contains(g1));
            Assert.IsTrue(g.Contains(g2));

        }

        [TestMethod]
        public void VertexNormalRecords()
        {
            var input = @"
vn 0 0 1
vn 0.707 0 -0.707
vn 1 2 3";

            var reader = new StringReader(input);
            var parser = new ObjFileParser(reader);

            Assert.AreEqual(new Vector(0, 0, 1), parser.Normals[0]);
            Assert.AreEqual(new Vector(0.707, 0, -0.707), parser.Normals[1]);
            Assert.AreEqual(new Vector(1, 2, 3), parser.Normals[2]);
        }

        [TestMethod]
        public void FacesWithNormals()
        {
            var input = @"
v 0 1 0
v -1 0 0
v 1 0 0

vn -1 0 0
vn 1 0 0
vn 0 1 0

f 1//3 2//1 3//2
f 1/0/3 2/102/1 3/14/2";

            var reader = new StringReader(input);
            var parser = new ObjFileParser(reader);

            var g = parser.DefaultGroup;
            var t1 = g[0] as SmoothTriangle;
            var t2 = g[1] as SmoothTriangle;

            Assert.AreEqual(parser.Vertices[0], t1.P1);
            Assert.AreEqual(parser.Vertices[1], t1.P2);
            Assert.AreEqual(parser.Vertices[2], t1.P3);

            Assert.AreEqual(parser.Normals[2], t1.N1);
            Assert.AreEqual(parser.Normals[0], t1.N2);
            Assert.AreEqual(parser.Normals[1], t1.N3);

            Assert.AreEqual(t1, t2);
        }
    }
}
