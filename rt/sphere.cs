using System;
using System.Collections.Generic;
using System.Text;

namespace rt
{
    public class Sphere : Shape
    {
        public override bool Equals(object obj) => obj is Sphere sphere && base.Equals(sphere);

        public override int GetHashCode() => base.GetHashCode();

        override protected Tuple LocalNormalAt(Tuple pt, Intersection i)
        {
            return pt - new Point(0, 0, 0);
        }

        protected override Intersections IntersectsInt(Ray ray)
        {
            var sphereToRay = ray.Origin - new Point(0, 0, 0);
            var a = Tuple.Dot(ray.Direction, ray.Direction);
            var b = 2 * Tuple.Dot(ray.Direction, sphereToRay);
            var c = Tuple.Dot(sphereToRay, sphereToRay) - 1;

            var discriminant = (b * b) - 4 * a * c;

            if (discriminant < 0)
            {
                return new Intersections();
            }

            double t1 = (-b - Math.Sqrt(discriminant)) / (2 * a);
            double t2 = (-b + Math.Sqrt(discriminant)) / (2 * a);

            var i1 = new Intersection(t1, this);
            var i2 = new Intersection(t2, this);
            return new Intersections(new List<Intersection> { i1, i2 });
        }

        public override BoundingBox Bounds
        {
            get => new BoundingBox(new Point(-1, -1, -1), new Point(1, 1, 1));
        }

        public static Sphere Glass()
        {
            return new Sphere
            {
                Material = new Material
                {
                    Transparency = 1,
                    RefractiveIndex = 1.5
                }
            };
        }
    }
}
