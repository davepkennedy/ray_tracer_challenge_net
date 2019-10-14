using System.Collections.Generic;
using System;

namespace rt
{
    public class Transform
    {
        public static Matrix Translation(double x, double y, double z)
        {
            var m = Matrix.Identity(4);
            m[0, 3] = x;
            m[1, 3] = y;
            m[2, 3] = z;
            return m;
        }

        public static Matrix Scaling(double x, double y, double z)
        {
            var m = Matrix.Identity(4);
            m[0, 0] = x;
            m[1, 1] = y;
            m[2, 2] = z;
            return m;
        }

        public static Matrix RotationX(double r)
        {
            return new Matrix(new List<double>{
                1,0,0,0,
			0,Math.Cos(r), -Math.Sin(r),0,
			0,Math.Sin(r), Math.Cos(r),0,
			0,0,0,1
    
        });
        }

        public static Matrix RotationY(double r)
        {
            return new Matrix(new List<double>{
                Math.Cos(r), 0, Math.Sin(r), 0,
                0, 1, 0, 0,
                -Math.Sin(r), 0, Math.Cos(r), 0,
                0,0,0,1
            });
        }

        public static Matrix RotationZ(double r)
        {
            return new Matrix(new List<double> {
                Math.Cos(r), -Math.Sin(r), 0, 0,
                Math.Sin(r), Math.Cos(r), 0, 0,
                0, 0, 1, 0,
                0,0,0,1
            });
        }

        public static Matrix Shearing(
            double xy, double xz,
            double yx, double yz,
            double zx, double zy)
        {
            return new Matrix(new List<double>{
                1, xy, xz, 0,
                yx, 1, yz, 0,
                zx, zy, 1, 0,
                0, 0, 0, 1
            });
        }

        public static Matrix View(Point from, Point to, Vector up)
        {
            var forward = (to - from).Normalize();
            var left = Tuple.Cross(forward, up.Normalize());
            var trueUp = Tuple.Cross(left, forward);
            var orientation = new Matrix(new List<double> {
                left.x, left.y, left.z, 0,
                trueUp.x, trueUp.y, trueUp.z, 0,
                -forward.x, -forward.y, -forward.z, 0,
                0, 0, 0, 1
            });
            return orientation * Transform.Translation(-from.x, -from.y, -from.z);
        }
    }
}
