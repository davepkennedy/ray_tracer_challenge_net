using System;
using System.Collections.Generic;
using System.Text;

namespace rt
{
    public class PointLight
    {
        public PointLight(Tuple position, Color color)
        {
            Position = position;
            Intensity = color;

        }

        public Tuple Position { get; }

        public Color Intensity { get; }

        public override bool Equals(object obj)
        {
            return obj is PointLight light &&
                   EqualityComparer<Tuple>.Default.Equals(Position, light.Position) &&
                   EqualityComparer<Color>.Default.Equals(Intensity, light.Intensity);
        }

        public override int GetHashCode()
        {
            var hashCode = 978863716;
            hashCode = hashCode * -1521134295 + EqualityComparer<Tuple>.Default.GetHashCode(Position);
            hashCode = hashCode * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(Intensity);
            return hashCode;
        }
    };

    public class Light
    {
        public static Color Lighting(Material mat, Shape shape, PointLight light,
            Point pt, Tuple eye, Tuple normal, bool inShadow)
        {
            Color diffuse = new Color(0, 0, 0);
            Color specular = new Color(0, 0, 0);

            Color color;
            if (mat.Pattern != null)
            {
                color = mat.Pattern.PatternAtObject(shape,pt);
            } else
            {
                color = mat.Color;
            }

            var effectiveColor = color * light.Intensity;
            var lightv = (light.Position - pt).Normalize();
            var ambient = effectiveColor * mat.Ambient;
            if (inShadow)
            {
                return ambient;
            }
            var lightDotNormal = Tuple.Dot(lightv, normal);
            if (lightDotNormal >= 0)
            {
                diffuse = effectiveColor * mat.Diffuse * lightDotNormal;
                var reflectv = (-lightv).ReflectOn(normal);
                var reflectDotEye = Tuple.Dot(reflectv, eye);

                if (reflectDotEye > 0)
                {
                    var factor = Math.Pow(reflectDotEye, mat.Shininess);
                    specular = light.Intensity * mat.Specular * factor;
                }
            }
            return ambient + diffuse + specular;
        }
    }
}
