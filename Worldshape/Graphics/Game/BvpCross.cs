using MinecraftStructureLib.Core;
using Worldshape.Graphics.Buffer;
using Worldshape.Graphics.Primitive;
using Worldshape.Graphics.Texture;

namespace Worldshape.Graphics.Game
{
	class BvpCross : IBlockVertexProducer
	{
		public static readonly BvpCross Instance = new BvpCross();

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

			vbi.Append(new Vertex(x, y, z + 1), new Vertex(FaceDir.NegZ), tc01);
			vbi.Append(new Vertex(x + 1, y, z), new Vertex(FaceDir.NegZ), tc11);
			vbi.Append(new Vertex(x + 1, y + 1, z), new Vertex(FaceDir.NegZ), tc10);
			vbi.Append(new Vertex(x, y + 1, z + 1), new Vertex(FaceDir.NegZ), tc00);

			vbi.Append(new Vertex(x, y, z), new Vertex(FaceDir.NegZ), tc01);
			vbi.Append(new Vertex(x + 1, y, z + 1), new Vertex(FaceDir.NegZ), tc11);
			vbi.Append(new Vertex(x + 1, y + 1, z + 1), new Vertex(FaceDir.NegZ), tc10);
			vbi.Append(new Vertex(x, y + 1, z), new Vertex(FaceDir.NegZ), tc00);
		}
	}
}