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
        private Camera camera;
        private Player player;
        private const float MIN_DISTANCE = 1;
        private const float MAX_DISTANCE = 25;
        private const float DEFAULT_HEIGHT = 5;
        private Vector2 prevMousePos;
        private float distance;

        public ThirdPersonControls(IWindow window) : base(window)
        {
            distance = Computation.Average(MIN_DISTANCE, MAX_DISTANCE);
        }

        protected override void OnMouseDown(IMouse mouse, MouseButton button)
        {
            base.OnMouseDown(mouse, button);
            mouse.Cursor.CursorMode = CursorMode.Disabled;
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

                float yawChange = deltaMouse.X * mouseSensitivity;
                float pitchChange = deltaMouse.Y * mouseSensitivity;
                if (isMBPressed[MouseButton.Right])
                    player.RotateY(-yawChange);

                RecalculateCamera(yawChange, pitchChange);
            }
        }

        private void RecalculateCamera(float yawChange, float pitchChange)
        {
            camera.ChangeYaw(yawChange);
            if (camera.ChangePitch(-pitchChange)) pitchChange = 0;

            Vector3 R = Vector3.Normalize(camera.Position - player.Focus);
            Vector3 orthToRinXZ = Vector3.Normalize(new Vector3(R.Z, 0, -R.X));
            var rot = Matrix4x4.CreateFromAxisAngle(orthToRinXZ, pitchChange);
            var rotY = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, yawChange);
            R = Computation.MatMul(rotY * rot, R);

            camera.Position = player.Focus + R * distance;
        }

        public override void OnUpdate(double deltaTime)
        {
            Vector3 dPos = Vector3.Zero;
            if (isPressed[Key.W])
                dPos += player.Front;
            if (isPressed[Key.S])
                dPos -= player.Front;
            if (isPressed[Key.A])
                dPos -= player.Right;
            if (isPressed[Key.D])
                dPos += player.Right;
            dPos = (dPos != Vector3.Zero) ? Vector3.Normalize(dPos) : Vector3.Zero;

            float distance = player.MovementSpeed * (float)deltaTime;
            player.DeltaPosition += dPos * distance;
        }

        protected override void OnKeyDown(IKeyboard keyboard, Key key, int mode)
        {
            base.OnKeyDown(keyboard, key, mode);
            switch (key)
            {
                case Key.Space:
                    player.Jump();
                    break;
                default:
                    break;
            }
        }

        protected override void OnScroll(IMouse mouse, ScrollWheel wheel)
        {
            distance -= wheel.Y;
            if (distance < MIN_DISTANCE) distance = MIN_DISTANCE;
            else if (distance > MAX_DISTANCE) distance = MAX_DISTANCE;
            RecalculateCamera(0, 0);
        }

        private void OnPlayerMove(Vector3 dPos)
        {
            camera.Translate(dPos);
        }

        public void SubscribePlayer(Player player)
        {
            this.player = player;
            this.player.Moved += OnPlayerMove;
        }

        public void SubscribeCamera(Camera camera)
        {
            this.camera = camera;
            float xzDistance = MathF.Sqrt(distance * distance - DEFAULT_HEIGHT * DEFAULT_HEIGHT);
            Vector3 relCamPos = xzDistance * (-player.Front) + DEFAULT_HEIGHT * Vector3.UnitY;
            camera.Position = relCamPos + player.Focus;
            camera.Front = player.Focus - camera.Position;
        }
    }
}