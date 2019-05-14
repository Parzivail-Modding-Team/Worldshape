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
        private bool _shouldDie;
        private KeyboardState _keyboard;
        private RenderEngine _renderEngine;
        private MappingEngine _mappingEngine;
        private Camera _camera;

        private float _zoom = 1;
        private float _prevZoom = 1;
        private Vector2 _translation = new Vector2(0, -25);
        private Vector3 _prevTranslation;
        private Vector3 _prevLook;

        private double _updateTimeAccumulator = 0;

        private Structure _structure;

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
            if (_shouldDie)
                Exit();

            Title = $"{RenderFrequency} FPS";

            // Grab the new keyboard state
            _keyboard = Keyboard.GetState();

            // Compute input-based rotations
            var delta = (float)e.Time;
            var amount = _keyboard[Key.LShift] || _keyboard[Key.RShift] ? 45 : 90;

            _prevZoom = _zoom;
            _prevTranslation = _camera.Position;
            _prevLook = _camera.GetForward();

            if (Focused)
            {
//                if (_keyboard[Key.Left])
//                    _rotation.Y += amount * delta;
//                if (_keyboard[Key.Right])
//                    _rotation.Y -= amount * delta;
//                if (_keyboard[Key.Up])
//                    _rotation.X += amount * delta;
//                if (_keyboard[Key.Down])
//                    _rotation.X -= amount * delta;
//                if (_keyboard[Key.R])
//                {
//                    _rotation.Y = 45;
//                    _rotation.X = 160;
//                    _translation = new Vector2(0, -25);
//                }

                if (_keyboard[Key.W])
                    _camera.Move(Vector3.UnitY * delta);
                if (_keyboard[Key.S])
                    _camera.Move(-Vector3.UnitY * delta);
                if (_keyboard[Key.A])
                    _camera.Move(-Vector3.UnitX * delta);
                if (_keyboard[Key.D])
                    _camera.Move(Vector3.UnitX * delta);
            }

            _renderEngine.Update();

            _updateTimeAccumulator = 0;
        }

        private void OnRender(object sender, FrameEventArgs e)
        {
            // Reset the view
            GL.Clear(ClearBufferMask.ColorBufferBit |
                     ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);

            _updateTimeAccumulator += e.Time;
            var partialTicks = (float)(_updateTimeAccumulator / TargetUpdatePeriod);

            // Reload the projection matrix
            var aspectRatio = Width / (float)Height;
            var zoom = (float)(_prevZoom + (_zoom - _prevZoom) * partialTicks);
            var scale = new Vector3(4 * (1 / zoom), -4 * (1 / zoom), 4 * (1 / zoom));
            var look = _prevLook + (_camera.GetForward() - _prevLook) * partialTicks;
            var pos = _prevTranslation + (_camera.Position - _prevTranslation) * partialTicks;

            var mProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 1024);
            var mModel = Matrix4.Identity; //Matrix4.LookAt(0, 0, 0, 0, 0, 0, 0, 1, 0);
            var mScale = Matrix4.CreateScale(scale);
            var mCamera = Matrix4.LookAt(pos, pos + look, Vector3.UnitY);
            var mLocalTranslate = Matrix4.CreateTranslation(-_structure.Width / 2f, -_structure.Height / 2f, -_structure.Length / 2f);
            var mView = mLocalTranslate * mCamera;

            _renderEngine.Render(mModel, mView, mProjection);

            // Swap the graphics buffer
            SwapBuffers();

//            var err = GL.GetError();
//            if (err != ErrorCode.NoError)
//                Lumberjack.Error(err.ToString());
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _zoom -= e.DeltaPrecise / 4f;

            if (_zoom < 0.5f)
                _zoom = 0.5f;
            if (_zoom > 35)
                _zoom = 35;
        }

        private void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            if (!e.Mouse.IsButtonDown(MouseButton.Left)) return;

            if (_keyboard[Key.ShiftLeft])
            {
                _translation.X += e.XDelta / 2f;
                _translation.Y -= e.YDelta / 2f;
            }
            else
            {
                _camera.Rotation.X -= e.XDelta / 2f;
                _camera.Rotation.Y -= e.YDelta / 2f;
            }
        }
    }
}