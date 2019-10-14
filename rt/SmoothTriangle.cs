using System;
using System.Collections.Generic;
using System.Text;

namespace rt
{
    public class SmoothTriangle : Triangle
    {
        /*
        public Point P1 { get; internal set; }
        public Point P2 { get; internal set; }
        public Point P3 { get; internal set; }
        */

        public Vector N1 { get; internal set; }
        public Vector N2 { get; internal set; }
        public Vector N3 { get; internal set; }
        public SmoothTriangle (
            Point p1, Point p2, Point p3,
            Vector n1, Vector n2, Vector n3)
            : base(p1,p2,p3)
        {

            N1 = n1;
            N2 = n2;
            N3 = n3;
        }

        protected override Tuple LocalNormalAt(Tuple pt, Intersection i)
        {
            return N2 * i.U.Value +
                N3 * i.V.Value +
                N1 * (1 - i.U.Value - i.V.Value);
        }
    }
}
