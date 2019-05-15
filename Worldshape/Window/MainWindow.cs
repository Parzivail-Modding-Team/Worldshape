using System;
using System.ComponentModel;
using System.Drawing;
using MinecraftStructureLib.Core;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Worldshape.Configuration;
using Worldshape.Graphics;
using Worldshape.Logging;

namespace Worldshape.Window
{
    public class MainWindow : GameWindow
    {
        private readonly string[] _args;
        private KeyboardState _keyboard;
        private RenderEngine _renderEngine;
        private MappingEngine _mappingEngine;
        private Camera _camera;
        private Structure _structure;

        private Point _lastMousePos;

        public MainWindow(string[] args) : base(960, 540, new GraphicsMode(32, 24, 0, 8))
        {
            _args = args;
            Load += OnLoad;
            Closing += OnClose;
            Resize += OnResize;
            UpdateFrame += OnUpdate;
            RenderFrame += OnRender;
            MouseWheel += OnMouseWheel;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseDown += OnMouseDown;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            Lumberjack.Debug("Loading window state");
            // Set up lighting
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.ColorMaterial);

            // Set up caps
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.RescaleNormal);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.CullFace);

            // Set up blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Set background color
            GL.ClearColor(Color.FromArgb(255, 13, 13, 13));

            // Init keyboard to ensure first frame won't NPE
            _keyboard = Keyboard.GetState();

            Lumberjack.Debug("Loading render engine");
            _mappingEngine = new MappingEngine();
            _renderEngine = new RenderEngine(this, _mappingEngine);

            _camera = new Camera();

            Lumberjack.Debug("Loading world");
            _structure = StructureLoader.Load(_args[0]);
            _renderEngine.LoadStructure(_structure);
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            _renderEngine.Kill();
        }

        private void OnResize(object sender, EventArgs e)
        {
            GL.Viewport(ClientRectangle);
            _renderEngine.OnResize();
        }

        private void OnUpdate(object sender, FrameEventArgs e)
        {
            Title = $"{RenderFrequency} FPS";

            // Grab the new keyboard state
            _keyboard = Keyboard.GetState();

            // Compute input-based rotations
            var delta = (float)e.Time;
            var speed = _keyboard[Key.LShift] || _keyboard[Key.RShift] ? 40 : 10;

            if (Focused)
            {
                if (_keyboard[Key.W])
                    _camera.Move(Vector3.UnitZ * delta, speed);
                if (_keyboard[Key.S])
                    _camera.Move(-Vector3.UnitZ * delta, speed);
                if (_keyboard[Key.A])
                    _camera.Move(Vector3.UnitX * delta, speed);
                if (_keyboard[Key.D])
                    _camera.Move(-Vector3.UnitX * delta, speed);

                if (_keyboard[Key.Escape])
                    Exit();
            }

            _renderEngine.Update();

            var mouse = Mouse.GetState();
            if (mouse.IsButtonDown(MouseButton.Left) && !CursorVisible)
            {
                var center = new Point(Width / 2, Height / 2);

                // Calculate the offset of the mouse position
                var deltaX = (mouse.X - _lastMousePos.X) * e.Time * 50;
                var deltaY = (mouse.Y - _lastMousePos.Y) * e.Time * 50;

                _lastMousePos = new Point(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Rotation.X += (float)deltaX;
                _camera.Rotation.Y += (float)deltaY; // reversed since y-coordinates range from bottom to top
                
                center = PointToScreen(center);
                Mouse.SetPosition(center.X, center.Y);
            }
        }

        private void OnRender(object sender, FrameEventArgs e)
        {
            // Reset the view
            GL.Clear(ClearBufferMask.ColorBufferBit |
                     ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);

            // Reload the projection matrix
            var aspectRatio = Width / (float) Height;
            var mProjection = Matrix4.CreatePerspectiveFieldOfView((float)(_camera.FieldOfView / 180 * Math.PI), aspectRatio, 1, 1024);
            var mModel = Matrix4.Identity; //Matrix4.CreateTranslation(-_structure.Width / 2f, -_structure.Height / 2f, -_structure.Length / 2f);
            var mCamera = _camera.GetTranslationMatrix() * _camera.GetRotationMatrix();
            var mView = mCamera;

            _renderEngine.Render(mModel, mView, mProjection);

            // Swap the graphics buffer
            SwapBuffers();

            //            var err = GL.GetError();
            //            if (err != ErrorCode.NoError)
            //                Lumberjack.Error(err.ToString());
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        private void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            CursorVisible = false;

            var mouse = Mouse.GetState();
            _lastMousePos = new Point(mouse.X, mouse.Y);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            CursorVisible = true;
        }
    }
}