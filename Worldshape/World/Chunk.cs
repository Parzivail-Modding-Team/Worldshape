using System;
using MinecraftStructureLib.Core;
using Worldshape.Extensions;
using Worldshape.Graphics.Buffer;
using Worldshape.Graphics.Game;
using Worldshape.Graphics.Primitive;
using Worldshape.Graphics.Texture;

namespace Worldshape.World
{
    public class Chunk
    {
        public int X { get; }
        public int Z { get; }

        private readonly VertexBuffer _vbo = new VertexBuffer();
        private readonly ChunkBuffer _vbi = new ChunkBuffer();

        public Chunk(int x, int z)
        {
            X = x;
            Z = z;
        }

        /// <summary>
        /// Initializes the VBO but does not modify graphics state
        /// </summary>
        public void Prerender(Structure structure, BlockAtlas blockAtlas)
        {
            _vbi.Reset();

            for (var pass = 0; pass <= 1; pass++)
            {
	            for (var x = X * 16; x < X * 16 + 16; x++)
	            for (var y = 0; y < 256; y++)
	            for (var z = Z * 16; z < Z * 16 + 16; z++)
	            {
		            if (!structure.Contains(x, y, z))
			            continue;

		            var block = structure[x, y, z];
		            if (block == null)
			            continue;

		            var blockData = blockAtlas[block.Id];
		            if (blockData == null || blockData.Properties.Render == "none" || blockData.Textures.Count == 0)
			            continue;

		            ChunkRenderer.Render(structure, x, y, z, blockAtlas, _vbi, pass);
	            }
            }
        }

        /// <summary>
        /// Uploads the VBO to OpenGL
        /// </summary>
        public void Render()
        {
            _vbo.InitializeVbo(_vbi);
            _vbi.Reset();
        }

        /// <summary>
        /// Renders the VBO
        /// </summary>
        public void Draw()
        {
            if (!_vbo.Initialized)
                return;

            _vbo.Render();
        }
    }
}