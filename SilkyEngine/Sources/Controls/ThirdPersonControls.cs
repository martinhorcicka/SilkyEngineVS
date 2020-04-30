using System;
using System.Drawing;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Input.Common;
using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Physics;
using SilkyEngine.Sources.Tools;

namespace SilkyEngine.Sources.Controls
{
    public class ThirdPersonControls : Controller, IPlayerController, ICameraController
    {
        private Camera camera;
        private Player player;
        private World world;
        private Func<float, float, float> HeightMap;
        private const float MIN_DISTANCE = 1;
        private const float MAX_DISTANCE = 25;
        private const float DEFAULT_HEIGHT = 5;
        private Vector2 prevMousePos;
        private float distance, verticalSpeed;
        private const float gravity = 10f;
        private bool inAir;


        public ThirdPersonControls(IWindow window) : base(window)
        {
            window.CreateInput().Mice[0].Cursor.CursorMode = CursorMode.Normal;
            verticalSpeed = 0;
            distance = Computation.Average(MIN_DISTANCE, MAX_DISTANCE);
            inAir = true;
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
            //float terraintHeight = HeightMap?.Invoke(camera.Position.X, camera.Position.Z) ?? 0.0f;
            //if (camera.Position.Y <= terraintHeight)
            //    camera.SetHeight(DEFAULT_HEIGHT + terraintHeight + player.Focus.Y);
            //else
            //    camera.SetHeight(DEFAULT_HEIGHT + player.Focus.Y);
            camera.ChangeYaw(yawChange);
            if (camera.ChangePitch(-pitchChange)) pitchChange = 0;
            //pitchChange = 0;

            Vector3 R = Vector3.Normalize(camera.Position - player.Focus);
            Vector3 orthToRinXZ = Vector3.Normalize(new Vector3(R.Z, 0, -R.X));
            var rot = Matrix4x4.CreateFromAxisAngle(orthToRinXZ, pitchChange);
            var rotY = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, yawChange);
            R = Computation.MatMul(rotY * rot, R);

            camera.Position = player.Focus + R * distance;
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

            if (world.IsWalkable(player.Position + dPos))
            {
                player.Translate(dPos * speed);
                camera.Translate(dPos * speed);
            }

            verticalSpeed -= gravity * (float)deltaTime;
            player.Translate(Vector3.UnitY * verticalSpeed * (float)deltaTime);
            camera.Translate(Vector3.UnitY * verticalSpeed * (float)deltaTime);

            float terraintHeight = HeightMap?.Invoke(player.Position.X, player.Position.Z) ?? 0f;
            if (player.Position.Y < terraintHeight)
            {
                verticalSpeed = 0;
                float dHeight = camera.Position.Y - player.Position.Y;
                player.SetHeight(terraintHeight);
                camera.SetHeight(terraintHeight + dHeight);
            }
            if (MathF.Abs(verticalSpeed) < 0.1f)
                inAir = false;
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
        protected override void OnScroll(IMouse mouse, ScrollWheel wheel)
        {
            distance -= wheel.Y;
            if (distance < MIN_DISTANCE) distance = MIN_DISTANCE;
            else if (distance > MAX_DISTANCE) distance = MAX_DISTANCE;
            RecalculateCamera(0, 0);
        }

        public void OnCollision(CollisionEventArgs eventArgs)
        {
            if (eventArgs.Unwrap(out Player p, out Entity e))
            {
                float speed = movementSpeed * (float)eventArgs.DeltaTime;

                Vector3 R = Vector3.Normalize(p.Position - e.Position);
                R = ToBoxNormal(R);
                if (R == Vector3.UnitY)
                {
                    verticalSpeed += gravity * (float)eventArgs.DeltaTime;
                    player.Translate(Vector3.UnitY * verticalSpeed * (float)eventArgs.DeltaTime);
                    camera.Translate(Vector3.UnitY * verticalSpeed * (float)eventArgs.DeltaTime);
                }

                p.Translate(R * speed);
                camera.Translate(R * speed);
            }
        }

        private Vector3 ToBoxNormal(Vector3 vec)
        {
            Func<float, float> abs = MathF.Abs;
            Func<float, int> sgn = MathF.Sign;
            int indexMax = 0;
            if (abs(vec.X) < abs(vec.Y))
            {
                indexMax = 1;
                if (abs(vec.Y) < abs(vec.Z))
                    indexMax = 2;
            }
            else if (abs(vec.X) < abs(vec.Z))
                indexMax = 2;


            if (indexMax == 0) return sgn(vec.X) * Vector3.UnitX;
            if (indexMax == 1) return sgn(vec.Y) * Vector3.UnitY;
            if (indexMax == 2) return sgn(vec.Z) * Vector3.UnitZ;

            return Vector3.UnitY;
        }

        private void Jump()
        {
            inAir = true;
            verticalSpeed = player.JumpPower;
        }

        public void SubscribePlayer(Player player) => this.player = player;

        public void SubscribeCamera(Camera camera)
        {
            this.camera = camera;
            float xzDistance = MathF.Sqrt(distance * distance - DEFAULT_HEIGHT * DEFAULT_HEIGHT);
            Vector3 relCamPos = xzDistance * (-player.Front) + DEFAULT_HEIGHT * Vector3.UnitY;
            camera.Position = relCamPos + player.Focus;
            camera.Front = player.Focus - camera.Position;
        }

        public void SubscribeWorld(World world) => this.world = world;

        public void SubscribeHeightMap(Func<float, float, float> HeightMap) => this.HeightMap = HeightMap;
    }
}