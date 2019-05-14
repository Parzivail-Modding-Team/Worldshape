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
            if (Rotation.Y > AngleMax)
                Rotation.Y = AngleMax;
            if (Rotation.Y < -AngleMax)
                Rotation.Y = -AngleMax;

            Vector3 cameraTargetVector;
            cameraTargetVector.X = (float)(Math.Sin(Rotation.X / 180 * Math.PI) * Math.Cos(Rotation.Y / 180 * Math.PI));
            cameraTargetVector.Y = (float)Math.Sin(Rotation.Y / 180 * Math.PI);
            cameraTargetVector.Z = (float)(Math.Cos(Rotation.X / 180 * Math.PI) * Math.Cos(Rotation.Y / 180 * Math.PI));
            return cameraTargetVector;
        }

        public void Move(Vector3 direction, float speed = 1)
        {
            var forward = GetForward();
            var offset = new Vector3();
            offset += direction.X * new Vector3(-forward.Z, 0, forward.X);
            offset += direction.Y * forward;
            offset.Y += direction.Z;
            offset.NormalizeFast();
            Position += Vector3.Multiply(offset, speed);
        }
    }
}
