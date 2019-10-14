using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using static rt.Constants;

namespace rt
{
    public class Matrix
    {

        private readonly List<double> items;

        public int Width { get; }
        public int Height { get; }

        public override string ToString()
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine("Matrix");
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x > 0)
                    {
                        writer.Write(" ");
                    }
                    writer.Write(this[x, y]);
                }
                writer.WriteLine();
            }
            return writer.ToString();
        }

        public Matrix (List<double> items)
        {
            this.Width = this.Height = (int)Math.Sqrt(items.Count);
            this.items = new List<double>(items);
            foreach (var item in items.Zip(Enumerable.Range(0, items.Count), (value, index) => new { index, value}))
            {
                int r = item.index % Width;
                int c = item.index / Width;
                this[c, r] = item.value;
            }
        }

        public double this[int column, int row]
        {
            get => items[row * Width + column];
            set { items[row * Width + column] = value; }
        }

        public static Matrix operator *(Matrix a, Matrix b)
		{

            Matrix result = new Matrix (new List<double>(Enumerable.Repeat(0.0, a.Width*a.Height)));
            for (int y = 0; y < a.Height; y++) {
                var r = b.Row(y);
                for (int x = 0; x < a.Width; x++) {
                    var c = a.Column(x);
                    result[x, y] = c.Zip(r, (first, second) => first * second).Sum();
                }
            }
			return result;
		}

        public static Tuple operator *(Matrix matrix, Tuple tuple) 
  		{
            List<double> l = new List<double>(Enumerable.Repeat(0.0, 4));

			for (int y = 0; y<matrix.Height; y++) {
				var r = matrix.Column(y);
                l[y] = r.Zip(tuple.ToList(), (first, second) => first * second).Sum();
    }
			return new Tuple(l);
		}

        public List<double> Row(int row)
        {
            var l = new List<double>(Width);
            for (int x = 0; x < Width; x++)
            {
                l.Add(this[x, row]);
            }
            return l;
        }

        public List<double> Column(int col)
        {
            var l = new List<double>(Height);
            for (int y = 0; y < Height; y++)
            {
                l.Add(this[col, y]);
            }
            return l;
        }

        public static Matrix Identity(int s)
        {
            Matrix m = new Matrix(new List<double>(Enumerable.Repeat(0.0, s * s)));
            for (int i = 0; i < s; i++)
            {
                m[i, i] = 1;
            }
            return m;
        }

        public Matrix Inverse() {

            double d = Determinant();
            if (d == 0) {
                throw new ApplicationException("matrix is not invertable - determinant is 0");
            }
            Matrix m = new Matrix(items);
            var cf = Cofactor().Transpose();
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    m[x,y] = cf[x,y] / d;
                }
            }
            return m;
        }

        public Matrix Transpose() {

            var m = new Matrix(items);
			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
                    m[y, x] = this[x, y];
				}
			}
			return m;
		}

        public double Determinant()
        {
            if (Width == 2)
            {
                return (this[0, 0] * this[1,1]) - (this[1,0] * this[0,1]);
            } else
            {
                double d = 0;
                for (int x = 0; x < Width; x++)
                {
                    d += this[x,0] * Minor(x, 0).Determinant() * ((x % 2) == 1 ? -1 : 1);
                }
                return d;
            }
        }

        public Matrix Cofactor()
        {
            if (Width == 2)
            {

                return new Matrix (new List<double> {
                    items[3],-items[2],
                    -items[1],items[0]});
            } else
            {
                Matrix m = new Matrix(new List<double>(Enumerable.Repeat(0.0, Width * Width)));
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        m[x,y] = Minor(x,y).Determinant() * ((x + y) % 2 == 1? -1 : 1);
                    }
                }
                return m;
            }
        }

        public Matrix Minor(int column, int row)
        {
            int w = Width - 1;
            Matrix m = new Matrix(new List<double>(Enumerable.Repeat(0.0, w * w)));

            int dx = 0;
            for (int sx = 0; sx < Width; sx++) {
                if (sx == column) {
                    continue;
                }
                int dy = 0;
                for (int sy = 0; sy < Height; sy++) {
                    if (sy == row) {
                        continue;
                    }
                    m[dx, dy] = this[sx, sy];
                    dy++;
                }
                dx++;
            }
            return m;
        }

        public override bool Equals(object obj)
        {
            if (obj is Matrix)
            {
                var matrix = obj as Matrix;
                int s = items.Zip(matrix.items, (first, second) => Math.Abs(first - second) <= EPSILON ? 0 : 1).Sum();
                bool listEquals = s == 0;//EqualityComparer<List<double>>.Default.Equals(items, matrix.items);
                bool widthEquals = Width == matrix.Width;
                bool heightEquals = Height == matrix.Height;
                return listEquals && widthEquals && heightEquals;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 1696098808;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<double>>.Default.GetHashCode(items);
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }
    }
}
