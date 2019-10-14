using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using static rt.Constants;

namespace rt
{
    public class Computation
    {
        public double T { get; internal set; }
        public Shape Shape { get; internal set; }
        public Point Point { get; internal set; }
        public Point OverPoint { get; internal set; }
        public Point UnderPoint { get; internal set; }
        public Tuple EyeV { get; internal set; }
        public Tuple NormalV { get; internal set; }
        public bool Inside { get; internal set; }
        public Tuple ReflectV { get; internal set; }

        public double N1 { get; internal set; }
        public double N2 { get; internal set; }

        public double Schlick()
        {
            var cos = Tuple.Dot(EyeV, NormalV);
            if (N1 > N2)
            {
                var n = N1 / N2;
                var sin2T = (n * n) * (1 - (cos * cos)); 
                if (sin2T > 1)
                {
                    return 1;
                }
                var cos_t = Math.Sqrt(1 - sin2T);
                cos = cos_t;
            }
            var r0 = Math.Pow((N1 - N2) / (N1 + N2), 2);
            return r0 + (1 - r0) * Math.Pow(1 - cos, 5);
        }
    }

    public class Intersection
    {
        public double T { get; private set; }
        public Shape Shape { get; private set;}

        public double? U { get; private set; }
        public double? V { get; private set; }

        public Intersection(double t, Shape shape)
            : this(t, shape, null, null)
        {
        }

        public Intersection (double t, Shape shape, double? u, double? v)
        {

            T = t;
            Shape = shape;
            U = u;
            V = v;
        }

        public override bool Equals(object obj)
        {
            return obj is Intersection intersection &&
                   T == intersection.T &&
                   EqualityComparer<Shape>.Default.Equals(Shape, intersection.Shape);
        }

        public override int GetHashCode()
        {
            var hashCode = -1435833727;
            hashCode = hashCode * -1521134295 + T.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Shape>.Default.GetHashCode(Shape);
            return hashCode;
        }

        public override string ToString() => $"Intersection({T}, {Shape.GetHashCode()})";

        public Computation PrepareComputations(Ray ray)
        {
            return PrepareComputations(ray, new Intersections(this));
        }

        public Computation PrepareComputations (Ray ray, Intersections xs)
        {
            var comps = new Computation();
            comps.T = T;
            comps.Shape = Shape;

            comps.Point = ray.Position(T);
            comps.EyeV = -ray.Direction;

            comps.NormalV = Shape.NormalAt(comps.Point, this);
            comps.ReflectV = ray.Direction.ReflectOn(comps.NormalV);

            if (Tuple.Dot(comps.NormalV, comps.EyeV) < 0)
            {
                comps.Inside = true;
                comps.NormalV = -comps.NormalV;
            }
            comps.OverPoint = (comps.Point + comps.NormalV * EPSILON).ToPoint();
            comps.UnderPoint = (comps.Point - comps.NormalV * EPSILON).ToPoint();

            List<Shape> containers = new List<Shape>();
            foreach (var i in xs) {
                if (i == this)
                {
                    if (containers.Count == 0)
                    {
                        comps.N1 = 1.0;
                    } else
                    {
                        comps.N1 = containers.Last().Material.RefractiveIndex;
                    }
                }

                if (containers.Contains(i.Shape))
                {
                    containers.Remove(i.Shape);
                } else
                {
                    containers.Add(i.Shape);
                }

                if (i == this)
                {
                    if (containers.Count == 0)
                    {
                        comps.N2 = 1.0;
                    } else
                    {
                        comps.N2 = containers.Last().Material.RefractiveIndex;
                    }
                    break;
                }
            }

            return comps;
        }
    }


    public class Intersections : IEnumerable<Intersection>
    {
        private readonly List<Intersection> intersections;

        public static readonly Intersections EMPTY = new Intersections();

        public static Intersections Merge (Intersections left, Intersections right)
        {
            var result = new Intersections(left.intersections);
            result.intersections.AddRange(right);
            result.intersections.Sort((a, b) => a.T == b.T ? 0 : a.T < b.T ? -1 : 1);
            return result;
        }

        public Intersections()
        {
            intersections = new List<Intersection>();
        }

        public Intersections (params Intersection[] intersections)
        {
            this.intersections = new List<Intersection> (intersections);
        }

        public Intersections(IEnumerable<Intersection> intersections)
        {
            this.intersections = new List<Intersection>(intersections);
        }

        public void Add (Intersection intersection)
        {
            intersections.Add(intersection);
        }

        public int Count { get => intersections.Count; }
        public Intersection this[int idx] { get => intersections[idx]; }

        public Intersections Filter(Func<Intersection, bool> f) =>
            new Intersections(new List<Intersection>(intersections.Where(f)));

        public Intersection First() => intersections.First();

        public Intersection Hit()
        {
            var positives = new List<Intersection>(intersections.Where((i) => i.T >= 0));
            positives.Sort((a, b) => a.T == b.T ? 0 : a.T < b.T ? -1 : 1);
            if (positives.Count > 0)
            {
                return positives.First();
            }
            return null;
        }

        public IEnumerator<Intersection> GetEnumerator()
        {
            return intersections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return intersections.GetEnumerator();
        }
    }
}
