using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace rt
{
   	public class Ray
    {
        public Tuple Origin { get; }
        public Tuple Direction { get; }

        public Ray(Tuple origin, Tuple direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Point Position(double t)
        {
            return (Origin + Direction * t).ToPoint();
        }

		public Ray Transform(Matrix m)
		{
			return new Ray(m* Origin, m* Direction);
		}

        public override string ToString()
        {
            return String.Format("Ray({0},{1})", Origin, Direction);
        }
    };
}
