using System;
using System.Collections.Generic;

using static rt.Constants;

namespace rt
{
    public class Tuple
    {
        public static double Dot (Tuple a, Tuple b)
        {
            return a.x * b.x +
            a.y * b.y +
            a.z * b.z +
            a.w * b.w;
        }

        public static Tuple Cross(Tuple a, Tuple b)
        {
            return new Vector(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }

        public double x { get; }
        public double y { get; }
        public double z { get; }
        public double w { get; }

        public Tuple(List<double> list)
        {
            if (list.Count != 4)
            {
                throw new ArgumentException("List Count is not 4");
            }
            x = list[0];
            y = list[1];
            z = list[2];
            w = list[3];
        }

        public Tuple (double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        internal List<double> ToList()
        {
            return new List<double> { x, y, z, w };
        }

        public static Tuple operator + (Tuple a, Tuple b)
        {
            return new Tuple(
                a.x + b.x,
                a.y + b.y,
                a.z + b.z,
                a.w + b.w
                );
        }

        public static Tuple operator -(Tuple a, Tuple b)
        {
            return new Tuple(
                a.x - b.x,
                a.y - b.y,
                a.z - b.z,
                a.w - b.w
                );
        }

        public static Tuple operator -(Tuple a)
        {
            return new Tuple(-a.x, -a.y, -a.z, -a.w);
        }

        public static Tuple operator* (Tuple a, double f)
        {
            return new Tuple(a.x * f, a.y * f, a.z * f, a.w * f);
        }

        public static Tuple operator /(Tuple a, double f)
        {
            return new Tuple(a.x / f, a.y / f, a.z / f, a.w / f);
        }

        public double Magnitude()
        {
            double t = 0;
			t += x * x;
            t += y * y;
            t += z * z;
            t += w * w;
			return System.Math.Sqrt (t);
		}

        public Tuple Normalize() {
            double m = Magnitude();
            if (m == 0)
            {
                return this;
            }
            return new Tuple(x/m, y/m, z/m, w);
		}

        public Tuple ReflectOn(Tuple normal)
        {
            return this - normal * 2 * Dot(this, normal);
        }

        public override string ToString() => $"Tuple({x},{y},{z},{w})";

        public override int GetHashCode()
        {
            var hashCode = -1743314642;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            hashCode = hashCode * -1521134295 + w.GetHashCode();
            return hashCode;
        }

        private static bool DoubleEquals(double a, double b)
        {
            return (Double.IsPositiveInfinity(a) && Double.IsPositiveInfinity(b)) ||
                (Double.IsNegativeInfinity(a) && Double.IsNegativeInfinity(b)) ||
                (Math.Abs(a - b) <= EPSILON);
        }

        public override bool Equals(object obj)
        {
            return obj is Tuple tuple &&
                DoubleEquals(x, tuple.x) &&
                DoubleEquals(y, tuple.y) &&
                DoubleEquals(z, tuple.z) &&
                Math.Abs(w - tuple.w) <= EPSILON;
        }

        public Point ToPoint() => new Point(x, y, z);
        public Vector ToVector() => new Vector(x, y, z);
    }

    public class Point : Tuple
    {
        public static Point MAX = new Point(Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity);
        public static Point MIN = new Point(Double.NegativeInfinity, Double.NegativeInfinity, Double.NegativeInfinity);

        public Point (double x, double y, double z) : base(x,y,z,1) { }
    }

    public class Vector : Tuple
    {
        public Vector(double x, double y, double z) : base(x, y, z, 0) { }
    }
}
