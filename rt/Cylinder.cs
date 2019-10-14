using System;
using System.Collections.Generic;
using System.Text;

using static rt.Constants;

namespace rt
{
    public class Cylinder : Shape
    {

        public Cylinder()
        {
            Minimum = Double.NegativeInfinity;
            Maximum = Double.PositiveInfinity;

            Closed = false;
        }

        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public bool Closed { get; set; }

        public override BoundingBox Bounds
        {
            get => new BoundingBox(new Point(-1, Minimum, -1), new Point(1, Maximum, 1));
        }

        protected override Intersections IntersectsInt(Ray r)
        {
            var a = (r.Direction.x * r.Direction.x) + 
                (r.Direction.z * r.Direction.z);

            var b = 2 * r.Origin.x * r.Direction.x +
                2 * r.Origin.z * r.Direction.z;
            var c = (r.Origin.x * r.Origin.x) +
                (r.Origin.z * r.Origin.z) - 1;
            var disc = (b * b) - 4 * a * c;

            if (disc < 0)
            {
                return Intersections.EMPTY;
            }

            var t0 = (-b - Math.Sqrt(disc)) / (2 * a);
            var t1 = (-b + Math.Sqrt(disc)) / (2 * a);

            if (t0 > t1)
            {
                Swap(ref t0, ref t1);
            }

            var xs = new Intersections();

            var y0 = r.Origin.y + t0 * r.Direction.y;
            if (Minimum < y0 && y0 < Maximum)
            {
                xs.Add(new Intersection(t0, this));
            }

            var y1 = r.Origin.y + t1 * r.Direction.y;
            if (Minimum < y1 && y1 < Maximum)
            {
                xs.Add(new Intersection(t1, this));
            }

            IntersectCaps(r, xs);

            return xs;
        }

        private void IntersectCaps (Ray r, Intersections xs)
        {
            if (!this.Closed || Math.Abs(r.Direction.y) < EPSILON)
            {
                return;
            }

            foreach (var v in new double[] { Minimum, Maximum })
            {
                var t = (v - r.Origin.y) / r.Direction.y;
                if (CheckCap(r, t))
                {
                    xs.Add(new Intersection(t, this));
                }
            }
        }

        private bool CheckCap (Ray r, double t)
        {
            var x = r.Origin.x + t * r.Direction.x;
            var z = r.Origin.z + t * r.Direction.z;

            return (x * x + z * z) <= 1;
        }

        protected override Tuple LocalNormalAt(Tuple pt, Intersection i)
        {
            var dist = (pt.x * pt.x) + (pt.z * pt.z);
            if (dist < 1 && pt.y >= (Maximum - EPSILON))
            {
                return new Vector(0, 1, 0);
            }
            if (dist < 1 && pt.y <= Minimum + EPSILON)
            {
                return new Vector(0, -1, 0);
            }
            return new Vector(pt.x, 0, pt.z);
        }
    }
}
