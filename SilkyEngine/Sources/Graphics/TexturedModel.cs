using Silk.NET.OpenGL;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Physics.Collisions;
using System.Numerics;

namespace SilkyEngine.Sources.Graphics
{
    public class TexturedModel : IBindable
    {
        private RawModel rawModel;
        private Texture texture;
        public uint VertCount => (uint)rawModel.VertexCount;
        public Vector3 Center => rawModel.Center;
     
        public TexturedModel (RawModel rawModel, Texture texture)
        {
            this.rawModel = rawModel;
            this.texture = texture;
        }


        public void Bind()
        {
            rawModel.Bind();
            texture.Bind();
        }

        public void Unbind()
        {
            rawModel.Unbind();
            texture.Unbind();
        }
    }
}