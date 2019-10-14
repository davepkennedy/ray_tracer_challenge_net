using System;
using System.Collections.Generic;
using System.Text;

namespace rt
{
    public abstract class Pattern
    {
        public Matrix Transform { get; set; }

        public Pattern ()
        {
            Transform = Matrix.Identity(4);
        }

        public abstract Color PatternAt(Point pt);

        public Color PatternAtObject(Shape shape, Point pt)
        {
            var objectPoint = shape.Transform.Inverse() * pt;
            var patternPoint = Transform.Inverse() * objectPoint;
            return PatternAt(patternPoint.ToPoint());
        }
    }

    public class StripePattern : Pattern
    {
        public readonly Color a;
        public readonly Color b;


        public StripePattern(Color a, Color b)
        {
            this.a = a;
            this.b = b;
        }

        public override Color PatternAt (Point pt)
        {
            if (Math.Floor(pt.x) % 2 == 0)
            {
                return a;
            }
            return b;
        }
    }

    public class GradientPattern : Pattern
    {
        public readonly Color a;
        public readonly Color b;

        public GradientPattern (Color a, Color b)
        {
            this.a = a;
            this.b = b;
        }

        public override Color PatternAt(Point pt)
        {
            return a + (b - a) * (pt.x - Math.Floor(pt.x));
        }
    }

    public class RingPattern : Pattern
    {
        public readonly Color a;
        public readonly Color b;

        public RingPattern(Color a, Color b)
        {
            this.a = a;
            this.b = b;
        }

        public override Color PatternAt(Point pt)
        {
            var xs = pt.x * pt.x;
            var zs = pt.z * pt.z;
            if (Math.Floor(Math.Sqrt(xs+zs)) % 2 == 0)
            {
                return a;
            }
            return b;
        }
    }

    public class CheckersPattern : Pattern
    {
        public readonly Color a;
        public readonly Color b;

        public CheckersPattern(Color a, Color b)
        {
            this.a = a;
            this.b = b;
        }

        public override Color PatternAt(Point pt)
        {
            if ((Math.Floor(pt.x) + Math.Floor(pt.y) + Math.Floor(pt.z)) % 2 == 0)
            {
                return a;
            }
            return b;
        }
    }

    public class PerlinPattern : Pattern
    {
        private Color a;
        private Color b;

        public PerlinPattern (Color a, Color b)
        {
            this.a = a;
            this.b = b;
        }

        public override Color PatternAt(Point pt)
        {
            var noise = ImprovedNoise.noise(pt.x, pt.y, pt.z);
            return a + (b - a) * noise;
        }
    }
}
