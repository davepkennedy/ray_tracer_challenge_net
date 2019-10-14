using System;
using System.Collections.Generic;
using System.Text;

using static rt.Constants;

namespace rt
{
    public class Cone : Shape
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public bool Closed { get; set; }
        public Cone()
        {
            Minimum = Double.NegativeInfinity;
            Maximum = Double.PositiveInfinity;

            Closed = false;
        }

        public override BoundingBox Bounds
        {
            get {
                var limit = Math.Max(Math.Abs(Minimum), Math.Abs(Maximum));
                return new BoundingBox(
                    new Point(-limit, Minimum, -limit),
                    new Point(limit, Maximum, limit));
            }
        }
        protected override Intersections IntersectsInt(Ray r)
        {
            var a = (r.Direction.x * r.Direction.x) -
                (r.Direction.y * r.Direction.y) +
                (r.Direction.z * r.Direction.z);

            var b = 2 * r.Origin.x * r.Direction.x -
                2 * r.Origin.y * r.Direction.y +
                2 * r.Origin.z * r.Direction.z;

            var c = (r.Origin.x * r.Origin.x) -
                (r.Origin.y * r.Origin.y) +
                (r.Origin.z * r.Origin.z);
            var disc = (b * b) - 4 * a * c;

            if (disc < 0)
            {
                return Intersections.EMPTY;
            }

            double[] ts;
            if (a == 0)
            {
                ts = new double[] { -c / (2*b) };
            } else {

                var t0 = (-b - Math.Sqrt(disc)) / (2 * a);
                var t1 = (-b + Math.Sqrt(disc)) / (2 * a);

                if (t0 > t1)
                {
                    Swap(ref t0, ref t1);
                }
                ts = new double[] { t0, t1 };
            }

            var xs = new Intersections();

            foreach (var t in ts) {
                var y = r.Origin.y + t * r.Direction.y;
                if (Minimum < y && y < Maximum)
                {
                    xs.Add(new Intersection(t, this));
                }
            }

            IntersectCaps(r, xs);

            return xs;
        }

        private void IntersectCaps(Ray r, Intersections xs)
        {
            if (!this.Closed || Math.Abs(r.Direction.y) < EPSILON)
            {
                return;
            }

            foreach (var v in new double[] { Minimum, Maximum })
            {
                var t = (v - r.Origin.y) / r.Direction.y;
                if (CheckCap(r, t, Math.Abs(v)))
                {
                    xs.Add(new Intersection(t, this));
                }
            }
        }

        private bool CheckCap(Ray r, double t, double radius)
        {
            var x = r.Origin.x + t * r.Direction.x;
            var z = r.Origin.z + t * r.Direction.z;

            return (x * x + z * z) <= radius;
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
            var y = Math.Sqrt(pt.x * pt.x + pt.z * pt.z);
            if (pt.y > 0)
            {
                y = -y;
            }
            return new Vector(pt.x, y, pt.z);
        }
    }
}
