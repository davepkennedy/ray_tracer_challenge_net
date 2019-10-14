using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace rt
{
    public class Group : Shape//, IEnumerable<Shape>
    {
        private List<Shape> shapes = new List<Shape>();

        public static Group Of(params Shape[] shapes)
        {
            return Of(new List<Shape>(shapes));
        }

        public static Group Of(List<Shape> shapes) {
            var g = new Group();
            foreach (var shape in shapes) {
                g.Add(shape);
            }
            return g;
        }

        private static Shape HexagonCorner()
        {
            return new Sphere
            {
                Transform = rt.Transform.Translation(0, 0, -1) *
                rt.Transform.Scaling(0.25, 0.25, 0.25)
            };
        }

        private static Shape HexagonEdge()
        {
            return new Cylinder
            {
                Minimum = 0,
                Maximum = 1,
                Transform = rt.Transform.Translation(0, 0, -1) *
                rt.Transform.RotationY(-Math.PI / 6) *
                rt.Transform.RotationZ(-Math.PI / 2) *
                rt.Transform.Scaling(0.25, 1, 0.25)
            };
        }

        private static Shape HexagonSide()
        {
            var side = new Group();
            side.Add(HexagonCorner());
            side.Add(HexagonEdge());
            return side;
        }

        public static Shape Hexagon()
        {
            var hex = new Group();
            for (int i = 0; i < 6; i++)
            {
                var side = HexagonSide();
                side.Transform = rt.Transform.RotationY(i * Math.PI / 3);
                hex.Add(side);
            }
            return hex;
        }

        protected override Intersections IntersectsInt(Ray r)
        {
            if (Bounds.Intersects(r))
            {
                List<Intersection> hits = new List<Intersection>();
                shapes.ForEach((shape) =>
                {
                    var xs = shape.Intersects(r);
                    foreach (var i in xs)
                    {
                        hits.Add(i);
                    }
                });
                hits.Sort((a, b) => a.T == b.T ? 0 : a.T < b.T ? -1 : 1);

                return new Intersections(hits);
            }
            return Intersections.EMPTY;
        }

        protected override Tuple LocalNormalAt(Tuple pt, Intersection i)
        {
            throw new NotImplementedException();
        }

        public int Count { get
            {
                return shapes.Count;
            }
        }

        public override BoundingBox Bounds
        {
            get
            {
                var box = new BoundingBox();
                shapes.ForEach(p => box.Add(p.ParentSpaceBounds()));
                return box;
            }
        }

        public void Add(Shape s)
        {
            shapes.Add(s);
            s.Parent = this;
        }

        public override bool Contains(Shape s)
        {
            return shapes.Contains(s);
        }

        public Shape this[int idx]
        {
            get => shapes[idx];
        }

        public (List<Shape>, List<Shape>) PartitionChildren()
        {
            var leftList = new List<Shape>();
            var rightList = new List<Shape>();

            var toRemove = new List<Shape>();

            (var left, var right) = Bounds.Split();
            foreach (var shape in shapes)
            {
                if (left.Contains(shape.ParentSpaceBounds()))
                {
                    leftList.Add(shape);
                    toRemove.Add(shape);
                }
                if (right.Contains(shape.ParentSpaceBounds()))
                {
                    rightList.Add(shape);
                    toRemove.Add(shape);
                }
            }

            toRemove.ForEach(s => shapes.Remove(s));

            return (leftList, rightList);
        }

        public void MakeSubgroup (params Shape[] shapes)
        {
            MakeSubgroup(new List<Shape>(shapes));
        }

        public void MakeSubgroup (List<Shape> shapes)
        {
            Add(Group.Of(shapes));
        }

        public override void Divide(int threshold)
        {
            if (threshold <= Count)
            {
                (var left, var right) = PartitionChildren();
                if (left.Count > 0)
                {
                    MakeSubgroup(left);
                }
                if (right.Count > 0)
                {
                    MakeSubgroup(right);
                }
            }
            shapes.ForEach(s => s.Divide(threshold));
        }

        private static bool CompareShapes (List<Shape> myShapes, List<Shape> theirShapes)
        {
            var firstNotSecond = myShapes.Except(theirShapes);
            var secondNotFirst = theirShapes.Except(myShapes);
            return !firstNotSecond.Any() && !secondNotFirst.Any();
        }

        public override bool Equals(object obj)
        {
            return obj is Group group &&
                   base.Equals(obj) &&
                   //EqualityComparer<List<Shape>>.Default.Equals(shapes, group.shapes) &&
                   CompareShapes(shapes, group.shapes) &&
                   Count == group.Count;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
