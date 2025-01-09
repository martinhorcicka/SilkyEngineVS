using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using SilkyEngine.Sources.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SilkyEngine.Sources.Graphics
{
    public class Texture : IBindable
    {
        private uint handle;
        private GL gl;
        public unsafe Texture(GL gl, string name)
        {
            handle = 0;
            this.gl = gl;
            Image<Rgba32> img = Image.Load<Rgba32>("Resources/Textures/" + name);
            img.Mutate(x => x.Flip(FlipMode.Vertical));

            byte[] bytes = new byte[4 * img.Width * img.Height];
            Span<byte> pixels = new Span<byte>(bytes);
            img.CopyPixelDataTo(pixels);
            fixed (void* data = &MemoryMarshal.GetReference(pixels))
            {
                Load(data, (uint)img.Width, (uint)img.Height);
            }
            img.Dispose();
        }

        public void Bind()
        {
            gl.BindTexture(TextureTarget.Texture2D, handle);
        }
        public void Unbind()
        {
            gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        private unsafe void Load(void* data, uint width, uint height)
        {
            handle = gl.GenTexture();
            Bind();

            gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            gl.GenerateMipmap(TextureTarget.Texture2D);
        }
    }
}