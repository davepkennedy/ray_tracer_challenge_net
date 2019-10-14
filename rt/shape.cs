using System;
using System.Collections.Generic;
using System.Text;

namespace rt
{
    public abstract class Shape
    {

        protected static void Swap(ref double a, ref double b)
        {
            var t = a;
            a = b;
            b = t;
        }
        protected abstract Tuple LocalNormalAt(Tuple pt, Intersection i);

        public Matrix Transform { get; set; }

        public Material Material { get; set; }

        public Shape Parent { get; set; }

        public abstract BoundingBox Bounds { get; }

        public  BoundingBox ParentSpaceBounds() {
            return Bounds.Transform(Transform);
        }

        public bool HasParent
        {
            get
            {
                return Parent != null;
            }
        }

        public Tuple NormalAt(Tuple pt, Intersection i)
        {
            var localPoint = WorldToObject(pt);
            var localNormal = LocalNormalAt(localPoint, i);
            return NormalToWorld(localNormal);
        }

        public Tuple WorldToObject(Tuple pt)
        {
            if (HasParent)
            {
                pt = Parent.WorldToObject(pt);
            }
            return (Transform.Inverse() * pt);
        }

        public Tuple NormalToWorld(Tuple normal)
        {
            normal = (Transform.Inverse().Transpose() * normal);
            normal = new Vector(normal.x, normal.y, normal.z);
            normal = normal.Normalize();
            //normal.w = 0;
            if (HasParent)
            {
                normal = Parent.NormalToWorld(normal);
            }

            return normal;
        }

        public override bool Equals(object obj)
        {
            return obj is Shape shape &&
                   EqualityComparer<Matrix>.Default.Equals(Transform, shape.Transform) &&
                   EqualityComparer<Material>.Default.Equals(Material, shape.Material);
        }

        public override int GetHashCode()
        {
            var hashCode = -2065766541;
            hashCode = hashCode * -1521134295 + EqualityComparer<Matrix>.Default.GetHashCode(Transform);
            hashCode = hashCode * -1521134295 + EqualityComparer<Material>.Default.GetHashCode(Material);
            return hashCode;
        }

        public Shape()
        {
            Transform = Matrix.Identity(4);
            Material = new Material();
        }

        protected abstract Intersections IntersectsInt(Ray r);

        public Intersections Intersects(Ray ray)
        {
            var r = ray.Transform(Transform.Inverse());
            return IntersectsInt(r);
        }

        public virtual bool Contains (Shape shape)
        {
            return this == shape;
        }

        public virtual void Divide(int threshold)
        {

        }
    }
}
