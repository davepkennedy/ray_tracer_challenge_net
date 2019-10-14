using System;
using System.Collections.Generic;

using static rt.Constants;

namespace rt
{
    public class BoundingBox
    {

        public Point Min { get; internal set; }
        public Point Max { get; internal set; }

        public BoundingBox()
        {
            Min = Point.MAX;
            Max = Point.MIN;
        }

        public BoundingBox(Point min, Point max)
        {
            Min = min;
            Max = max;
        }

        public BoundingBox Bounds
        {
            get => this;
        }

        protected static void Swap(ref double a, ref double b)
        {
            var t = a;
            a = b;
            b = t;
        }

        public void Add(Point point)
        {
            if (point.x < Min.x) { Min = new Point(point.x, Min.y, Min.z); }
            if (point.x > Max.x) { Max = new Point(point.x, Max.y, Max.z); }

            if (point.y < Min.y) { Min = new Point(Min.x, point.y, Min.z); }
            if (point.y > Max.y) { Max = new Point(Max.x, point.y, Max.z); }

            if (point.z < Min.z) { Min = new Point(Min.x, Min.y, point.z); }
            if (point.z > Max.z) { Max = new Point(Max.x, Max.y, point.z); }
        }

        public void Add (BoundingBox other)
        {
            Add(other.Min);
            Add(other.Max);
        }

        public bool Contains(Point p)
        {
            return (Min.x <= p.x && p.x <= Max.x &&
                Min.y <= p.y && p.y <= Max.y &&
                Min.z <= p.z && p.z <= Max.z);
        }

        public bool Contains(BoundingBox box)
        {
            return Contains(box.Min) && Contains(box.Max);
        }

        public BoundingBox Transform(Matrix matrix)
        {
            var points = new List<Point> {
                Min,
                new Point(Min.x, Min.y, Max.z),
                new Point(Min.x, Max.y, Min.z),
                new Point(Min.x, Max.y, Max.z),
                new Point(Max.x, Min.y, Min.z),
                new Point(Max.x, Min.y, Max.z),
                new Point(Max.x, Max.y, Min.z),
                Max
            };

            var box = new BoundingBox();

            points.ForEach(p => box.Add((matrix * p).ToPoint()));

            return box;
        }

        private (double, double) CheckAxis(double min, double max, double origin, double direction)
        {
            double tmin, tmax;
            var tminNumerator = (min - origin);
            var tmaxNumerator = (max - origin);

            if (Math.Abs(direction) >= EPSILON)
            {
                tmin = tminNumerator / direction;
                tmax = tmaxNumerator / direction;
            }
            else
            {
                tmin = tminNumerator * Double.PositiveInfinity;
                tmax = tmaxNumerator * Double.PositiveInfinity;
            }

            if (tmin > tmax) { Swap(ref tmin, ref tmax); }
            return (tmin, tmax);
        }

        public bool Intersects(Ray ray)
        {
            (var xtmin, var xtmax) = CheckAxis(Min.x, Max.x, ray.Origin.x, ray.Direction.x);
            (var ytmin, var ytmax) = CheckAxis(Min.y, Max.y, ray.Origin.y, ray.Direction.y);
            (var ztmin, var ztmax) = CheckAxis(Min.z, Max.z, ray.Origin.z, ray.Direction.z);

            var tmin = Math.Max(xtmin, Math.Max(ytmin, ztmin));
            var tmax = Math.Min(xtmax, Math.Min(ytmax, ztmax));

            if (tmin > tmax)
            {
                return false;
            }

            return true;
        }

        public (BoundingBox, BoundingBox) Split()
        {
            // figure out the box's largest dimension
            var dx = Max.x - Min.x;
            var dy = Max.y - Min.y;
            var dz = Max.z - Min.z;

            var greatest = Math.Max(dx, Math.Max(dy, dz));

            // variables to help construct the points on
            // the dividing plane
            var x0 = Min.x;
            var y0 = Min.y;
            var z0 = Min.z;

            var x1 = Max.x;
            var y1 = Max.y;
            var z1 = Max.z;

            // adjust the points so that they lie on the
            // dividing plane
            if (greatest == dx) {
                x0 = x1 = x0 + dx / 2.0;
            }
            else if (greatest == dy) {
                y0 = y1 = y0 + dy / 2.0;
            }
            else {
                z0 = z1 = z0 + dz / 2.0;
            }

            var mid_min = new Point(x0, y0, z0);
            var mid_max = new Point(x1, y1, z1);

            // construct and return the two halves of
            //  the bounding box
            var left = new BoundingBox(Min, mid_max);
            var right = new BoundingBox(mid_min, Max);

            return (left, right);
        }
    }
}
 