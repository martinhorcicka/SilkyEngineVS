using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Input.Common;
using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace SilkyEngine.Sources.Controls
{
    public abstract class Controller : Behavior
    {
        private IWindow window;
        protected const float MAX_PITCH = 89f * MathF.PI / 180f;
        protected static Dictionary<Key, bool> isPressed;
        protected static Key[] controlKeys = new Key[] { Key.W, Key.A, Key.S, Key.D, Key.Space, Key.ShiftLeft };
        protected static Dictionary<MouseButton, bool> isMBPressed;
        protected static MouseButton[] mouseButtons = new MouseButton[] { MouseButton.Left, MouseButton.Right };
        protected float movementSpeed, mouseSensitivity;


        public Controller(IWindow window, float movementSpeed = 10f, float mouseSensitivity = 0.002f) : base(window)
        {
            this.window = window;
            isMBPressed = new Dictionary<MouseButton, bool>(mouseButtons.Length);
            foreach (var mb in mouseButtons)
                isMBPressed.Add(mb, false);

            isPressed = new Dictionary<Key, bool>(controlKeys.Length);
            foreach (var k in controlKeys)
                isPressed.Add(k, false);

            var input = window.CreateInput();
            var mouse = input.Mice[0];
            CreateCursor("res/textures/arrow.png", mouse.Cursor);
            mouse.MouseMove += OnMouseMove;
            mouse.MouseDown += OnMouseDown;
            mouse.MouseUp += OnMouseUp;

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
        }
        protected virtual void OnKeyUp(IKeyboard keyboard, Key key, int mode)
        {
            if (controlKeys.Contains(key))
                isPressed[key] = false;
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