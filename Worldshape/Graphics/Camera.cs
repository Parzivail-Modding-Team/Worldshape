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
		public Vector3 Position = new Vector3(-64, -100, -64);
		public Vector2 Rotation = new Vector2(-45, 45);
		public float FieldOfView { get; set; } = 70;

		public Matrix4 GetRotationMatrix()
		{
			return Matrix4.CreateRotationY((float)(Rotation.X / 180 * Math.PI)) * Matrix4.CreateRotationX((float)(Rotation.Y / 180 * Math.PI));
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
