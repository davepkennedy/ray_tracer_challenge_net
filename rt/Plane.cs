using System;
using System.Collections.Generic;
using System.Text;

using static rt.Constants;

namespace rt
{
    public class Plane : Shape
    {
        private static Vector NORMAL = new Vector(0, 1, 0);

        public override BoundingBox Bounds
        {
            get => new BoundingBox(
                new Point(Double.NegativeInfinity, 0, Double.NegativeInfinity),
                new Point(Double.PositiveInfinity, 0, Double.PositiveInfinity));
        }

        protected override Intersections IntersectsInt(Ray ray)
        {
            if (Math.Abs(ray.Direction.y) < EPSILON)
            {
                return new Intersections();
            }
            var t = -ray.Origin.y / ray.Direction.y;
            return new Intersections(new Intersection(t, this));
        }

        protected override Tuple LocalNormalAt(Tuple pt, Intersection i)
        {
            return NORMAL;
        }
    }
}
