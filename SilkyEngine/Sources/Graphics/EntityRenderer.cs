using System.Numerics;
using Silk.NET.OpenGL;
using SilkyEngine.Sources.Entities;

namespace SilkyEngine.Sources.Graphics
{
    public static class EntityRenderer
    {
        public static void Draw(GL gl, Entity entity, Shader shader)
        {
            var modelMatrix = MakeModelMatrix(entity);
            shader.UpdateUniform("model", modelMatrix);
            Matrix4x4.Invert(modelMatrix, out modelMatrix);
            modelMatrix = Matrix4x4.Transpose(modelMatrix);
            shader.UpdateUniform("itModel", modelMatrix);
            gl.DrawArrays(GLEnum.Triangles, 0, entity.TexturedModel.VertCount);
        }

        private static Matrix4x4 MakeModelMatrix(Entity entity)
        {
            Matrix4x4 retMat = Matrix4x4.CreateScale(entity.Scale);
            retMat *= Matrix4x4.CreateRotationX(entity.Rotation.X);
            retMat *= Matrix4x4.CreateRotationY(entity.Rotation.Y);
            retMat *= Matrix4x4.CreateRotationZ(entity.Rotation.Z);
            retMat *= Matrix4x4.CreateTranslation(entity.Position);
            return retMat;
        }
    }
}