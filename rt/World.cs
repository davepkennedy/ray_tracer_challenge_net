using System;
using System.Collections.Generic;
using System.Linq;

namespace rt
{
    public class World
    {
        private readonly List<Shape> shapes = new List<Shape>();

        public Shape this[int pos] { get => shapes[pos]; }

        public PointLight Light { get; set; }
        public bool Empty { get => shapes.Count == 0; }

        public static World Default()
        {
            var w = new World();
            w.Light = new PointLight(new Point(-10, 10, -10), new Color(1, 1, 1));
            var s1 = new Sphere();
            s1.Material.Color = new Color(0.8f, 1.0f, 0.6f);
            s1.Material.Diffuse = 0.7f;
            s1.Material.Specular = 0.2f;
            var s2 = new Sphere();
            s2.Transform = Transform.Scaling(0.5f, 0.5f, 0.5f);
            w.Add(s1);
            w.Add(s2);
            return w;
        }

        public void Add (Shape shape)
        {
            shapes.Add(shape);
        }

        public bool Contains(Shape shape)
        {
            return shapes.Contains(shape);
        }

        public Intersections Intersect(Ray ray)
        {
            return new Intersections(shapes.SelectMany(shape => shape.Intersects(ray)).OrderBy(i => i.T));
        }

        public Color Shade(Computation comps, int stackDepth)
        {
            bool shadowed = IsShadowed(comps.OverPoint);
            var surface = rt.Light.Lighting(comps.Shape.Material, comps.Shape,  
                Light, comps.OverPoint, comps.EyeV, comps.NormalV, shadowed);
            var reflected = ReflectedColor(comps, stackDepth);
            var refracted = RefractedColor(comps, stackDepth);

            if (comps.Shape.Material.Reflective > 0 && comps.Shape.Material.Transparency > 0)
            {
                var reflectance = comps.Schlick();
                return surface + (reflected * reflectance) +
                    (refracted * (1 - reflectance));
            }

            return surface + reflected + refracted;
        }

        public Color ColorAt(Ray r, int stackDepth)
        {
            var intersections = Intersect(r);
            var intersection = intersections.Hit();
            if (intersection != null)
            {
                var comps = intersection.PrepareComputations(r, intersections);
                return Shade(comps, stackDepth);
            }
            return Color.BLACK;
        }

        public bool IsShadowed(Tuple point)
        {
            var v = Light.Position - point;
            var distance = v.Magnitude();
            var direction = v.Normalize();
            var ray = new Ray(point, direction);
            var intersections = Intersect(ray);
            var h = intersections.Hit();
            if (h != null && h.T < distance)
            {
                return true;
            }
            return false;
        }

        public Color ReflectedColor(Computation comps, int stackDepth)
        {
            if (stackDepth <= 0)
            {
                return Color.BLACK;
            }
            if (comps.Shape.Material.Reflective == 0)
            {
                return Color.BLACK;
            }
            var reflectRay = new Ray(comps.OverPoint, comps.ReflectV);
            var color = ColorAt(reflectRay, stackDepth-1);
            return color * comps.Shape.Material.Reflective;
        }

        public Color RefractedColor(Computation comps, int remaining)
        {
            if (remaining == 0)
            {
                return Color.BLACK;
            }
            if (comps.Shape.Material.Transparency == 0)
            {
                return Color.BLACK;
            }

            var nRatio = comps.N1 / comps.N2;
            var cosI = Tuple.Dot(comps.EyeV, comps.NormalV);
            var sin2t = (nRatio * nRatio) * (1 - (cosI * cosI));
            if (sin2t > 1)
            {
                return Color.BLACK;
            }

            var cosT = Math.Sqrt(1.0 - sin2t);
            var direction = comps.NormalV * (nRatio * cosI - cosT) -
                comps.EyeV * nRatio;
            var refractRay = new Ray(comps.UnderPoint, direction);
            return ColorAt(refractRay, remaining - 1) * comps.Shape.Material.Transparency;
        }
    }
}
