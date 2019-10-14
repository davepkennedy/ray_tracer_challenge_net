using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rt
{
    public class Canvas
    {
        private List<Color> pixels;
        public Canvas (int width, int height)
        {
            Width = width;
            Height = height;

            pixels = new List<Color>(width * height);
            pixels.AddRange(Enumerable.Repeat(new Color(0, 0, 0), width * height));
        }

        public int Width { get; }

        public int Height { get; }

        public Color this[int x, int y]
        {
            get
            {
                return pixels[Width * y + x];
            }
            set
            {
                pixels[Width * y + x] = value;
            }
        }

        public List<Color>.Enumerator GetEnumerator() => pixels.GetEnumerator();

        private static int clamp(double f)
        {
            if (f < 0) { return 0; }
            if (f > 255) { return 255; }
            return (int)(Math.Ceiling(f));
        }

        private static String ToString(Color c)
        {
            return String.Format("{0} {1} {2}", clamp(255 * c.Red), clamp(255 * c.Green), clamp(255 * c.Blue));
        }
        public string toPpm() {

            StringWriter buf = new StringWriter();
            buf.WriteLine("P3");
            buf.WriteLine(String.Format("{0} {1}", Width, Height));
			buf.WriteLine(255);

			for (int y = 0; y<Height; y++) {
				for (int x = 0; x<Width; x++) {
					if (x>0) { buf.Write(" "); }
                    buf.Write(ToString(this[x, y]));
				}
                buf.WriteLine();
			}

            return buf.ToString();
		}
    }
}
