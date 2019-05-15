using MinecraftStructureLib.Core;
using Worldshape.Extensions;
using Worldshape.Graphics.Buffer;
using Worldshape.Graphics.Primitive;
using Worldshape.Graphics.Texture;

namespace Worldshape.Graphics.Game.Block
{
	class BvpColumn : IBlockVertexProducer
	{
		public static readonly BvpColumn Instance = new BvpColumn();

		public void Render(Structure structure, int x, int y, int z, BlockAtlas blockAtlas, ChunkBuffer vbi)
		{
			var block = structure[x, y, z];
			var blockData = blockAtlas[block.Id];
			var tex = blockData.Textures[0];

			Uv tc00, tc10, tc01, tc11;
			TexCoord front, back, left, right;
			front = back = left = right = blockData.Textures[1];

			var top = blockData.Textures[0];
			var bottom = blockData.Textures[2];
			
			const float d = 0.0002f; // bleed compensation

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.PosX, blockAtlas))
			{
				CreateUv(right, d, out tc00, out tc10, out tc01, out tc11);

				vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.PosX), tc00);
				vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosX), tc10);
				vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.PosX), tc11);
				vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.PosX), tc01);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.NegX, blockAtlas))
			{
				CreateUv(left, d, out tc00, out tc10, out tc01, out tc11);

				vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegX), tc01);
				vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.NegX), tc11);
				vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.NegX), tc10);
				vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.NegX), tc00);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.PosY, blockAtlas))
			{
				CreateUv(top, d, out tc00, out tc10, out tc01, out tc11);

				vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.PosY), tc00);
				vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosY), tc10);
				vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.PosY), tc11);
				vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.PosY), tc01);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.NegY, blockAtlas))
			{
				CreateUv(bottom, d, out tc00, out tc10, out tc01, out tc11);

				vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegY), tc01);
				vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.NegY), tc11);
				vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.NegY), tc10);
				vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.NegY), tc00);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.PosZ, blockAtlas))
			{
				CreateUv(back, d, out tc00, out tc10, out tc01, out tc11);

				vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.PosZ), tc01);
				vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.PosZ), tc11);
				vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosZ), tc10);
				vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.PosZ), tc00);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.NegZ, blockAtlas))
			{
				CreateUv(front, d, out tc00, out tc10, out tc01, out tc11);

				vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.NegZ), tc00);
				vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.NegZ), tc10);
				vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.NegZ), tc11);
				vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegZ), tc01);
			}
		}

		public bool ShouldRenderInPass(int pass)
		{
			return pass == 0;
		}

		private static void CreateUv(TexCoord texture, float bleed, out Uv tc00, out Uv tc10, out Uv tc01, out Uv tc11)
		{
			tc00 = new Uv(texture.MinU + bleed, texture.MinV + bleed);
			tc10 = new Uv(texture.MaxU - bleed, texture.MinV + bleed);
			tc01 = new Uv(texture.MinU + bleed, texture.MaxV - bleed);
			tc11 = new Uv(texture.MaxU - bleed, texture.MaxV - bleed);
		}
	}
}