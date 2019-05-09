using MinecraftStructureLib.Core;
using Worldshape.Extensions;
using Worldshape.Graphics.Buffer;
using Worldshape.Graphics.Primitive;
using Worldshape.Graphics.Texture;

namespace Worldshape.Graphics.Game
{
	class BvpSolid : IBlockVertexProducer
	{
		public static readonly BvpSolid Instance = new BvpSolid();

		public void Render(Structure structure, int x, int y, int z, BlockAtlas blockAtlas, ChunkBuffer vbi)
		{
			var block = structure[x, y, z];
			var blockData = blockAtlas[block.Id];
			var tex = blockData.Textures[0];

			var tc00 = new Uv(0, 0);
			var tc10 = new Uv(1, 0);
			var tc01 = new Uv(0, 1);
			var tc11 = new Uv(1, 1);

			if (tex != null)
			{
				var d = 0.0002f; // bleed compensation
				tc00 = new Uv(tex.MinU + d, tex.MinV + d);
				tc10 = new Uv(tex.MaxU - d, tex.MinV + d);
				tc01 = new Uv(tex.MinU + d, tex.MaxV - d);
				tc11 = new Uv(tex.MaxU - d, tex.MaxV - d);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.PosX, blockAtlas))
			{
				vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.PosX), tc01);
				vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.PosX), tc11);
				vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosX), tc10);
				vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.PosX), tc00);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.NegX, blockAtlas))
			{
				vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegX), tc01);
				vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.NegX), tc11);
				vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.NegX), tc10);
				vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.NegX), tc00);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.PosY, blockAtlas))
			{
				vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.PosY), tc01);
				vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.PosY), tc11);
				vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosY), tc10);
				vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.PosY), tc00);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.NegY, blockAtlas))
			{
				vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegY), tc01);
				vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.NegY), tc11);
				vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.NegY), tc10);
				vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.NegY), tc00);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.PosZ, blockAtlas))
			{
				vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.PosZ), tc01);
				vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.PosZ), tc11);
				vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.PosZ), tc10);
				vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.PosZ), tc00);
			}

			if (structure.IsBorderingTransparent(x, y, z, FaceDir.NegZ, blockAtlas))
			{
				vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegZ), tc01);
				vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.NegZ), tc11);
				vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.NegZ), tc10);
				vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.NegZ), tc00);
			}
		}
	}
}