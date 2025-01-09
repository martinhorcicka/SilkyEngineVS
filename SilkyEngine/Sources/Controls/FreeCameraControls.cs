using System;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Windowing;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Interfaces;

namespace SilkyEngine.Sources.Controls
{
    public class FreeCameraControls : Controller, ICameraController
    {
        private Vector2 prevMousePos;
        private Camera camera;
        private float movementSpeed;

        public FreeCameraControls(IWindow window, float movementSpeed = 10f) : base(window)
        {
            window.CreateInput().Mice[0].Cursor.CursorMode = CursorMode.Disabled;
            this.movementSpeed = movementSpeed;
        }

        public void SubscribeCamera(Camera camera) => this.camera = camera;

        protected override void OnMouseMove(IMouse mouse, Vector2 point)
        {
            Vector2 mousePos = new Vector2(point.X, point.Y);
            Vector2 deltaMouse = mousePos - prevMousePos;
            prevMousePos = mousePos;

            camera.ChangeYaw(deltaMouse.X * mouseSensitivity);
            camera.ChangePitch(-deltaMouse.Y * mouseSensitivity);
        }

        protected override void OnScroll(IMouse mouse, ScrollWheel wheel)
        {
            throw new NotImplementedException();
        }

        public override void OnUpdate(double deltaTime)
        {
            float speed = movementSpeed * (float)deltaTime;
            Vector3 dPos = Vector3.Zero;
            if (isPressed[Key.W])
                dPos += camera.Front;
            if (isPressed[Key.S])
                dPos -= camera.Front;
            if (isPressed[Key.A])
                dPos -= camera.Right;
            if (isPressed[Key.D])
                dPos += camera.Right;
            if (isPressed[Key.Space])
                dPos += camera.Up;
            if (isPressed[Key.ShiftLeft])
                dPos -= camera.Up;

            camera.Translate(dPos * speed);
        }
    }
}