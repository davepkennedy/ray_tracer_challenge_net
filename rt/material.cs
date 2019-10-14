using System.Collections.Generic;

namespace rt
{
    public class Material
    {
        public Material()
        {
            Color = new Color(1, 1, 1);
            Ambient = 0.1;
            Diffuse = 0.9;
            Specular = 0.9;
            Shininess = 200.0;
            Reflective = 0.0;
            Transparency = 0;
            RefractiveIndex = 1;
        }

        public Color Color { get; set; }
        public double Ambient { get; set; }
        public double Diffuse { get; set; }
        public double Specular { get; set; }
        public double Shininess { get; set; }
        public Pattern Pattern { get; set; }
        public double Reflective { get; set; }
        public double Transparency { get; set; }
        public double RefractiveIndex { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Material material &&
                   EqualityComparer<Color>.Default.Equals(Color, material.Color) &&
                   Ambient == material.Ambient &&
                   Diffuse == material.Diffuse &&
                   Specular == material.Specular &&
                   Shininess == material.Shininess;
        }

        public override int GetHashCode()
        {
            var hashCode = -1214519685;
            hashCode = hashCode * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(Color);
            hashCode = hashCode * -1521134295 + Ambient.GetHashCode();
            hashCode = hashCode * -1521134295 + Diffuse.GetHashCode();
            hashCode = hashCode * -1521134295 + Specular.GetHashCode();
            hashCode = hashCode * -1521134295 + Shininess.GetHashCode();
            return hashCode;
        }
    }
}
