using System;
using System.Numerics;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Tools;

namespace SilkyEngine.Sources.Graphics
{
    public class Camera
    {
        private static Func<float, float> sin = MathF.Sin, cos = MathF.Cos;
        private static float MAX_PITCH { get; } = Computation.DegToRad(80f);
        private Vector3 position, front, up = Vector3.UnitY;
        private float pitch, yaw;

        public void SetHeight(float newHeight) => position.Y = newHeight;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector3 Front
        {
            get { return front; }
            set
            {
                front = Vector3.Normalize(value);
                RecalculateEulers();
            }
        }
        public Vector3 Up => up;
        public Vector3 Right
        {
            get { return Vector3.Normalize(Vector3.Cross(front, up)); }
        }

        public Camera(ICameraController controls, Vector3 position, Vector3 target)
        {
            this.position = position;
            Front = target - position;
            controls.SubscribeCamera(this);

            RecalculateEulers();
        }

        public void UpdateView(Shader shader)
        {
            Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(position, position + front, up);
            shader.UpdateUniform("view", viewMatrix);
            shader.UpdateUniform("viewPos", position);
        }

        private void RecalculateVectors()
        {
            float sy = sin(yaw), cy = cos(yaw), sp = sin(pitch), cp = cos(pitch);
            front.X = cy * cp;
            front.Y = sp;
            front.Z = sy * cp;
        }

        private void RecalculateEulers()
        {
            Vector3 frontInXZ = front;
            frontInXZ.Y = 0;
            yaw = Computation.AngleBetweenVectors(Vector3.UnitX, frontInXZ);
            if (MathF.Abs(yaw - MathF.PI) > Computation.DegToRad(1f))
                yaw *= MathF.Sign(Vector3.Dot(Vector3.UnitZ, frontInXZ));

            pitch = Computation.AngleBetweenVectors(front, frontInXZ);
            pitch *= MathF.Sign(Vector3.Dot(front, up));
        }

        public void Translate(Vector3 dp) => position += dp;
        public void ChangeYaw(float angle)
        {
            yaw += angle;
            RecalculateVectors();
        }
        public bool ChangePitch(float angle)
        {
            bool isConstrained = false;
            pitch += angle;
            if (MathF.Abs(pitch) > MAX_PITCH)
            {
                isConstrained = true;
                pitch = MathF.Sign(pitch) * MAX_PITCH;
            }

            RecalculateVectors();
            return isConstrained;
        }
    }
}