using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace rt
{
    public interface Progress
    {
        int Max { get; set; }
        void Progress();
    }
    public class Camera
    {

        public Progress Progress { get; set; }

        public Camera(int hsize, int vsize, double fieldOfView)
        {
            Hsize = hsize;
            Vsize = vsize;
            FieldOfView = fieldOfView;
            Transform = Matrix.Identity(4);

            var half_view = Math.Tan(FieldOfView / 2);
            var aspect = hsize / (double)vsize;

            if (aspect >= 1)
            {
                HalfWidth = half_view;
                HalfHeight = half_view / aspect;
            } else
            {
                HalfWidth = half_view * aspect;
                HalfHeight = half_view;
            }

            PixelSize = (HalfWidth * 2) / Hsize;
        }

        public int Hsize { get; }

        public int Vsize { get; }

        public double FieldOfView { get; }

        public Matrix Transform { get; set; }

        public double PixelSize { get; }

        public double HalfWidth { get; }
        public double HalfHeight { get; }

        public Ray RayForPixel(double px, double py)
        {
            var xoffset = (px + 0.5) * PixelSize;
            var yoffset = (py + 0.5) * PixelSize;

            var worldX = HalfWidth - xoffset;
            var worldY = HalfHeight - yoffset;

            var pixel = Transform.Inverse() * new Point(worldX, worldY, -1);
            var origin = Transform.Inverse() * new Point(0, 0, 0);
            var direction = (pixel - origin).Normalize();

            return new Ray(origin, direction);
        }

        public Canvas Render (World world)
        {
            var image = new Canvas(Hsize, Vsize);
            /**
            for (int y = 0; y < Vsize-1; y++)
            {
                for (int x = 0; x < Hsize - 1; x++) {
                    var ray = RayForPixel(x, y);
                    var color = world.ColorAt(ray,5);
                    image[x, y] = color;
                }
            }
            /**/
            /**/
            if (Progress != null)
            {
                Progress.Max = Hsize * Vsize;
            }
            Parallel.For(0, Vsize - 1, delegate (int y) {
                for (int x = 0; x < Hsize - 1; x++)
                {
                    var ray = RayForPixel(x, y);
                    var color = world.ColorAt(ray, 5);
                    image[x, y] = color;
                    if (Progress != null)
                    {
                        Progress.Progress();
                    }
                }
            });
            /**/

            return image;
        }
    }
}
