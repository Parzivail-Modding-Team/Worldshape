using System.Collections.Generic;
using MinecraftStructureLib.Core;
using Worldshape.Graphics.Buffer;
using Worldshape.Graphics.Texture;

namespace Worldshape.Graphics.Game
{
	class ChunkRenderer
	{
		private static readonly Dictionary<string, IBlockVertexProducer> VertexProducers = new Dictionary<string, IBlockVertexProducer>()
		{
			{"solid", BvpSolid.Instance},
			{"transparent", BvpSolid.Instance},
			{"liquid", BvpSolid.Instance},
			{"column", BvpColumn.Instance},
			{"cross", BvpCross.Instance}
		};

		public static void Render(Structure structure, int x, int y, int z, BlockAtlas blockAtlas, ChunkBuffer vbi)
		{
			var block = structure[x, y, z];
			var renderType = blockAtlas[block.Id].Properties.Render;
			if (!VertexProducers.TryGetValue(renderType, out var producer))
				return;

			producer.Render(structure, x, y, z, blockAtlas, vbi);
		}
	}
}
