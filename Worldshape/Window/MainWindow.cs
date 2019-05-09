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

        private float _zoom = 1;
        private float _prevZoom = 1;
        private Vector2 _translation = new Vector2(0, -25);
        private Vector2 _prevTranslation = new Vector2(0, -25);
        private Vector2 _rotation = new Vector2(160, 45);
        private Vector2 _prevRotation = new Vector2(160, 45);

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
            _prevTranslation = new Vector2(_translation.X, _translation.Y);
            _prevRotation = new Vector2(_rotation.X, _rotation.Y);

            if (Focused)
            {
                if (_keyboard[Key.Left])
                    _rotation.Y += amount * delta;
                if (_keyboard[Key.Right])
                    _rotation.Y -= amount * delta;
                if (_keyboard[Key.Up])
                    _rotation.X += amount * delta;
                if (_keyboard[Key.Down])
                    _rotation.X -= amount * delta;
                if (_keyboard[Key.R])
                {
                    _rotation.Y = 45;
                    _rotation.X = 160;
                    _translation = new Vector2(0, -25);
                }
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
            var partialTicks = _updateTimeAccumulator / TargetUpdatePeriod;

            // Reload the projection matrix
            var aspectRatio = Width / (float)Height;
            var zoom = (float)(_prevZoom + (_zoom - _prevZoom) * partialTicks);
            var scale = new Vector3(4 * (1 / zoom), -4 * (1 / zoom), 4 * (1 / zoom));
            var rotX = _prevRotation.X + (_rotation.X - _prevRotation.X) * partialTicks;
            var rotY = _prevRotation.Y + (_rotation.Y - _prevRotation.Y) * partialTicks;
            var transX = _prevTranslation.X + (_translation.X - _prevTranslation.X) * partialTicks;
            var transY = _prevTranslation.Y + (_translation.Y - _prevTranslation.Y) * partialTicks;

            var mProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 1024);
            var mModel = Matrix4.LookAt(0, 128, 256, 0, 0, 0, 0, 1, 0);
            var mTranslate = Matrix4.CreateTranslation((float)transX, (float)transY, 0);
            var mScale = Matrix4.CreateScale(scale);
            var mRotX = Matrix4.CreateRotationX((float)(rotX / 180 * Math.PI));
            var mRotY = Matrix4.CreateRotationY((float)(rotY / 180 * Math.PI));
            var mLocalTranslate = Matrix4.CreateTranslation(-_structure.Width / 2f, -_structure.Height / 2f, -_structure.Length / 2f);
            var mView = mLocalTranslate * mRotY * mRotX * mScale * mTranslate;

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
                _rotation.X -= e.YDelta / 2f;
                _rotation.Y -= e.XDelta / 2f;
            }
        }
    }
}