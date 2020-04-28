using System;
using System.Numerics;
using SilkyEngine.Sources.Interfaces;

namespace SilkyEngine.Sources.Graphics
{
    public class Camera
    {
        private static float degToRad(float deg) => deg * MathF.PI / 180f;
        private static float radToDeg(float rad) => rad / MathF.PI * 180f;
        private static Func<float, float> sin = MathF.Sin, cos = MathF.Cos;
        private static float angleBetweenVectors(Vector3 v, Vector3 w) => MathF.Acos(Vector3.Dot(v, w) / (v.Length() * w.Length()));
        private static float MAX_PITCH { get; } = degToRad(89f);
        private Vector3 position, front, right, up = Vector3.UnitY;
        private float pitch, yaw, distance;

        public void SetHeight(float newHeight) => position.Y = newHeight;
        public Vector3 Position => position;
        public Vector3 Front => front;
        public Vector3 Up => up;
        public Vector3 Right => right;
        
        public Camera(ICameraController controls, Vector3 position, Vector3 target)
        {
            controls.SubscribeCamera(this);
            this.position = position;
            var tmp = target - position;
            distance = tmp.Length();
            this.front = Vector3.Normalize(tmp);
            right = Vector3.Normalize(Vector3.Cross(front, up));

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
            right = Vector3.Normalize(Vector3.Cross(front, up));
        }

        private void RecalculateEulers()
        {
            Vector3 frontInXZ = front;
            frontInXZ.Y = 0;
            yaw = angleBetweenVectors(Vector3.UnitX, frontInXZ);
            if (MathF.Abs(yaw - MathF.PI) > degToRad(1f))
                yaw *= MathF.Sign(Vector3.Dot(Vector3.UnitZ, frontInXZ));

            pitch = angleBetweenVectors(front, frontInXZ);
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