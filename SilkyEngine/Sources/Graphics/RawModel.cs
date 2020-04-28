
using System;
using Silk.NET.OpenGL;
using SilkyEngine.Sources.Interfaces;

namespace SilkyEngine.Sources.Graphics
{
    public class RawModel : IBindable, IDisposable
    {
        private uint VAO;
        public int VertexCount { get; }
        private GL gl;

        public RawModel(GL gl, uint vAO, int vertexCount)
        {
            VAO = vAO;
            VertexCount = vertexCount;
            this.gl = gl;
        }

        public void Bind()
        {
            gl.BindVertexArray(VAO);
        }
        public void Unbind()
        {
            gl.BindVertexArray(0);
        }

        public void Dispose()
        {
            gl.DeleteVertexArray(VAO);
        }
    }
}