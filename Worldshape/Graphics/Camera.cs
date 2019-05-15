using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Worldshape.Graphics
{
    class Camera
    {
        private const float AngleMax = 89.99f;
        public Vector3 Position = new Vector3(0, 0, -128);
        public Vector2 Rotation = new Vector2(0, 0);

        public Vector3 GetForward()
        {
            Vector3 cameraTargetVector;
            cameraTargetVector.X = (float)(Math.Sin(Rotation.X / 180 * Math.PI) * Math.Cos(Rotation.Y / 180 * Math.PI));
            cameraTargetVector.Y = (float)Math.Sin(Rotation.Y / 180 * Math.PI);
            cameraTargetVector.Z = (float)(Math.Cos(Rotation.X / 180 * Math.PI) * Math.Cos(Rotation.Y / 180 * Math.PI));
            return cameraTargetVector;
        }

        public Matrix4 GetRotationMatrix()
        {
            return Matrix4.CreateRotationY(Rotation.X) * Matrix4.CreateRotationX(Rotation.Y);
        }

        public Matrix4 GetTranslationMatrix()
        {
            return Matrix4.CreateTranslation(Position);
        }

        public void Move(Vector3 direction, float speed = 1)
        {
            var matrix = GetRotationMatrix();
            var offset = matrix * new Vector4(direction);
            Position += offset.Xyz * speed;
        }
    }
}
