using System.Drawing;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SilkyEngine.Sources.Graphics
{
    public class HeightMap
    {
        private ImgData imgData;
        private RectangleF area;
        float minHeight, maxHeight;
        public HeightMap(string heightMapName, RectangleF area, float minHeight = -1, float maxHeight = 1)
        {
            this.area = area;
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
            Image<Rgba32> img = Image.Load<Rgba32>("Resources/Textures/" + heightMapName);
            img.Mutate(x => x.Flip(FlipMode.Vertical));

            var data = new byte[img.Width, img.Height];
            imgData = new ImgData(data);
            var pixelSpan = img.GetPixelSpan();
            for (int i = 0; i < pixelSpan.Length; i++)
            {
                imgData[i % img.Width, i / img.Width] = pixelSpan[i].R;
            }
            img.Dispose();
        }

        public float GetHeight(float x, float y)
        {
            ImgCoords(ref x, ref y);
            return BLerp(x, y);
        }

        private float BLerp(float x, float y)
        {
            int i = (int)x, j = (int)y;

            float leftDown = Rescale(imgData[i, j]);
            float rightDown = Rescale(imgData[i + 1, j]);
            float leftUp = Rescale(imgData[i, j + 1]);
            float rightUp = Rescale(imgData[i + 1, j + 1]);

            Vector2 yy = new Vector2(j + 1 - y, y - j);
            Vector2 tmp = new Vector2(leftDown * yy.X + leftUp * yy.Y, rightDown * yy.X + rightUp * yy.Y);
            Vector2 xx = new Vector2(i + 1 - x, x - i);
            return Vector2.Dot(xx, tmp);
        }

        private float Carp(float x, float y) => Rescale(imgData[(int)x, (int)y]);

        private float Rescale(byte heightByte) => (maxHeight - minHeight) * (heightByte / 256f) - minHeight;

        private void ImgCoords(ref float x, ref float y)
        {
            x -= area.Location.X;
            x /= area.Width;
            x *= imgData.Width;

            y -= area.Location.Y;
            y /= area.Height;
            y *= imgData.Height;
        }
    }
}