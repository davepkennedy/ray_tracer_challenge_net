using System;
using System.Collections.Generic;
using System.Text;

using static System.Math;
using static rt.Constants;

namespace rt
{
    public class Cube : Shape
    {

        public override BoundingBox Bounds
        {
            get => new BoundingBox(
                new Point(-1, -1, -1),
                new Point(1, 1, 1));
        }

        private (double,double) CheckAxis(double origin, double direction)
        {
            double tmin, tmax;
            var tminNumerator = (-1 - origin);
            var tmaxNumerator = (1 - origin);

            if (Math.Abs(direction) >= EPSILON)
            {
                tmin = tminNumerator / direction;
                tmax = tmaxNumerator / direction;
            } else
            {
                tmin = tminNumerator * Double.PositiveInfinity;
                tmax = tmaxNumerator * Double.PositiveInfinity;
            }

            if (tmin > tmax) { Swap(ref tmin, ref tmax); }
            return (tmin, tmax);
        }

        protected override Intersections IntersectsInt(Ray r)
        {
            (var xtmin, var xtmax) = CheckAxis(r.Origin.x, r.Direction.x);
            (var ytmin, var ytmax) = CheckAxis(r.Origin.y, r.Direction.y);
            (var ztmin, var ztmax) = CheckAxis(r.Origin.z, r.Direction.z);

            var tmin = Math.Max(xtmin, Math.Max(ytmin, ztmin));
            var tmax = Math.Min(xtmax, Math.Min(ytmax, ztmax));

            if (tmin > tmax)
            {
                return new Intersections();
            }

            return new Intersections(
                new Intersection(tmin, this),
                new Intersection(tmax, this));
        }

        protected override Tuple LocalNormalAt(Tuple pt, Intersection i)
        {
            var maxc = Max(Abs(pt.x), Max(Abs(pt.y), Abs(pt.z)));
            if (maxc == Abs(pt.x))
            {
                return new Vector(pt.x, 0, 0);
            }
            else if (maxc == Abs(pt.y))
            {
                return new Vector(0, pt.y, 0);
            }
            return new Vector(0, 0, pt.z);
        }
    }
}
