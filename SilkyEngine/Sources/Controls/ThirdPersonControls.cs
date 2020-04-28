using System;
using System.Drawing;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Input.Common;
using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Tools;

namespace SilkyEngine.Sources.Controls
{
    public class ThirdPersonControls : Controller, IPlayerController, ICameraController
    {
        private static float angleBetweenVectors(Vector3 v, Vector3 w) => MathF.Acos(Vector3.Dot(v, w) / (v.Length() * w.Length()));
        private Camera camera;
        private Player player;
        private Func<float, float, float> HeightMap;
        private Vector2 prevMousePos;
        private float distance, verticalSpeed;
        private const float gravity = 10f;
        private bool inAir;


        public ThirdPersonControls(IWindow window) : base(window)
        {
            window.CreateInput().Mice[0].Cursor.CursorMode = CursorMode.Normal;
            verticalSpeed = 0;
            inAir = true;
        }

        protected override void OnMouseDown(IMouse mouse, MouseButton button)
        {
            base.OnMouseDown(mouse, button);
            mouse.Cursor.CursorMode = CursorMode.Hidden;
            if (isMBPressed[MouseButton.Right])
                player.SnapToFront(camera.Front);

        }
        protected override void OnMouseUp(IMouse mouse, MouseButton button)
        {
            base.OnMouseUp(mouse, button);
            mouse.Cursor.CursorMode = CursorMode.Normal;
        }
        protected override void OnMouseMove(IMouse mouse, PointF point)
        {
            Vector2 mousePos = new Vector2(point.X, point.Y);
            Vector2 deltaMouse = mousePos - prevMousePos;
            prevMousePos = mousePos;

            if (isMBPressed[MouseButton.Right] || isMBPressed[MouseButton.Left])
            {
                float terraintHeight = HeightMap?.Invoke(camera.Position.X, camera.Position.Z) ?? 0.1f;
                Vector3 dPos = Vector3.Zero;

                float yawChange = deltaMouse.X * mouseSensitivity;
                float pitchChange = deltaMouse.Y * mouseSensitivity;
                if (isMBPressed[MouseButton.Right])
                    player.RotateY(-yawChange);

                camera.ChangeYaw(yawChange);
                if (camera.ChangePitch(-pitchChange)) pitchChange = 0;

                float cy = MathF.Cos(yawChange), sy = MathF.Sin(yawChange);
                float cp = MathF.Cos(pitchChange), sp = MathF.Cos(pitchChange);

                distance = Vector3.Distance(player.Focus, camera.Position);
                Vector3 R = Vector3.Normalize(camera.Position - player.Focus);
                Vector3 orthToRinXZ = Vector3.Normalize(new Vector3(-R.Z, 0, R.X));
                var rot = Matrix4x4.CreateFromAxisAngle(orthToRinXZ, -pitchChange);
                R = Computation.MatMul(rot, R);

                Vector3 nR;
                nR.X = cy * R.X - sy * R.Z;
                nR.Y = R.Y;
                nR.Z = sy * R.X + cy * R.Z;

                dPos = player.Focus + nR * distance - camera.Position;
                camera.Translate(dPos);
            }
        }

        protected override void OnUpdate(double deltaTime)
        {
            float speed = movementSpeed * (float)deltaTime;

            Vector3 dPos = Vector3.Zero;
            if (isPressed[Key.W])
                dPos += player.Front;
            if (isPressed[Key.S])
                dPos -= player.Front;
            if (isPressed[Key.A])
                dPos -= player.Right;
            if (isPressed[Key.D])
                dPos += player.Right;

            player.Translate(dPos * speed);
            camera.Translate(dPos * speed);

            verticalSpeed -= gravity * (float)deltaTime;
            player.Translate(Vector3.UnitY * verticalSpeed * (float)deltaTime);
            camera.Translate(Vector3.UnitY * verticalSpeed * (float)deltaTime);

            float terraintHeight = HeightMap?.Invoke(player.Position.X, player.Position.Z) ?? 0f;
            if (player.Position.Y < terraintHeight)
            {
                verticalSpeed = 0;
                inAir = false;
                float dHeight = camera.Position.Y - player.Position.Y; 
                player.SetHeight(terraintHeight);
                camera.SetHeight(terraintHeight + dHeight);
            }
        }

        protected override void OnKeyDown(IKeyboard keyboard, Key key, int mode)
        {
            base.OnKeyDown(keyboard, key, mode);
            switch (key)
            {
                case Key.Space:
                    if (!inAir) Jump();
                    break;
                default:
                    break;
            }
        }

        private void Jump()
        {
            inAir = true;
            verticalSpeed = player.JumpPower;
        }

        public void SubscribePlayer(Player player) => this.player = player;

        public void SubscribeCamera(Camera camera) => this.camera = camera;

        public void SubscribeHeightMap(Func<float, float, float> HeightMap) => this.HeightMap = HeightMap;
    }
}