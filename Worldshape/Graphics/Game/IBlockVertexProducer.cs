using MinecraftStructureLib.Core;
using Worldshape.Graphics.Buffer;
using Worldshape.Graphics.Texture;

namespace Worldshape.Graphics.Game
{
	internal interface IBlockVertexProducer
	{
		void Render(Structure structure, int x, int y, int z, BlockAtlas blockAtlas, ChunkBuffer vbi);
		bool ShouldRenderInPass(int pass);
	}
}