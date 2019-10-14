using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace rt
{
    public class ObjFileParser
    {
        private List<Point> vertices = new List<Point>();
        public IList<Point> Vertices { get => vertices.AsReadOnly(); }

        private List<Vector> normals = new List<Vector>();
        public IList<Vector> Normals { get => normals.AsReadOnly(); }

        public int Ignored { get; internal set; }

        private Group defaultGroup = new Group();
        public Group DefaultGroup { get => defaultGroup; }

        private Dictionary<String, Group> groups = new Dictionary<string, Group>();
        public Group this[String groupName] { get => groups[groupName]; }

        public static ObjFileParser FromFile(String path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return new ObjFileParser(reader);
            }
        }

        public ObjFileParser(TextReader reader)
        {
            String line;
            Group group = defaultGroup;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                var parts = line.Split(' ');
                if (line.Length > 0)
                {
                    switch (parts[0])
                    {
                        case "v":
                            vertices.Add(PointFromString(line.Substring(1).Trim()));
                            break;
                        case "vn":
                            normals.Add(PointFromString(line.Substring(2).Trim()).ToVector());
                            break;
                        case "f":
                            FacesFromString(line.Substring(1).Trim()).ForEach(s => group.Add(s));
                            break;
                        case "g":
                            String groupName = line.Substring(1).Trim();
                            group = new Group();
                            groups[groupName] = group;
                            break;
                        default:
                            Ignored++;
                            break;
                    }
                }
            }
        }

        private List<int> ParseFaceEntry(string v)
        {
            return v.Split('/').Select(s => {
                if (s.Length > 0)
                {
                    try
                    {
                        return Int32.Parse(s) - 1;
                    } catch (FormatException e)
                    {
                        System.Console.WriteLine(e.Message);
                        System.Console.WriteLine(v);
                    }
                }
                return 0;
            }).ToList();
        }

        private List<List<int>> ParseFace (string v)
        {
            return v.Split(' ').Select(ParseFaceEntry).ToList();
        }

        private List<Shape> FacesFromString(string v)
        {
            var parts = ParseFace(v);
            List<Shape> faces = new List<Shape>();
            for (int i = 1; i < parts.Count - 1; i++)
            {
                if (parts[0].Count > 1 && (parts[0][2] >= 0) &&
                    parts[i].Count > 1 && (parts[i][2] >= 0) &&
                    parts[i + 1].Count > 1 && (parts[i + 1][2] >= 0)) {
                    faces.Add(new SmoothTriangle(
                        vertices[parts[0][0]],
                        vertices[parts[i][0]],
                        vertices[parts[i + 1][0]],
                        normals[parts[0][2]],
                        normals[parts[i][2]],
                        normals[parts[i + 1][2]]));
                }
                else
                {
                    faces.Add(new Triangle(
                        vertices[parts[0][0]],
                        vertices[parts[i][0]],
                        vertices[parts[i + 1][0]]));
                }
            }
            return faces;
        }

        private Point PointFromString(string v)
        {
            var parts = v.Split(' ').Select(s => {
                try
                {
                    return Double.Parse(s);
                }
                catch (FormatException e)
                {
                    System.Console.WriteLine(s);
                    System.Console.WriteLine(v);
                }
                return 0;
            }).ToArray();

            return new Point(parts[0], parts[1], parts[2]);
        }

        public Group ToGroup()
        {
            Group g = new Group();
            g.Add(DefaultGroup);
            groups.ToList().ForEach(p => g.Add(p.Value));

            return g;
        }
    }
}
