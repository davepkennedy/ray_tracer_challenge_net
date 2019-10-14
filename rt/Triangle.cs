using System;
using System.Collections.Generic;
using System.Text;

using static rt.Constants;

namespace rt
{
    public class Triangle : Shape
    {

        public Point P1 { get; internal set; }
        public Point P2 { get; internal set; }
        public Point P3 { get; internal set; }

        public Vector E1 { get; internal set; }
        public Vector E2 { get; internal set; }
        public Vector Normal { get; internal set; }

        public override BoundingBox Bounds {
            get
            {
                var bounds = new BoundingBox();
                bounds.Add(P1);
                bounds.Add(P2);
                bounds.Add(P3);
                return bounds;
            }
        }

        public Triangle(Point p1, Point p2, Point p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;

            E1 = (p2 - p1).ToVector();
            E2 = (p3 - p1).ToVector();

            Normal = Tuple.Cross(E2, E1).Normalize().ToVector();
        }

        protected override Tuple LocalNormalAt(Tuple pt, Intersection i)
        {
            return Normal;
        }

        protected override Intersections IntersectsInt(Ray r)
        {
            var dir_cross_e2 = Tuple.Cross(r.Direction, E2);
            var det = Tuple.Dot(E1, dir_cross_e2);
            if (Math.Abs(det) < EPSILON)
            {
                return Intersections.EMPTY;
            }

            var f = 1.0 / det;
            var p1_to_origin = r.Origin - P1;
            var u = f * Tuple.Dot(p1_to_origin, dir_cross_e2);
            if (u < 0 || u > 1)
            {
                return Intersections.EMPTY;
            }

            var origin_cross_e1 = Tuple.Cross(p1_to_origin, E1);
            var v = f * Tuple.Dot(r.Direction, origin_cross_e1);
            if (v < 0 || (u+v) > 1)
            {
                return Intersections.EMPTY;
            }

            var t = f * Tuple.Dot(E2, origin_cross_e1);
            return new Intersections(
                new Intersection(t, this, u, v));
        }
    }
}
