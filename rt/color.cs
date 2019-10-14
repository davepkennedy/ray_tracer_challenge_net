using System;
using System.Collections.Generic;
using System.Text;

using static rt.Constants;

namespace rt
{
    public class Color
    {

        public static Color BLACK = new Color(0, 0, 0);
        public static Color WHITE = new Color(1, 1, 1);
        public Color (double red, double green, double blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public double Red { get; }

        public double Green { get; }

        public double Blue { get; }

        public override bool Equals(object obj)
        {
            return obj is Color color &&
                   Math.Abs(Red - color.Red) <= EPSILON &&
                   Math.Abs(Green - color.Green) <= EPSILON &&
                   Math.Abs(Blue - color.Blue) <= EPSILON;
        }

        public override int GetHashCode()
        {
            var hashCode = -1058441243;
            hashCode = hashCode * -1521134295 + Red.GetHashCode();
            hashCode = hashCode * -1521134295 + Green.GetHashCode();
            hashCode = hashCode * -1521134295 + Blue.GetHashCode();
            return hashCode;
        }

        public override string ToString() => $"Color({Red},{Blue},{Green})";

        public static Color operator+ (Color a, Color b)
        {
            return new Color(a.Red + b.Red, a.Green + b.Green, a.Blue + b.Blue);
        }
        public static Color operator- (Color a, Color b)
        {
            return new Color(a.Red - b.Red, a.Green - b.Green, a.Blue - b.Blue);
        }
        public static Color operator *(Color c, double f)
        {
            return new Color(c.Red * f, c.Green * f, c.Blue * f);
        }
        public static Color operator *(Color a, Color b)
        {
            return new Color(a.Red * b.Red, a.Green * b.Green, a.Blue * b.Blue);
        }
        public static Color operator /(Color c, double f)
        {
            return new Color(c.Red / f, c.Green / f, c.Blue / f);
        }


    }
}
