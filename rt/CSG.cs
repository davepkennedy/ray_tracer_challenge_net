using System;
using System.Collections.Generic;
using System.Text;

namespace rt
{
    public abstract class Operation
    {
        public static Operation UNION = new UnionOp();
        public static Operation INTERSECTION = new IntersectionOp();
        public static Operation DIFFERENCE = new DifferenceOp();
        public abstract bool IntersectionAllowed(bool lhit, bool inl, bool inr);

        public static Operation OfKind (string op)
        {
            switch (op)
            {
                case "union":
                    return UNION;
                case "intersection":
                    return INTERSECTION;
                case "difference":
                    return DIFFERENCE; 
            }
            return null;
        }
    }

    class UnionOp : Operation
    {
        public override bool IntersectionAllowed(bool lhit, bool inl, bool inr)
        {
            return (lhit && !inr) || (!lhit && !inl);
        }
    }

    class IntersectionOp : Operation
    {
        public override bool IntersectionAllowed(bool lhit, bool inl, bool inr)
        {
            return (lhit && inr) || (!lhit && inl);
        }
    }

    class DifferenceOp : Operation
    {
        public override bool IntersectionAllowed(bool lhit, bool inl, bool inr)
        {
            return (lhit && !inr) || (!lhit && inl);
        }
    }

    public class CSG : Shape
    {
        public Operation Operation { get; private set; }
        public Shape Left { get; private set; }
        public Shape Right { get; private set; }

        public override BoundingBox Bounds
        {
            get
            {
                var box = new BoundingBox();
                box.Add(Left.ParentSpaceBounds());
                box.Add(Right.ParentSpaceBounds());
                return box;
            }
        }

        public CSG(Operation operation, Shape left, Shape right)
        {
            Operation = operation;
            Left = left;
            Right = right;

            left.Parent = this;
            right.Parent = this;
        }

        public override bool Contains(Shape shape)
        {
            return Left.Contains(shape) || Right.Contains(shape);
        }

        protected override Tuple LocalNormalAt(Tuple pt, Intersection i)
        {
            throw new NotImplementedException();
        }

        protected override Intersections IntersectsInt(Ray r)
        {
            if (Bounds.Intersects(r))
            {
                return FilterIntersections(Intersections.Merge(Left.Intersects(r), Right.Intersects(r)));
            }
            return Intersections.EMPTY;
        }

        public Intersections FilterIntersections(Intersections xs)
        { 
            var inl = false;
            var inr = false;

            var result = new Intersections();

            foreach (var i in xs)
            {
                var lhit = this.Left.Contains(i.Shape);

                if (this.Operation.IntersectionAllowed(lhit, inl, inr))
                {
                    result.Add(i);
                }
                if (lhit)
                {
                    inl = !inl;
                } else
                {
                    inr = !inr;
                }
            }

            return result;
        }

        public override void Divide(int threshold)
        {
            Left.Divide(threshold);
            Right.Divide(threshold);
        }
    }
}
