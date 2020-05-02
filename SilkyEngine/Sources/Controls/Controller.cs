using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Silk.NET.Input;
using Silk.NET.Input.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Behaviors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;


namespace SilkyEngine.Sources.Controls
{
    public abstract class Controller : Behavior
    {
        private IWindow window;
        protected const float MAX_PITCH = 89f * MathF.PI / 180f;
        protected static Key[] controlKeys = new Key[] { Key.W, Key.A, Key.S, Key.D, Key.Space, Key.ShiftLeft };
        protected static MouseButton[] mouseButtons = new MouseButton[] { MouseButton.Left, MouseButton.Right };
        protected static Dictionary<Key, bool> isPressed = new Dictionary<Key, bool>(controlKeys.Length);
        protected static Dictionary<MouseButton, bool> isMBPressed = new Dictionary<MouseButton, bool>(mouseButtons.Length);
        protected float movementSpeed, mouseSensitivity;

        public Controller(IWindow window, float movementSpeed = 10f, float mouseSensitivity = 0.002f) : base(window)
        {
            this.window = window;

            foreach (var mb in mouseButtons)
                isMBPressed.Add(mb, false);

            foreach (var k in controlKeys)
                isPressed.Add(k, false);

            var input = window.CreateInput();
            var mouse = input.Mice[0];
            CreateCursor("Resources/Textures/arrow.png", mouse.Cursor);
            mouse.MouseMove += OnMouseMove;
            mouse.MouseDown += OnMouseDown;
            mouse.MouseUp += OnMouseUp;
            mouse.Scroll += OnScroll;

            foreach (var kb in input.Keyboards)
            {
                kb.KeyDown += OnKeyDown;
                kb.KeyUp += OnKeyUp;
            }

            this.movementSpeed = movementSpeed;
            this.mouseSensitivity = mouseSensitivity;
        }

        protected override abstract void OnUpdate(double deltaTime);
        protected abstract void OnMouseMove(IMouse mouse, PointF point);
        protected abstract void OnScroll(IMouse mouse, ScrollWheel wheel);
        protected virtual void OnMouseDown(IMouse mouse, MouseButton button)
        {
            if (mouseButtons.Contains(button))
                isMBPressed[button] = true;
        }

        protected virtual void OnMouseUp(IMouse mouse, MouseButton button)
        {
            if (mouseButtons.Contains(button))
                isMBPressed[button] = false;
        }

        protected virtual void OnKeyDown(IKeyboard keyboard, Key key, int mode)
        {
            if (controlKeys.Contains(key))
                isPressed[key] = true;

            switch (key)
            {
                case Key.Escape:
                    window.Close();
                    break;
            }
        }

        protected virtual void OnKeyUp(IKeyboard keyboard, Key key, int mode)
        {
            if (controlKeys.Contains(key))
                isPressed[key] = false;

            GL gl;
            switch (key)
            {
                case Key.F11:
                    window.WindowState = (window.WindowState == WindowState.Normal) ? WindowState.Fullscreen : WindowState.Normal;
                    break;
                case Key.M:
                    window.CreateInput().Mice[0].Cursor.CursorMode = (window.CreateInput().Mice[0].Cursor.CursorMode == CursorMode.Disabled) ? CursorMode.Normal : CursorMode.Disabled;
                    break;
                case Key.V:
                    window.VSync = (window.VSync == VSyncMode.Off) ? VSyncMode.On : VSyncMode.Off;
                    break;
                case Key.P:
                    gl = GL.GetApi();
                    gl.PolygonMode(GLEnum.FrontAndBack, PolygonMode.Fill);
                    break;
                case Key.L:
                    gl = GL.GetApi();
                    gl.PolygonMode(GLEnum.FrontAndBack, PolygonMode.Line);
                    break;
            }
        }

        private void CreateCursor(string cursorIconPath, ICursor cursor)
        {
            try
            {
                Image<Rgba32> cursorImg = Image.Load<Rgba32>(cursorIconPath);
                cursor.Type = CursorType.Custom;
                cursor.Width = cursorImg.Width;
                cursor.Height = cursorImg.Height;
                byte[] pixels = new byte[cursor.Width * cursor.Height * 4];
                var rgbaSpan = cursorImg.GetPixelSpan();
                for (int i = 0; i < rgbaSpan.Length; i++)
                {
                    pixels[4 * i + 0] = rgbaSpan[i].R;
                    pixels[4 * i + 1] = rgbaSpan[i].G;
                    pixels[4 * i + 2] = rgbaSpan[i].B;
                    pixels[4 * i + 3] = rgbaSpan[i].A;
                }
                cursor.Pixels = pixels;
            }
            catch
            {
                cursor.Type = CursorType.Standard;
            }
        }
    }
}